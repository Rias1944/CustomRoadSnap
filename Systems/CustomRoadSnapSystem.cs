using Game;
using Game.Net;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace CustomRoadSnap
{
    // -------------------------------------------------------------------------
    // Shared constants  –  axes are computed dynamically from the SnapAngle setting
    // -------------------------------------------------------------------------
    internal static class SnapConstants
    {
        // Returns axis offsets for the current SnapAngle (e.g. 30° → 6 axes at 0,30,60,…150°)
        public static float[] GetAxisOffsets()
        {
            float angleDeg = Mod.m_Setting?.SnapAngle ?? 30f;
            angleDeg = math.clamp(angleDeg, 1f, 90f);
            float angleRad = angleDeg * math.PI / 180f;
            // Number of unique axes in 180° half-circle
            int count = (int)math.round(math.PI / angleRad);
            if (count < 1) count = 1;
            var offsets = new float[count];
            for (int i = 0; i < count; i++)
                offsets[i] = i * angleRad;
            return offsets;
        }

        // Snap greift nur ein, wenn der Cursor innerhalb dieses Winkels zur Achse liegt
        public const float SnapThresholdDeg = 3f;
        public static readonly float SnapThresholdRad = SnapThresholdDeg * math.PI / 180f;

        public static float DeltaAngle(float a, float b)
        {
            float d = (b - a) % (math.PI * 2f);
            if (d >  math.PI) d -= math.PI * 2f;
            if (d < -math.PI) d += math.PI * 2f;
            return d;
        }
    }

    // Shared state written by PostSystem, read by HarmonyPatch every frame.
    internal static class SnapState
    {
        public static bool   HasAnchor;
        public static float  BaseAngle;
        public static float3 AnchorPos;
    }

    // -------------------------------------------------------------------------
    // PRE-PHASE  –  UpdateBefore<NetToolSystem>
    // -------------------------------------------------------------------------
    // Removes StraightDirection so SnapJob does NOT apply 90 deg snap.
    // Removing it sets m_ForceUpdate = true, which causes fullUpdate = true,
    // which means SnapJob ALWAYS runs this frame and clears m_SnapLines.
    // PostSystem then adds our 120 deg guide lines after SnapJob.
    // -------------------------------------------------------------------------
    public partial class CustomRoadSnapPreSystem : GameSystemBase
    {
        private ToolSystem    m_ToolSystem;
        private NetToolSystem m_NetToolSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_ToolSystem    = World.GetOrCreateSystemManaged<ToolSystem>();
            m_NetToolSystem = World.GetOrCreateSystemManaged<NetToolSystem>();
        }

        protected override void OnUpdate()
        {
            if (!(Mod.m_Setting?.SnapEnabled ?? false))
            {
                // Restore vanilla 90-deg snap when mod is disabled
                if ((m_NetToolSystem.selectedSnap & Snap.StraightDirection) == Snap.None)
                    m_NetToolSystem.selectedSnap |= Snap.StraightDirection;
                return;
            }

            if (m_ToolSystem.activeTool != m_NetToolSystem)
                return;

            // Remove 90-deg snap flag so NetToolSystem does NOT apply 90-deg snap.
            if ((m_NetToolSystem.selectedSnap & Snap.StraightDirection) != Snap.None)
                m_NetToolSystem.selectedSnap &= ~Snap.StraightDirection;
        }
    }

    // -------------------------------------------------------------------------
    // POST-PHASE  –  UpdateAfter<NetToolSystem>
    // -------------------------------------------------------------------------
    // After SnapJob has run (and cleared m_SnapLines):
    //   1. Compute 120 deg snap from the CURRENT raw raycast (m_HitPosition).
    //   2. Write snapped position back to m_ControlPoints[last].
    //   3. Add GuideLine SnapLines through anchor position.
    //   4. Restore StraightDirection for next frame base state.
    //   5. Fix NetCourse.m_Curve on Temp preview entities.
    // -------------------------------------------------------------------------
    public partial class CustomRoadSnapPostSystem : GameSystemBase
    {
        private ToolSystem    m_ToolSystem;
        private NetToolSystem m_NetToolSystem;
        private EntityQuery   m_TempNetCourseQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_ToolSystem    = World.GetOrCreateSystemManaged<ToolSystem>();
            m_NetToolSystem = World.GetOrCreateSystemManaged<NetToolSystem>();

            m_TempNetCourseQuery = GetEntityQuery(
                ComponentType.ReadOnly<Game.Tools.Temp>(),
                ComponentType.ReadWrite<NetCourse>());
        }

        protected override void OnUpdate()
        {
            if (!(Mod.m_Setting?.SnapEnabled ?? false))
            {
                SnapState.HasAnchor = false;
                NetToolSystem_GetRaycastResult_Patch.Active = false;
                return;
            }

            if (m_ToolSystem.activeTool != m_NetToolSystem)
            {
                SnapState.HasAnchor = false;
                NetToolSystem_GetRaycastResult_Patch.Active = false;
                return;
            }

            // Wait for SnapJob to finish (clears m_SnapLines, writes m_ControlPoints).
            var snapLines     = m_NetToolSystem.GetSnapLines(out var slDeps);
            var controlPoints = m_NetToolSystem.GetControlPoints(out var cpDeps);
            JobHandle.CombineDependencies(slDeps, cpDeps).Complete();

            // Update SnapState so the Harmony patch knows the current anchor/baseAngle.
            if (controlPoints.IsCreated && controlPoints.Length >= 2)
            {
                int anchorIdx = controlPoints.Length - 2;
                ControlPoint anchor = controlPoints[anchorIdx];

                float2 prevDir = anchor.m_Direction;
                if (math.lengthsq(prevDir) < 1e-6f)
                {
                    if (anchorIdx >= 1)
                        prevDir = math.normalizesafe(anchor.m_Position.xz - controlPoints[anchorIdx - 1].m_Position.xz);
                    if (math.lengthsq(prevDir) < 1e-6f)
                        prevDir = new float2(0f, 1f);
                }

                SnapState.HasAnchor = true;
                SnapState.AnchorPos = anchor.m_Position;
                SnapState.BaseAngle = math.atan2(prevDir.x, prevDir.y);
                NetToolSystem_GetRaycastResult_Patch.Active = true;
            }
            else
            {
                SnapState.HasAnchor = false;
                NetToolSystem_GetRaycastResult_Patch.Active = false;
            }

            if (!SnapState.HasAnchor)
                return;

            float  baseAngle = SnapState.BaseAngle;
            float3 anchorPos = SnapState.AnchorPos;

            // Add GuideLine SnapLines for each of the 3 axes.
            // SnapJob already cleared m_SnapLines, so we add fresh each frame.
            ControlPoint lineCP = controlPoints[controlPoints.Length - 1];
            lineCP.m_SnapPriority = new float2(0f, 0f);

            foreach (float off in SnapConstants.GetAxisOffsets())
            {
                float  a  = baseAngle + off;
                float2 d2 = new float2(math.sin(a), math.cos(a));
                float3 d3 = new float3(d2.x, 0f, d2.y);
                float3 ls = new float3(anchorPos.x - d3.x * 1000f, anchorPos.y, anchorPos.z - d3.z * 1000f);
                float3 le = new float3(anchorPos.x + d3.x * 1000f, anchorPos.y, anchorPos.z + d3.z * 1000f);

                lineCP.m_Direction = d2;
                snapLines.Add(new SnapLine(lineCP, NetUtils.StraightCurve(ls, le), SnapLineFlags.GuideLine, 0f));
            }

            // Fix NetCourse curves on Temp preview entities.
            CompleteDependency();
            if (m_TempNetCourseQuery.IsEmpty)
                return;

            var entities = m_TempNetCourseQuery.ToEntityArray(Allocator.Temp);
            for (int i = 0; i < entities.Length; i++)
            {
                var    course = EntityManager.GetComponentData<NetCourse>(entities[i]);
                float3 start  = course.m_Curve.a;
                float3 end    = course.m_Curve.d;
                float2 raw    = end.xz - start.xz;
                float  len    = math.length(raw);
                if (len < 1e-3f) continue;

                float curAngle = math.atan2(raw.x, raw.y);
                float bestDiff = float.MaxValue;
                float bestA    = curAngle;

                foreach (float off in SnapConstants.GetAxisOffsets())
                {
                    float cand = baseAngle + off;
                    float diff = math.abs(SnapConstants.DeltaAngle(curAngle, cand));
                    if (diff < bestDiff) { bestDiff = diff; bestA = cand; }

                    float candNeg = baseAngle - off;
                    float diffNeg = math.abs(SnapConstants.DeltaAngle(curAngle, candNeg));
                    if (diffNeg < bestDiff) { bestDiff = diffNeg; bestA = candNeg; }
                }

                float2 sd     = new float2(math.sin(bestA), math.cos(bestA));
                float3 newEnd = new float3(start.x + sd.x * len, end.y, start.z + sd.y * len);
                float3 dv     = newEnd - start;
                course.m_Curve.b = start + dv * (1f / 3f);
                course.m_Curve.c = start + dv * (2f / 3f);
                course.m_Curve.d = newEnd;
                EntityManager.SetComponentData(entities[i], course);
            }
            entities.Dispose();
        }
    }
}

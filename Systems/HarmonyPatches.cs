using Game.Tools;
using HarmonyLib;
using Unity.Mathematics;

namespace RoadSnap120
{
    // -------------------------------------------------------------------------
    // Harmony patch on NetToolSystem.GetRaycastResult(out ControlPoint, out bool)
    //
    // This Postfix runs immediately after GetRaycastResult returns.
    // At this point controlPoint holds the raw raycast position.
    // We project it onto the nearest 120° axis before NetToolSystem writes it
    // into m_ControlPoints[last] – giving us a true lock, identical to how
    // vanilla 90° snap works internally.
    // -------------------------------------------------------------------------
    [HarmonyPatch(typeof(NetToolSystem), "GetRaycastResult",
        new[] { typeof(ControlPoint), typeof(bool) },
        new[] { ArgumentType.Out,     ArgumentType.Out })]
    internal static class NetToolSystem_GetRaycastResult_Patch
    {
        internal static bool Active = false;

        static void Postfix(ref ControlPoint controlPoint, bool __result)
        {
            if (!Active || !__result)
                return;

            if (!SnapState.HasAnchor)
                return;

            float3 anchorPos = SnapState.AnchorPos;
            float  baseAngle = SnapState.BaseAngle;
            float3 hitPos    = controlPoint.m_HitPosition;

            // Nur snappen wenn Cursor weit genug vom Ankerpunkt entfernt ist
            float distFromAnchor = math.length((hitPos - anchorPos).xz);
            if (distFromAnchor < 1f)
                return;

            float  bestDist = float.MaxValue;
            float3 snapPos  = hitPos;
            float2 snapDir  = new float2(math.sin(baseAngle), math.cos(baseAngle));

            foreach (float off in SnapConstants.AxisOffsets)
            {
                float  a    = baseAngle + off;
                float2 d2   = new float2(math.sin(a), math.cos(a));
                float3 d3   = new float3(d2.x, 0f, d2.y);
                float  t    = math.dot(hitPos - anchorPos, d3);
                float3 proj = anchorPos + d3 * t;
                float  perp = math.length((hitPos - proj).xz);

                if (perp < bestDist)
                {
                    bestDist = perp;
                    snapPos  = new float3(proj.x, hitPos.y, proj.z);
                    snapDir  = math.select(d2, -d2, t < 0f);
                }
            }

            // Nur einrasten wenn Winkelabstand zur nächsten Achse unter dem Schwellenwert liegt (15°)
            float snapAngle = math.asin(math.clamp(bestDist / distFromAnchor, 0f, 1f));
            if (snapAngle > SnapConstants.SnapThresholdRad)
                return;

            controlPoint.m_HitPosition = snapPos;
            controlPoint.m_Position    = snapPos;
            controlPoint.m_Direction   = snapDir;
        }
    }
}

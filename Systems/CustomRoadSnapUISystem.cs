using Colossal.Serialization.Entities;
using Colossal.UI;
using Colossal.UI.Binding;
using Game;
using Game.SceneFlow;
using Game.UI;

namespace CustomRoadSnap
{
    /// <summary>
    /// Stellt der UI einen ValueBinding für den Snap-Zustand und einen TriggerBinding
    /// für den Toggle-Button zur Verfügung.
    /// </summary>
    public partial class CustomRoadSnapUISystem : UISystemBase
    {
        private ValueBinding<bool> m_SnapEnabledBinding;
        private ValueBinding<int> m_SnapAngleBinding;

        protected override void OnCreate()
        {
            base.OnCreate();

            // Assembly.Location ist unter Mono leer – Pfad über ModManager ermitteln
            if (GameManager.instance.modManager.TryGetExecutableAsset(Mod.instance, out var asset))
            {
                string modDir = System.IO.Path.GetDirectoryName(asset.path);
                UIManager.defaultUISystem.AddHostLocation("ui-mods", modDir, false);
            }

            AddBinding(m_SnapEnabledBinding = new ValueBinding<bool>(
                "customRoadSnap", "snapEnabled",
                Mod.m_Setting?.SnapEnabled ?? false));

            AddBinding(m_SnapAngleBinding = new ValueBinding<int>(
                "customRoadSnap", "snapAngle",
                (int)(Mod.m_Setting?.SnapAngle ?? 30f)));

            AddBinding(new TriggerBinding(
                "customRoadSnap", "toggleSnap",
                () =>
                {
                    if (Mod.m_Setting == null) return;
                    Mod.m_Setting.SnapEnabled = !Mod.m_Setting.SnapEnabled;
                    Mod.m_Setting.ApplyAndSave();
                    Mod.ApplySnapEnabled(Mod.m_Setting.SnapEnabled);
                    Mod.log.Info($"Custom Snap (UI) {(Mod.m_Setting.SnapEnabled ? "enabled" : "disabled")}");
                }));

            AddBinding(new TriggerBinding<int>(
                "customRoadSnap", "setSnapAngle",
                (int newAngle) =>
                {
                    if (Mod.m_Setting == null) return;
                    Mod.m_Setting.SnapAngle = newAngle;
                    Mod.m_Setting.ApplyAndSave();
                    Mod.log.Info($"Snap angle set to {newAngle}°");
                }));
        }

        protected override void OnGamePreload(Purpose purpose, GameMode mode)
        {
            // Intentionally do NOT call base — UISystemBase.OnGamePreload calls set_Enabled
            // before the ECS state is ready, which causes an InvalidOperationException crash.
        }

        protected override void OnUpdate()
        {
            m_SnapEnabledBinding.Update(Mod.m_Setting?.SnapEnabled ?? false);
            m_SnapAngleBinding.Update((int)(Mod.m_Setting?.SnapAngle ?? 30f));
        }
    }
}

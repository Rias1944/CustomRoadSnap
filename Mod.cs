using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Input;
using Game.Modding;
using Game.SceneFlow;
using Game.Tools;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace RoadSnap120
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(RoadSnap120)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        public static Setting m_Setting;
        public static ProxyAction m_ToggleAction;
        public static Mod instance;
        private Harmony _harmony;

        public const string kToggleActionName = "ToggleSnap";

        public void OnLoad(UpdateSystem updateSystem)
        {
            instance = this;
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));

            m_Setting.RegisterKeyBindings();

            _harmony = new Harmony("com.roadsnap120");
            _harmony.PatchAll(typeof(Mod).Assembly);

            m_ToggleAction = m_Setting.GetAction(kToggleActionName);
            m_ToggleAction.shouldBeEnabled = true;
            m_ToggleAction.onInteraction += OnToggleSnap;

            AssetDatabase.global.LoadSettings(nameof(RoadSnap120), m_Setting, new Setting(this));

            // Pre-phase: runs BEFORE NetToolSystem (patches HitPosition, removes 90° snap flag)
            updateSystem.UpdateBefore<RoadSnap120PreSystem, NetToolSystem>(SystemUpdatePhase.ToolUpdate);

            // Post-phase: runs AFTER NetToolSystem (restores flag, adds guide lines, fixes curves)
            updateSystem.UpdateAfter<RoadSnap120PostSystem, NetToolSystem>(SystemUpdatePhase.ToolUpdate);

            // UI-System für den Toolbar-Button
            updateSystem.UpdateAt<RoadSnapUISystem>(SystemUpdatePhase.UIUpdate);
        }

        private void OnToggleSnap(ProxyAction action, UnityEngine.InputSystem.InputActionPhase phase)
        {
            if (phase != UnityEngine.InputSystem.InputActionPhase.Performed)
                return;

            m_Setting.SnapEnabled = !m_Setting.SnapEnabled;
            m_Setting.ApplyAndSave();
            ApplySnapEnabled(m_Setting.SnapEnabled);
            log.Info($"120° Snap {(m_Setting.SnapEnabled ? "enabled" : "disabled")}");
        }

        public static void ApplySnapEnabled(bool enabled)
        {
            // Systems check Mod.m_Setting.SnapEnabled directly in OnUpdate.
            // Nothing extra needed here – kept for UISystem/keybind callbacks.
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            _harmony?.UnpatchAll("com.roadsnap120");
            _harmony = null;
            if (m_ToggleAction != null)
                m_ToggleAction.onInteraction -= OnToggleSnap;

            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}

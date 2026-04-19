using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Input;
using Game.Modding;
using Game.Settings;
using Game.UI;
using System.Collections.Generic;

namespace CustomRoadSnap
{
    [FileLocation(nameof(CustomRoadSnap))]
    [SettingsUIGroupOrder(kSnapGroup, kKeybindingGroup)]
    [SettingsUIShowGroupName(kSnapGroup, kKeybindingGroup)]
    [SettingsUIKeyboardAction(Mod.kToggleActionName, ActionType.Button, usages: new string[] { Usages.kDefaultUsage }, interactions: new string[] { "Press" })]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kSnapGroup = "Snap";
        public const string kKeybindingGroup = "KeyBinding";

        public Setting(IMod mod) : base(mod) { }

        [SettingsUISection(kSection, kSnapGroup)]
        public bool SnapEnabled { get; set; }

        /// <summary>Snap angle in degrees. The snap grid uses this angle and its multiples. Range 1–90°, default 30°.</summary>
        [SettingsUISection(kSection, kSnapGroup)]
        [SettingsUISlider(min = 1f, max = 90f, step = 1f, scalarMultiplier = 1f, unit = Unit.kAngle)]
        public float SnapAngle { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.None, Mod.kToggleActionName)]
        [SettingsUISection(kSection, kKeybindingGroup)]
        public ProxyBinding ToggleSnapBinding { get; set; }

        [SettingsUIButton]
        [SettingsUISection(kSection, kKeybindingGroup)]
        public bool ResetBindings
        {
            set
            {
                Mod.log.Info("Reset key bindings");
                ResetKeyBindings();
            }
        }

        public override void SetDefaults()
        {
            SnapEnabled = false;
            SnapAngle   = 30f;
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(),                              "Custom Road Snap" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection),             "Main" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kSnapGroup),         "Snap Settings" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kKeybindingGroup),   "Key Bindings" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SnapEnabled)), "Enable Custom Snap" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SnapEnabled)),  "Snaps road direction to multiples of the configured snap angle." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SnapAngle)),   "Snap Angle" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SnapAngle)),    "The angle increment for road snapping (in degrees). E.g. 30° gives axes at 0°, 30°, 60°, …" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ToggleSnapBinding)), "Toggle Snap" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ToggleSnapBinding)),  "Keyboard shortcut to toggle the custom snap on/off." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "Reset Key Bindings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)),  "Resets all key bindings to their defaults." },

                { m_Setting.GetBindingMapLocaleID(),                              "Custom Road Snap" },
                { m_Setting.GetBindingKeyLocaleID(Mod.kToggleActionName),         "Toggle Snap key" },
            };
        }

        public void Unload() { }
    }
}

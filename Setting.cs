using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Input;
using Game.Modding;
using Game.Settings;
using System.Collections.Generic;

namespace RoadSnap120
{
    [FileLocation(nameof(RoadSnap120))]
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
                { m_Setting.GetSettingsLocaleID(), "Road Snap 120" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kSnapGroup), "Snap Settings" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kKeybindingGroup), "Key Bindings" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SnapEnabled)), "Enable 120 Snap" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SnapEnabled)), "Snaps road direction to 120 degree increments instead of 90 degrees." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ToggleSnapBinding)), "Toggle Snap" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ToggleSnapBinding)), "Keyboard shortcut to toggle the 120 degree snap on/off." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "Reset Key Bindings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), "Resets all key bindings to their defaults." },

                { m_Setting.GetBindingMapLocaleID(), "Road Snap 120" },
                { m_Setting.GetBindingKeyLocaleID(Mod.kToggleActionName), "Toggle Snap key" },
            };
        }

        public void Unload() { }
    }
}

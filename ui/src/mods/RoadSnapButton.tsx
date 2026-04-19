import { createElement } from "react";
import { bindValue, trigger, useValue } from "cs2/api";
import { tool } from "cs2/bindings";
import { useLocalization } from "cs2/l10n";
import { ModuleRegistryExtend } from "cs2/modding";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver";

const snapEnabled$ = bindValue<boolean>("customRoadSnap", "snapEnabled", false);

export const RoadSnapSection: ModuleRegistryExtend = (Component: any) => {
    return (props: any) => {
        const snapEnabled = useValue(snapEnabled$);
        const isNetTool = useValue(tool.activeTool$)?.id === "Net Tool";
        const { translate } = useLocalization();

        const result = Component();

        if (isNetTool) {
            const sectionTitle = translate("ToolOptions.SECTION[RoadSnap120.SnapSection]", "120° Snap");
            const buttonTooltip = translate("ToolOptions.TOOLTIP_DESCRIPTION[RoadSnap120.ToggleSnap]", "120° Straßen-Einrasten ein-/ausschalten");

            result?.props?.children?.push(
                createElement(VanillaComponentResolver.instance.Section, { title: sectionTitle },
                    createElement(VanillaComponentResolver.instance.ToolButton, {
                        selected: snapEnabled,
                        tooltip: buttonTooltip,
                        onSelect: () => trigger("customRoadSnap", "toggleSnap"),
                        src: "Media/Glyphs/Angle.svg",
                        focusKey: VanillaComponentResolver.instance.FOCUS_DISABLED,
                        className: VanillaComponentResolver.instance.toolButtonTheme.ToolButton,
                    })
                )
            );
        }

        return result;
    };
};

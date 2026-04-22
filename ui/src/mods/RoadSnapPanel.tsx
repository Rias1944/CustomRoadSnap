import { createElement, useState } from "react";
import { bindValue, trigger, useValue } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import { ModuleRegistryExtend } from "cs2/modding";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver";
import styles from "./RoadSnapPanel.module.scss";

// Bindings to C# backend
const snapEnabled$ = bindValue<boolean>("customRoadSnap", "snapEnabled", false);
const snapAngle$ = bindValue<number>("customRoadSnap", "snapAngle", 30);

// Main panel component (injected into toolbar toggles)
export const RoadSnapPanelToggle: ModuleRegistryExtend = (Component: any) => {
    return (props: any) => {
        const [isPanelOpen, setIsPanelOpen] = useState(false);
        const snapEnabled = useValue(snapEnabled$);
        const snapAngle = useValue(snapAngle$);
        const { translate } = useLocalization();

        // Original component
        const result = Component();

        // Icon button
        const iconButton = createElement("button", {
            className: styles.iconButton,
            onClick: () => setIsPanelOpen(!isPanelOpen),
            title: translate("CustomRoadSnap.OpenSettings", "Custom Road Snap Settings"),
        },
            createElement("img", {
                src: "coui://ui-mods/images/angle-icon.svg",
                className: styles.iconImage,
            })
        );

        // Settings panel
        const panel = isPanelOpen && createElement("div", { 
            className: styles.panel,
            onClick: (e: any) => e.stopPropagation(), // Prevent closing when clicking inside
        },
            // Header
            createElement("div", { className: styles.header },
                createElement("img", { 
                    src: "coui://ui-mods/images/angle-icon.svg", 
                    className: styles.headerIcon 
                }),
                createElement("div", { className: styles.headerTitle }, 
                    translate("CustomRoadSnap.Title", "Custom Road Snap")
                )
            ),

            // Content
            createElement("div", { className: styles.content },
                // Enable/Disable Toggle
                createElement("div", { className: styles.settingRow },
                    createElement("div", { className: styles.toggleContainer },
                        createElement("div", { className: styles.settingLabel }, 
                            translate("CustomRoadSnap.EnableSnapping", "Enable Snapping")
                        ),
                        createElement(VanillaComponentResolver.instance.ToggleSwitch, {
                            value: snapEnabled,
                            onValueToggle: () => trigger("customRoadSnap", "toggleSnap"),
                        })
                    )
                ),

                // Snap Angle Slider
                createElement("div", { className: styles.sliderContainer },
                    createElement("div", { className: styles.settingLabel },
                        translate("CustomRoadSnap.SnapAngle", "Snap Angle"),
                        createElement("span", { className: styles.settingValue }, `${snapAngle}°`)
                    ),
                    createElement("input", {
                        type: "range",
                        min: 1,
                        max: 90,
                        value: snapAngle,
                        className: styles.slider,
                        onChange: (e: any) => {
                            const newValue = parseInt(e.target.value);
                            trigger("customRoadSnap", "setSnapAngle", newValue);
                        },
                    })
                )
            )
        );

        // Wrapper container
        const container = createElement("div", { className: styles.container },
            iconButton,
            panel
        );

        // Inject into result
        if (result?.props?.children) {
            if (Array.isArray(result.props.children)) {
                result.props.children = [...result.props.children, container];
            } else {
                result.props.children = [result.props.children, container];
            }
        }

        return result;
    };
};

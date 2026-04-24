import { ModRegistrar } from "cs2/modding";
import { RoadSnapSection } from "mods/RoadSnapButton";
import { RoadSnapPanelToggle } from "mods/RoadSnapPanel";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver";

const register: ModRegistrar = (moduleRegistry) => {
    VanillaComponentResolver.setRegistry(moduleRegistry);

    // Add button to tool options (existing)
    moduleRegistry.extend(
        "game-ui/game/components/tool-options/mouse-tool-options/mouse-tool-options.tsx",
        "MouseToolOptions",
        RoadSnapSection
    );

    // Add settings panel toggle to photo mode buttons (top-left)
    moduleRegistry.extend(
        "game-ui/game/components/photo-mode/photo-mode-panel.tsx",
        "PhotoModePanel",
        RoadSnapPanelToggle
    );
};

export default register;

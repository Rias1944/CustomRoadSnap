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

    // Add settings panel toggle to game UI
    moduleRegistry.extend(
        "game-ui/game/components/game-main-screen/game-main-screen.tsx",
        "GameMainScreen",
        RoadSnapPanelToggle
    );
};

export default register;

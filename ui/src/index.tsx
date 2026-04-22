import { ModRegistrar } from "cs2/modding";
import { RoadSnapSection } from "mods/RoadSnapButton";
import { RoadSnapIconButton } from "mods/RoadSnapPanel";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver";

const register: ModRegistrar = (moduleRegistry) => {
    VanillaComponentResolver.setRegistry(moduleRegistry);

    // Add button to tool options (existing)
    moduleRegistry.extend(
        "game-ui/game/components/tool-options/mouse-tool-options/mouse-tool-options.tsx",
        "MouseToolOptions",
        RoadSnapSection
    );

    // Add icon button to photo mode panel (top-left corner)
    moduleRegistry.extend(
        "game-ui/game/components/photo-mode/photo-mode.tsx",
        "PhotoMode",
        RoadSnapIconButton
    );
};

export default register;

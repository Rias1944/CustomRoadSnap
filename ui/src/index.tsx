import { ModRegistrar } from "cs2/modding";
import { RoadSnapSection } from "mods/RoadSnapButton";
import { VanillaComponentResolver } from "mods/VanillaComponentResolver";

const register: ModRegistrar = (moduleRegistry) => {
    VanillaComponentResolver.setRegistry(moduleRegistry);

    moduleRegistry.extend(
        "game-ui/game/components/tool-options/mouse-tool-options/mouse-tool-options.tsx",
        "MouseToolOptions",
        RoadSnapSection
    );
};

export default register;

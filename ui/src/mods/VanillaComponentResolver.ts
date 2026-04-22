import { ModuleRegistry } from "cs2/modding";

export class VanillaComponentResolver {
    static _instance: VanillaComponentResolver;

    static get instance(): VanillaComponentResolver {
        return this._instance;
    }

    static setRegistry(registry: ModuleRegistry) {
        this._instance = new VanillaComponentResolver(registry);
    }

    registryData: ModuleRegistry;
    cachedData: Record<string, any> = {};

    static registryMap: Record<string, [string, string]> = {
        Section:          ["game-ui/game/components/tool-options/mouse-tool-options/mouse-tool-options.tsx", "Section"],
        ToolButton:       ["game-ui/game/components/tool-options/tool-button/tool-button.tsx", "ToolButton"],
        toolButtonTheme:  ["game-ui/game/components/tool-options/tool-button/tool-button.module.scss", "classes"],
        FOCUS_DISABLED:   ["game-ui/common/focus/focus-key.ts", "FOCUS_DISABLED"],
        ToggleSwitch:     ["game-ui/common/input/toggle-switch/toggle-switch.tsx", "ToggleSwitch"],
    };

    constructor(registry: ModuleRegistry) {
        this.registryData = registry;
    }

    updateCache(key: string): any {
        const [module, exportName] = VanillaComponentResolver.registryMap[key];
        return (this.cachedData[key] = (this.registryData.registry.get(module) as any)[exportName]);
    }

    get Section(): any    { return this.cachedData["Section"]         ?? this.updateCache("Section"); }
    get ToolButton(): any { return this.cachedData["ToolButton"]      ?? this.updateCache("ToolButton"); }
    get toolButtonTheme(): any { return this.cachedData["toolButtonTheme"] ?? this.updateCache("toolButtonTheme"); }
    get FOCUS_DISABLED(): any  { return this.cachedData["FOCUS_DISABLED"]  ?? this.updateCache("FOCUS_DISABLED"); }
    get ToggleSwitch(): any    { return this.cachedData["ToggleSwitch"]    ?? this.updateCache("ToggleSwitch"); }
}

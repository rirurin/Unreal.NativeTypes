using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using System.Diagnostics;
using Unreal.NativeTypes.Configuration;
using Unreal.NativeTypes.Interfaces;
using Unreal.NativeTypes.Template;

namespace Unreal.NativeTypes
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public class Mod : ModBase, IExports // <= Do not Remove.
    {
        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private readonly IModLoader _modLoader;

        /// <summary>
        /// Provides access to the Reloaded.Hooks API.
        /// </summary>
        /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
        private readonly IReloadedHooks? _hooks;

        /// <summary>
        /// Provides access to the Reloaded logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Entry point into the mod, instance that created this class.
        /// </summary>
        private readonly IMod _owner;

        /// <summary>
        /// Provides access to this mod's configuration.
        /// </summary>
        private Config _configuration;

        /// <summary>
        /// The configuration of the currently executing mod.
        /// </summary>
        private readonly IModConfig _modConfig;
        private MemoryMethods _memoryMethods;
        private long baseAddress;

        public Mod(ModContext context)
        {
            _modLoader = context.ModLoader;
            _hooks = context.Hooks;
            _logger = context.Logger;
            _owner = context.Owner;
            _configuration = context.Configuration;
            _modConfig = context.ModConfig;

            var mainModule = Process.GetCurrentProcess().MainModule;
            if (mainModule == null) throw new Exception($"[{_modConfig.ModName}] Could not get main module");
            baseAddress = mainModule.BaseAddress;
            _modLoader.GetController<IStartupScanner>().TryGetTarget(out var startupScanner);
            _modLoader.GetController<ISharedScans>().TryGetTarget(out var sharedScans);
            if (_hooks == null) throw new Exception($"[{_modConfig.ModName}] Could not get Reloaded hooks");
            if (sharedScans == null) throw new Exception($"[{_modConfig.ModName}] Could not get controller for Shared Scans");
            if (startupScanner == null) throw new Exception($"[{_modConfig.ModName}] Could not get controller for Startup Scanner");
            _memoryMethods = new(startupScanner, baseAddress, _logger, _hooks, _modConfig.ModName);
            _modLoader.AddOrReplaceController<IMemoryMethods>(_owner, _memoryMethods);
        }

        #region Standard Overrides
        public override void ConfigurationUpdated(Config configuration)
        {
            // Apply settings from configuration.
            // ... your code here.
            _configuration = configuration;
            _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
        }
        #endregion

        #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Mod() { }
#pragma warning restore CS8618
        #endregion

        public Type[] GetTypes() => new[] { typeof(IMemoryMethods) };
    }
}
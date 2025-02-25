using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

internal sealed class WireConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;
    public static WireConfig Instance;

    [Label("Parallel thread counts")]
    [Tooltip("Set to 1 if there are bugs, higher value requests more memory when preprocessing. Only Avaliable when No Order in Wires is set to false.")]
    [DefaultValue(1)]
    public int ThreadCount { get; set; }

    [Label("No Order in Wires")]
    [Tooltip("If the order on the same wire counts, enable it will increase preprocessing speed, but the circuit may be slightly different from vanilla logic.")]
    [DefaultValue(true)]
    public bool NoWireOrder { get; set; }

    [Label("Enable AOT Mode")]
    [Tooltip("When enabled, the game will do additional caching to generate an impl.cpp file for AOT mode. This process is slow and not required for JIT mode.")]
    [DefaultValue(false)]
    public bool EnableCodeEmit { get; set; }

    public override void OnLoaded()
    {
        Instance = this;
    }

    public override void OnChanged()
    {
        WireShark.WireAccelerator.threadCount = NoWireOrder ? 1 : ThreadCount;
        WireShark.WireAccelerator.noWireOrder = NoWireOrder;
    }
}

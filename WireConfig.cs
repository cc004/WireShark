using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace WireShark
{
    public abstract class WireConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Label("Use Vanilla Pixel Box Logic")]
        [Tooltip("Pixel box will act as vanilla terraria and its pass will be run once a gate is done.")]
        [DefaultValue(true)]
        public bool UseVanillaPixelBoxLogic { get; set; }

        [Label("Parallel thread counts")]
        [Tooltip("Set to 1 if there are bugs, higher value requests more memory when preprocessing. Only Avaliable when No Order in Wires is set to false")]
        [DefaultValue(1)]
        public int ThreadCount { get; set; }


        [Label("No Order in Wires")]
        [Tooltip("If the order on the same wire counts, enable it will increase preprocessing speed, but the circuit may be slightly different from vanilla logic")]
        [DefaultValue(true)]
        public bool NoWireOrder { get; set; }

        public override void OnChanged()
        {
            WiringWrapper.SetPixelBoxBehaviour(UseVanillaPixelBoxLogic);
            WireAccelerator.threadCount = NoWireOrder ? 1 : ThreadCount;
            WireAccelerator.noWireOrder = NoWireOrder;
        }
    }
}

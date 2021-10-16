using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WireShark {
    public class MyModPlayer : ModPlayer {

        public override void OnEnterWorld(Player player) {
            WiringWrapper._wireAccelerator.Preprocess();
            WiringWrapper.Initialize_GatesDone();
            WiringWrapper.Initialize_LogicLamps();
        }
    }
}

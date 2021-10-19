using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WireShark {
    public class MyModSystem : ModSystem {
        public override void OnWorldLoad()
        {
            WiringWrapper._wireAccelerator.Preprocess();
            WiringWrapper.Initialize_GatesDone();
            WiringWrapper.Initialize_LogicLamps();
        }
    }
}

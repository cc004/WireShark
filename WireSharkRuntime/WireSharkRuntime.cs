using Terraria.ModLoader;
using Terraria.IO;
using WireShark;
using Terraria.GameInput;

namespace WireSharkRuntime {
    public class WireSharkRuntime : Mod {

        private static void Preprocess()
        {
            WireAccelerator.Preprocess();
        }

        public override void Load()
        {
            WorldFile.OnWorldLoad += Preprocess;
            Terraria.On_Wiring.HitSwitch += Wiring_HitSwitch;
            Terraria.On_Wiring.SkipWire_int_int += Wiring_SkipWire_int_int;
            Terraria.On_Wiring.SkipWire_Point16 += Wiring_SkipWire_Point16;
            Terraria.On_Wiring.TripWire += Wiring_TripWire;
            Terraria.On_WorldGen.StartRoomCheck += WorldGen_StartRoomCheck;

            DLLManager.Load();
        }

        private bool WorldGen_StartRoomCheck(Terraria.On_WorldGen.orig_StartRoomCheck orig, int x, int y) {
            return false;
        }

        private void Wiring_SkipWire_Point16(Terraria.On_Wiring.orig_SkipWire_Point16 orig, Terraria.DataStructures.Point16 point) {
            
        }

        private void Wiring_SkipWire_int_int(Terraria.On_Wiring.orig_SkipWire_int_int orig, int x, int y) {
            
        }

        private void Wiring_HitSwitch(Terraria.On_Wiring.orig_HitSwitch orig, int i, int j) {
            WiringWrapper.HitSwitch(i, j);
        }

        private void Wiring_TripWire(Terraria.On_Wiring.orig_TripWire orig, int left, int top, int width, int height) {
            WiringWrapper.BigTripWire(left, top, width, height);
        }


        public override void Unload()
        {
            WorldFile.OnWorldLoad -= Preprocess;
            Terraria.On_Wiring.HitSwitch -= Wiring_HitSwitch;
            Terraria.On_Wiring.SkipWire_int_int -= Wiring_SkipWire_int_int;
            Terraria.On_Wiring.SkipWire_Point16 -= Wiring_SkipWire_Point16;
            Terraria.On_Wiring.TripWire -= Wiring_TripWire;
            Terraria.On_WorldGen.StartRoomCheck -= WorldGen_StartRoomCheck;
        }
    }
}

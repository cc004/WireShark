using Terraria;
using Terraria.ModLoader;
using Terraria.IO;

namespace WireShark {
    public class WireShark : Mod {

        private static void Preprocess()
        {
            WiringWrapper.Initialize();
            WiringWrapper._wireAccelerator.Preprocess();
            WiringWrapper.Initialize_GatesDone();
            WiringWrapper.Initialize_LogicLamps();
        }
        public override void Load()
        {
            WorldFile.OnWorldLoad += Preprocess;
            Terraria.On_Wiring.Actuate += Wiring_Actuate;
            Terraria.On_Wiring.ActuateForced += Wiring_ActuateForced;
            Terraria.On_Wiring.CheckMech += Wiring_CheckMech;
            Terraria.On_Wiring.DeActive += Wiring_DeActive;
            Terraria.On_Wiring.HitSwitch += Wiring_HitSwitch;
            Terraria.On_Wiring.Initialize += Wiring_Initialize;
            Terraria.On_Wiring.PokeLogicGate += Wiring_PokeLogicGate;
            Terraria.On_Wiring.ReActive += Wiring_ReActive;
            Terraria.On_Wiring.SetCurrentUser += Wiring_SetCurrentUser;
            Terraria.On_Wiring.SkipWire_int_int += Wiring_SkipWire_int_int;
            Terraria.On_Wiring.SkipWire_Point16 += Wiring_SkipWire_Point16;
            Terraria.On_Wiring.TripWire += Wiring_TripWire;
            Terraria.On_Wiring.UpdateMech += Wiring_UpdateMech;
            Terraria.On_WorldGen.StartRoomCheck += WorldGen_StartRoomCheck;
        }

        private bool WorldGen_StartRoomCheck(Terraria.On_WorldGen.orig_StartRoomCheck orig, int x, int y) {
            return false;
        }

        private void Wiring_UpdateMech(Terraria.On_Wiring.orig_UpdateMech orig) {
            WiringWrapper.UpdateMech();
        }

        private void Wiring_SkipWire_Point16(Terraria.On_Wiring.orig_SkipWire_Point16 orig, Terraria.DataStructures.Point16 point) {
            
        }

        private void Wiring_SkipWire_int_int(Terraria.On_Wiring.orig_SkipWire_int_int orig, int x, int y) {
            
        }

        private void Wiring_SetCurrentUser(Terraria.On_Wiring.orig_SetCurrentUser orig, int plr) {
            WiringWrapper.SetCurrentUser(plr);
        }

        private void Wiring_ReActive(Terraria.On_Wiring.orig_ReActive orig, int i, int j) {
            WiringWrapper.ReActive(i, j);
        }

        private void Wiring_PokeLogicGate(Terraria.On_Wiring.orig_PokeLogicGate orig, int lampX, int lampY) {
            WiringWrapper.PokeLogicGate(lampX, lampY);
        }
        
        private void Wiring_Initialize(Terraria.On_Wiring.orig_Initialize orig) {
            WiringWrapper.Initialize();
        }


        private void Wiring_HitSwitch(Terraria.On_Wiring.orig_HitSwitch orig, int i, int j) {
            WiringWrapper.HitSwitch(i, j);
        }

        private void Wiring_DeActive(Terraria.On_Wiring.orig_DeActive orig, int i, int j) {
            WiringWrapper.DeActive(i, j);
        }

        private bool Wiring_CheckMech(Terraria.On_Wiring.orig_CheckMech orig, int i, int j, int time) {
            return WiringWrapper.CheckMech(i, j, time);
        }

        private void Wiring_ActuateForced(Terraria.On_Wiring.orig_ActuateForced orig, int i, int j) {
            WiringWrapper.ActuateForced(i, j);
        }

        private bool Wiring_Actuate(Terraria.On_Wiring.orig_Actuate orig, int i, int j) {
            return WiringWrapper.Actuate(i, j);
        }


        private void Wiring_TripWire(Terraria.On_Wiring.orig_TripWire orig, int left, int top, int width, int height) {
            WiringWrapper.BigTripWire(left, top, width, height);
        }


        public override void Unload()
        {
            WorldFile.OnWorldLoad -= Preprocess;
            Terraria.On_Wiring.Actuate -= Wiring_Actuate;
            Terraria.On_Wiring.ActuateForced -= Wiring_ActuateForced;
            Terraria.On_Wiring.CheckMech -= Wiring_CheckMech;
            Terraria.On_Wiring.DeActive -= Wiring_DeActive;
            Terraria.On_Wiring.HitSwitch -= Wiring_HitSwitch;
            Terraria.On_Wiring.Initialize -= Wiring_Initialize;
            Terraria.On_Wiring.PokeLogicGate -= Wiring_PokeLogicGate;
            Terraria.On_Wiring.ReActive -= Wiring_ReActive;
            Terraria.On_Wiring.SetCurrentUser -= Wiring_SetCurrentUser;
            Terraria.On_Wiring.SkipWire_int_int -= Wiring_SkipWire_int_int;
            Terraria.On_Wiring.SkipWire_Point16 -= Wiring_SkipWire_Point16;
            Terraria.On_Wiring.TripWire -= Wiring_TripWire;
            Terraria.On_Wiring.UpdateMech -= Wiring_UpdateMech;
            Terraria.On_WorldGen.StartRoomCheck -= WorldGen_StartRoomCheck;
        }
    }
}

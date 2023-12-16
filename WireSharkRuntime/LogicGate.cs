using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ModLoader;
using WireShark.Tiles;

namespace WireShark
{
    //just a ref for state
    public class PixelBox
    {
        [Flags]
        public enum PixelBoxState
        {
            None = 0,
            Vertical = 1,
            Horizontal = 2
        }

        public PixelBoxState state;
        public Tile tile;
        public int x, y;
    }

    internal abstract unsafe class LogicGate
    {
        public int x, y;
        public Tile mapTile;
        public int lamptotal;
        public int* lampon = (int*) Marshal.AllocHGlobal(sizeof(int));
        public bool erroronly = false;

        public abstract void UpdateLogicGate();

        ~LogicGate()
        {
            Marshal.FreeHGlobal((IntPtr) lampon);
        }
    }
}

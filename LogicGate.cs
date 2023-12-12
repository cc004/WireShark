using System;
using System.Runtime.CompilerServices;
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

    internal abstract class PixelBoxBase : TileInfo
    {
        protected PixelBox box;
        protected PixelBoxBase(PixelBox box, int i, int j)
        {
            this.box = box;
            this.tile = box.tile;
            this.i = i;
            this.j = j;
            this.hash = ((long) i << 32) + j;
        }
        protected override void HitWireInternal()
        {
            if (box.state == PixelBox.PixelBoxState.None)
                WireAccelerator._refreshedBoxes[WireAccelerator.boxCount++] = box;
        }
    }

    internal class PixelBoxVertical : PixelBoxBase
    {
        protected override void HitWireInternal()
        {
            base.HitWireInternal();
            box.state |= PixelBox.PixelBoxState.Vertical;
        }

        public PixelBoxVertical(PixelBox box, int i, int j) : base(box, i, j)
        {
        }
    }

    internal class PixelBoxHorizontal : PixelBoxBase
    {
        protected override void HitWireInternal()
        {
            base.HitWireInternal();
            box.state |= PixelBox.PixelBoxState.Horizontal;
        }

        public PixelBoxHorizontal(PixelBox box, int i, int j) : base(box, i, j)
        {
        }
    }

    internal abstract class LogicGate
    {
        public int lampon, x, y;
        public Tile mapTile;
        public int lamptotal;
        public bool erroronly = false;

        public abstract void UpdateLogicGate();
    }
}

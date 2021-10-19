﻿using System;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ModLoader;

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
        }
    }

    internal class PixelBoxVertical : PixelBoxBase
    {
        protected override void HitWireInternal()
        {
            box.state |= PixelBox.PixelBoxState.Vertical;
            if (box.x == Items.Test.x && box.y == Items.Test.y) Main.NewText($"pixel box set vertical => {box.state}, triggered by={WireAccelerator.triggeredBy}");
        }

        public PixelBoxVertical(PixelBox box, int i, int j) : base(box, i, j)
        {
        }
    }

    internal class PixelBoxHorizontal : PixelBoxBase
    {
        protected override void HitWireInternal()
        {
            box.state |= PixelBox.PixelBoxState.Horizontal;
            if (box.x == Items.Test.x && box.y == Items.Test.y) Main.NewText($"pixel box set horizontal => {box.state}, triggered by={WireAccelerator.triggeredBy}");
        }

        public PixelBoxHorizontal(PixelBox box, int i, int j) : base(box, i, j)
        {
        }
    }

    internal abstract class LogicGate
    {
        public int lampon, x, y;
        public bool active;
        public Tile mapTile;
        public int lamptotal;
        public bool erroronly = false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract bool GetState();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void UpdateLogicGate()
        {
            bool cur = GetState();
            //Main.NewText($"update {GetType().Name} => {active} to {cur}, {lampon} / {lamptotal} @({x}, {y})");
            if (active ^ cur)
            {
                active = cur;
                mapTile.frameX = (short)(cur ? 18 : 0);
                if (WiringWrapper._GatesDone[x, y] != WiringWrapper.cur_gatesdone) WiringWrapper._GatesNext.Enqueue(new Point16(x, y));
            }
        }
    }
}

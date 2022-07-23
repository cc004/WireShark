using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SteelSeries.GameSense.DeviceZone;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace WireShark
{
    public abstract class TileInfo
    {
        private class ModTileInfo : TileInfo
        {
            private readonly ModTile _modTile;
            public ModTileInfo(ModTile tile)
            {
                _modTile = tile;
            }

            protected override void HitWireInternal()
            {
                _modTile.HitWire(i, j);
            }
        }

        private class ActuatorTile : TileInfo
        {
            protected override void HitWireInternal()
            {
            }
        }

        public override string ToString()
        {
            return $"{tile}@({i},{j})";
        }

        public int i, j;

        public ushort type 
        { 
            get => tile.TileType; 
            set => tile.TileType = value; 
        }

        public Tile tile;

        private static Dictionary<int, Type> tileinfo = new Dictionary<int, Type>();


        static TileInfo()
        {
            foreach (var type in typeof(TileInfo).Assembly.GetTypes())
            {
                if (!typeof(TileInfo).IsAssignableFrom(type) || type.IsAbstract) continue;
                var name = type.Name;
                if (int.TryParse(name.Substring(4), out var id))
                    tileinfo.Add(id, type);
            }
        }
        
        public static TileInfo CreateTileInfo(int x, int y)
        {
            TileInfo result;
            var mtile = ModContent.GetModTile(Main.tile[x, y].TileType);
            if (mtile != null)
            {
                result = new ModTileInfo(mtile);
            }
            else if (tileinfo.TryGetValue(Main.tile[x, y].TileType, out var t))
            {
                result = Activator.CreateInstance(t) as TileInfo;
            }
            else
            {
                result = new ActuatorTile();
            }

            result.tile = Main.tile[x, y];
            result.i = x;
            result.j = y;
            result.type = result.tile.TileType;
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void HitWireInternal();

        [MethodImpl]

        public void HitWire()
        {
            if (tile.HasActuator)
                WiringWrapper.ActuateForced(i, j);
            //if (!TileLoader.PreHitWire(i, j, type)) return;
            HitWireInternal();
            //TileLoader.HitWire(i, j, type);
        }
    }
}
/*
if (TileLoader.CloseDoorID(Main.tile[i, j]) >= 0)
{
    if (WorldGen.CloseDoor(i, j, true)) return;
}
else if (TileLoader.OpenDoorID(Main.tile[i, j]) >= 0)
{
    int num66 = 1;
    if (Main.rand.Next(2) == 0)
    {
        num66 = -1;
    }

    if (WorldGen.OpenDoor(i, j, num66))
    {
        return;
    }

    if (WorldGen.OpenDoor(i, j, -num66))
    {
        return;
    }
}
else if (TileLoader.IsTorch(type))
{
    if (tile.frameX < 66)
    {
        Tile tile4 = tile;
        tile4.frameX += 66;
    }
    else
    {
        Tile tile5 = tile;
        tile5.frameX -= 66;
    }

    return;
}
else if (TileLoader.IsModMusicBox(tile))
{
    WorldGen.SwitchMB(i, j);
    return;
}
*/

using System;
using System.Runtime.CompilerServices;

namespace WireShark.Tiles;

public class Tile419Error : Tile419
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override void HitWireInternal()
    {
        WiringWrapper._LampsToCheck.AddLast(lgate);
    }
}
public class Tile419Normal : Tile419
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override void HitWireInternal()
    {
        var frame = (short)(18 - tile.TileFrameX);
        lgate.lampon += frame == 18 ? 1 : -1;
        WiringWrapper._LampsToCheck.AddLast(lgate);
        tile.TileFrameX = frame;
    }
}
public class Tile419NormalOnError : Tile419
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override void HitWireInternal()
    {
        var frame = (short) (18 - tile.TileFrameX);
        lgate.lampon += frame == 18 ? 1 : -1;
        tile.TileFrameX = frame;
    }
}
public class Tile419NormalUnconnected : Tile419
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override void HitWireInternal()
    {
        var frame = (short)(18 - tile.TileFrameX);
        tile.TileFrameX = frame;
    }
}
public class Tile419ErrorUnconnected : Tile419
{
    protected override void HitWireInternal()
    {
    }
}

public class Tile419 : TileInfo
{
    internal LogicGate lgate;
    protected override void HitWireInternal()
    {
        throw new NotImplementedException();
    }
}
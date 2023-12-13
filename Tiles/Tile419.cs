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
public unsafe class Tile419Normal : Tile419
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override void HitWireInternal()
    {
        //lgate.lampon += (add = -add);
        //WiringWrapper._LampsToCheck.AddLast(lgate);
        
        var frame = (short)(18 - *frameX);
        lgate.lampon += frame == 18 ? 1 : -1;
        WiringWrapper._LampsToCheck.AddLast(lgate);
        *frameX = frame;
    }
}
public unsafe class Tile419NormalOnError : Tile419
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override void HitWireInternal()
    {
        //lgate.lampon += (add = -add);
        
        var frame = (short) (18 - *frameX);
        lgate.lampon += frame == 18 ? 1 : -1;
        *frameX = frame;
    }
}
public unsafe class Tile419NormalUnconnected : Tile419
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override void HitWireInternal()
    {
        /*
        var frame = (short)(18 - *frameX);
        *frameX = frame;*/
    }
}
public class Tile419ErrorUnconnected : Tile419
{
    protected override void HitWireInternal()
    {
    }
}

public unsafe class Tile419 : TileInfo
{
    internal LogicGate lgate;
    internal short* frameX;
    internal int add;
    protected override void HitWireInternal()
    {
        throw new NotImplementedException();
    }
}
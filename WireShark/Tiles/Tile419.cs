using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
        *lampon += add = -add;
        WiringWrapper._LampsToCheck.AddLast(lgate);
    }
}
public unsafe class Tile419NormalOnError : Tile419
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override void HitWireInternal()
    {
        *lampon += add = -add;
    }
}
public unsafe class Tile419NormalOnOneError : Tile419
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override void HitWireInternal()
    {
        *lampon = 1 - *lampon;
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
    // internal short* frameX;
    internal int* lampon;
    internal GCHandle? handle;
    internal int add;
    protected override void HitWireInternal()
    {
        throw new NotImplementedException();
    }

    ~Tile419()
    {
        handle?.Free();
    }
}
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.ModLoader;

namespace WireSharkRuntime;

public static unsafe class DLLManager
{
    private const string libPath = "libWireSharkLib.dll";

    [StructLayout(LayoutKind.Sequential)]
    private struct PixelBox
    {
        public int state, x, y;
        public short* TileFrameX;

        public override string ToString()
        {
            return $"pixelBox(x: {x}, y: {y}, state: {state})";
        }
    }

    [DllImport(libPath)]
    public static extern void BigTripWire(int x, int y, int w, int h);

    [DllImport(libPath)]
    private static extern PixelBox* GetPixelBoxPointer();
    [DllImport(libPath)]
    private static extern int GetPixelBoxCount();

    [StructLayout(LayoutKind.Sequential)]
    private struct UpdateInfo
    {
        public float* teleports;
        public int teleport_len;
        public int* triggers;
        public int trigger_len;
    };

    [DllImport(libPath)]
    private static extern void RetrieveUpdates(UpdateInfo* info);

    public static void Load()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(),
            (lib, _, _) =>
                lib == libPath
                    ? NativeLibrary.Load(Path.GetFullPath(Path.Combine(ModLoader.ModPath, libPath)))
                    : IntPtr.Zero);

        var count = GetPixelBoxCount();
        var ptr = GetPixelBoxPointer();

        for (var i = 0; i < count; i++)
        {
            var tile = Main.tile[ptr[i].x, ptr[i].y];
            ptr[i].TileFrameX = (short *) Unsafe.AsPointer(ref tile.TileFrameX);
        }
    }

    public static void PostUpdate()
    {
        UpdateInfo info;
        RetrieveUpdates(&info);

        for (var i = 0; i < info.trigger_len; ++i)
        {
            var x = info.triggers[2 * i];
            var y = info.triggers[2 * i + 1];

            WireAccelerator._tileCache[x, y].HitWire();
        }
    }
}
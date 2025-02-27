using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using WireShark.Tiles;
using static System.Net.WebRequestMethods;
using static System.Threading.Thread;
using static WireShark.WiringWrapper;

namespace WireShark
{
    public static unsafe class WireAccelerator
    {
        private static readonly HashSet<int> _sourceTable = new HashSet<int>()
        {
            135, 314, 428, 442, 440, 136, 144, 441, 468, 132, 411, TileID.LogicGate, TileID.LogicSensor
        };

        private struct Node
        {
            public int X, Y;
            public int Dir;

            public Node(int x, int y, int dir)
            {
                X = x;
                Y = y;
                Dir = dir;
            }
        };

        // D, U, R, L
        private static readonly int[] dx = { 0, 0, 1, -1 };
        private static readonly int[] dy = { 1, -1, 0, 0 };
        public static TileInfo[][] _connectionInfos;
        private static int[,,] _inputConnectedCompoents;
        private static ConnectionQ[,,] _inputConnectedCompoentsQ;

        private static byte GetWireID(int X, int Y)
        {
            var tile = Main.tile[X, Y];
            if (tile == null) return 0;
            byte mask = 0;
            if (tile.BlueWire) mask |= 1;
            if (tile.GreenWire) mask |= 2;
            if (tile.RedWire) mask |= 4;
            if (tile.YellowWire) mask |= 8;
            return mask;
        }

        private static ConnectionQ now_number;

        static WireAccelerator()
        {
            now_number = new ConnectionQ
            {
                arr = Array.Empty<TileInfo>(),
                skipIndex = 0
            };
        }

        // internal static Point16 triggeredBy;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void Activate(int x, int y, int wire)
        {
            var info = _inputConnectedCompoentsQ[x, y, wire];

            var k = 0;
            for (; k < info.skipIndex; ++k)
                info.arr[k].HitWire();
            for (++k; k < info.arr.Length; ++k)
                info.arr[k].HitWire();
        }

        private static ConcurrentQueue<Ref<int[,,,]>> disposing = new();

        [ThreadStatic] private static Ref<int[,,,]> __vis;

        private static int[,,,] _vis
        {
            get
            {
                if (__vis?.Value == null)
                {
                    var ___vis = new int[Main.maxTilesX, Main.maxTilesY, 4, 3];
                    for (var i = 0; i < Main.maxTilesX; i++)
                    {
                        for (var j = 0; j < Main.maxTilesY; j++)
                        {
                            for (var k = 0; k < 4; k++)
                            {
                                ___vis[i, j, k, 0] = -1;
                                ___vis[i, j, k, 1] = -1;
                                ___vis[i, j, k, 2] = -1;
                            }
                        }
                    }

                    __vis = new Ref<int[,,,]>(___vis);
                    disposing.Enqueue(__vis);
                }

                return __vis.Value;
            }
        }

        private static int[,,] _visIndexCache;
        private static byte[,] _wireCache;
        private static TileInfo[,] _tileCache;
        internal static bool noWireOrder = true;

        public static void Preprocess()
        {
            _inputConnectedCompoents = new int[Main.maxTilesX, Main.maxTilesY, 4];
            _inputConnectedCompoentsQ = new ConnectionQ[Main.maxTilesX, Main.maxTilesY, 4];
            _boxes = new();
            _pixelBoxMap = new();
            _visIndexCache = new int[Main.maxTilesX, Main.maxTilesY, 4];
            _tileCache = new TileInfo[Main.maxTilesX, Main.maxTilesY];
            _wireCache = new byte[Main.maxTilesX, Main.maxTilesY];

            for (var i = 0; i < Main.maxTilesX; i++)
            {
                for (var j = 0; j < Main.maxTilesY; j++)
                {
                    _wireCache[i, j] = GetWireID(i, j);
                    for (var k = 0; k < 4; k++)
                    {
                        _inputConnectedCompoents[i, j, k] = -1;

                        var curTile = Main.tile[i, j];
                        var dir = k;
                        if (curTile != null)
                        {
                            if (curTile.TileType == TileID.WirePipe)
                            {
                                var s = GetWireBoxIndex(curTile, dir);
                                _visIndexCache[i, j, k] = s;
                            }
                            else if (curTile.TileType == TileID.PixelBox)
                            {
                                _visIndexCache[i, j, k] = dir / 2;
                            }
                            else
                            {
                                _visIndexCache[i, j, k] = 0;
                            }
                        }
                    }
                }

                if (i % 100 == 0) Main.statusText = $"preprocess initializing {i * 1f / Main.maxTilesX:P1}";
            }

            var count = 0;

            var tasks = new List<(int, int, int, int)>();

            for (var j = 0; j < Main.maxTilesY; j++)
            {
                for (var i = 0; i < Main.maxTilesX; i++)
                {
                    if (Main.tile[i, j] != null)
                    {
                        if (!_sourceTable.Contains(Main.tile[i, j].TileType)) continue;
                        int wireid = _wireCache[i, j];
                        if (wireid == 0) continue;
                        for (var k = 0; k < 4; k++)
                        {
                            if (((wireid >> k) & 1) == 0) continue;
                            _inputConnectedCompoents[i, j, k] = count;
                            tasks.Add((count++, k, i, j));
                            //var info = BFSWires(_connectionInfos.Count, k, i, j);
                            //_inputConnectedCompoents[i, j, k] = _connectionInfos.Count;
                            //_connectionInfos.Add(info);
                        }

                    }
                }

                if (j % 100 == 0) Main.statusText = $"generating tasks {j * 1f / Main.maxTilesY:P1}";
            }

            _connectionInfos = new TileInfo[count][];
            int finished = 0, total = tasks.Count;
            new Thread(() =>
            {
                while (finished != total)
                {
                    if (finished > 0)
                        Main.statusText = $"preprocessing circuit {finished * 1f / total:P1}";
                    Sleep(100);
                }
            }).Start();
            tasks.AsParallel().WithDegreeOfParallelism(threadCount).ForAll(task =>
            {
                if (noWireOrder && _vis[task.Item3, task.Item4, task.Item2, 0] != -1)
                    _connectionInfos[task.Item1] = _connectionInfos[_vis[task.Item3, task.Item4, task.Item2, 0]];
                else
                    _connectionInfos[task.Item1] = BFSWires(_vis, task.Item1, task.Item2, task.Item3, task.Item4);
                Interlocked.Increment(ref finished);
            });

            _tileCache = null;
            _pixelBoxMap = null;

            foreach (var r in disposing)
                r.Value = null;
            disposing.Clear();
            _refreshedBoxes = new PixelBox[_boxes.Count];
            boxCount = 0;
            _boxes = null;

        }

        private class ConnectionQ
        {
            public TileInfo[] arr;
            public int skipIndex;
        }


        private static bool IsAppliance(int i, int j)
        {
            var tile = Main.tile[i, j];
            var type = (int)tile.TileType;
            if (ModContent.GetModTile(type) != null)
                return true;
            if (tile.HasActuator) return true;
            if (!tile.HasTile) return false;
            switch (type)
            {
                case 144:
                case 421 when !tile.HasActuator:
                case 422 when !tile.HasActuator:
                case >= 255 and <= 268 when !tile.HasActuator:
                case 419:
                case 406:
                case 452:
                case 411:
                case 425:
                case 405:
                case 209:
                case 212:
                case 215:
                case 130:
                case 131:
                case 387:
                case 386:
                case 389:
                case 388:
                case 11:
                case 10:
                case 216:
                case 497:
                case 15 when tile.TileFrameY / 40 == 1:
                case 15 when tile.TileFrameY / 40 == 20:
                case 335:
                case 338:
                case 235:
                case 4:
                case 429:
                case 149:
                case 244:
                case 565:
                case 42:
                case 93:
                case 126:
                case 95:
                case 100:
                case 173:
                case 564:
                case 593:
                case 594:
                case 34:
                case 314:
                case 33:
                case 174:
                case 49:
                case 372:
                case 92:
                case 137:
                case 443:
                case 531:
                case 139:
                case 35:
                case 207:
                case 410:
                case 480:
                case 509:
                case 455:
                case 141:
                case 210:
                case 142:
                case 143:
                case 105:
                case 349:
                case 506:
                case 546:
                case 557:
                    return true;
            }

            return false;
        }

        private static int GetWireBoxIndex2(Tile tile, int dir, int i)
        {
            var frame = tile.TileFrameX / 18;
            if (frame == 0)
            {
                if (i != dir) return 0;
                if (dir == 0 || dir == 1) return 1;
                else return 2;
            }
            else if (frame == 1)
            {
                if ((dir == 0 && i != 3) || (dir == 3 && i != 0) || (dir == 1 && i != 2) || (dir == 2 && i != 1))
                {
                    return 0;
                }

                if (dir == 0 || dir == 3) return 1;
                else return 2;
            }
            else
            {
                if ((dir == 0 && i != 2) || (dir == 2 && i != 0) || (dir == 1 && i != 3) || (dir == 3 && i != 1))
                {
                    return 0;
                }

                if (dir == 0 || dir == 3) return 1;
                else return 2;
            }
        }

        private static int GetWireBoxIndex(Tile tile, int dir)
        {
            var frame = tile.TileFrameX / 18;
            if (frame == 0)
            {
                if (dir == 0 || dir == 1) return 1;
                else return 2;
            }
            else if (frame == 1)
            {
                if (dir == 0 || dir == 2) return 1;
                else return 2;
            }
            else
            {
                if (dir == 0 || dir == 3) return 1;
                else return 2;
            }
        }

        private static List<PixelBox> _boxes;
        public static PixelBox[] _refreshedBoxes;
        public static int boxCount = 0;
        private static Dictionary<Point16, PixelBox> _pixelBoxMap;
        internal static int threadCount = 1;

        private static TileInfo[] BFSWires(int[,,,] _vis, int id, int wireid, int x, int y)
        {
            var Q = new Queue<Node>();
            Q.Enqueue(new Node(x, y, 0));
            var outputs = new List<TileInfo>();
            var wirebit = 1 << wireid;
            while (Q.Count > 0)
            {
                var node = Q.Dequeue();
                //if (node.X == 2129 && node.Y == 282) Debugger.Break();
                // 到达当前点使用的是哪个方向 | 
                var dir = node.Dir;
                var curTile = Main.tile[node.X, node.Y];
                var index = _visIndexCache[node.X, node.Y, dir];

                try
                {
                    if (_vis[node.X, node.Y, wireid, index] == id) continue;
                    _vis[node.X, node.Y, wireid, index] = id;
                }
                catch (Exception e)
                {
                    throw new Exception($"{node.X}, {node.Y}, {wireid}, {index}");
                }

                var pt = new Point16(node.X, node.Y);

                if (curTile.HasTile)
                {
                    if (Main.tile[node.X, node.Y].TileType == TileID.PixelBox)
                    {
                        if (!_pixelBoxMap.TryGetValue(pt, out var box))
                            _pixelBoxMap.Add(pt, box = new PixelBox()
                            {
                                tile = Main.tile[node.X, node.Y],
                                x = node.X,
                                y = node.Y
                            });
                        _boxes.Add(box);
                        TileInfo tile = dir < 2
                            ? new PixelBoxVertical(box, node.X, node.Y)
                            : new PixelBoxHorizontal(box, node.X, node.Y);
                        outputs.Add(tile);
                        _tileCache[node.X, node.Y] = tile;
                    }
                    else if (_tileCache[node.X, node.Y] != null)
                    {
                        outputs.Add(_tileCache[node.X, node.Y]);
                    }
                    else if (IsAppliance(node.X, node.Y))
                    {
                        var tile = TileInfo.CreateTileInfo(node.X, node.Y);
                        outputs.Add(tile);
                        _tileCache[node.X, node.Y] = tile;
                    }
                }

                for (var i = 0; i < 4; i++)
                {
                    var nx = dx[i] + node.X;
                    var ny = dy[i] + node.Y;
                    if (nx < 2 || nx >= Main.maxTilesX - 2 || ny < 2 || ny >= Main.maxTilesY - 2) continue;
                    var tile = Main.tile[nx, ny];
                    if (tile == null) continue;
                    if (curTile.TileType == TileID.WirePipe)
                    {
                        if (GetWireBoxIndex2(curTile, dir, i) == 0) continue;
                    }
                    else if (curTile.TileType == TileID.PixelBox)
                    {
                        if (dir != i) continue;
                    }

                    if ((_wireCache[nx, ny] & wirebit) != 0)
                    {
                        Q.Enqueue(new Node(nx, ny, i));
                    }
                }
            }

            return outputs.ToArray();
        }

        public static void Postprocess()
        {
            for (var i = 0; i < Main.maxTilesX; i++)
            {
                for (var j = 0; j < Main.maxTilesY; j++)
                {
                    for (var k = 0; k < 4; k++)
                    {
                        if (_inputConnectedCompoents[i, j, k] == -1)
                            _inputConnectedCompoentsQ[i, j, k] = now_number;
                        else
                        {
                            var q = new ConnectionQ
                            {
                                arr = _connectionInfos[_inputConnectedCompoents[i, j, k]]
                            };

                            var t = 0;
                            for (; t < q.arr.Length; t++)
                            {
                                var info = q.arr[t];
                                if (info.i == i && info.j == j) break;
                            }

                            q.skipIndex = t;

                            _inputConnectedCompoentsQ[i, j, k] = q;
                        }
                    }
                }

                if (i % 100 == 0) Main.statusText = $"caching {i * 1f / Main.maxTilesX:P1}";
            }

            // Checks to run AOT mode via ModConfig system.
            if (WireConfig.Instance.EnableCodeEmit)
            {
                using var sw = new StreamWriter(new FileStream(
                Path.Combine(ModLoader.ModPath, "impl.cpp"),
                FileMode.Create, FileAccess.Write));

                CodeEmit(sw);
            }
            // do codegen
            /*
            using var sw = new StreamWriter(new FileStream(
                @"D:\Users\Administrator\Documents\My Games\Terraria\tModLoader\ModSources\WireSharkLib\impl.cpp",
                FileMode.Create, FileAccess.Write));

            sw.WriteLine("""
                         #include <iostream>

                         #include "interface.h"
                         #include "tile.h"
                         #include "logicgate.h"
                         #include "runtime.h"

                         namespace
                         {
                         """);

            var types = new HashSet<string>();

            var added = new Dictionary<TileInfo[], int>();
            var logicGates = new HashSet<Tile419>();

            foreach (var wire in _connectionInfos)
            {
                if (added.ContainsKey(wire)) continue;
                var id = added[wire] = added.Count + 1;

                var tileTypes = new List<string>();
                var tileNames = new List<string>();

                foreach (var tile in wire)
                {
                    var name = $"tile_{tile.i}_{tile.j}";

                    if (types.Contains(name)) continue;
                    types.Add(name);

                    tileNames.Add(name);

                    var type = "invoke_tile";
                    var initializer = $"{{{tile.i}, {tile.j}}}";

                    if (tile is Tile419 t)
                    {
                        if (t.lgate != null)
                        {
                            logicGates.Add(t);
                        }

                        type = t switch
                        {
                            Tile419Normal a => "normal_lamp",
                            Tile419Error a => "error_lamp",
                            Tile419NormalOnError a => "normal_on_error_lamp",
                            Tile419NormalUnconnected a => "unconnected_lamp",
                            Tile419ErrorUnconnected a => "unconnected_lamp",
                            _ => "invoke_tile"
                        };

                        initializer = string.Empty;
                    }
                    else if (tile is WireState ws)
                    {
                        type = "wire_state";
                        initializer = $"{{{(ws.state ? "true" : "false")}}}";
                    }

                    tileTypes.Add(type);
                    sw.WriteLine($"{type} {name}{initializer};");
                }

                var tileType = string.Concat(tileTypes.Select(x => $", {x}"));
                var allLogic = tileTypes.All(x => x != "invoke_tile");
                var tileInst = string.Join(", ", tileNames.Select(n => $"&{n}"));
                sw.WriteLine($"auto wire_{id} = wire<{(allLogic ? "true" : "false")}{tileType}> {{{{{tileInst}}}}};");

            }

            foreach (var t in logicGates)
            {
                var gname = $"gate_{t.lgate.x}_{t.lgate.y}";
                if (types.Contains(gname)) continue;
                types.Add(gname);

                var classname = t.lgate switch
                {
                    AllOnGate => "all_on_gate",
                    AnyOnGate => "any_on_gate",
                    AllOffGate => "all_off_gate",
                    AnyOffGate => "any_off_gate",
                    OneOnGate => "one_on_gate",
                    NotOneOnGate => "not_one_on_gate",
                    ErrorGate => "error_gate",
                    OneErrorGate => "one_error_gate",
                    _ => "unreachable"
                };
                var output = new[]
                    {
                        _inputConnectedCompoents[t.lgate.x, t.lgate.y, 0],
                        _inputConnectedCompoents[t.lgate.x, t.lgate.y, 1],
                        _inputConnectedCompoents[t.lgate.x, t.lgate.y, 2],
                        _inputConnectedCompoents[t.lgate.x, t.lgate.y, 3],
                    }
                    .Where(x => x != -1)
                    .Select(x => $"wire_{added[_connectionInfos[x]]}").ToArray();

                var tileType = string.Join(", ", output.Select(x => $"decltype({x})"));
                var tileInst = string.Join(", ", output.Select(x => $"&{x}"));
                tileInst = $"{{{t.lgate.lampon}, {t.lgate.lamptotal}, {(t.lgate.mapTile.TileFrameX == 18 ? "true" : "false")}, {{{tileInst}}}}}";

                if (t.lgate is OneErrorGate gate)
                {
                    var states = new[]
                    {
                        gate.state1, gate.state2, gate.state3, gate.state4
                    }.Where(s => s != OneErrorGate.alwaysFalse).ToArray();
                    var stateInit = string.Join(", ", states.Select(s => $"&tile_{s.i}_{s.j}"));
                    tileType = $"{states.Length}, {tileType}";
                    tileInst = $"{tileInst}, {(gate.originalState ? "true" : "false")}, {{{stateInit}}}";
                }

                sw.WriteLine($"auto {gname} = {classname}<{tileType}> {{{tileInst}}};");
            }

            sw.WriteLine();

            sw.WriteLine("""
                         }

                         void WireInit()
                         {
                         """);

            foreach (var tile in logicGates)
            {
                var name = $"tile_{tile.i}_{tile.j}";
                var gate = $"gate_{tile.lgate.x}_{tile.lgate.y}";
                var lamp_on = $"&{gate}.lamp_on";
                var checker = $"{{decltype({gate})::check, &{gate}}}";
                switch (tile)
                {
                    case Tile419Normal:
                        sw.WriteLine($"{name} = normal_lamp{{{tile.add}, {lamp_on}, {checker}}};");
                        break;
                    case Tile419Error:
                        sw.WriteLine($"{name} = error_lamp{{{checker}}};");
                        break;
                    case Tile419NormalOnError:
                        sw.WriteLine($"{name} = normal_on_error_lamp{{{tile.add}, {lamp_on}}};");
                        break;
                }
            }

            for (var i = 0; i < Main.maxTilesX; i++)
            {
                for (var j = 0; j < Main.maxTilesY; j++)
                {
                    for (var k = 0; k < 4; ++k)
                    {
                        var id = _inputConnectedCompoents[i, j, k];
                        if (id == -1) continue;

                        var name = $"wire_{added[_connectionInfos[id]]}";
                        sw.WriteLine($"    wires[{i}][{j}][{k}] = {{decltype({name})::wrapper, &{name}}};");
                    }
                }
            }

            sw.WriteLine("""
                         }
                         """);
            */
            _connectionInfos = null;
            _inputConnectedCompoents = null;

        }

        private struct Task : IEquatable<Task>
        {
            public int i, j;
            public TileInfo[] conn;

            public bool Equals(Task other)
            {
                return other.conn.SequenceEqual(conn);
            }

            public override bool Equals(object obj)
            {
                return obj is Task other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return conn.Aggregate(0, (current, t) => current * 31 + t.GetHashCode());
                }
            }
        }

        public static bool IsTrigger(int i, int j)
        {
            var tile = Main.tile[i, j];
            if (ModContent.GetModTile(tile.TileType) != null)
                return true;
            if (!tile.HasTile) return false;
            switch (tile.TileType)
            {
                case TileID.LogicGate:
                case TileID.Lever:
                case TileID.Switches:
                case TileID.PressurePlates:
                case TileID.WeightedPressurePlate:
                case TileID.ProjectilePressurePad:
                case TileID.Timers:
                    return true;
            }

            return false;
        }

        private static void CodeEmit(StreamWriter sw)
        {
            // Write necessary includes.
            sw.WriteLine("""
                         #include "pixel_box.h"
                         #include "logic_gate.h"
                         #include "common.h"
                         #include "interop.h"
                         """);

            // Dictionaries to track unique IDs for various components.
            var gatesId = new Dictionary<LogicGate, int>();  // Maps LogicGate to a unique ID.
            var gates2Id = new Dictionary<LogicGate, int>(); // Maps specialized LogicGate (e.g., OneErrorGate) to unique ID.
            var boxId = new Dictionary<PixelBox, int>();     // Maps PixelBox to a unique ID.
            var funcId = new Dictionary<Task, int>();        // Maps Task (wire connections) to a unique ID.
            var addId = new Dictionary<Tile419, int>();      // Maps Tile419 (tile types) to a unique ID.
            var stateId = new Dictionary<WireState, int>();  // Maps WireState to a unique ID.

            var sb = new StringBuilder();                    // StringBuilder for accumulating function bodies.
            var funcs = new int[8400, 2400];                 // 2D array to store function IDs for tile coordinates.

            // Iterate through all tiles in the map
            for (int i = 0; i < Main.maxTilesX; ++i)
            {
                for (int j = 0; j < Main.maxTilesY; ++j)
                {
                    // Skip if the current tile is not a trigger.
                    if (!IsTrigger(i, j)) continue;

                    var conns = new List<TileInfo[]>(); // Store connections for each tile.

                    // Gather connection info for the current tile.
                    for (int k = 0; k < 4; ++k)
                    {
                        var id = _inputConnectedCompoents[i, j, k];
                        if (id != -1) conns.Add(_connectionInfos[id]);
                    }

                    // Flatten the connections and filter out connections to the current tile.
                    var conn = conns.SelectMany(x => x).Where(t => t.i != i || t.j != j).ToArray();
                    if (conn.Length == 0) continue; // Skip if no connections exist.

                    var wire = new Task
                    {
                        i = i,
                        j = j,
                        conn = conn, // Store connections for the wire task.
                    };

                    try
                    {
                        // Skip if the wire has already been processed.
                        if (funcId.ContainsKey(wire))
                        {
                            continue;
                        }
                        // Assign a unique function ID to the wire.
                        funcId[wire] = funcId.Count;
                    }
                    finally
                    {
                        // Store the function ID in the funcs array.
                        funcs[i, j] = funcId[wire] + 1;
                    }

                    // Append the function definition to the StringBuilder.
                    sb.AppendLine($"static void func_{funcId[wire]}() {{");

                    // Iterate over each connection for the current wire task.
                    foreach (var tile in wire.conn)
                    {
                        switch (tile)
                        {
                            case Tile419 t:
                            {
                                // Handle logic gates (e.g., AND, OR, NOT).
                                if (t.lgate != null)
                                {
                                    if (t.lgate is OneErrorGate)
                                        gates2Id.TryAdd(t.lgate, gates2Id.Count);
                                    else
                                        gatesId.TryAdd(t.lgate, gatesId.Count);
                                }

                                // Track the tile in addId.
                                addId.TryAdd(t, addId.Count);

                                // Determine the gate checker based on the gate type.
                                var gate_checker = t.lgate switch
                                {
                                    AllOnGate => $"all_on_gate_checker<{t.lgate.lamptotal}>",
                                    AllOffGate => "all_off_gate_checker",
                                    AnyOffGate => $"any_off_gate_checker<{t.lgate.lamptotal}>",
                                    AnyOnGate => "any_on_gate_checker",
                                    OneOnGate => "one_on_gate_checker",
                                    NotOneOnGate => "not_one_on_gate_checker",
                                    OneErrorGate => "one_error_gate_checker",
                                    ErrorGate => $"error_gate_checker<{t.lgate.lamptotal}>",
                                    _ => string.Empty
                                };

                                // Handle different tile types and generate corresponding logic.
                                switch (t)
                                {
                                    case Tile419Normal:
                                    {
                                        sb.AppendLine($"""
                                                           gates[{gatesId[t.lgate]}].lamp_on += adds[{addId[t]}] = -adds[{addId[t]}];
                                                           lamps_to_check.push({gate_checker}, &gates[{gatesId[t.lgate]}]);
                                                       """);
                                        break;
                                    }
                                    case Tile419NormalOnError:
                                    {
                                        sb.AppendLine($"""
                                                           gates[{gatesId[t.lgate]}].lamp_on += adds[{addId[t]}] = -adds[{addId[t]}];
                                                       """);
                                        break;
                                    }
                                    case Tile419Error:
                                    {
                                        if (t.lgate is OneErrorGate err)
                                        {
                                            // Determine the number of active states for OneErrorGate.
                                            int stateNum = 0;
                                            if (err.state1 != OneErrorGate.alwaysFalse) ++stateNum;
                                            if (err.state2 != OneErrorGate.alwaysFalse) ++stateNum;
                                            if (err.state3 != OneErrorGate.alwaysFalse) ++stateNum;
                                            if (err.state4 != OneErrorGate.alwaysFalse) ++stateNum;
                                            sb.AppendLine($"""
                                                               lamps_to_check.push({gate_checker}<{stateNum}>, &one_error_gates[{gates2Id[t.lgate]}]);
                                                           """);
                                        }
                                        else
                                        {
                                            sb.AppendLine($"""
                                                               lamps_to_check.push({gate_checker}, &gates[{gatesId[t.lgate]}]);
                                                           """);
                                        }
                                        break;
                                    }
                                    case Tile419NormalOnOneError:
                                    {
                                        sb.AppendLine($"""
                                                           one_error_gates[{gates2Id[t.lgate]}].lamp_on = 1 - one_error_gates[{gates2Id[t.lgate]}].lamp_on;
                                                       """);
                                        break;
                                    }
                                }

                                break;
                            }
                            case PixelBoxBase t:
                            {
                                // Handle PixelBox objects (store in boxId).
                                boxId.TryAdd(t.box, boxId.Count);
                                var state = t is PixelBoxVertical ? 2 : 1;
                                sb.AppendLine($"""
                                                   boxes[{boxId[t.box]}].state |= {state};
                                                   box_to_update.push(&boxes[{boxId[t.box]}]);
                                               """);
                                break;
                            }
                            case WireState t:
                            {
                                // Handle WireState objects (toggle state).
                                stateId.TryAdd(t, stateId.Count);
                                sb.AppendLine($"""
                                                   states[{stateId[t]}] ^= true;
                                               """);
                                break;
                            }
                            default:
                            {
                                // Handle default tile case.
                                sb.AppendLine($"""
                                                   HitTile({tile.i}, {tile.j});
                                               """);
                                break;
                            }
                        }
                    }

                    sb.AppendLine("}"); // Close function body.
                }

                // Display status message.
                if (i % 100 == 0) Main.statusText = $"generating cache data {i * 1f / Main.maxTilesX:P1}";
            }

            // === Write all static arrays first ===
            // Fix: Use ternary operator's for handeling empty count cases.
            var arr = string.Join(", ", gatesId.OrderBy(p => p.Value)
                .Select(pair => $"{{{*pair.Key.lampon}, 0, {(pair.Key.mapTile.TileFrameX == 18 ? "true" : "false")}," +
                                $"{{{pair.Key.x}, {pair.Key.y}, &gates[{pair.Value}].last_active}}}}"));
            sw.WriteLine($"static logic_gate gates[{Math.Max(gatesId.Count, 1)}] = {{{arr}}};");

            arr = string.Join(", ", stateId.OrderBy(p => p.Value)
                .Select(pair => pair.Key.state ? "true" : "false"));
            sw.WriteLine($"static bool states[{Math.Max(stateId.Count, 1)}] = {{{arr}}};");

            stateId[OneErrorGate.alwaysFalse] = -1; // Handle alwaysFalse state.
            arr = string.Join(", ", gates2Id.OrderBy(p => p.Value)
                .Select(pair =>
                {
                    var l = pair.Key as OneErrorGate;
                    return $"{{{{&states[{stateId[l.state1]}], &states[{stateId[l.state2]}], " +
                           $"&states[{stateId[l.state3]}], &states[{stateId[l.state4]}]}}, {(l.originalState ? "true" : "false")}, " +
                           $"{{{pair.Key.x}, {pair.Key.y}, &one_error_gates[{pair.Value}].last_active}}}}";
                }));
            sw.WriteLine($"static one_error_gate one_error_gates[{Math.Max(gates2Id.Count, 1)}] = {{{arr}}};");

            arr = string.Join(", ", boxId.OrderBy(p => p.Value)
                .Select(pair => $"{{0, {pair.Key.x}, {pair.Key.y}, nullptr}}"));
            sw.WriteLine($"static pixel_box boxes[{Math.Max(boxId.Count, 1)}] = {{{arr}}};");

            arr = string.Join(", ", addId.OrderBy(p => p.Value)
                .Select(pair => $"{pair.Key.add}"));
            sw.WriteLine($"static int adds[{Math.Max(addId.Count, 1)}] = {{{arr}}};");

            // Display status message. Do not put this in the loop, will slow the thread.
            Main.statusText = $"saving cache data...";

            // === Now write all function bodies ===
            // Use StringReader to split by lines and avoid spliting by chunks. This is faster and causes no write errors.
            using (StringReader reader = new StringReader(sb.ToString()))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    sw.WriteLine(line);
                }
            }

            // Write connection input initialization.
            arr = string.Join(",", funcs.OfType<int>().Select(x => x == 0 ? "0" : $"func_{x - 1}"));
            sw.WriteLine($"Connection inputConnectedCompoents[maxTilesX][maxTilesY] = {{{arr}}};");

            // Write GetPixelBoxPointer and GetPixelBoxCount methods.
            sw.WriteLine($$"""
                           pixel_box* GetPixelBoxPointer()
                           {
                               return boxes;
                           }
                           int GetPixelBoxCount()
                           {
                               return {{boxId.Count}};
                           }
                           """);
        }
    }
}

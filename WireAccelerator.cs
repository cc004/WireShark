using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using WireShark.Tiles;
using static System.Threading.Thread;
using static WireShark.WiringWrapper;

namespace WireShark {
    public class WireAccelerator
    {
        private static readonly HashSet<int> _sourceTable = new HashSet<int>() {
            135, 314, 428, 442, 440, 136, 144, 441, 468, 132, 411, TileID.LogicGate, TileID.LogicSensor
        };

        private struct Node {
            public int X, Y;
            public int Dir;
            public Node(int x, int y, int dir) {
                X = x;
                Y = y;
                Dir = dir;
            }
        };
        
        // D, U, R, L
        private static readonly int[] dx = { 0, 0, 1, -1 };
        private static readonly int[] dy = { 1, -1, 0, 0 };
        public TileInfo[][] _connectionInfos;
        private int[,,] _inputConnectedCompoents;

        private byte GetWireID(int X, int Y) {
            var tile = Main.tile[X, Y];
            if (tile == null) return 0;
            byte mask = 0;
            if (tile.BlueWire) mask |= 1;
            if (tile.GreenWire) mask |= 2;
            if (tile.RedWire) mask |= 4;
            if (tile.YellowWire) mask |= 8;
            return mask;
        }
        
        private int[] visited;
        private int now_number;

        public void ResetVisited()
        {
            ++now_number;
        }

        internal static Point16 triggeredBy;

        public void Activiate(int x, int y, int wire)
        {
            int wireid = _wireCache[x, y];
            if (wireid == 0) return;
            if (((wireid >> wire) & 1) == 0) return;
            var id = _inputConnectedCompoents[x, y, wire];
            if (id == -1 || visited[id] == now_number) return;
            var info = _connectionInfos[id];
            triggeredBy = new Point16(x, y);
            foreach (var tile in info) {
                //File.AppendAllText("wire.log",$"logic gate {triggeredBy} triggers tile {tile}\n");
                if (tile.i != x || tile.j != y)
                    tile.HitWire();
            }
            visited[id] = now_number;
        }

        private static ConcurrentQueue<Ref<int[,,,]>> disposing = new();

        [ThreadStatic]
        private static Ref<int[,,,]> __vis;

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

        private int[,,] _visIndexCache;
        private byte[,] _wireCache;
        private TileInfo[,] _tileCache;
        internal static bool noWireOrder = true;

        public void Preprocess() {
            _inputConnectedCompoents = new int[Main.maxTilesX, Main.maxTilesY, 4];
            _boxes = new ();
            _pixelBoxMap = new();
            _visIndexCache = new int[Main.maxTilesX, Main.maxTilesY, 4];
            _tileCache = new TileInfo[Main.maxTilesX, Main.maxTilesY];
            _wireCache = new byte[Main.maxTilesX, Main.maxTilesY];

            for (var i = 0; i < Main.maxTilesX; i++) {
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
            }

            var count = 0;
            var tasks = new List<(int, int, int, int)>();

            for (var j = 0; j < Main.maxTilesY; j++)
            {
                for (var i = 0; i < Main.maxTilesX; i++) {
                    if (Main.tile[i, j] != null)
                    {
                        if (!_sourceTable.Contains(Main.tile[i, j].TileType)) continue;
                        int wireid = _wireCache[i, j];
                        if (wireid == 0) continue;
                        for (var k = 0; k < 4; k++) {
                            if (((wireid >> k) & 1) == 0) continue;
                            _inputConnectedCompoents[i, j, k] = count;
                            tasks.Add((count++, k, i, j));
                            //var info = BFSWires(_connectionInfos.Count, k, i, j);
                            //_inputConnectedCompoents[i, j, k] = _connectionInfos.Count;
                            //_connectionInfos.Add(info);
                        }

                    }
                }
            }

            _connectionInfos = new TileInfo[count][];
            int finished = 0, total = tasks.Count;
            new Thread(() =>
            {
                while (finished != total)
                {
                    if (finished > 0)
                        Main.statusText =$"preprocessing circuit {finished * 1f / total:P1}";
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
            visited = new int[count];
            now_number = 1;

            foreach (var r in disposing)
                r.Value = null;
            disposing.Clear();
            _refreshedBoxes = new PixelBox[_boxes.Count];
            boxCount = 0;
            _boxes = null;

            GC.Collect();
        }


        private static bool IsAppliance(int i, int j) {
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

        private int GetWireBoxIndex2(Tile tile, int dir, int i) {
            var frame = tile.TileFrameX / 18;
            if (frame == 0) {
                if (i != dir) return 0;
                if (dir == 0 || dir == 1) return 1;
                else return 2;
            } else if (frame == 1) {
                if ((dir == 0 && i != 3) || (dir == 3 && i != 0) || (dir == 1 && i != 2) || (dir == 2 && i != 1)) {
                    return 0;
                }
                if (dir == 0 || dir == 3) return 1;
                else return 2;
            } else {
                if ((dir == 0 && i != 2) || (dir == 2 && i != 0) || (dir == 1 && i != 3) || (dir == 3 && i != 1)) {
                    return 0;
                }
                if (dir == 0 || dir == 3) return 1;
                else return 2;
            }
        }

        private int GetWireBoxIndex(Tile tile, int dir) {
            var frame = tile.TileFrameX / 18;
            if (frame == 0) {
                if (dir == 0 || dir == 1) return 1;
                else return 2;
            } else if (frame == 1) {
                if (dir == 0 || dir == 2) return 1;
                else return 2;
            } else {
                if (dir == 0 || dir == 3) return 1;
                else return 2;
            }
        }

        public List<PixelBox> _boxes;
        public PixelBox[] _refreshedBoxes;
        public int boxCount = 0;
        private Dictionary<Point16, PixelBox> _pixelBoxMap;
        internal static int threadCount = 1;

        private TileInfo[] BFSWires(int[,,,] _vis, int id, int wireid, int x, int y) {
            var Q = new Queue<Node>();
            Q.Enqueue(new Node(x, y, 0));
            var outputs = new List<TileInfo>();
            var wirebit = 1 << wireid;
            while (Q.Count > 0) {
                var node = Q.Dequeue();
                //if (node.X == 2129 && node.Y == 282) Debugger.Break();
                // 到达当前点使用的是哪个方向
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

                if (curTile.HasTile) {
                    if (Main.tile[node.X, node.Y].TileType == TileID.PixelBox) {
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
                    } else if (_tileCache[node.X, node.Y] != null)
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

                for (var i = 0; i < 4; i++) {
                    var nx = dx[i] + node.X;
                    var ny = dy[i] + node.Y;
                    if (nx < 2 || nx >= Main.maxTilesX - 2 || ny < 2 || ny >= Main.maxTilesY - 2) continue;
                    var tile = Main.tile[nx, ny];
                    if (tile == null) continue;
                    if (curTile.TileType == TileID.WirePipe) {
                        if (GetWireBoxIndex2(curTile, dir, i) == 0) continue;
                    } else if (curTile.TileType == TileID.PixelBox)
                    {
                        if (dir != i) continue;
                    }
                    if ((_wireCache[nx, ny] & wirebit) != 0) {
                        Q.Enqueue(new Node(nx, ny, i));
                    }
                }
            }

            return outputs.ToArray();
        }

    }
}

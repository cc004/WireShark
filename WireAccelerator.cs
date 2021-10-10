using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using static WireShark.WiringWarpper;

namespace WireShark {
    public class WireAccelerator {

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

        private struct ConnectionInfo {
            public TileInfo[] OutputTiles;
            public KeyValuePair<Point16, byte>[] PixelBoxTriggers;
        };


        private int[,,,] _vis;
        public Dictionary<Point16, byte> _pixelBoxTriggers = new Dictionary<Point16, byte>();
        // D, U, R, L
        private static readonly int[] dx = { 0, 0, 1, -1 };
        private static readonly int[] dy = { 1, -1, 0, 0 };
        private List<ConnectionInfo> _connectionInfos;
        private int[,,] _inputConnectedCompoents;

        private int GetWireID(int X, int Y) {
            Tile tile = Main.tile[X, Y];
            if (tile == null) return 0;
            int mask = 0;
            if (tile.BlueWire) mask |= 1;
            if (tile.GreenWire) mask |= 2;
            if (tile.RedWire) mask |= 4;
            if (tile.YellowWire) mask |= 8;
            return mask;
        }

        public void ActiviateAll(int x, int y, HashSet<int> visited) {
            WiringWarpper.BigTripWire(x, y, 1, 1);
        }
        private int[] visited;
        private int now_number;

        public void ResetVisited()
        {
            ++now_number;
        }

        public void Activiate(int x, int y, int wire) {
            int wireid = GetWireID(x, y);
            if (wireid == 0) return;
            if (((wireid >> wire) & 1) == 0) return;
            int id = _inputConnectedCompoents[x, y, wire];
            if (id == -1 ||/* visited.Contains(id)*/ visited[id] == now_number) return;
            var info = _connectionInfos[id];

            foreach (var tile in info.OutputTiles) {
                tile.HitWire();
            }
            foreach (var pBox in info.PixelBoxTriggers) {
                if (_pixelBoxTriggers.ContainsKey(pBox.Key)) {
                    byte v = _pixelBoxTriggers[pBox.Key];
                    v |= pBox.Value;
                    _pixelBoxTriggers[pBox.Key] = v;
                } else {
                    _pixelBoxTriggers.Add(pBox.Key, pBox.Value);
                }
                // Main.NewText($"{_pixelBoxTriggers[pBox.Key]}");
            }
            visited[id] = now_number;
        }


        public void Preprocess() {
            _inputConnectedCompoents = new int[Main.maxTilesX, Main.maxTilesY, 4];

            _connectionInfos = new List<ConnectionInfo>();
            _vis = new int[Main.maxTilesX, Main.maxTilesY, 4, 3];
            _pixelBoxTriggers = new Dictionary<Point16, byte>();
            for (int i = 0; i < Main.maxTilesX; i++) {
                for (int j = 0; j < Main.maxTilesY; j++) {
                    for (int k = 0; k < 4; k++) {
                        _vis[i, j, k, 0] = -1;
                        _vis[i, j, k, 1] = -1;
                        _vis[i, j, k, 2] = -1;
                        _inputConnectedCompoents[i, j, k] = -1;
                        _inputConnectedCompoents[i, j, k] = -1;
                        _inputConnectedCompoents[i, j, k] = -1;
                    }
                }
            }
            for (int i = 0; i < Main.maxTilesX; i++) {
                for (int j = 0; j < Main.maxTilesY; j++) {
                    if (Main.tile[i, j] != null) {
                        int wireid = GetWireID(i, j);
                        if (wireid == 0 || Main.tile[i, j].type == TileID.WirePipe ||
                                    Main.tile[i, j].type == TileID.PixelBox) continue;

                        for (int k = 0; k < 4; k++) {
                            if (((wireid >> k) & 1) == 0 || _vis[i, j, k, 0] != -1) continue;
                            var info = BFSWires(_connectionInfos.Count, k, i, j);
                            _connectionInfos.Add(info);
                        }

                    }
                }
            }
            _vis = null;
            visited = new int[_connectionInfos.Count];
            now_number = 1;
            GC.Collect();
        }


        private static bool IsAppliance(int i, int j) {
            Tile tile = Main.tile[i, j];
            int type = (int)tile.type;
            if (tile.HasActuator) return true;
            if (tile.IsActive) {
                if (type == 144) return true;
                else if (type == 421 && !tile.HasActuator) return true;
                else if (type == 422 && !tile.HasActuator) return true;
                if (type >= 255 && type <= 268 && !tile.HasActuator) return true;
                else {
                    if (type == 419) return true;
                    if (type == 406) return true;
                    if (type == 452) return true;
                    if (type == 411) return true;
                    if (type == 425) return true;
                    else {
                        if (type == 405) return true;
                        if (type == 209) return true;
                        else if (type == 212) return true;
                        else {
                            if (type == 215) return true;
                            if (type == 130) return true;
                            else {
                                if (type == 131) return true;
                                if (type == 387 || type == 386) return true;
                                else {
                                    if (type == 389 || type == 388) return true;
                                    if (type == 11) return true;
                                    else if (type == 10) return true;
                                    else {
                                        if (type == 216) return true;
                                        if (type == 497 || (type == 15 && tile.frameY / 40 == 1) || (type == 15 && tile.frameY / 40 == 20)) return true;
                                        else if (type == 335) return true;
                                        else if (type == 338) return true;
                                        else if (type == 235) return true;
                                        else {
                                            if (type == 4) return true;
                                            if (type == 429) return true;
                                            if (type == 149) return true;
                                            if (type == 244) return true;
                                            if (type == 565) return true;
                                            if (type == 42) return true;
                                            if (type == 93) return true;
                                            if (type == 126 || type == 95 || type == 100 || type == 173 || type == 564) return true;
                                            if (type == 593) return true;
                                            if (type == 594) return true;
                                            if (type == 34) return true;
                                            if (type == 314) return true;
                                            if (type == 33 || type == 174 || type == 49 || type == 372) return true;
                                            if (type == 92) return true;
                                            if (type == 137) return true;
                                            if (type == 443) return true;
                                            if (type == 531) return true;
                                            if (type == 139 || type == 35) return true;
                                            if (type == 207) return true;
                                            if (type == 410 || type == 480 || type == 509) return true;
                                            if (type == 455) return true;
                                            if (type == 141) return true;
                                            if (type == 210) return true;
                                            if (type == 142 || type == 143) return true;
                                            if (type == 105) return true;
                                            if (type == 349) return true;
                                            if (type == 506) return true;
                                            if (type == 546) return true;
                                            if (type == 557) return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private int GetWireBoxIndex2(Tile tile, int dir, int i) {
            int frame = tile.frameX / 18;
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
            int frame = tile.frameX / 18;
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


        private ConnectionInfo BFSWires(int id, int wireid, int x, int y) {
            //_toProcess.Clear();
            //_toProcess.Add(new Point(x, y), 4);

            Queue<Node> Q = new Queue<Node>();
            Q.Enqueue(new Node(x, y, 0));

            List<TileInfo> outputs = new List<TileInfo>();
            Dictionary<Point16, byte> pixels = new Dictionary<Point16, byte>();
            while (Q.Count > 0) {
                var node = Q.Peek();
                Q.Dequeue();

                // 到达当前点使用的是哪个方向
                int dir = node.Dir;
                Tile curTile = Main.tile[node.X, node.Y];
                if (curTile.type == TileID.WirePipe) {
                    int s = GetWireBoxIndex(curTile, dir);
                    if (_vis[node.X, node.Y, wireid, s] != -1) continue;
                    _vis[node.X, node.Y, wireid, s] = id;
                } else if (curTile.type == TileID.PixelBox) {
                    if (_vis[node.X, node.Y, wireid, dir / 2] != -1) continue;
                    _vis[node.X, node.Y, wireid, dir / 2] = id;
                } else {
                    if (_vis[node.X, node.Y, wireid, 0] != -1) continue;
                    _vis[node.X, node.Y, wireid, 0] = id;
                }

                if (curTile == null) continue;

                if (curTile.IsActive && curTile.type != 0) {
                    if (_sourceTable.Contains(curTile.type)) {
                        _inputConnectedCompoents[node.X, node.Y, wireid] = id;
                    } else if (IsAppliance(node.X, node.Y)) {
                        outputs.Add(TileInfo.CreateTileInfo(node.X, node.Y));
                    }
                }

                for (int i = 0; i < 4; i++) {
                    int nx = dx[i] + node.X;
                    int ny = dy[i] + node.Y;
                    if (nx < 2 || nx >= Main.maxTilesX - 2 || ny < 2 || ny >= Main.maxTilesY - 2) continue;
                    Tile tile = Main.tile[nx, ny];
                    if (tile == null) continue;
                    if (curTile.type == TileID.WirePipe) {
                        int s = GetWireBoxIndex2(curTile, dir, i);
                        if (s == 0) continue;
                    } else if (curTile.type == TileID.PixelBox) {
                        if (dir != i) continue;
                        Point16 pt = new Point16(node.X, node.Y);
                        if (!pixels.ContainsKey(pt)) {
                            pixels.Add(pt, 0);
                        }
                        if (i / 2 < 1) {
                            pixels[pt] |= 2;
                        } else {
                            pixels[pt] |= 1;
                        }
                    }
                    if (((GetWireID(nx, ny) >> wireid) & 1) != 0) {
                        Q.Enqueue(new Node(nx, ny, i));
                    }
                }
            }
            return new ConnectionInfo {
                OutputTiles = outputs.ToArray(),
                PixelBoxTriggers = pixels.ToArray()
            };
        }

    }
}

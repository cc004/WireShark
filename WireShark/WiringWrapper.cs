﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using log4net.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.Map;
using Terraria.ModLoader;
using WireShark.Tiles;

namespace WireShark
{
    public static unsafe class WiringWrapper
    {

        // public static WireAccelerator _wireAccelerator;

        // Token: 0x06000753 RID: 1875 RVA: 0x0035517C File Offset: 0x0035337C
        public static void SetCurrentUser(int plr = -1)
        {
            if (plr < 0 || plr >= 255)
            {
                plr = 254;
            }
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                plr = Main.myPlayer;
            }
            CurrentUser = plr;
        }

        // Token: 0x06000754 RID: 1876 RVA: 0x003551A8 File Offset: 0x003533A8
        public static void Initialize()
        {
            onLogicLampChange = new LogicGate[Main.maxTilesX, Main.maxTilesY];
            // _wireAccelerator = new WireAccelerator();
            _wireList = new QuickLinkedList<Point16>();
            // _wireDirectionList = new DoubleStack<byte>(1024, 0);
            _GatesCurrent = new QuickLinkedList<Point16>();
            _GatesNext = new QuickLinkedList<Point16>();
            _LampsToCheck = new QuickLinkedList<LogicGate>();
            _inPumpX = new int[20];
            _inPumpY = new int[20];
            _outPumpX = new int[20];
            _outPumpY = new int[20];
            _mechX = new int[1000];
            _mechY = new int[1000];
            _mechTime = new int[1000];
        }

        public static void Unload()
        {
            onLogicLampChange = null;
            _inPumpX = null;
            _inPumpY = null;
            _outPumpX = null;
            _outPumpY = null;
            _mechX = null;
            _mechY = null;
            _mechTime = null;
        }

        // Token: 0x06000757 RID: 1879 RVA: 0x003552A8 File Offset: 0x003534A8

        // Mech 应该就是可以激活的计时器 | It should be a timer that can be activated
        public static void UpdateMech()
        {
            SetCurrentUser(-1);
            for (var i = _numMechs - 1; i >= 0; i--)
            {
                _mechTime[i]--;
                if (Main.tile[_mechX[i], _mechY[i]].HasTile && Main.tile[_mechX[i], _mechY[i]].TileType == 144)
                {
                    if (Main.tile[_mechX[i], _mechY[i]].TileFrameY == 0)
                    {
                        _mechTime[i] = 0;
                    }
                    else
                    {
                        var num = Main.tile[_mechX[i], _mechY[i]].TileFrameX / 18;
                        if (num == 0)
                        {
                            num = 60;
                        }
                        else if (num == 1)
                        {
                            num = 180;
                        }
                        else if (num == 2)
                        {
                            num = 300;
                        }
                        if (Math.IEEERemainder(_mechTime[i], num) == 0.0)
                        {
                            _mechTime[i] = 18000;
                            BigTripWire(_mechX[i], _mechY[i], 1, 1);
                        }
                    }
                }
                if (_mechTime[i] <= 0)
                {
                    if (Main.tile[_mechX[i], _mechY[i]].HasTile && Main.tile[_mechX[i], _mechY[i]].TileType == 144)
                    {
                        Main.tile[_mechX[i], _mechY[i]].TileFrameY = 0;

                    }
                    if (Main.tile[_mechX[i], _mechY[i]].HasTile && Main.tile[_mechX[i], _mechY[i]].TileType == 411)
                    {
                        var tile = Main.tile[_mechX[i], _mechY[i]];
                        var num2 = tile.TileFrameX % 36 / 18;
                        var num3 = tile.TileFrameY % 36 / 18;
                        var num4 = _mechX[i] - num2;
                        var num5 = _mechY[i] - num3;
                        var num6 = 36;
                        if (Main.tile[num4, num5].TileFrameX >= 36)
                        {
                            num6 = -36;
                        }
                        for (var j = num4; j < num4 + 2; j++)
                        {
                            for (var k = num5; k < num5 + 2; k++)
                            {
                                Main.tile[j, k].TileFrameX = (short)(Main.tile[j, k].TileFrameX + num6);
                            }
                        }

                    }
                    for (var l = i; l < _numMechs; l++)
                    {
                        _mechX[l] = _mechX[l + 1];
                        _mechY[l] = _mechY[l + 1];
                        _mechTime[l] = _mechTime[l + 1];
                    }
                    _numMechs--;
                }
            }
        }

        // Token: 0x06000758 RID: 1880 RVA: 0x003555B8 File Offset: 0x003537B8
        public static void HitSwitch(int i, int j)
        {
            if (!WorldGen.InWorld(i, j, 0))
            {
                return;
            }
            if (Main.tile[i, j] == null)
            {
                return;
            }
            if (Main.tile[i, j].TileType == 135 || Main.tile[i, j].TileType == 314 || Main.tile[i, j].TileType == 423 || Main.tile[i, j].TileType == 428 || Main.tile[i, j].TileType == 442)
            {
                SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
                BigTripWire(i, j, 1, 1);
                return;
            }
            if (Main.tile[i, j].TileType == 440)
            {
                SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16 + 16, j * 16 + 16));
                BigTripWire(i, j, 3, 3);
                return;
            }
            if (Main.tile[i, j].TileType == 136)
            {
                if (Main.tile[i, j].TileFrameY == 0)
                {
                    Main.tile[i, j].TileFrameY = 18;
                }
                else
                {
                    Main.tile[i, j].TileFrameY = 0;
                }
                SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
                BigTripWire(i, j, 1, 1);
                return;
            }
            if (Main.tile[i, j].TileType == 144)
            {
                if (Main.tile[i, j].TileFrameY == 0)
                {
                    Main.tile[i, j].TileFrameY = 18;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        CheckMech(i, j, 18000);
                    }
                }
                else
                {
                    Main.tile[i, j].TileFrameY = 0;
                }
                SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
                return;
            }
            if (Main.tile[i, j].TileType == 441 || Main.tile[i, j].TileType == 468)
            {
                var num = Main.tile[i, j].TileFrameX / 18 * -1;
                var num2 = Main.tile[i, j].TileFrameY / 18 * -1;
                num %= 4;
                if (num < -1)
                {
                    num += 2;
                }
                num += i;
                num2 += j;
                SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
                BigTripWire(num, num2, 2, 2);
                return;
            }
            if (Main.tile[i, j].TileType == 132 || Main.tile[i, j].TileType == 411)
            {
                short num3 = 36;
                var num4 = Main.tile[i, j].TileFrameX / 18 * -1;
                var num5 = Main.tile[i, j].TileFrameY / 18 * -1;
                num4 %= 4;
                if (num4 < -1)
                {
                    num4 += 2;
                    num3 = -36;
                }
                num4 += i;
                num5 += j;
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.tile[num4, num5].TileType == 411)
                {
                    CheckMech(num4, num5, 60);
                }
                for (var k = num4; k < num4 + 2; k++)
                {
                    for (var l = num5; l < num5 + 2; l++)
                    {
                        if (Main.tile[k, l].TileType == 132 || Main.tile[k, l].TileType == 411)
                        {
                            var tile = Main.tile[k, l];
                            tile.TileFrameX += num3;
                        }
                    }
                }
                WorldGen.TileFrame(num4, num5, false, false);
                SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
                BigTripWire(num4, num5, 2, 2);
            }
        }

        // Token: 0x06000759 RID: 1881 RVA: 0x0035599F File Offset: 0x00353B9F
        public static void PokeLogicGate(int lampX, int lampY)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            _LampsToCheck.AddLast(onLogicLampChange[lampX, lampY]);
            LogicGatePassVanilla();
        }

        // Token: 0x0600075A RID: 1882 RVA: 0x003559C0 File Offset: 0x00353BC0
        public static bool Actuate(int i, int j)
        {
            var tile = Main.tile[i, j];
            if (!tile.HasActuator)
            {
                return false;
            }
            if ((tile.TileType != 226 || j <= Main.worldSurface || NPC.downedPlantBoss) && (j <= Main.worldSurface || NPC.downedGolemBoss || Main.tile[i, j - 1].TileType != 237))
            {
                if (!tile.HasTile)
                {
                    ReActive(i, j);
                }
                else
                {
                    DeActive(i, j);
                }
            }
            return true;
        }

        // Token: 0x0600075B RID: 1883 RVA: 0x00355A44 File Offset: 0x00353C44
        public static void ActuateForced(int i, int j)
        {
            var tile = Main.tile[i, j];
            if (tile.TileType == 226 && j > Main.worldSurface && !NPC.downedPlantBoss)
            {
                return;
            }
            if (tile.IsActuated)
            {
                ReActive(i, j);
                return;
            }
            DeActive(i, j);
        }

        // Token: 0x0600075D RID: 1885 RVA: 0x00355BB8 File Offset: 0x00353DB8
        public static bool CheckMech(int i, int j, int time)
        {
            for (var k = 0; k < _numMechs; k++)
            {
                if (_mechX[k] == i && _mechY[k] == j)
                {
                    return false;
                }
            }
            if (_numMechs < 999)
            {
                _mechX[_numMechs] = i;
                _mechY[_numMechs] = j;
                _mechTime[_numMechs] = time;
                _numMechs++;
                return true;
            }
            return false;
        }

        // Token: 0x0600075E RID: 1886 RVA: 0x00355C2C File Offset: 0x00353E2C
        private static void XferWater()
        {
            for (var i = 0; i < _numInPump; i++)
            {
                var num = _inPumpX[i];
                var num2 = _inPumpY[i];
                var liquid = Main.tile[num, num2].LiquidType;
                if (liquid > 0)
                {
                    var flag = (Main.tile[num, num2].LiquidType == LiquidID.Lava);
                    var flag2 = (Main.tile[num, num2].LiquidType == LiquidID.Honey);
                    for (var j = 0; j < _numOutPump; j++)
                    {
                        var num3 = _outPumpX[j];
                        var num4 = _outPumpY[j];
                        var liquid2 = Main.tile[num3, num4].LiquidType;
                        if (liquid2 < 255)
                        {
                            var flag3 = (Main.tile[num3, num4].LiquidType == LiquidID.Lava);
                            var flag4 = (Main.tile[num3, num4].LiquidType == LiquidID.Honey);
                            if (liquid2 == 0)
                            {
                                flag3 = flag;
                                flag4 = flag2;
                            }
                            if (flag == flag3 && flag2 == flag4)
                            {
                                var num5 = liquid;
                                if (num5 + liquid2 > 255)
                                {
                                    num5 = 255 - liquid2;
                                }
                                var tile = Main.tile[num3, num4];
                                tile.LiquidType += (byte)num5;
                                var tile2 = Main.tile[num, num2];
                                tile2.LiquidType -= (byte)num5;
                                liquid = Main.tile[num, num2].LiquidType;
                                if (flag)
                                {
                                    var testTile = Main.tile[num3, num4];
                                    testTile.LiquidType = LiquidID.Lava;
                                }
                                if (flag2)
                                {
                                    var testTile = Main.tile[num3, num4];
                                    testTile.LiquidType = LiquidID.Honey;
                                }
                                WorldGen.SquareTileFrame(num3, num4, true);
                                if (Main.tile[num, num2].LiquidType == 0)
                                {
                                    Main.tile[num, num2].LiquidAmount = 0;
                                    WorldGen.SquareTileFrame(num, num2, true);
                                    break;
                                }
                            }
                        }
                    }
                    WorldGen.SquareTileFrame(num, num2, true);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void TripWireWithLogicVanillaSingle(int l, int t)
        {
            TripWireSingle(l, t);
            PixelBoxPass();
            LogicGatePassVanilla();
        }

        // entry point
        public static void BigTripWire(int l, int t, int w, int h)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            if (w == 1 && h == 1)
                TripWireSingle(l, t);
            else
                TripWire(l, t, w, h);
            PixelBoxPass();
            LogicGatePassVanilla();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TripWire(int left, int top, int width, int height)
        {
            Wiring.running = true;
            // 清除队列 | clear queue
            _wireList.Clear();
            // _wireDirectionList.Clear(true);

            var array = stackalloc Vector2[8];
            var num = 0;

            var teleport = stackalloc Vector2[2];
            WiringWrapper._teleport = teleport;
            // fixed (Vector2* _teleport = WiringWrapper._teleport)
            {
                _teleport[0].X = -1f;
                _teleport[0].Y = -1f;
                _teleport[1].X = -1f;
                _teleport[1].Y = -1f;
                for (var m = left; m < left + width; m++)
                {
                    for (var n = top; n < top + height; n++)
                    {
                        var tile3 = Main.tile[m, n];
                        if (tile3 != null && tile3.RedWire)
                        {
                            var back3 = new Point16(m, n);
                            _wireList.AddLast(back3);
                        }
                    }
                }

                if (_wireList.Count > 0)
                {
                    _numInPump = 0;
                    _numOutPump = 0;
                    HitWire(2);
                    if (_numInPump > 0 && _numOutPump > 0)
                    {
                        XferWater();
                    }
                }

                array[0] = _teleport[0];
                array[1] = _teleport[1];



                for (var i = left; i < left + width; i++)
                {
                    for (var j = top; j < top + height; j++)
                    {
                        var tile = Main.tile[i, j];
                        if (tile != null && tile.BlueWire)
                        {
                            var back = new Point16(i, j);
                            _wireList.AddLast(back);
                        }
                    }
                }

                _teleport[0].X = -1f;
                _teleport[0].Y = -1f;
                _teleport[1].X = -1f;
                _teleport[1].Y = -1f;
                if (_wireList.Count > 0)
                {
                    _numInPump = 0;
                    _numOutPump = 0;
                    HitWire(0);
                    if (_numInPump > 0 && _numOutPump > 0)
                    {
                        XferWater();
                    }
                }

                array[2] = _teleport[0];
                array[3] = _teleport[1];


                for (var k = left; k < left + width; k++)
                {
                    for (var l = top; l < top + height; l++)
                    {
                        var tile2 = Main.tile[k, l];
                        if (tile2 != null && tile2.GreenWire)
                        {
                            var back2 = new Point16(k, l);
                            _wireList.AddLast(back2);
                        }
                    }
                }

                _teleport[0].X = -1f;
                _teleport[0].Y = -1f;
                _teleport[1].X = -1f;
                _teleport[1].Y = -1f;
                if (_wireList.Count > 0)
                {
                    _numInPump = 0;
                    _numOutPump = 0;
                    HitWire(1);
                    if (_numInPump > 0 && _numOutPump > 0)
                    {
                        XferWater();
                    }
                }

                array[4] = _teleport[0];
                array[5] = _teleport[1];

                _teleport[0].X = -1f;
                _teleport[0].Y = -1f;
                _teleport[1].X = -1f;
                _teleport[1].Y = -1f;
                for (var num2 = left; num2 < left + width; num2++)
                {
                    for (var num3 = top; num3 < top + height; num3++)
                    {
                        var tile4 = Main.tile[num2, num3];
                        if (tile4 != null && tile4.YellowWire)
                        {
                            var back4 = new Point16(num2, num3);
                            _wireList.AddLast(back4);
                        }
                    }
                }

                if (_wireList.Count > 0)
                {
                    _numInPump = 0;
                    _numOutPump = 0;
                    HitWire(3);
                    if (_numInPump > 0 && _numOutPump > 0)
                    {
                        XferWater();
                    }
                }

                array[6] = _teleport[0];
                array[7] = _teleport[1];


                for (var num4 = 0; num4 < 8; num4 += 2)
                {
                    _teleport[0] = array[num4];
                    _teleport[1] = array[num4 + 1];
                    if (_teleport[0].X >= 0f && _teleport[1].X >= 0f)
                    {
                        Teleport();
                    }
                }

            }
        }

        // 优化方向：根据tile emit代码 | Optimization direction: based on tile emit code

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static void TripWireSingle(int left, int top)
        {

            Wiring.running = true;
            // 清除队列 | clear queue
            _wireList.Clear();
            // _wireDirectionList.Clear(true);

            var array = stackalloc Vector2[8];
            var num = 0;

            var teleport = stackalloc Vector2[2];
            WiringWrapper._teleport = teleport;

            {
                _teleport[0].X = -1f;
                _teleport[0].Y = -1f;
                _teleport[1].X = -1f;
                _teleport[1].Y = -1f;

                var tile3 = Main.tile[left, top];
                if (tile3 != null && tile3.RedWire)
                {
                    _numInPump = 0;
                    _numOutPump = 0;
                    WireAccelerator.Activate(left, top, 2);
                    if (_numInPump > 0 && _numOutPump > 0)
                    {
                        XferWater();
                    }
                }

                array[0] = _teleport[0];
                array[1] = _teleport[1];


                var tile = Main.tile[left, top];
                if (tile != null && tile.BlueWire)
                {
                    _numInPump = 0;
                    _numOutPump = 0;
                    WireAccelerator.Activate(left, top, 0);
                    if (_numInPump > 0 && _numOutPump > 0)
                    {
                        XferWater();
                    }
                }

                _teleport[0].X = -1f;
                _teleport[0].Y = -1f;
                _teleport[1].X = -1f;
                _teleport[1].Y = -1f;

                array[2] = _teleport[0];
                array[3] = _teleport[1];


                var tile2 = Main.tile[left, top];
                if (tile2 != null && tile2.GreenWire)
                {
                    _numInPump = 0;
                    _numOutPump = 0;
                    WireAccelerator.Activate(left, top, 1);
                    if (_numInPump > 0 && _numOutPump > 0)
                    {
                        XferWater();
                    }
                }

                _teleport[0].X = -1f;
                _teleport[0].Y = -1f;
                _teleport[1].X = -1f;
                _teleport[1].Y = -1f;

                array[4] = _teleport[0];
                array[5] = _teleport[1];

                _teleport[0].X = -1f;
                _teleport[0].Y = -1f;
                _teleport[1].X = -1f;
                _teleport[1].Y = -1f;

                var tile4 = Main.tile[left, top];
                if (tile4 != null && tile4.YellowWire)
                {
                    _numInPump = 0;
                    _numOutPump = 0;
                    WireAccelerator.Activate(left, top, 3);
                    if (_numInPump > 0 && _numOutPump > 0)
                    {
                        XferWater();
                    }
                }

                array[6] = _teleport[0];
                array[7] = _teleport[1];


                for (var num4 = 0; num4 < 8; num4 += 2)
                {
                    _teleport[0] = array[num4];
                    _teleport[1] = array[num4 + 1];
                    if (_teleport[0].X >= 0f && _teleport[1].X >= 0f)
                    {
                        Teleport();
                    }
                }

            }
        }

        // Token: 0x06000760 RID: 1888 RVA: 0x00356308 File Offset: 0x00354508
        private static readonly short[] frames = {-1, -1, 0, 18};

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PixelBoxPass()
        {
            while (WireAccelerator.boxCount > 0)
            {
                var box = WireAccelerator._refreshedBoxes[--WireAccelerator.boxCount];
                if (box.state.HasFlag(PixelBox.PixelBoxState.Vertical) && box.state.HasFlag(PixelBox.PixelBoxState.Horizontal) && box.tile.TileFrameX == 18)
                    box.tile.TileFrameX = 0;
                else if (box.state.HasFlag(PixelBox.PixelBoxState.Vertical) && box.state.HasFlag(PixelBox.PixelBoxState.Horizontal) && box.tile.TileFrameX == 0)
                    box.tile.TileFrameX = 18;
                box.state = PixelBox.PixelBoxState.None;
            }
        }
        /*
        // Token: 0x06000761 RID: 1889 RVA: 0x0035647C File Offset: 0x0035467C
        private static Action LogicGatePass = LogicGatePassVanilla;
        private static Action<int, int, int, int> TripWireWithLogic = TripWireWithLogicVanilla;

        public static void SetPixelBoxBehaviour(bool isVanilla)
        {
            LogicGatePass = isVanilla ? LogicGatePassVanilla : LogicGatePassAdvanced;
            TripWireWithLogic = isVanilla ? TripWireWithLogicVanilla : TripWireWithLogicAdvanced;
        }
        */

        private static void LogicGatePassVanilla()
        {
            if (_GatesCurrent.Count == 0)
            {
                Clear_Gates();
                while (_LampsToCheck.Count > 0)
                {
                    for (int i = 0; i < _LampsToCheck.Count; ++i)
                    {
                        _LampsToCheck.cachePtr[i].UpdateLogicGate();
                    }

                    _LampsToCheck.Clear();
                    /*
                    while (_LampsToCheck.Count > 0)
                    {
                        _LampsToCheck.Dequeue().UpdateLogicGate();

                        // Point16 point = ;
                       //  CheckLogicGate((int)point.X, (int)point.Y);
                    } */

                    //if (_GatesNext.Count > 0)
                    //    Main.NewText($"gate counts to check = {_GatesNext.Count}");
                    while (_GatesNext.Count > 0)
                    {
                        Utils.Swap(ref _GatesCurrent, ref _GatesNext);
                        for (int i = 0; i < _GatesCurrent.Count; ++i)
                        {
                            var key = _GatesCurrent.cachePtr[i];
                            if (_GatesDone[key.X, key.Y] != cur_gatesdone)
                            {
                                _GatesDone[key.X, key.Y] = cur_gatesdone;
                                TripWireWithLogicVanillaSingle(key.X, key.Y);
                            }
                        }

                        _GatesCurrent.Clear();
                        /*
                        while (_GatesCurrent.Count > 0)
                        {
                            var key = _GatesCurrent.Peek();
                            if (_GatesDone[key.X, key.Y] != cur_gatesdone)
                            {
                                _GatesDone[key.X, key.Y] = cur_gatesdone;
                                TripWireWithLogicVanilla(key.X, key.Y, 1, 1);
                            }
                            _GatesCurrent.Dequeue();
                        }*/
                    }
                }
                if (Wiring.blockPlayerTeleportationForOneIteration)
                {
                    Wiring.blockPlayerTeleportationForOneIteration = false;
                }
            }
        }

        internal static LogicGate[,] onLogicLampChange;

        internal class AllOnGate : LogicGate
        {
            public override void UpdateLogicGate()
            {
                var cur = *lampon == lamptotal;
                //Main.NewText($"update {GetType().Name} => {active} to {cur}, {lampon} / {lamptotal} @({x}, {y})");
                if ((mapTile.TileFrameX == 18) ^ cur)
                {
                    mapTile.TileFrameX = (short)(18 - mapTile.TileFrameX);
                    if (_GatesDone[x, y] != cur_gatesdone) _GatesNext.AddLast(new Point16(x, y));
                }
            }
        }

        internal class AnyOnGate : LogicGate
        {
            public override void UpdateLogicGate()
            {
                var cur = *lampon > 0;
                //Main.NewText($"update {GetType().Name} => {active} to {cur}, {lampon} / {lamptotal} @({x}, {y})");
                if ((mapTile.TileFrameX == 18) ^ cur)
                {
                    mapTile.TileFrameX = (short)(18 - mapTile.TileFrameX);
                    if (_GatesDone[x, y] != cur_gatesdone) _GatesNext.AddLast(new Point16(x, y));
                }
            }
        }

        internal class AnyOffGate : LogicGate
        {
            public override void UpdateLogicGate()
            {
                var cur = *lampon != lamptotal;
                //Main.NewText($"update {GetType().Name} => {active} to {cur}, {lampon} / {lamptotal} @({x}, {y})");
                if ((mapTile.TileFrameX == 18) ^ cur)
                {
                    mapTile.TileFrameX = (short)(18 - mapTile.TileFrameX);
                    if (_GatesDone[x, y] != cur_gatesdone) _GatesNext.AddLast(new Point16(x, y));
                }
            }
        }

        internal class AllOffGate : LogicGate
        {
            public override void UpdateLogicGate()
            {
                var cur = *lampon == 0;
                //Main.NewText($"update {GetType().Name} => {active} to {cur}, {lampon} / {lamptotal} @({x}, {y})");
                if ((mapTile.TileFrameX == 18) ^ cur)
                {
                    mapTile.TileFrameX = (short)(18 - mapTile.TileFrameX);
                    if (_GatesDone[x, y] != cur_gatesdone) _GatesNext.AddLast(new Point16(x, y));
                }
            }
        }

        internal class OneOnGate : LogicGate
        {
            public override void UpdateLogicGate()
            {
                var cur = *lampon == 1;
                //Main.NewText($"update {GetType().Name} => {active} to {cur}, {lampon} / {lamptotal} @({x}, {y})");
                if ((mapTile.TileFrameX == 18) ^ cur)
                {
                    mapTile.TileFrameX = (short)(18 - mapTile.TileFrameX);
                    if (_GatesDone[x, y] != cur_gatesdone) _GatesNext.AddLast(new Point16(x, y));
                }
            }
        }

        internal class NotOneOnGate : LogicGate
        {
            public override void UpdateLogicGate()
            {
                var cur = *lampon != 1;
                //Main.NewText($"update {GetType().Name} => {active} to {cur}, {lampon} / {lamptotal} @({x}, {y})");
                if ((mapTile.TileFrameX == 18) ^ cur)
                {
                    mapTile.TileFrameX = (short)(18 - mapTile.TileFrameX);
                    if (_GatesDone[x, y] != cur_gatesdone) _GatesNext.AddLast(new Point16(x, y));
                }
            }
        }

        internal class ErrorGate : LogicGate
        {
            public ErrorGate()
            {
                erroronly = true;
            }

            public override void UpdateLogicGate()
            {
                if (Main.rand.NextDouble() * lamptotal < *lampon)
                    if (_GatesDone[x, y] != cur_gatesdone) _GatesNext.AddLast(new Point16(x, y));
            }

        }

        internal class WireState : TileInfo
        {
            public bool state;
            protected override void HitWireInternal()
            {
                state ^= true;
            }
        }

        internal unsafe class OneErrorGate : LogicGate
        {
            public bool originalState;

            public WireState state1, state2, state3, state4;
            public static WireState alwaysFalse = new WireState {state = false};

            public OneErrorGate()
            {
                state1 = state2 = state3 = state4 = alwaysFalse;
            }
            
            public override void UpdateLogicGate()
            {
                if (originalState ^ state1.state ^ state2.state ^ state3.state ^ state4.state)
                    if (_GatesDone[x, y] != cur_gatesdone) _GatesNext.AddLast(new Point16(x, y));
            }

        }
        
        private static void CacheLogicGate(int x, int y)
        {
            LogicGate lgate;
            var tile = Main.tile[x, y];
            var lamps = new List<Tile>();        // lamps before one error gate
            var lampTriggers = new List<Tile>(); // all lamps
            var countend = false;
            var onnum = 0;
            for (var j = y - 1; j > 0; --j)
            {
                var tile2 = Main.tile[x, j];
                if (!tile2.HasTile || tile2.TileType != TileID.LogicGateLamp)
                    break;

                lampTriggers.Add(tile2);

                if (tile2.TileFrameX == 36)
                {
                    countend = true;
                    // break;
                }

                if (!countend)
                {
                    lamps.Add(tile2);
                    if (tile2.TileFrameX == 18)
                        ++onnum;
                }
            }
            if (lamps.Count == 0) return;
            if (countend)
            {
                if (lamps.Count == 1) lgate = new OneErrorGate();
                else lgate = new ErrorGate();
            }
            else
            {
                switch (tile.TileFrameY / 18)
                {
                    case 0: lgate = new AllOnGate(); break;
                    case 1: lgate = new AnyOnGate(); break;
                    case 2: lgate = new AnyOffGate(); break;
                    case 3: lgate = new AllOffGate(); break;
                    case 4: lgate = new OneOnGate(); break;
                    case 5: lgate = new NotOneOnGate(); break;
                    default: return;
                }
            }

            lgate.lamptotal = lamps.Count;
            *lgate.lampon = onnum;
            lgate.mapTile = tile;
            lgate.x = x;
            lgate.y = y;

            for (var i = 0; i < lampTriggers.Count; ++i)
            {
                if (i < lamps.Count || lampTriggers[i].TileFrameX == 36)
                    onLogicLampChange[x, y - i - 1] = lgate;
            }
        }

        public static void Initialize_LogicLamps()
        {

            for (var i = 0; i < Main.maxTilesX; ++i)

            {
                for (var j = 0; j < Main.maxTilesY; ++j)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.LogicGate)
                        CacheLogicGate(i, j);

                }

                if (i % 100 == 0) Main.statusText = $"processing logic gates {i * 1f / Main.maxTilesX:P1}";
            }

            var lampClearing = new bool[Main.maxTilesX, Main.maxTilesY];

            /*
            tasks.AsParallel().WithDegreeOfParallelism(threadCount).ForAll(task =>
            {
                if (noWireOrder && _vis[task.Item3, task.Item4, task.Item2, 0] != -1)
                    _connectionInfos[task.Item1] = _connectionInfos[_vis[task.Item3, task.Item4, task.Item2, 0]];
                else
                    _connectionInfos[task.Item1] = BFSWires(_vis, task.Item1, task.Item2, task.Item3, task.Item4);
            });*/

            var uniqueConnection = WireAccelerator._connectionInfos.ToHashSet()
                .ToDictionary(arr => arr, arr =>
                {
                    var result = new List<TileInfo>();
                    WireState state = null;

                    foreach (var info in arr)
                    {
                        if (info is Tile419 && onLogicLampChange[info.i, info.j] is OneErrorGate gate)
                        {
                            gate.originalState = *gate.lampon > 0;
                            if (info.tile.TileFrameX != 36)
                            {
                                state ??= new WireState {i = info.i, j = info.j, tile = info.tile, hash = ((long) info.i << 32) + info.j};

                                lock (gate)
                                {
                                    if (gate.state1 == OneErrorGate.alwaysFalse) gate.state1 = state;
                                    else if (gate.state2 == OneErrorGate.alwaysFalse) gate.state2 = state;
                                    else if (gate.state3 == OneErrorGate.alwaysFalse) gate.state3 = state;
                                    else if (gate.state4 == OneErrorGate.alwaysFalse) gate.state4 = state;
                                    else throw new InvalidOperationException();
                                }

                                lampClearing[info.i, info.j] = true;
                                continue;
                            }
                        }

                        result.Add(info);
                    }

                    if (state != null) result.Add(state);
                    return result.ToArray();
                });

            for (var i = 0; i < Main.maxTilesX; ++i)
            {
                for (var j = 0; j < Main.maxTilesY; ++j)
                {
                    if (lampClearing[i, j]) onLogicLampChange[i, j] = null;
                }

                if (i % 100 == 0) Main.statusText = $"clearing redundant logic cache {i * 1f / Main.maxTilesX:P1}";
            }

            var p = 0;

            var changedDict = new Dictionary<TileInfo, TileInfo>();

            foreach (var arr in uniqueConnection.Values)
            {
                for (int i = 0; i < arr.Length; ++i)
                {
                    if (arr[i] is Tile419)
                    {
                        if (changedDict.TryGetValue(arr[i], out var val))
                        {
                            arr[i] = val;
                            continue;
                        }
                        var lgate = onLogicLampChange[arr[i].i, arr[i].j];
                        Tile419 newTile;
                        if (lgate == null)
                        {
                            if (arr[i].tile.TileFrameX == 36) newTile = new Tile419ErrorUnconnected();
                            else newTile = new Tile419NormalUnconnected();
                        }
                        else
                        {
                            if (arr[i].tile.TileFrameX == 36) newTile = new Tile419Error();
                            else if (lgate.erroronly)
                            {
                                if (lgate is OneErrorGate) newTile = new Tile419NormalOnOneError();
                                else newTile = new Tile419NormalOnError();
                            }
                            else newTile = new Tile419Normal();
                        }

                        newTile.i = arr[i].i;
                        newTile.j = arr[i].j;
                        newTile.hash = arr[i].hash;
                        newTile.tile = arr[i].tile;
                        newTile.lgate = lgate;
                        newTile.add = newTile.tile.TileFrameX == 18 ? 1 : -1;
                        if (lgate != null)
                        {
                            newTile.lampon = lgate.lampon;
                        }

                        changedDict[arr[i]] = newTile;

                        arr[i] = newTile;
                    }
                }

                if ((++p) % 100 == 0) Main.statusText = $"optimizing logic lamps {p * 1f / uniqueConnection.Count:P1}";
            }

            for (int i = 0; i < WireAccelerator._connectionInfos.Length; ++i)
            {
                WireAccelerator._connectionInfos[i] = uniqueConnection[WireAccelerator._connectionInfos[i]];
            }

        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void HitWire(int wireType)
        {
            _currentWireColor = wireType + 1;
            // _wireDirectionList.Clear(true);
            for (var i = 0; i < _wireList.Count; i++)
            {
                var point = _wireList.cachePtr[i];
                WireAccelerator.Activate(point.X, point.Y, wireType);
            }
            _wireList.Clear();
        }
        
        // Token: 0x06000765 RID: 1893 RVA: 0x00359ABC File Offset: 0x00357CBC
        private static void Teleport()
        {
            if (_teleport[0].X < _teleport[1].X + 3f && _teleport[0].X > _teleport[1].X - 3f && _teleport[0].Y > _teleport[1].Y - 3f && _teleport[0].Y < _teleport[1].Y)
            {
                return;
            }
            var array = new Rectangle[2];
            array[0].X = (int)(_teleport[0].X * 16f);
            array[0].Width = 48;
            array[0].Height = 48;
            array[0].Y = (int)(_teleport[0].Y * 16f - array[0].Height);
            array[1].X = (int)(_teleport[1].X * 16f);
            array[1].Width = 48;
            array[1].Height = 48;
            array[1].Y = (int)(_teleport[1].Y * 16f - array[1].Height);
            for (var i = 0; i < 2; i++)
            {
                var value = new Vector2(array[1].X - array[0].X, array[1].Y - array[0].Y);
                if (i == 1)
                {
                    value = new Vector2(array[0].X - array[1].X, array[0].Y - array[1].Y);
                }
                if (!Wiring.blockPlayerTeleportationForOneIteration)
                {
                    for (var j = 0; j < 255; j++)
                    {
                        if (Main.player[j].active && !Main.player[j].dead && !Main.player[j].teleporting && array[i].Intersects(Main.player[j].getRect()))
                        {
                            var vector = Main.player[j].position + value;
                            Main.player[j].teleporting = true;
                            if (Main.netMode == NetmodeID.Server)
                            {
                                RemoteClient.CheckSection(j, vector, 1);
                            }
                            Main.player[j].Teleport(vector, 0, 0);
                            if (Main.netMode == NetmodeID.Server)
                            {

                            }
                        }
                    }
                }
                for (var k = 0; k < 200; k++)
                {
                    if (Main.npc[k].active && !Main.npc[k].teleporting && Main.npc[k].lifeMax > 5 && !Main.npc[k].boss && !Main.npc[k].noTileCollide)
                    {
                        var type = Main.npc[k].type;
                        if (!NPCID.Sets.TeleportationImmune[type] && array[i].Intersects(Main.npc[k].getRect()))
                        {
                            Main.npc[k].teleporting = true;
                            Main.npc[k].Teleport(Main.npc[k].position + value, 0, 0);
                        }
                    }
                }
            }
            for (var l = 0; l < 255; l++)
            {
                Main.player[l].teleporting = false;
            }
            for (var m = 0; m < 200; m++)
            {
                Main.npc[m].teleporting = false;
            }
        }

        // Token: 0x06000766 RID: 1894 RVA: 0x00359EC4 File Offset: 0x003580C4
        public static void DeActive(int i, int j)
        {
            // XX: removed actuate condition
            var testTile = Main.tile[i, j];
            testTile.IsActuated = true;

            WorldGen.SquareTileFrame(i, j, false);
        }

        // Token: 0x06000767 RID: 1895 RVA: 0x0035A018 File Offset: 0x00358218
        public static void ReActive(int i, int j)
        {
            var testTile = Main.tile[i, j];
            testTile.IsActuated = false;

            WorldGen.SquareTileFrame(i, j, false);
        }

        // Token: 0x04000C71 RID: 3185
        internal static QuickLinkedList<Point16> _wireList;

        // Token: 0x04000C74 RID: 3188
        private static QuickLinkedList<Point16> _GatesCurrent;

        // Token: 0x04000C75 RID: 3189
        internal static QuickLinkedList<LogicGate> _LampsToCheck;

        // Token: 0x04000C76 RID: 3190
        internal static QuickLinkedList<Point16> _GatesNext;

        // Token: 0x04000C77 RID: 3191
        internal static int[,] _GatesDone;
        internal static int cur_gatesdone;

        public static void Initialize_GatesDone()
        {
            _GatesDone = new int[Main.maxTilesX, Main.maxTilesY];
            cur_gatesdone = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Clear_Gates()
        {
            ++cur_gatesdone;
        }

        // Token: 0x04000C79 RID: 3193
        public static Vector2* _teleport;

        // Token: 0x04000C7A RID: 3194
        public static int[] _inPumpX;

        // Token: 0x04000C7B RID: 3195
        public static int[] _inPumpY;

        // Token: 0x04000C7C RID: 3196
        public static int _numInPump;

        // Token: 0x04000C7D RID: 3197
        public static int[] _outPumpX;

        // Token: 0x04000C7E RID: 3198
        public static int[] _outPumpY;

        // Token: 0x04000C7F RID: 3199
        public static int _numOutPump;

        // Token: 0x04000C80 RID: 3200
        private static int[] _mechX;

        // Token: 0x04000C81 RID: 3201
        private static int[] _mechY;

        // Token: 0x04000C82 RID: 3202
        private static int _numMechs;

        // Token: 0x04000C83 RID: 3203
        private static int[] _mechTime;

        // Token: 0x04000C84 RID: 3204
        public static int _currentWireColor;

        // Token: 0x04000C85 RID: 3205
        internal static int CurrentUser = 254;
    }
}
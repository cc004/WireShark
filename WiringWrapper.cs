using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace WireShark
{
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
                if (WiringWarpper._GatesDone[x, y] != WiringWarpper.cur_gatesdone) WiringWarpper._GatesNext.Enqueue(new Point16(x, y));
            }
        }
    }

    public static class WiringWarpper
    {

        private static WireAccelerator _wireAccelerator;

        public static WireAccelerator GetWireAccelerator()
        {
            return _wireAccelerator;
        }

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
            _wireAccelerator = new WireAccelerator();
            _wireList = new DoubleStack<Point16>(1024, 0);
            _wireDirectionList = new DoubleStack<byte>(1024, 0);
            _toProcess = new Dictionary<Point16, byte>();
            _GatesCurrent = new Queue<Point16>();
            _GatesNext = new Queue<Point16>();
            _LampsToCheck = new Queue<LogicGate>();
            _inPumpX = new int[20];
            _inPumpY = new int[20];
            _outPumpX = new int[20];
            _outPumpY = new int[20];
            _teleport = new Vector2[2];
            _mechX = new int[1000];
            _mechY = new int[1000];
            _mechTime = new int[1000];
        }

        // Token: 0x06000757 RID: 1879 RVA: 0x003552A8 File Offset: 0x003534A8

        // Mech 应该就是可以激活的计时器
        public static void UpdateMech()
        {
            SetCurrentUser(-1);
            for (int i = _numMechs - 1; i >= 0; i--)
            {
                _mechTime[i]--;
                if (Main.tile[_mechX[i], _mechY[i]].IsActive && Main.tile[_mechX[i], _mechY[i]].type == 144)
                {
                    if (Main.tile[_mechX[i], _mechY[i]].frameY == 0)
                    {
                        _mechTime[i] = 0;
                    }
                    else
                    {
                        int num = Main.tile[_mechX[i], _mechY[i]].frameX / 18;
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
                    if (Main.tile[_mechX[i], _mechY[i]].IsActive && Main.tile[_mechX[i], _mechY[i]].type == 144)
                    {
                        Main.tile[_mechX[i], _mechY[i]].frameY = 0;

                    }
                    if (Main.tile[_mechX[i], _mechY[i]].IsActive && Main.tile[_mechX[i], _mechY[i]].type == 411)
                    {
                        Tile tile = Main.tile[_mechX[i], _mechY[i]];
                        int num2 = tile.frameX % 36 / 18;
                        int num3 = tile.frameY % 36 / 18;
                        int num4 = _mechX[i] - num2;
                        int num5 = _mechY[i] - num3;
                        int num6 = 36;
                        if (Main.tile[num4, num5].frameX >= 36)
                        {
                            num6 = -36;
                        }
                        for (int j = num4; j < num4 + 2; j++)
                        {
                            for (int k = num5; k < num5 + 2; k++)
                            {
                                Main.tile[j, k].frameX = (short)(Main.tile[j, k].frameX + num6);
                            }
                        }

                    }
                    for (int l = i; l < _numMechs; l++)
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
            if (Main.tile[i, j].type == 135 || Main.tile[i, j].type == 314 || Main.tile[i, j].type == 423 || Main.tile[i, j].type == 428 || Main.tile[i, j].type == 442)
            {
                SoundEngine.PlaySound(SoundID.Mech, i * 16, j * 16, 0, 1f, 0f);
                BigTripWire(i, j, 1, 1);
                return;
            }
            if (Main.tile[i, j].type == 440)
            {
                SoundEngine.PlaySound(SoundID.Mech, i * 16 + 16, j * 16 + 16, 0, 1f, 0f);
                BigTripWire(i, j, 3, 3);
                return;
            }
            if (Main.tile[i, j].type == 136)
            {
                if (Main.tile[i, j].frameY == 0)
                {
                    Main.tile[i, j].frameY = 18;
                }
                else
                {
                    Main.tile[i, j].frameY = 0;
                }
                SoundEngine.PlaySound(SoundID.Mech, i * 16, j * 16, 0, 1f, 0f);
                BigTripWire(i, j, 1, 1);
                return;
            }
            if (Main.tile[i, j].type == 144)
            {
                if (Main.tile[i, j].frameY == 0)
                {
                    Main.tile[i, j].frameY = 18;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        CheckMech(i, j, 18000);
                    }
                }
                else
                {
                    Main.tile[i, j].frameY = 0;
                }
                SoundEngine.PlaySound(SoundID.Mech, i * 16, j * 16, 0, 1f, 0f);
                return;
            }
            if (Main.tile[i, j].type == 441 || Main.tile[i, j].type == 468)
            {
                int num = Main.tile[i, j].frameX / 18 * -1;
                int num2 = Main.tile[i, j].frameY / 18 * -1;
                num %= 4;
                if (num < -1)
                {
                    num += 2;
                }
                num += i;
                num2 += j;
                SoundEngine.PlaySound(SoundID.Mech, i * 16, j * 16, 0, 1f, 0f);
                BigTripWire(num, num2, 2, 2);
                return;
            }
            if (Main.tile[i, j].type == 132 || Main.tile[i, j].type == 411)
            {
                short num3 = 36;
                int num4 = Main.tile[i, j].frameX / 18 * -1;
                int num5 = Main.tile[i, j].frameY / 18 * -1;
                num4 %= 4;
                if (num4 < -1)
                {
                    num4 += 2;
                    num3 = -36;
                }
                num4 += i;
                num5 += j;
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.tile[num4, num5].type == 411)
                {
                    CheckMech(num4, num5, 60);
                }
                for (int k = num4; k < num4 + 2; k++)
                {
                    for (int l = num5; l < num5 + 2; l++)
                    {
                        if (Main.tile[k, l].type == 132 || Main.tile[k, l].type == 411)
                        {
                            Tile tile = Main.tile[k, l];
                            tile.frameX += num3;
                        }
                    }
                }
                WorldGen.TileFrame(num4, num5, false, false);
                SoundEngine.PlaySound(SoundID.Mech, i * 16, j * 16, 0, 1f, 0f);
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
            _LampsToCheck.Enqueue(onLogicLampChange[lampX, lampY]);
            LogicGatePass();
        }

        // Token: 0x0600075A RID: 1882 RVA: 0x003559C0 File Offset: 0x00353BC0
        public static bool Actuate(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (!tile.HasActuator)
            {
                return false;
            }
            if ((tile.type != 226 || j <= Main.worldSurface || NPC.downedPlantBoss) && (j <= Main.worldSurface || NPC.downedGolemBoss || Main.tile[i, j - 1].type != 237))
            {
                if (!tile.IsActive)
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
            Tile tile = Main.tile[i, j];
            if (tile.type == 226 && j > Main.worldSurface && !NPC.downedPlantBoss)
            {
                return;
            }
            if (!tile.IsActive)
            {
                ReActive(i, j);
                return;
            }
            DeActive(i, j);
        }

        // Token: 0x0600075D RID: 1885 RVA: 0x00355BB8 File Offset: 0x00353DB8
        public static bool CheckMech(int i, int j, int time)
        {
            for (int k = 0; k < _numMechs; k++)
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
            for (int i = 0; i < _numInPump; i++)
            {
                int num = _inPumpX[i];
                int num2 = _inPumpY[i];
                int liquid = Main.tile[num, num2].LiquidType;
                if (liquid > 0)
                {
                    bool flag = (Main.tile[num, num2].LiquidType == LiquidID.Lava);
                    bool flag2 = (Main.tile[num, num2].LiquidType == LiquidID.Honey);
                    for (int j = 0; j < _numOutPump; j++)
                    {
                        int num3 = _outPumpX[j];
                        int num4 = _outPumpY[j];
                        int liquid2 = Main.tile[num3, num4].LiquidType;
                        if (liquid2 < 255)
                        {
                            bool flag3 = (Main.tile[num3, num4].LiquidType == LiquidID.Lava);
                            bool flag4 = (Main.tile[num3, num4].LiquidType == LiquidID.Honey);
            if (liquid2 == 0)
                            {
                                flag3 = flag;
                                flag4 = flag2;
                            }
                            if (flag == flag3 && flag2 == flag4)
                            {
                                int num5 = liquid;
                                if (num5 + liquid2 > 255)
                                {
                                    num5 = 255 - liquid2;
                                }
                                Tile tile = Main.tile[num3, num4];
                                tile.LiquidType += (byte)num5;
                                Tile tile2 = Main.tile[num, num2];
                                tile2.LiquidType -= (byte)num5;
                                liquid = Main.tile[num, num2].LiquidType;
                                if (flag)
                                {
                                    Main.tile[num3, num4].LiquidType = LiquidID.Lava;
                                }
                                if (flag2)
                                {
                                    Main.tile[num3, num4].LiquidType = LiquidID.Honey;
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

        public static void TripWireWithLogic(int l, int t, int w, int h)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            TripWire(l, t, w, h);
            LogicGatePass();
        }

        public static void BigTripWire(int l, int t, int w, int h)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            TripWire(l, t, w, h);
            PixelBoxPass();
            LogicGatePass();
        }

        // Token: 0x0600075F RID: 1887 RVA: 0x00355E08 File Offset: 0x00354008
        private static void TripWire(int left, int top, int width, int height)
        {
            Wiring.running = true;
            // 清除队列
            if (_wireList.Count != 0)
            {
                _wireList.Clear(true);
            }
            if (_wireDirectionList.Count != 0)
            {
                _wireDirectionList.Clear(true);
            }
            Vector2[] array = new Vector2[8];
            int num = 0;
            for (int i = left; i < left + width; i++)
            {
                for (int j = top; j < top + height; j++)
                {
                    Point16 back = new Point16(i, j);
                    Tile tile = Main.tile[i, j];
                    if (tile != null && tile.BlueWire)
                    {
                        _wireList.PushBack(back);
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
                HitWire(_wireList, 1);
                if (_numInPump > 0 && _numOutPump > 0)
                {
                    XferWater();
                }
            }
            array[num++] = _teleport[0];
            array[num++] = _teleport[1];
            for (int k = left; k < left + width; k++)
            {
                for (int l = top; l < top + height; l++)
                {
                    Point16 back2 = new Point16(k, l);
                    Tile tile2 = Main.tile[k, l];
                    if (tile2 != null && tile2.GreenWire)
                    {
                        _wireList.PushBack(back2);
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
                HitWire(_wireList, 2);
                if (_numInPump > 0 && _numOutPump > 0)
                {
                    XferWater();
                }
            }
            array[num++] = _teleport[0];
            array[num++] = _teleport[1];
            _teleport[0].X = -1f;
            _teleport[0].Y = -1f;
            _teleport[1].X = -1f;
            _teleport[1].Y = -1f;
            for (int m = left; m < left + width; m++)
            {
                for (int n = top; n < top + height; n++)
                {
                    Point16 back3 = new Point16(m, n);
                    Tile tile3 = Main.tile[m, n];
                    if (tile3 != null && tile3.RedWire)
                    {
                        _wireList.PushBack(back3);
                    }
                }
            }
            if (_wireList.Count > 0)
            {
                _numInPump = 0;
                _numOutPump = 0;
                HitWire(_wireList, 3);
                if (_numInPump > 0 && _numOutPump > 0)
                {
                    XferWater();
                }
            }
            array[num++] = _teleport[0];
            array[num++] = _teleport[1];
            _teleport[0].X = -1f;
            _teleport[0].Y = -1f;
            _teleport[1].X = -1f;
            _teleport[1].Y = -1f;
            for (int num2 = left; num2 < left + width; num2++)
            {
                for (int num3 = top; num3 < top + height; num3++)
                {
                    Point16 back4 = new Point16(num2, num3);
                    Tile tile4 = Main.tile[num2, num3];
                    if (tile4 != null && tile4.YellowWire)
                    {
                        _wireList.PushBack(back4);
                    }
                }
            }
            if (_wireList.Count > 0)
            {
                _numInPump = 0;
                _numOutPump = 0;
                HitWire(_wireList, 4);
                if (_numInPump > 0 && _numOutPump > 0)
                {
                    XferWater();
                }
            }
            array[num++] = _teleport[0];
            array[num++] = _teleport[1];
            for (int num4 = 0; num4 < 8; num4 += 2)
            {
                _teleport[0] = array[num4];
                _teleport[1] = array[num4 + 1];
                if (_teleport[0].X >= 0f && _teleport[1].X >= 0f)
                {
                    Teleport();
                }
            }

        }

        // Token: 0x06000760 RID: 1888 RVA: 0x00356308 File Offset: 0x00354508
        private static void PixelBoxPass()
        {
            foreach (KeyValuePair<Point16, byte> current in _wireAccelerator._pixelBoxTriggers)
            {
                if (current.Value != 2)
                {
                    if (current.Value == 1)
                    {
                        if (Main.tile[current.Key.X, current.Key.Y].frameX != 0)
                        {
                            Main.tile[current.Key.X, current.Key.Y].frameX = 0;

                        }
                    }
                    else if (current.Value == 3 && Main.tile[current.Key.X, current.Key.Y].frameX != 18)
                    {
                        Main.tile[current.Key.X, current.Key.Y].frameX = 18;

                    }
                }
            }
            _wireAccelerator._pixelBoxTriggers.Clear();
        }

        // Token: 0x06000761 RID: 1889 RVA: 0x0035647C File Offset: 0x0035467C
        private static void LogicGatePass()
        {
            if (_GatesCurrent.Count == 0)
            {
                Clear_Gates();
                while (_LampsToCheck.Count > 0)
                {
                    while (_LampsToCheck.Count > 0)
                    {
                        _LampsToCheck.Dequeue().UpdateLogicGate();
                        /*
                        Point16 point = ;
                        CheckLogicGate((int)point.X, (int)point.Y);*/
                    }
                    while (_GatesNext.Count > 0)
                    {
                        Utils.Swap<Queue<Point16>>(ref _GatesCurrent, ref _GatesNext);
                        while (_GatesCurrent.Count > 0)
                        {
                            Point16 key = _GatesCurrent.Peek();
                            if (_GatesDone[key.X, key.Y] == cur_gatesdone)
                            {
                                _GatesCurrent.Dequeue();
                            }
                            else
                            {
                                _GatesDone[key.X, key.Y] = cur_gatesdone;
                                TripWireWithLogic(key.X, key.Y, 1, 1);
                                _GatesCurrent.Dequeue();
                            }
                        }
                        PixelBoxPass();
                    }
                }
                Clear_Gates();
                if (Wiring.blockPlayerTeleportationForOneIteration)
                {
                    Wiring.blockPlayerTeleportationForOneIteration = false;
                }
            }
        }

        internal static LogicGate[,] onLogicLampChange;

        private class AllOnGate : LogicGate
        {
            protected override bool GetState()
            {
                return lampon == lamptotal;
            }
        }

        private class AnyOnGate : LogicGate
        {
            protected override bool GetState()
            {
                return lampon > 0;
            }
        }

        private class AnyOffGate : LogicGate
        {
            protected override bool GetState()
            {
                return lampon != lamptotal;
            }
        }

        private class AllOffGate : LogicGate
        {
            protected override bool GetState()
            {
                return lampon == 0;
            }
        }

        private class OneOnGate : LogicGate
        {
            protected override bool GetState()
            {
                return lampon == 1;
            }
        }

        private class NotOneOnGate : LogicGate
        {
            protected override bool GetState()
            {
                return lampon != 1;
            }
        }

        private class ErrorGate : LogicGate
        {
            protected override bool GetState()
            {
                throw new NotImplementedException();
            }

            public ErrorGate()
            {
                erroronly = true;
            }

            public override void UpdateLogicGate()
            {
                if (Main.rand.NextDouble() * lamptotal < lampon)
                    if (_GatesDone[x, y] != cur_gatesdone) _GatesNext.Enqueue(new Point16(x, y));
            }

        }

        private class OneErrorGate : LogicGate
        {
            protected override bool GetState()
            {
                throw new NotImplementedException();
            }

            public OneErrorGate()
            {
                erroronly = true;
            }

            public override void UpdateLogicGate()
            {
                if (lampon != 0)
                    if (_GatesDone[x, y] != cur_gatesdone) _GatesNext.Enqueue(new Point16(x, y));
            }
        }

        private static void CacheLogicGate(int x, int y)
        {
            LogicGate lgate;
            Tile tile = Main.tile[x, y];
            List<Tile> lamps = new List<Tile>(); // lamps before one error gate
            List<Tile> lampTriggers = new List<Tile>(); // all lamps
            bool countend = false;
            int onnum = 0;
            for (int j = y - 1; j > 0; --j)
            {
                Tile tile2 = Main.tile[x, j];
                if (!tile2.IsActive || tile2.type != TileID.LogicGateLamp)
                    break;
                lampTriggers.Add(tile2);

                if (tile2.frameX == 36)
                    countend = true;

                if (!countend)
                {
                    lamps.Add(tile2);
                    if (tile2.frameX == 18)
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
                switch (tile.frameY / 18)
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
            lgate.lampon = onnum;
            lgate.mapTile = tile;
            lgate.x = x;
            lgate.y = y;
            lgate.active = tile.frameX == 18;

            for (int i = 0; i < lampTriggers.Count; ++i)
            {
                if (i < lamps.Count || lampTriggers[i].frameX == 36)
                    onLogicLampChange[x, y - i - 1] = lgate;
            }
        }

        public static void Initialize_LogicLamps()
        {
            onLogicLampChange = new LogicGate[Main.maxTilesX, Main.maxTilesY];
            for (int i = 0; i < Main.maxTilesX; ++i)
                for (int j = 0; j < Main.maxTilesY; ++j)
                    if (Main.tile[i, j].IsActive && Main.tile[i, j].type == TileID.LogicGate)
                        CacheLogicGate(i, j);
        }
        /*
        // Token: 0x06000762 RID: 1890 RVA: 0x00356578 File Offset: 0x00354778
        private static void CheckLogicGate(int lampX, int lampY) {
            if (!WorldGen.InWorld(lampX, lampY, 1)) {
                return;
            }
            int i = lampY;
            while (i < Main.maxTilesY) {
                Tile tile = Main.tile[lampX, i];
                if (!tile.active()) {
                    return;
                }
                // 逻辑门本体而不是输入端
                if (tile.type == 420) {
                    bool flag = _GatesDone[lampX, i] == cur_gatesdone; //gate is done
                    int num = (int)(tile.frameY / 18);

                    // 逻辑门是否已经处于激活状态
                    bool flag2 = tile.frameX == 18; // gate was active
                    bool flag3 = tile.frameX == 36; // gate is err state
                    if (num < 0) {
                        return;
                    }
                    int num2 = 0;
                    int num3 = 0;
                    bool flag4 = false;
                    for (int j = i - 1; j > 0; j--) {
                        Tile tile2 = Main.tile[lampX, j];
                        if (!tile2.active() || tile2.type != 419) { // all lamps
                            break;
                        }
                        //???
                        if (tile2.frameX == 36) {
                            flag4 = true; // any lamp in err state
                            break;
                        }
                        num2++; //lamp count
                        num3 += (tile2.frameX == 18).ToInt(); //on lamp count
                    }
                    // 逻辑门有没有被激活
                    bool flag5; // active
                    switch (num) {
                        case 0:
                            flag5 = (num2 == num3); // all on
                            break;
                        case 1:
                            flag5 = (num3 > 0); // any on
                            break;
                        case 2:
                            flag5 = (num2 != num3); // any off
                            break;
                        case 3:
                            flag5 = (num3 == 0); // all off
                            break;
                        case 4:
                            flag5 = (num3 == 1); // one on
                            break;
                        case 5:
                            flag5 = (num3 != 1); // not (one on)
                            break;
                        default:
                            return;
                    }
                    bool flag6 = !flag4 && flag3; // no err lamp but gate is err
                    bool flag7 = false;
                    if (flag4 && Framing.GetTileSafely(lampX, lampY).frameX == 36) { // cur lamp in err state
                        flag7 = true;
                    }
                    /*
                    1. if no err lamp but gate is err: change frame but not add to next
                    2. if err lamp and cur gate is err, cal err and add to next if cur light is err
                    3. if no err lamp and gate is not err: change frame and add to next
                    4. if err lamp but gate is not err, set gate to err if cur lamp is err
                    *//*
                    if (flag5 != flag2 || flag6 || flag7) { // gate changed or gate in err state or cur lamp in err state
                        //short num4 = (short)(tile.frameX % 18 / 18);
                        tile.frameX = (short)(18 * flag5.ToInt()); //set now state
                        if (flag4) { //any lamp in err state
                            tile.frameX = 36; //then gate in err state
                        }
                        
                        //WorldGen.SquareTileFrame(lampX, i, true);
                        
                        bool flag8 = !flag4 || flag7; // no lamp in err state
                        if (flag7) { //cur lamp in err state
                            flag8 = (Main.rand.NextFloat() < (float)num3 / (float)num2); // err on
                        }
                        if (flag6) {
                            flag8 = false;
                        }
                        if (flag8) { //err on
                            if (!flag) { // not done
                                _GatesNext.Enqueue(new Point16(lampX, i));
                                return;
                            }
                            //Vector2 position = new Vector2((float)lampX, (float)i) * 16f - new Vector2(10f);
                            //Utils.PoofOfSmoke(position);
                            
                        }
                    }
                    return;
                } else {
                    if (tile.type != 419) {
                        return;
                    }
                    i++;
                }
            }
        }
        */
        // Token: 0x06000763 RID: 1891 RVA: 0x003567F4 File Offset: 0x003549F4
        private static void HitWire(DoubleStack<Point16> next, int wireType)
        {
            _wireDirectionList.Clear(true);
            GetWireAccelerator().ResetVisited();
            for (int i = 0; i < next.Count; i++)
            {
                Point16 point = next.PopFront();
                GetWireAccelerator().Activiate(point.X, point.Y, wireType - 1);
            }

            _currentWireColor = wireType;
            //while (next.Count > 0) {
            //    Point16 point2 = next.PopFront();
            //    int num = (int)_wireDirectionList.PopFront();
            //    int x = (int)point2.X;
            //    int y = (int)point2.Y;
            //    if (!_wireSkip.ContainsKey(point2)) {
            //        HitWireSingle(x, y);
            //    }
            //    for (int j = 0; j < 4; j++) {
            //        int num2;
            //        int num3;
            //        switch (j) {
            //            case 0:
            //                num2 = x;
            //                num3 = y + 1;
            //                break;
            //            case 1:
            //                num2 = x;
            //                num3 = y - 1;
            //                break;
            //            case 2:
            //                num2 = x + 1;
            //                num3 = y;
            //                break;
            //            case 3:
            //                num2 = x - 1;
            //                num3 = y;
            //                break;
            //            default:
            //                num2 = x;
            //                num3 = y + 1;
            //                break;
            //        }
            //        if (num2 >= 2 && num2 < Main.maxTilesX - 2 && num3 >= 2 && num3 < Main.maxTilesY - 2) {
            //            Tile tile = Main.tile[num2, num3];
            //            if (tile != null) {
            //                Tile tile2 = Main.tile[x, y];
            //                if (tile2 != null) {
            //                    byte b = 3;
            //                    if (tile.type == 424 || tile.type == 445) {
            //                        b = 0;
            //                    }
            //                    if (tile2.type == 424) {
            //                        switch (tile2.frameX / 18) {
            //                            case 0:
            //                                if (j != num) {
            //                                    goto IL_315;
            //                                }
            //                                break;
            //                            case 1:
            //                                if ((num != 0 || j != 3) && (num != 3 || j != 0) && (num != 1 || j != 2)) {
            //                                    if (num != 2) {
            //                                        goto IL_315;
            //                                    }
            //                                    if (j != 1) {
            //                                        goto IL_315;
            //                                    }
            //                                }
            //                                break;
            //                            case 2:
            //                                if ((num != 0 || j != 2) && (num != 2 || j != 0) && (num != 1 || j != 3) && (num != 3 || j != 1)) {
            //                                    goto IL_315;
            //                                }
            //                                break;
            //                        }
            //                    }
            //                    if (tile2.type == 445) {
            //                        if (j != num) {
            //                            goto IL_315;
            //                        }
            //                        if (_PixelBoxTriggers.ContainsKey(point2)) {
            //                            Dictionary<Point16, byte> pixelBoxTriggers;
            //                            Point16 key;
            //                            (pixelBoxTriggers = _PixelBoxTriggers)[key = point2] = ((byte)(pixelBoxTriggers[key] | ((j == 0 | j == 1) ? 2 : 1)));
            //                        } else {
            //                            _PixelBoxTriggers[point2] = (byte)((j == 0 | j == 1) ? 2 : 1);
            //                        }
            //                    }
            //                    bool flag;
            //                    switch (wireType) {
            //                        case 1:
            //                            flag = tile.wire();
            //                            break;
            //                        case 2:
            //                            flag = tile.wire2();
            //                            break;
            //                        case 3:
            //                            flag = tile.wire3();
            //                            break;
            //                        case 4:
            //                            flag = tile.wire4();
            //                            break;
            //                        default:
            //                            flag = false;
            //                            break;
            //                    }
            //                    if (flag) {
            //                        Point16 point3 = new Point16(num2, num3);
            //                        byte b2;
            //                        if (_toProcess.TryGetValue(point3, out b2)) {
            //                            b2 -= 1;
            //                            if (b2 == 0) {
            //                                _toProcess.Remove(point3);
            //                            } else {
            //                                _toProcess[point3] = b2;
            //                            }
            //                        } else {
            //                            next.PushBack(point3);
            //                            _wireDirectionList.PushBack((byte)j);
            //                            if (b > 0) {
            //                                _toProcess.Add(point3, b);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    IL_315:;
            //    }
            //}
            //_wireSkip.Clear();
            //_toProcess.Clear();
            Wiring.running = false;
        }

        // Token: 0x06000764 RID: 1892 RVA: 0x00356B4C File Offset: 0x00354D4C
        internal static void HitWireSingle(int i, int j, Tile tile, int type)
        {
        }

        // Token: 0x06000765 RID: 1893 RVA: 0x00359ABC File Offset: 0x00357CBC
        private static void Teleport()
        {
            if (_teleport[0].X < _teleport[1].X + 3f && _teleport[0].X > _teleport[1].X - 3f && _teleport[0].Y > _teleport[1].Y - 3f && _teleport[0].Y < _teleport[1].Y)
            {
                return;
            }
            Rectangle[] array = new Rectangle[2];
            array[0].X = (int)(_teleport[0].X * 16f);
            array[0].Width = 48;
            array[0].Height = 48;
            array[0].Y = (int)(_teleport[0].Y * 16f - array[0].Height);
            array[1].X = (int)(_teleport[1].X * 16f);
            array[1].Width = 48;
            array[1].Height = 48;
            array[1].Y = (int)(_teleport[1].Y * 16f - array[1].Height);
            for (int i = 0; i < 2; i++)
            {
                Vector2 value = new Vector2(array[1].X - array[0].X, array[1].Y - array[0].Y);
                if (i == 1)
                {
                    value = new Vector2(array[0].X - array[1].X, array[0].Y - array[1].Y);
                }
                if (!Wiring.blockPlayerTeleportationForOneIteration)
                {
                    for (int j = 0; j < 255; j++)
                    {
                        if (Main.player[j].active && !Main.player[j].dead && !Main.player[j].teleporting && array[i].Intersects(Main.player[j].getRect()))
                        {
                            Vector2 vector = Main.player[j].position + value;
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
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].active && !Main.npc[k].teleporting && Main.npc[k].lifeMax > 5 && !Main.npc[k].boss && !Main.npc[k].noTileCollide)
                    {
                        int type = Main.npc[k].type;
                        if (!NPCID.Sets.TeleportationImmune[type] && array[i].Intersects(Main.npc[k].getRect()))
                        {
                            Main.npc[k].teleporting = true;
                            Main.npc[k].Teleport(Main.npc[k].position + value, 0, 0);
                        }
                    }
                }
            }
            for (int l = 0; l < 255; l++)
            {
                Main.player[l].teleporting = false;
            }
            for (int m = 0; m < 200; m++)
            {
                Main.npc[m].teleporting = false;
            }
        }

        // Token: 0x06000766 RID: 1894 RVA: 0x00359EC4 File Offset: 0x003580C4
        public static void DeActive(int i, int j)
        {
            if (!Main.tile[i, j].IsActive)
            {
                return;
            }
            bool flag = Main.tileSolid[Main.tile[i, j].type] && !TileID.Sets.NotReallySolid[Main.tile[i, j].type];
            ushort type = Main.tile[i, j].type;
            if (type == 314 || (uint)(type - 386) <= 3u)
            {
                flag = false;
            }
            if (!flag)
            {
                return;
            }
            if (Main.tile[i, j - 1].IsActive && (Main.tile[i, j - 1].type == 5 || TileID.Sets.BasicChest[Main.tile[i, j - 1].type] || Main.tile[i, j - 1].type == 26 || Main.tile[i, j - 1].type == 77 || Main.tile[i, j - 1].type == 72 || Main.tile[i, j - 1].type == 88))
            {
                return;
            }
            Main.tile[i, j].IsActive = false;
            WorldGen.SquareTileFrame(i, j, false);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {

            }
        }

        // Token: 0x06000767 RID: 1895 RVA: 0x0035A018 File Offset: 0x00358218
        public static void ReActive(int i, int j)
        {
            Main.tile[i, j].IsActive = true;
            WorldGen.SquareTileFrame(i, j, false);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {

            }
        }

        // Token: 0x04000C71 RID: 3185
        public static DoubleStack<Point16> _wireList;

        // Token: 0x04000C72 RID: 3186
        public static DoubleStack<byte> _wireDirectionList;

        // Token: 0x04000C73 RID: 3187
        public static Dictionary<Point16, byte> _toProcess;

        // Token: 0x04000C74 RID: 3188
        private static Queue<Point16> _GatesCurrent;

        // Token: 0x04000C75 RID: 3189
        internal static Queue<LogicGate> _LampsToCheck;

        // Token: 0x04000C76 RID: 3190
        public static Queue<Point16> _GatesNext;

        // Token: 0x04000C77 RID: 3191
        internal static int[,] _GatesDone;
        internal static int cur_gatesdone;

        public static void Initialize_GatesDone()
        {
            _GatesDone = new int[Main.maxTilesX, Main.maxTilesY];
            cur_gatesdone = 0;
        }

        private static void Clear_Gates()
        {
            ++cur_gatesdone;
        }

        // Token: 0x04000C79 RID: 3193
        public static Vector2[] _teleport;

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

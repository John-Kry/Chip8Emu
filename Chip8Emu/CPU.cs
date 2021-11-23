using System;
using System.Collections.Generic;

namespace Chip8Emu
{
    public class CPU
    {
        public byte[] RAM = new byte[4096];
        public byte[] V = new byte[16];
        public ushort I = 0;
        public Stack<ushort> stack = new Stack<ushort>(24);
        public byte DelayTimer;
        public byte SoundTimer;
        public ushort PC = 0;
        public bool[,] Display = new bool[64, 32];
        private Random _random = new Random();
        private Keyboard _keyboard;
        public byte saveKeypressIntoThisVx;

        public CPU(Keyboard keyboard)
        {
            _keyboard = keyboard;
        }

        public void ExecuteOp(OperationData opData)
        {
            switch (opData.op)
            {
                case 0:
                    // may be wrong
                    if (opData.nn == 0xE0)
                    {
                        for (var x = 0; x < Display.GetLength(0); x++)
                        {
                            for (var y = 0; y < Display.GetLength(1); y++)
                            {
                                Display[x, y] = false;
                            }
                        }
                    }
                    else if (opData.nn == 0xEE)
                    {
                        this.PC = stack.Pop();
                    }
                    else
                    {
                        LogNotImplemented(opData);
                    }

                    break;
                case 1:
                    this.PC = opData.nnn;
                    break;
                case 2:
                    stack.Push(this.PC);
                    this.PC = opData.nnn;
                    break;
                case 3:
                    if (this.V[opData.x] == opData.nn)
                    {
                        this.PC += 2;
                    }

                    break;
                case 4:
                    if (this.V[opData.x] != opData.nn)
                    {
                        this.PC += 2;
                    }

                    break;
                case 5:
                    if (this.V[opData.x] == this.V[opData.y])
                    {
                        this.PC += 2;
                    }

                    break;
                case 6:
                    this.V[opData.x] = opData.nn;
                    break;


                case 7:
                    this.V[opData.x] += opData.nn;
                    break;
                case 8:
                    switch (opData.n)
                    {
                        case 0:
                            this.V[opData.x] = this.V[opData.y];
                            break;
                        case 1:
                            this.V[opData.x] =
                                (byte) (this.V[opData.x] | this.V[opData.y]);
                            break;
                        case 2:
                            this.V[opData.x] =
                                (byte) (this.V[opData.x] & this.V[opData.y]);
                            break;
                        case 3:
                            this.V[opData.x] =
                                (byte) (this.V[opData.x] ^ this.V[opData.y]);
                            break;
                        case 4:
                            var result = this.V[opData.x] + this.V[opData.y];
                            this.V[0xF] = 0;
                            if (result > 0xFF)
                            {
                                this.V[0xF] = 1;
                            }

                            this.V[opData.x] = (byte) result;
                            break;

                        case 5:
                            V[0xF] = 0;
                            if (V[opData.x] > V[opData.y])
                            {
                                V[0xF] = 1;
                            }

                            V[opData.x] -= V[opData.y];
                            break;
                        case 6:
                            V[0xF] = (byte) (V[opData.x] & 0x1);
                            V[opData.x] >>= 1;
                            break;
                        case 7:
                            V[0xF] = 0;
                            if (V[opData.y] > V[opData.x])
                            {
                                V[0xF] = 1;
                            }

                            V[opData.x] = (byte) (V[opData.y] - V[opData.x]);
                            break;
                        case 0xE:
                            V[0xF] = (byte) (V[opData.x] & 0x80);
                            V[opData.x] <<= 1;
                            break;
                        default:
                            LogNotImplemented(opData);
                            break;
                    }

                    break;
                case 0xA:
                    this.I = opData.nnn;
                    break;
                case 0xB:
                    PC = (ushort) (opData.nnn + V[0x0]);
                    break;
                case 0xC:
                    V[opData.x] = (byte) (_random.Next(255) & opData.nn);
                    break;
                case 0xD:
                    var xVal = this.V[opData.x];
                    var yVal = this.V[opData.y];

                    var xCoord = xVal & 63;
                    var yCoord = yVal & 31;
                    this.V[0xF] = 0;

                    var index = 0;
                    for (var row = 0; row < opData.n; row++)
                    {
                        var y = yCoord + row;
                        if (y >= 32)
                        {
                            break;
                        }

                        var sprite = this.RAM[this.I + index];
                        // For each column (x)
                        for (var col = 0; col < 8; col++)
                        {
                            // set x to the position
                            var x = xCoord + col;
                            // break if past max x length
                            if (x >= 64)
                            {
                                break;
                            }

                            var oldPixel = Display[x, y];
                            var currentPos = 7 - col;
                            var toShift = 1 << currentPos;

                            // check the current bit is on or not
                            var newPixel = (sprite & toShift) != 0x0;
                            if (oldPixel && newPixel)
                            {
                                Display[x, y] = false;
                                this.V[0xF] = 1;
                            }
                            else if (newPixel && !oldPixel)
                            {
                                Display[x, y] = true;
                            }
                        }

                        // next sprite 
                        index++;
                    }

                    break;
                case 0xE:
                    if (opData.nn == 0x9E)
                    {
                        if (_keyboard.IsKeyPressed(V[opData.x]))
                        {
                            PC += 2;
                        }
                    }
                    else if (opData.nn == 0xA1)
                    {
                        if (!_keyboard.IsKeyPressed(V[opData.x]))
                        {
                            PC += 2;
                        }
                    }
                    else
                    {
                        LogNotImplemented(opData);
                    }

                    break;
                case 0xF:
                    if (opData.nn == 0x07)
                    {
                        V[opData.x] = DelayTimer;
                    }
                    else if (opData.nn == 0x0A)
                    {
                        _keyboard.IsWaiting = true;
                        saveKeypressIntoThisVx = this.V[opData.x];
                    }
                    else if (opData.nn == 0x15)
                    {
                        DelayTimer = V[opData.x];
                    }
                    else if (opData.nn == 0x18)
                    {
                        SoundTimer = V[opData.x];
                    }
                    else if (opData.nn == 0x1E)
                    {
                        I += V[opData.x];
                    }
                    else if (opData.nn == 0x29)
                    {
                        I = (ushort) (V[opData.x] * 5);
                    }
                    else if (opData.nn == 0x33)
                    {
                        var temp = V[opData.x];
                        this.RAM[I + 2] = (byte) (temp % 10);
                        temp /= 10;
                        this.RAM[I + 1] = (byte) (temp % 10);
                        temp /= 10;
                        this.RAM[I] = temp;
                    }
                    else if (opData.nn == 0x55)
                    {
                        for (var registerIndex = 0; registerIndex <= opData.x; registerIndex++)
                        {
                            this.RAM[this.I + registerIndex] = V[registerIndex];
                        }
                    }

                    else if (opData.nn == 0x65)
                    {
                        for (var registerIndex = 0; registerIndex <= opData.x; registerIndex++)
                        {
                            V[registerIndex] = this.RAM[this.I + registerIndex];
                        }
                    }
                    else
                    {
                        LogNotImplemented(opData);
                    }

                    break;

                default:
                    LogNotImplemented(opData);
                    break;
            }
        }

        static void LogNotImplemented(OperationData opData)
        {
            Console.WriteLine($"Not Implemented: op: {opData.op}, nn: {opData.nn}, n: {opData.n}");
        }
    }
}
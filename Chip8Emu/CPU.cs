using System;
using System.Collections.Generic;

namespace Chip8Emu
{

    public class CPU
    {
        public byte[] RAM = new byte[4096];
        public byte[] Registers = new byte[16];
        public ushort I = 0;
        public Stack<ushort> stack = new Stack<ushort>(24);
        public byte DelayTimer;
        public byte SoundTimer;
        public ushort PC = 0;
        public byte Keyboard;
        public bool[,] Display = new bool[64, 32];

        public void ExecuteOp(OperationData operationData)
        {
            switch (operationData.op)
            {
                case 0:
                    if (operationData.y == 0xF)
                    {
                        for (var x = 0; x < Display.GetLength(0); x++)
                        {
                            for (var y = 0; y < Display.GetLength(1); y++)
                            {
                                Display[x, y] = false;
                            }
                        }
                    }

                    break;
                case 1:
                    this.PC = operationData.nnn;
                    break;
                case 6:
                    this.Registers[operationData.x] = operationData.nn;
                    break;


                // For this instruction, this is not the case. If V0 contains FF and you execute 6001, the CHIP-8’s flag register VF is not affected.
                case 7:
                    this.Registers[operationData.x] += operationData.nn;
                    break;
                case 0xA:
                    this.I = operationData.nnn;
                    break;
                case 0xD:
                    var xVal = this.Registers[operationData.x];
                    var yVal = this.Registers[operationData.y];

                    var xCoord = xVal & 63;
                    var yCoord = yVal & 31;
                    this.Registers[0xF] = 0;

                    var index = 0;
                    for (var row = 0; row < operationData.n; row++)
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
                                this.Registers[0xF] = 1;
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

                default:
                    Console.WriteLine("NAH");
                    throw new Exception("Null");
                    break;
            }
        }
    }
}
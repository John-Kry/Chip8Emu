using System;
using System.IO;
using System.Threading;

namespace Chip8Emu
{
    public class Chip
    {
        private byte[] readBytes;
        public CPU _cpu;
        public bool IsRunning;

        public void LoadRom()
        {
            using (BinaryReader reader = new BinaryReader(new FileStream("roms/BC_test.ch8", FileMode.Open)))
            {
                readBytes = reader.ReadBytes(4096);
            }

            _cpu = new CPU();
        }

        public void Start()
        {
            for (int i = 0; i < readBytes.Length; i++)
            {
                _cpu.RAM[512 + i] = readBytes[i];
            }

            Console.OutputEncoding = System.Text.Encoding.GetEncoding(28591);


            IsRunning = true;

            _cpu.PC = 512;
        }

        public void Update()
        {
            for (var instructionIndex = 0; instructionIndex < 16; instructionIndex++)
            {
                byte byte1 = _cpu.RAM[_cpu.PC];
                byte byte2 = _cpu.RAM[_cpu.PC + 1];
                _cpu.PC += 2;
                var operationData = new OperationData
                {
                    op = (byte) ((byte1 >> 4) & 0xF),
                    x = (byte) (byte1 & 0xF),
                    y = (byte) ((byte2 >> 4) & 0xF),
                    n = (byte) (byte2 & 0xF),
                    nn = byte2,
                    nnn = (ushort) ((byte1 & 0xF) << 8 | byte2),
                };
                _cpu.ExecuteOp(operationData);
            }
        }
    }
}
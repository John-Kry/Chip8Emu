using System;
using System.IO;
using System.Threading;

namespace Chip8Emu
{
    public class Chip
    {
        private byte[] readBytes;
        public CPU _cpu;
        public Keyboard _Keyboard;

        public void LoadRom()
        {
            using (BinaryReader reader = new BinaryReader(new FileStream("roms/UFO", FileMode.Open)))
            {
                readBytes = reader.ReadBytes(4096);
            }

            _Keyboard = new Keyboard();
            _cpu = new CPU(_Keyboard);
            LoadSprites();
            _Keyboard.Initialize();
        }

        public bool IsWaitingForInput()
        {
            return _Keyboard.IsWaiting;
        }

        public void Start()
        {
            for (int i = 0; i < readBytes.Length; i++)
            {
                _cpu.RAM[512 + i] = readBytes[i];
            }

            _cpu.PC = 512;
            
        }

        private void LoadSprites()
        {
            var sprites = new byte[]{  
                0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
                0x20, 0x60, 0x20, 0x20, 0x70, // 1
                0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
                0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
                0x90, 0x90, 0xF0, 0x10, 0x10, // 4
                0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
                0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
                0xF0, 0x10, 0x20, 0x40, 0x40, // 7
                0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
                0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
                0xF0, 0x90, 0xF0, 0x90, 0x90, // A
                0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
                0xF0, 0x80, 0x80, 0x80, 0xF0, // C
                0xE0, 0x90, 0x90, 0x90, 0xE0, // D
                0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
                0xF0, 0x80, 0xF0, 0x80, 0x80  // F
            };
            for (var i = 0; i < sprites.Length; i++)
            {
                _cpu.RAM[i] = sprites[i];
            }
        }

        public void Update()
        {
            if (_cpu.saveKeypressIntoThisVx != 0x0)
            {
                _cpu.V[_cpu.saveKeypressIntoThisVx] = _Keyboard.GetMostRecentKey();
                _cpu.saveKeypressIntoThisVx = 0x0;
            }
            if (_cpu.DelayTimer > 0)
            {
                _cpu.DelayTimer -= 1;
            }

            if (_cpu.SoundTimer > 0)
            {
                _cpu.SoundTimer -= 1;
            }
            for (var instructionIndex = 0; instructionIndex < 10; instructionIndex++)
            {
                // Skip execution until a key has been entered
                if (_Keyboard.IsWaiting)
                {
                    return;
                }
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
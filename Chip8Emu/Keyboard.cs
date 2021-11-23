using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework.Input;

namespace Chip8Emu
{
    public class Keyboard
    {
        private Dictionary<int, byte> _availableKeys;
        private readonly byte[] _pressedKeys = new byte[16];
        public bool IsWaiting = false;

        public void Initialize()
        {
            _availableKeys = new Dictionary<int, byte>()
            {
                {(int)Keys.D1, 0x1}, // 1
                {(int)Keys.D2, 0x2}, // 2
                {(int)Keys.D3, 0x3}, // 3
                {(int)Keys.D4, 0xC}, // 4
                {(int)Keys.Q, 0x4}, // Q
                {(int)Keys.W, 0x5}, // W
                {(int)Keys.E, 0x6}, // E
                {(int)Keys.R, 0xD}, // R
                {(int)Keys.A, 0x7}, // A
                {(int)Keys.S, 0x8}, // S
                {(int)Keys.D, 0x9}, // D
                {(int)Keys.F, 0xE}, // F
                {(int)Keys.Z, 0xA}, // Z
                {(int)Keys.X, 0x0}, // X
                {(int)Keys.C, 0xB}, // C
                {(int)Keys.V, 0xF}, // V
            };
        }

        public byte GetMostRecentKey()
        {
            return (byte)Array.FindIndex(_pressedKeys, row => row == 0x1);
        }

        private byte GetKeyByte(int keyInt)
        {
            _availableKeys.TryGetValue(keyInt, out var value);
            return value;
        }

        public void AddKeyToPressed(int keyInt)
        {
            var keybyte = GetKeyByte(keyInt);
            _pressedKeys[keybyte] = 0x1;
        }

        public void ProcessState(KeyboardState currentState)
        {
            for (var i = 0; i < _pressedKeys.Length; i++)
            {
                _pressedKeys[i] = 0x0;
            }
            var keysPressedCurrently = currentState.GetPressedKeys();

            for (var i = 0; i < keysPressedCurrently.Length; i++)
            {
                AddKeyToPressed((int) keysPressedCurrently[i]);
                IsWaiting = false;
            }

        }


        public bool IsKeyPressed(byte keyCode)
        {
            return _pressedKeys[keyCode] == 0x1;
        }
    }
}
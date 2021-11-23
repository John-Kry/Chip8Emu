using System;
using System.Drawing;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Chip8Emu
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Chip _chip;
        private readonly int _scaleBy = 12;
        SoundEffect mySound;
        SoundEffectInstance mySoundInstance;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 64 * _scaleBy;
            _graphics.PreferredBackBufferHeight = 32 * _scaleBy;
            _graphics.ApplyChanges();
            
            var fs = new FileStream("smw2_coin.wav", FileMode.Open);
            mySound = SoundEffect.FromStream(fs);
            mySoundInstance = mySound.CreateInstance();
            mySoundInstance.IsLooped = true;

            
            _chip = new Chip();
            _chip.LoadRom();
            _chip.Start();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            
            _chip._Keyboard.ProcessState(Microsoft.Xna.Framework.Input.Keyboard.GetState());
            if (_chip._cpu.SoundTimer > 0)
            {
                mySoundInstance.Play();
            }
            else
            {
                mySoundInstance.Stop();
            }

            // TODO: Add your update logic here
            if (!_chip.IsWaitingForInput())
            {
                _chip.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            Texture2D _texture;
            _texture = new Texture2D(GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] {Color.DarkCyan});


            _spriteBatch.Begin();
            for (var y = 0; y < 32; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    if (_chip._cpu.Display[x, y])
                    {
                        _spriteBatch.Draw(_texture, new Rectangle(x * _scaleBy,
                            y * _scaleBy,
                            1 * _scaleBy,
                            1 * _scaleBy), Color.LightGreen);
                    }
                    else
                    {
                        _spriteBatch.Draw(_texture, new Rectangle(x * _scaleBy,
                            y * _scaleBy,
                            1 * _scaleBy,
                            1 * _scaleBy), Color.Black);
                    }
                }
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
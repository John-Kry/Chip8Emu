using System.IO;
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
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Chip _chip;
        private const int ScaleBy = 12;
        private SoundEffect _mySound;
        private SoundEffectInstance _mySoundInstance;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 64 * ScaleBy;
            _graphics.PreferredBackBufferHeight = 32 * ScaleBy;
            _graphics.ApplyChanges();
            
            var fs = new FileStream("smw2_coin.wav", FileMode.Open);
            _mySound = SoundEffect.FromStream(fs);
            _mySoundInstance = _mySound.CreateInstance();
            _mySoundInstance.IsLooped = true;

            
            _chip = new Chip();
            _chip.LoadRom();
            _chip.Start();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            
            _chip._Keyboard.ProcessState(Microsoft.Xna.Framework.Input.Keyboard.GetState());
            if (_chip._cpu.SoundTimer > 0)
            {
                _mySoundInstance.Play();
            }
            else
            {
                _mySoundInstance.Stop();
            }

            if (!_chip.IsWaitingForInput())
            {
                _chip.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new Color[] {Color.DarkCyan});


            _spriteBatch.Begin();
            for (var y = 0; y < 32; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    var rect = new Rectangle(x * ScaleBy,
                                                y * ScaleBy,
                                                1 * ScaleBy,
                                                1 * ScaleBy);
                    if (_chip._cpu.Display[x, y])
                    {
                        _spriteBatch.Draw(texture, rect, Color.LightGreen);
                    }
                    else
                    {
                        _spriteBatch.Draw(texture, rect, Color.Black);
                    }
                }
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
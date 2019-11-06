using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChipEight.Net.Hardware;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Key = SFML.Window.Keyboard.Key;

namespace ChipEight.Net
{
    public class Program
    {
        private const uint ForegroundColor = 0x088620ff;
        private const uint BackgroundColor = 0x000000ff;

        private bool _pause;
        private readonly Chip8 _chip8;
        private readonly RenderWindow _window;

        private readonly Dictionary<Key, int> _keyMap = new Dictionary<Key, int>
        {
            {Key.Num1, 0x1 }, {Key.Num2, 0x2 }, {Key.Num3, 0x3 }, {Key.Num4, 0xC },
            {Key.Q, 0x4 },  {Key.W, 0x5 },  {Key.E, 0x6 },  {Key.R, 0xD },
            {Key.A, 0x7 },  {Key.S, 0x8 },  {Key.D, 0x9 },  {Key.F, 0xE },
            {Key.Z, 0xA },  {Key.X, 0x0 },  {Key.C, 0xB},   {Key.V, 0xF },
        };

        public static void Main(string[] args)
        {
            if (!args.Any())
            {
                PrintHelp();
                return;
            }

            var cartridge = LoadGameCartridge(args[0]);
            if (cartridge == null)
            {
                PrintHelp();
                return;
            }

            new Program(cartridge).RunGameLoop();
        }

        public Program(byte[] cartridge)
        {
            const uint sampleRate = 44100;
            var soundData = SoundHelpers.GenerateSquareWave(sampleRate, 2);
            var buffer = new SoundBuffer(soundData, 1, sampleRate);
            var sound = new Sound(buffer);

            _chip8 = new Chip8(new Registers(), new Stack(), new Memory(), new Display(), new RngProvider());
            _chip8.OnStartSound += () => sound.Play();
            _chip8.OnStopSound += () => sound.Stop();
            
            _chip8.Load(cartridge);

            _window = new RenderWindow(new VideoMode(512, 256), "ChipEight.Net", Styles.Default);
            _window.KeyPressed += KeyDown;
            _window.KeyReleased += KeyUp;
            _window.Closed += WindowCloseButtonPressed;
        }

        public void RunGameLoop()
        {
            var clock = new Clock();
            var texture = new Texture(64, 32);
            var textureMap = new byte[64 * 32 * 4];
            var frameBuffer = new Sprite(texture);

            frameBuffer.Scale = new Vector2f(_window.Size.X / frameBuffer.GetLocalBounds().Width, _window.Size.Y / frameBuffer.GetLocalBounds().Height);

            while (_window.IsOpen)
            {
               _window.DispatchEvents();
               _window.Display();
               _window.Draw(frameBuffer);

                if (clock.ElapsedTime < Time.FromMilliseconds(1) || _pause)
                    continue;

                _chip8.Tick();

                UpdateTextureMap(textureMap, _chip8.Display);
                texture.Update(textureMap);
                clock.Restart();
            }
        }

        private static byte[] LoadGameCartridge(string path)
        {
            return File.Exists(path)
                ? File.ReadAllBytes(path)
                : null;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("\nUsage: ChipEight.Net [path to game file]\n");
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {
            if (_keyMap.ContainsKey(e.Code))
                _chip8?.KeyUp(_keyMap[e.Code]);
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Code == Key.Escape)
            {
                _pause = !_pause;
            }

            if (_keyMap.ContainsKey(e.Code))
                _chip8?.KeyDown(_keyMap[e.Code]);
        }

        private static void WindowCloseButtonPressed(object sender, EventArgs e)
        {
            (sender as Window)?.Close();
        }

        public void UpdateTextureMap(byte[] textureMap, Display display)
        {
            for (var y = 0; y < 32; y++)
            {
                for (var x = 0; x < 64; x++)
                {
                    var pos = (y * 64) + x;

                    var color = display.GetPixel(x, y) ? ForegroundColor : BackgroundColor;

                    pos *= 4;
                    textureMap[pos + 0] = (byte)((color >> 8) & 255);
                    textureMap[pos + 1] = (byte)((color >> 16) & 255);
                    textureMap[pos + 2] = (byte)((color >> 24) & 255);
                    textureMap[pos + 3] = 0xff;
                }
            }
        }
    }
}

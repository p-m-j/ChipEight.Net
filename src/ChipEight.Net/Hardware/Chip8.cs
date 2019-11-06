using System;
using System.Collections.Generic;
using System.Timers;

namespace ChipEight.Net.Hardware
{
    public partial class Chip8
    {
        public delegate void SoundEvent();

        public event SoundEvent OnStartSound;
        public event SoundEvent OnStopSound;

        public const int KeyboardSize = 16;
        public const int StartInstruction = 0x200;
        public const int InstructionSize = 0x2;

        public Display Display { get; }
        public ushort Opcode { get; private set; }
        public ushort I { get; private set; }
        public ushort PC { get; private set; }
        public byte DelayTimer { get; private set; }
        public byte SoundTimer { get; private set; }
        public bool DrawRequired { get; private set; }

        private readonly bool[] _keyboard = new bool[KeyboardSize];
        private readonly Registers _registers;
        private readonly Stack _stack;
        private readonly Memory _memory;
        private readonly IRngProvider _rng;
        private readonly IDictionary<ushort, Action<ushort>> _handlers;

        private bool _waitingForInput;

        public Chip8(Registers registers, Stack stack, Memory memory, Display display, IRngProvider rng)
        {
            Display = display ?? throw new ArgumentNullException(nameof(display));

            _stack = stack ?? throw new ArgumentNullException(nameof(stack));
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _rng = rng ?? throw new ArgumentNullException(nameof(rng));
            _registers = registers ?? throw new ArgumentNullException(nameof(registers));

            _handlers = new Dictionary<ushort, Action<ushort>>
            {
                {0x0, Handle0MostSignificantNibble},
                {0x1, HandleJump},
                {0x2, HandleCall},
                {0x3, HandleSkipEqualConst},
                {0x4, HandleSkipNotEqualConst},
                {0x5, HandleSkipRegistersEqual},
                {0x6, HandleLoadRegister},
                {0x7, HandleAddConstantToRegister},
                {0x8, Handle8MostSignificantNibble},
                {0x9, HandleRegisterSNE},
                {0xA, HandleSetIndexToConstant},
                {0xB, HandleJumpToV0WithOffset},
                {0xC, HandleRandomPlusConst},
                {0xD, HandleDrawSprite},
                {0xE, HandleKeyboardSkip},
                {0xF, HandleFMostSignificantNibble},
            };

            Reset();

            var timer = new Timer(16);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateDelayTimer();
            UpdateSoundTimer();
        }

        public void Reset()
        {
            PC = StartInstruction;
            Opcode = 0;
            I = 0;

            DelayTimer = 0;
            SoundTimer = 0;

            Display.Clear();
            _keyboard.Clear();
            _stack.Clear();
            _registers.Clear();
            _memory.Clear();

            LoadFontSet();
        }

        private void LoadFontSet()
        {
            byte[] fontSet =
            {
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

            for (var i = 0; i < fontSet.Length; i++)
            {
                _memory[i] = fontSet[i];
            }
        }

        public void Tick()
        {
            DrawRequired = false;

            if(_waitingForInput)
                return;

            // Get next instruction
            Opcode = (ushort)(_memory[PC++] << 8 | _memory[PC++]);

            // Handle
            var handler = GetHandler(Opcode);

            handler(Opcode);
        }

        public void Load(byte[] program)
        {
            for (var i = 0; i < program.Length; i++)
                _memory[StartInstruction + i] = program[i];
        }

        private void UpdateDelayTimer()
        {
            if (DelayTimer <= 0)
                return;

            DelayTimer--;
        }

        private void UpdateSoundTimer()
        {
            if (SoundTimer <= 0)
                return;

            SoundTimer--;

            if (SoundTimer < 1)
            {
                OnStopSound?.Invoke();
            }
        }

        private Action<ushort> GetHandler(ushort instruction)
        {
            return _handlers[instruction.GetNibble(1)];
        }

        public void KeyDown(int i)
        {
            _keyboard[i] = true;
            _waitingForInput = false;
        }

        public void KeyUp(int i)
        {
            _keyboard[i] = false;
            _waitingForInput = false;
        }
    }
}

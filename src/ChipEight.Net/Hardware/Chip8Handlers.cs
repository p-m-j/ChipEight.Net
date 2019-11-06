using System;

namespace ChipEight.Net.Hardware
{
    public partial class Chip8
    {
        private void Handle0MostSignificantNibble(ushort instruction)
        {
            if ((instruction & 0x000F) == 0x000E)
            {
                HandleReturn();
                return;
            }

            HandleClearScreen();
        }

        private void HandleClearScreen()
        {
            DrawRequired = true;
            Display.Clear();
        }

        private void HandleReturn()
        {
            PC = _stack.Pop();
        }

        private void HandleJump(ushort instruction)
        {
            PC = (ushort)(instruction & 0x0FFF);
        }

        private void HandleCall(ushort instruction)
        {
            _stack.Push(PC);
            PC = (ushort)(instruction & 0x0FFF);
        }

        /// <summary>
        /// 3xkk - SE Vx, byte
        /// </summary>
        private void HandleSkipEqualConst(ushort instruction)
        {
            var x = instruction.GetNibble(2); // register
            var kk = instruction.GetByte(WordParts.Low); // value

            if (_registers[x] == kk)
                PC += InstructionSize;
        }

        /// <summary>
        /// 4xkk - SNE Vx, byte
        /// </summary>
        private void HandleSkipNotEqualConst(ushort instruction)
        {
            var x = instruction.GetNibble(2); // register
            var kk = instruction.GetByte(WordParts.Low); // value

            if(_registers[x] != kk)
                PC += InstructionSize;
        }

        /// <summary>
        /// 5xy0 - SE Vx, Vy
        /// </summary>
        private void HandleSkipRegistersEqual(ushort instruction)
        {
            var x = instruction.GetNibble(2);
            var y = instruction.GetNibble(3);

            if(_registers[x] == _registers[y])
                PC += InstructionSize;
        }

        /// <summary>
        /// 6xkk - LD Vx, byte
        /// </summary>
        private void HandleLoadRegister(ushort instruction)
        {
            var i = (instruction & 0x0F00) / 256;
            var value = (byte)(instruction & 0x00FF);
            _registers[i] = value;
        }

        /// <summary>
        /// 7xkk - ADD Vx, byte
        /// </summary>
        private void HandleAddConstantToRegister(ushort instruction)
        {
            var x = instruction.GetNibble(2); // register
            var kk = instruction.GetByte(WordParts.Low); // value

            _registers[x] = (byte) (_registers[x] + kk);
        }

        private void Handle8MostSignificantNibble(ushort instruction)
        {
            var lsb = instruction.GetNibble(4);

            switch (lsb)
            {
                case 0x0:
                    HandleLoadVyIntoVx(instruction);
                    break;
                case 0x1:
                    HandleRegisterOr(instruction);
                    break;
                case 0x2:
                    HandleRegisterAnd(instruction);
                    break;
                case 0x3:
                    HandleRegisterXor(instruction);
                    break;
                case 0x4:
                    HandleRegisterAdd(instruction);
                    break;
                case 0x5:
                    HandleRegisterSub(instruction);
                    break;
                case 0x6:
                    HandleRegisterSHR(instruction);
                    break;
                case 0x7:
                    HandleRegisterSUBN(instruction);
                    break;
                case 0xE:
                    HandleRegisterSHL(instruction);
                    break;
                default:
                    throw new NotImplementedException();
            }
            
        }

        /// <summary>
        /// 8xy0 - LD Vx, Vy
        /// </summary>
        private void HandleLoadVyIntoVx(ushort instruction)
        {
            var x = instruction.GetNibble(2);
            var y = instruction.GetNibble(3);

            _registers[x] = _registers[y];
        }

        /// <summary>
        /// 8xy1 - OR Vx, Vy
        /// </summary>
        private void HandleRegisterOr(ushort instruction)
        {
            var x = instruction.GetNibble(2);
            var y = instruction.GetNibble(3);

            _registers[x] = (byte) (_registers[x] | _registers[y]);
        }

        /// <summary>
        /// 8xy2 - AND Vx, Vy
        /// </summary>
        private void HandleRegisterAnd(ushort instruction)
        {
            var x = instruction.GetNibble(2);
            var y = instruction.GetNibble(3);

            _registers[x] = (byte)(_registers[x] & _registers[y]);
        }

        /// <summary>
        /// 8xy3 - XOR Vx, Vy
        /// </summary>
        private void HandleRegisterXor(ushort instruction)
        {
            var x = instruction.GetNibble(2);
            var y = instruction.GetNibble(3);

            _registers[x] = (byte)(_registers[x] ^ _registers[y]);
        }

        /// <summary>
        /// 8xy4 - ADD Vx, Vy
        /// </summary>
        private void HandleRegisterAdd(ushort instruction)
        {
            var x = instruction.GetNibble(2);
            var y = instruction.GetNibble(3);

            var result = _registers[x] + _registers[y];

            _registers[x] = (byte)result;
            _registers[0xF] = (byte)(result > 255 ? 1 : 0);
        }

        /// <summary>
        /// 8xy5 - SUB Vx, Vy
        /// </summary>
        private void HandleRegisterSub(ushort instruction)
        {
            var ix = instruction.GetNibble(2);
            var iy = instruction.GetNibble(3);

            var x = _registers[ix];
            var y = _registers[iy];

            var result = x - y;

            _registers[ix] = (byte)result;
            _registers[0xF] = (byte)(x > y ? 1 : 0);
        }

        /// <summary>
        /// 8xy6 - SHR Vx {, Vy}
        /// </summary>
        private void HandleRegisterSHR(ushort instruction)
        {
            var ix = instruction.GetNibble(2);

            var x = _registers[ix];

            var result = x >> 1;

            _registers[ix] = (byte)result;
            // store least significant bit in VF
            _registers[0xF] = (byte)((x & 0x01) == 0x01 ? 1 : 0);
        }

        /// <summary>
        /// 8xy7 - SUBN Vx, Vy
        /// </summary>
        private void HandleRegisterSUBN(ushort instruction)
        {
            var ix = instruction.GetNibble(2);
            var iy = instruction.GetNibble(3);

            var x = _registers[ix];
            var y = _registers[iy];

            var result = y - x;

            _registers[ix] = (byte)result;
            _registers[0xF] = (byte)(y > x ? 1 : 0);
        }

        /// <summary>
        /// 8xyE - SHL Vx {, Vy}
        /// </summary>
        private void HandleRegisterSHL(ushort instruction)
        {
            var ix = instruction.GetNibble(2);

            var x = _registers[ix];

            var result = x << 1;

            _registers[ix] = (byte)result;
            // store most significant bit in VF
            _registers[0xF] = (byte)((x & 0x80) == 0x80 ? 1 : 0);
        }

        /// <summary>
        /// 9xy0 - SNE Vx, Vy
        /// </summary>
        private void HandleRegisterSNE(ushort instruction)
        {
            var ix = instruction.GetNibble(2);
            var iy = instruction.GetNibble(3);

            var x = _registers[ix];
            var y = _registers[iy];

            if (x != y)
                PC += 2;
        }

        /// <summary>
        /// Annn - LD I, addr
        /// </summary>
        private void HandleSetIndexToConstant(ushort instruction)
        {
            I = (ushort) (instruction & 0x0FFF);
        }

        /// <summary>
        /// Bnnn - JP V0, addr
        /// </summary>
        private void HandleJumpToV0WithOffset(ushort instruction)
        {
            var offset = (ushort)(instruction & 0x0FFF);
            var v0 = _registers[0];
            var address =(ushort) (v0 + offset);
            PC = address;
        }

        /// <summary>
        /// Cxkk - RND Vx, byte
        /// </summary>
        private void HandleRandomPlusConst(ushort instruction)
        {
            var randomByte = _rng.Next();
            var constant = instruction.GetByte(WordParts.Low);
            var result = (byte) (randomByte & constant);

            var ix = instruction.GetNibble(2);

            _registers[ix] = result;
        }

        private void HandleDrawSprite(ushort instruction)
        {
            DrawRequired = true;
            var ix = instruction.GetNibble(2);
            var iy = instruction.GetNibble(3);

            var x = _registers[ix];
            var y = _registers[iy];

            var n = instruction.GetNibble(4);

            var collision = false;

            for (var i = 0; i < n; i++)
            {
                collision = collision | Display.DrawSprite(x, y + i, _memory[I + i]);
            }

            _registers[0xf] = (byte)(collision ? 0x01 : 0x00);
        }

        private void HandleKeyboardSkip(ushort instruction)
        {
            var lsn = instruction.GetNibble(4);

            if(lsn == 0xE)
            {
                HandleSkipKeyPressed(instruction);
                return;
            }

            HandleSkipKeyNotPressed(instruction);
        }

        private void HandleSkipKeyPressed(ushort instruction)
        {
            var ix = instruction.GetNibble(2);
            var key = _registers[ix];

            if (_keyboard[key])
                PC += 2;
        }

        private void HandleSkipKeyNotPressed(ushort instruction)
        {
            var ix = instruction.GetNibble(2);
            var key = _registers[ix];

            if (!_keyboard[key])
                PC += 2;
        }

        private void HandleFMostSignificantNibble(ushort instruction)
        {
            var lsb = instruction.GetByte(WordParts.Low);

            switch (lsb)
            {
                case 0x07:
                    HandleSetVxToDt(instruction);
                    break;
                case 0x15:
                    HandleSetDelayTimer(instruction);
                    break;
                case 0x18:
                    HandleSetSoundTimer(instruction);
                    break;
                case 0x1E:
                    HandleAddToIndex(instruction);
                    break;
                case 0x29:
                    HandleLoadFontCharOffset(instruction);
                    break;
                case 0x33:
                    HandleBCDFromRegister(instruction);
                    break;
                case 0x55:
                    HandleRegDump(instruction);
                    break;
                case 0x65:
                    HandleRegLoad(instruction);
                    break;
                case 0x0A:
                    _waitingForInput = true;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void HandleLoadFontCharOffset(ushort instruction)
        {
            var ix = instruction.GetNibble(2);
            var x = _registers[ix];

            if (x > 0xF)
                return;

            I = (ushort) (5 * x);
        }

        private void HandleSetVxToDt(ushort instruction)
        {
            var ix = instruction.GetNibble(2);

            _registers[ix] = DelayTimer;
        }

        private void HandleRegDump(ushort instruction)
        {
            var ix = instruction.GetNibble(2);
      
            for (var i = 0; i <= ix; i++)
            {
                _memory[I + i] = _registers[i];
            }
        }

        private void HandleRegLoad(ushort instruction)
        {
            var ix = instruction.GetNibble(2);
 
            for (var i = 0; i <= ix; i++)
            {
                _registers[i] = _memory[I + i];
            }
        }


        private void HandleAddToIndex(ushort instruction)
        {
            var ix = instruction.GetNibble(2);
            var value = _registers[ix];

            I += value;
        }

        private void HandleBCDFromRegister(ushort instruction)
        {
            var ix = instruction.GetNibble(2);
            var value = _registers[ix];
            
            _memory[I + 2] = (byte)(value % 10);
            _memory[I + 1] = (byte)((value /  10) % 10);
            _memory[I + 0] = (byte)((value / 100) % 10);
        }

        private void HandleSetDelayTimer(ushort instruction)
        {
            var ix = instruction.GetNibble(2);
            var value = _registers[ix];

            DelayTimer = value;
        }

        private void HandleSetSoundTimer(ushort instruction)
        {
            var ix = instruction.GetNibble(2);
            var value = _registers[ix];

            SoundTimer = value;

            if(SoundTimer > 0)
                OnStartSound?.Invoke();
        }
    }
}

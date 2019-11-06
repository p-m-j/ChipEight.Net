using AutoFixture.Xunit2;
using ChipEight.Net.Hardware;
using NSubstitute;
using Shouldly;
using Xunit;

namespace ChipEight.Net.Tests.Hardware
{
    public class Chip8Tests
    {
        private const int Skip = 2 * Chip8.InstructionSize;

        [Theory, AutoNSubstituteData]
        public void test_jump_is_handled(Chip8 sut)
        {
            sut.Load(new byte[] { 0x12, 0x34 });
            sut.Tick();
            sut.PC.ShouldBe((ushort)0x0234);
        }

        [Theory, AutoNSubstituteData]
        public void test_call_adds_return_address_to_stack(
            [Frozen] Stack stack,
            Chip8 sut)
        {
            sut.Load(new byte[] { 0x22, 0x34 });
            sut.Tick();
            stack.Head.ShouldBe((ushort) (Chip8.StartInstruction + Chip8.InstructionSize));
        }

        [Theory, AutoNSubstituteData]
        public void test_call_updates_program_counter(
            Chip8 sut)
        {
            sut.Load(new byte[] { 0x22, 0x34 });
            sut.Tick();
            sut.PC.ShouldBe((ushort)0x0234);
        }

        [Theory, AutoNSubstituteData]
        public void test_SE_skips_if_equal(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x11;
            sut.Load(new byte[] { 0x33, 0x11 });
            sut.Tick();
            sut.PC.ShouldBe((ushort)(Chip8.StartInstruction + Skip));
        }

        [Theory, AutoNSubstituteData]
        public void test_SE_does_not_skip_if_not_equal(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x99;
            sut.Load(new byte[] { 0x33, 0x11 });
            sut.Tick();
            sut.PC.ShouldBe((ushort)(Chip8.StartInstruction + Chip8.InstructionSize));
        }

        [Theory, AutoNSubstituteData]
        public void test_SNE_skips_if_not_equal(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x11;
            sut.Load(new byte[] { 0x43, 0x12 });
            sut.Tick();
            sut.PC.ShouldBe((ushort)(Chip8.StartInstruction + Skip));
        }

        [Theory, AutoNSubstituteData]
        public void test_SNE_does_not_skip_if_equal(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x11;
            sut.Load(new byte[] { 0x43, 0x11 });
            sut.Tick();
            sut.PC.ShouldBe((ushort)(Chip8.StartInstruction + Chip8.InstructionSize));
        }

        [Theory, AutoNSubstituteData]
        public void test_SE_register_skips_if_equal(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0xFA;
            registers[7] = 0xFA;
            sut.Load(new byte[] { 0x53, 0x70 });
            sut.Tick();
            sut.PC.ShouldBe((ushort)(Chip8.StartInstruction + Skip));
        }
        [Theory, AutoNSubstituteData]
        public void test_SE_register_does_not_skip_if_not_equal(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0xFA;
            registers[7] = 0xFF;
            sut.Load(new byte[] { 0x53, 0x70 });
            sut.Tick();
            sut.PC.ShouldBe((ushort)(Chip8.StartInstruction + Chip8.InstructionSize));
        }


        [Theory, AutoNSubstituteData]
        public void test_LD_loads_into_register(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            sut.Load(new byte[] { 0x6A, 0xFA });
            sut.Tick();
            registers[0xA].ShouldBe((byte)0xFA);
        }


        [Theory, AutoNSubstituteData]
        public void test_add_works_as_expected(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[0xA] = 0x12;
            sut.Load(new byte[] { 0x7A, 0x43 });
            sut.Tick();
            registers[0xA].ShouldBe((byte)(0x12 + 0x43));
        }

        [Theory, AutoNSubstituteData]
        public void test_load_x_into_y_works_as_expected(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x42;
            sut.Load(new byte[] { 0x82, 0x30 });
            sut.Tick();
            registers[2].ShouldBe((byte)(0x42));
        }

        [Theory, AutoNSubstituteData]
        public void test_register_or_works_as_expected(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x12;
            registers[4] = 0x34;
            sut.Load(new byte[] { 0x83, 0x41 });
            sut.Tick();
            registers[3].ShouldBe((byte)(0x12 | 0x34));
        }

        [Theory, AutoNSubstituteData]
        public void test_register_and_works_as_expected(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x12;
            registers[4] = 0x34;
            sut.Load(new byte[] { 0x83, 0x42 });
            sut.Tick();
            registers[3].ShouldBe((byte)(0x12 & 0x34));
        }

        [Theory, AutoNSubstituteData]
        public void test_register_xor_works_as_expected(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x12;
            registers[4] = 0x34;
            sut.Load(new byte[] { 0x83, 0x43 });
            sut.Tick();
            registers[3].ShouldBe((byte)(0x12 ^ 0x34));
        }

        [Theory, AutoNSubstituteData]
        public void test_register_add_works_as_expected_no_overflow(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x12;
            registers[4] = 0x34;
            sut.Load(new byte[] { 0x83, 0x44 });
            sut.Tick();
            registers[3].ShouldBe((byte)(0x12 + 0x34));
            registers[0xF].ShouldBe((byte) 0);
        }

        [Theory, AutoNSubstituteData]
        public void test_register_add_works_as_expected_with_overflow(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0xFF;
            registers[4] = 0x01;
            sut.Load(new byte[] { 0x83, 0x44 });
            sut.Tick();
            registers[3].ShouldBe((byte) 0); // Oh no, overflow!
            registers[0xF].ShouldBe((byte)1);
        }

        [Theory, AutoNSubstituteData]
        public void test_register_sub_works_as_expected_no_overflow(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x10;
            registers[4] = 0x05;
            sut.Load(new byte[] { 0x83, 0x45 });
            sut.Tick();
            registers[3].ShouldBe(unchecked((byte)(0x10 - 0x05)));
            registers[0xF].ShouldBe((byte)1);
        }

        [Theory, AutoNSubstituteData]
        public void test_register_sub_works_as_expected_with_overflow(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x05;
            registers[4] = 0x10;
            sut.Load(new byte[] { 0x83, 0x45 });
            sut.Tick();
            registers[3].ShouldBe(unchecked((byte)(0x05 - 0x10)));
            registers[0xF].ShouldBe((byte)0);
        }

        [Theory, AutoNSubstituteData]
        public void test_register_subn_works_as_expected_no_overflow(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x05;
            registers[4] = 0x10;
            sut.Load(new byte[] { 0x83, 0x47 });
            sut.Tick();
            registers[3].ShouldBe(unchecked((byte)( 0x10 - 0x05)));
            registers[0xF].ShouldBe((byte)1);
        }

        [Theory, AutoNSubstituteData]
        public void test_register_subn_works_as_expected_with_overflow(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x10;
            registers[4] = 0x05;
            sut.Load(new byte[] { 0x83, 0x47 });
            sut.Tick();
            registers[3].ShouldBe(unchecked((byte)(0x05 - 0x10)));
            registers[0xF].ShouldBe((byte)0);
        }

        [Theory, AutoNSubstituteData]
        public void test_register_shr_works_as_expected_0_lsb(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0b00000100;
            sut.Load(new byte[] { 0x83, 0x06 });
            sut.Tick();
            registers[3].ShouldBe(((byte)(0b00000010)));
            registers[0xF].ShouldBe((byte)0);
        }

        [Theory, AutoNSubstituteData]
        public void test_register_shr_works_as_expected_1_lsb(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0b00000101;
            sut.Load(new byte[] { 0x83, 0x06 });
            sut.Tick();
            registers[3].ShouldBe(((byte)(0b00000010)));
            registers[0xF].ShouldBe((byte)1);
        }

        [Theory, AutoNSubstituteData]
        public void test_register_shl_works_as_expected_0_msb(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0b01000010;
            sut.Load(new byte[] { 0x83, 0x0E });
            sut.Tick();
            registers[3].ShouldBe(((byte)(0b10000100)));
            registers[0xF].ShouldBe((byte)0);
        }

        [Theory, AutoNSubstituteData]
        public void test_register_shl_works_as_expected_1_msb(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0b11000010;
            sut.Load(new byte[] { 0x83, 0x0E });
            sut.Tick();
            registers[3].ShouldBe(((byte)(0b10000100)));
            registers[0xF].ShouldBe((byte)1);
        }

        [Theory, AutoNSubstituteData]
        public void test_SNE_register_does_not_skip_if_equal(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x11;
            registers[4] = 0x11;
            sut.Load(new byte[] { 0x93, 0x40 });
            sut.Tick();
            sut.PC.ShouldBe((ushort)(Chip8.StartInstruction + Chip8.InstructionSize));
        }

        [Theory, AutoNSubstituteData]
        public void test_SNE_register_skips_if_not_equal(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x11;
            registers[4] = 0x12;
            sut.Load(new byte[] { 0x93, 0x40 });
            sut.Tick();
            sut.PC.ShouldBe((ushort)(Chip8.StartInstruction + Skip));
        }

        [Theory, AutoNSubstituteData]
        public void test_LD_I_sets_correct_value(
            Chip8 sut)
        {
            sut.Load(new byte[] { 0xA1, 0x23 });
            sut.Tick();
            sut.I.ShouldBe((ushort) 0x0123);
            sut.PC.ShouldBe((ushort)(Chip8.StartInstruction + Chip8.InstructionSize));
        }

        [Theory, AutoNSubstituteData]
        public void test_jump_to_v0_plus_offset_updates_pc(
            Chip8 sut)
        {
            sut.Load(new byte[] { 0xB1, 0x23 });
            sut.Tick();
            sut.PC.ShouldBe((ushort)0x0123);
        }

        [Theory, AutoNSubstituteData]
        public void test_rng_and_sets_value_in_specified_register(
            [Frozen] Registers registers,
            [Frozen] IRngProvider rng,
            Chip8 sut)
        {
            rng.Next().Returns((byte) 0xAB);

            sut.Load(new byte[] { 0xCA, 0x22 });
            sut.Tick();
            registers[0xA].ShouldBe((byte)(0xAB & 0x22));
        }

        [Theory, AutoNSubstituteData]
        public void test_bcd_stores_correctly(
            [Frozen] Registers registers,
            [Frozen] Memory memory,
            Chip8 sut)
        {
            registers[0x9] = 249;
            
            sut.Load(new byte[] { 0xF9, 0x33 });
            sut.Tick();

            memory[0].ShouldBe((byte)2);
            memory[1].ShouldBe((byte)4);
            memory[2].ShouldBe((byte)9);
        }

        [Theory, AutoNSubstituteData]
        public void test_set_delay_timer_works(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[0x9] = 132;

            sut.Load(new byte[] { 0xF9, 0x15 });
            sut.Tick();

            sut.DelayTimer.ShouldBe((byte)132);
        }

        [Theory, AutoNSubstituteData]
        public void test_set_sound_timer_works(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[0x9] = 219;

            sut.Load(new byte[] { 0xF9, 0x18 });
            sut.Tick();

            sut.SoundTimer.ShouldBe((byte)219);
        }

        [Theory, AutoNSubstituteData]
        public void test_add_to_index_works(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[3] = 0x20;

            sut.Load(new byte[]
            {
                0xA1, 0x11, // Set I to 0x0111
                0xF3, 0x1E  // I += *(V3)
            });

            sut.Tick();
            sut.I.ShouldBe((ushort) 0x0111);

            sut.Tick();
            sut.I.ShouldBe((ushort) 0x0131);
        }

        [Theory, AutoNSubstituteData]
        public void test_set_vx_to_dt_works(
            [Frozen] Registers registers,
            Chip8 sut)
        {
            registers[0x4] = 123;
            registers[0x2] = 0;

            sut.Load(new byte[]
            {
                0xF4, 0x15, // DT = *(V4)
                0xF2, 0x07  // *(V2) = DT
            });

            sut.Tick();
            sut.Tick();

            registers[2].ShouldBe((byte) 123);
        }

        [Theory, AutoNSubstituteData]
        public void test_reg_dump_works(
            [Frozen] Registers registers,
            [Frozen] Memory memory,
            Chip8 sut)
        {
            registers[0x0] = 42;
            registers[0x1] = 43;
            registers[0x2] = 44;
            registers[0x3] = 45;
            registers[0x4] = 219;

            registers[0xA] = 3;


            sut.Load(new byte[]
            {
                0xA0, 0xFF, // Set I to 255
                0xFA, 0x55  // Dump V0 - V3 (inclusive) reg into memory
            });

            sut.Tick();
            sut.I.ShouldBe((ushort)0xFF);

            sut.Tick();
            memory[255].ShouldBe((byte)42);
            memory[256].ShouldBe((byte)43);
            memory[257].ShouldBe((byte)44);
            memory[258].ShouldBe((byte)45);
            memory[259].ShouldBe((byte)219);

            // I is unchanged
            sut.I.ShouldBe((ushort)0xFF);
        }

        [Theory, AutoNSubstituteData]
        public void test_reg_load_works(
            [Frozen] Registers registers,
            [Frozen] Memory memory,
            Chip8 sut)
        {
            memory[255] = 42;
            memory[256] = 43;
            memory[257] = 44;
            memory[258] = 45;
            memory[259] = 46;

            registers[0xA] = 3;


            sut.Load(new byte[]
            {
                0xA0, 0xFF, // Set I to 255
                0xFA, 0x65  // Load memory starting at i into V0 - V3 (inclusive) 
            });

            sut.Tick();
            sut.I.ShouldBe((ushort) 0xFF);

            sut.Tick();
            registers[0].ShouldBe((byte) 42);
            registers[1].ShouldBe((byte) 43);
            registers[2].ShouldBe((byte) 44);
            registers[3].ShouldBe((byte) 45);
            registers[4].ShouldBe((byte) 46);

            // I is unchanged
            sut.I.ShouldBe((ushort) 0xFF);
        }

        [Theory, AutoNSubstituteData]
        public void test_load_font_char(
            [Frozen] Memory memory,
            [Frozen] Registers registers,
            Chip8 sut)
        {
           
            registers[0xA] = 4;


            sut.Load(new byte[]
            {
                0xFA, 0x29
            });

            sut.Tick();
            sut.I.ShouldBe((ushort) (4 * 5));

            memory[sut.I].ShouldBe((byte) 0x90);
        }
    }
}

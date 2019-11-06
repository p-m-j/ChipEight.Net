using ChipEight.Net.Hardware;
using Shouldly;
using Xunit;

namespace ChipEight.Net.Tests.Hardware
{
    public class NibbleHelperTests
    {
        [Fact]
        public void test_get_byte()
        {
            const ushort word = 0xABCD;

            word.GetByte(WordParts.High).ShouldBe((byte)0xAB);
            word.GetByte(WordParts.Low).ShouldBe((byte)0xCD);
        }

        [Fact]
        public void test_get_nibble_from_byte()
        {
            const byte @byte = 0xAB;

            @byte.GetNibble(NibbleParts.Upper).ShouldBe((byte)0xA);
            @byte.GetNibble(NibbleParts.Lower).ShouldBe((byte)0xB);
        }


        [Fact]
        public void test_get_nibble_from_word()
        {
            const ushort word = 0xABCD;

            word.GetNibble(WordParts.High, NibbleParts.Upper).ShouldBe((byte)0xA);
            word.GetNibble(WordParts.High, NibbleParts.Lower).ShouldBe((byte)0xB);
            word.GetNibble(WordParts.Low, NibbleParts.Upper).ShouldBe((byte)0xC);
            word.GetNibble(WordParts.Low, NibbleParts.Lower).ShouldBe((byte)0xD);
        }
    }
}

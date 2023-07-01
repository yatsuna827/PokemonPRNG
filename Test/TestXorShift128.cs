using PokemonPRNG.XorShift128;


namespace Test
{
    public class TestXorShift128
    {
        [Fact]
        public void TestPrev()
        {
            var seed = (0x12345678u, 0xBEEFFACEu, 0xBADFACEu, 0x01010101u);

            Assert.NotEqual(seed.Next(), seed);
            Assert.Equal(seed.Next().Prev(), seed);
        }

        [Fact]
        public void TestJump()
        {
            var seed = (0x12345678u, 0xBEEFFACEu, 0xBADFACEu, 0x01010101u);

            Assert.Equal(seed.Next().Next().Next(), seed.Next(3));
        }

        [Fact]
        public void TestJumpBack()
        {
            var seed = (0x12345678u, 0xBEEFFACEu, 0xBADFACEu, 0x01010101u);

            Assert.Equal(seed.Prev().Prev().Prev(), seed.Prev(3));
        }
    }
}

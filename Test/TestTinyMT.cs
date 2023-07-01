using PokemonPRNG.TinyMT;


namespace Test
{
    public class TestTinyMT
    {
        [Fact]
        public void TestPrev()
        {
            var seed = (0x12345678u, 0xBEEFFACEu, 0xBADFACEu, 0x01010101u);

            Assert.NotEqual(seed.Next(), seed);
            Assert.Equal(seed.Next().Prev(), seed);
        }

        /// <summary>
        /// 127bitÇµÇ©égÇÌÇÍÇƒÇ¢Ç»Ç¢ÇÃÇ≈ÅAPrevÇ‚BackÇ≈ñﬂÇ∑Ç∆S0ÇÃç≈è„à bitÇ™óéÇøÇÈ
        /// </summary>
        [Fact]
        public void Test127bit()
        {
            var seed = (0xFFFFFFFFu, 0xBEEFFACEu, 0xBADFACEu, 0x01010101u);
            var next = seed.Next();

            Assert.NotEqual(next.Prev(), seed);

            seed.Item1 &= 0x7FFFFFFF;
            Assert.Equal(next.Prev(), seed);
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

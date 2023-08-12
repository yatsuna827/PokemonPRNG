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
            Assert.Equal(seed.Next(255).Next().Next(), seed.Next(257));
            Assert.Equal(seed.Next(1000).Next(1000), seed.Next(2000));
        }

        [Fact]
        public void TestJumpBack()
        {
            var seed = (0x12345678u, 0xBEEFFACEu, 0xBADFACEu, 0x01010101u);

            Assert.Equal(seed.Prev().Prev().Prev().ToU127(), seed.Prev(3).ToU127());
            Assert.Equal(seed.Prev(257).Next(257).ToU127(), seed.ToU127());
            Assert.Equal(seed.Prev(1000).Next(1000).ToU127(), seed.ToU127());
        }
    }

    static class Helper
    {
        public static (uint, uint, uint, uint) ToU127(this (uint, uint, uint, uint) seed)
        {
            var (s0, s1, s2, s3) = seed;
            return (s0 & 0x7FFF_FFFF, s1, s2, s3);
        }
    }
}

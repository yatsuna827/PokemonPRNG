using PokemonPRNG.SFMT;


namespace Test
{
    public class TestSFMT
    {
        [Theory]
        [InlineData(0x0u)]
        [InlineData(0xBEEFCAFEu)]
        public void TestCached(uint initialSeed)
        {
            var sfmt = new SFMT(initialSeed);
            var cached = new CachedSFMT(initialSeed, 1);

            var sum1 = 0L;
            for (int i = 0; i < 10000; i++)
                sum1 += sfmt.GetRand32();

            var sum2 = 0L;
            for (int i = 0; i < 10000; i++, cached.MoveNext())
                sum2 += cached.GetRand32();

            Assert.Equal(sum1, sum2);
            Assert.Equal(sfmt.Index32, cached.Index32);
        }

        [Theory]
        [InlineData(0x0u)]
        public void TestCachedBadUse(uint initialSeed)
        {
            var sfmt = new SFMT(initialSeed);
            var cached = new CachedSFMT(initialSeed, 1);

            var sum1 = 0L;
            for (int i = 0; i < 624; i++)
                sum1 += sfmt.GetRand32();

            var sum2 = 0L;
            for (int i = 0; i < 624; i++)
                sum2 += cached.GetRand32();

            Assert.Equal(sum1, sum2);

            if (Util.IsDebug)
            {
                Assert.Throws<Exception>(() => cached.GetRand32());
            }
            else
            {
                // ÀÛ‚ÉƒGƒ‰[‚ğ“f‚­‚Ì‚Íæ“Ç‚İ‚µ‚Ä‚ ‚é—Ìˆæ‚ğH‚¢‚Â‚Ô‚µ‚Ä‚©‚ç
                sfmt.Advance(624);
                cached.Advance(624);
                Assert.NotEqual(sfmt.GetRand32(), cached.GetRand32());
            }
        }
    }
}

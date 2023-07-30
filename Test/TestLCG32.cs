using PokemonPRNG.LCG32.StandardLCG;

namespace Test
{
    public class TestLCG32
    {
        [Fact]
        public void TestNextSeed()
        {
            var seed = 0xBEEFFACE;

            Assert.Equal(0xB3ECEE29u, seed.NextSeed());
        }

        [Fact]
        public void TestJump()
        {
            var seed = 0xBEEFFACE;

            Assert.Equal(seed.NextSeed().NextSeed().NextSeed(), seed.NextSeed(3));
        }

        [Fact]
        public void TestGetIndex()
        {
            Assert.Equal(12345678u, 0x0u.NextSeed(12345678).GetIndex());
            Assert.Equal(12345678u, 0xBEEFu.NextSeed(12345678).GetIndex(0xBEEFu));
        }
    }
}

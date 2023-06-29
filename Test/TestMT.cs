using PokemonPRNG.MT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class TestMT
    {
        [Theory]
        [InlineData(0x0u)]
        [InlineData(0xBEEFCAFEu)]
        public void TestCached(uint initialSeed)
        {
            var mt = new MT(initialSeed);
            var cached = new CachedMT(initialSeed, 1);

            var sum1 = 0L;
            for (int i = 0; i < 10000; i++)
                sum1 += mt.GetRand();

            var sum2 = 0L;
            for (int i = 0; i < 10000; i++, cached.MoveNext())
                sum2 += cached.GetRand();

            Assert.Equal(sum1, sum2);
            Assert.Equal(mt.Index, cached.Index);
        }

        [Theory]
        [InlineData(0x0u)]
        public void TestCachedBadUse(uint initialSeed)
        {
            var mt = new MT(initialSeed);
            var cached = new CachedMT(initialSeed, 1);

            var sum1 = 0L;
            for (int i = 0; i < 624; i++)
                sum1 += mt.GetRand();

            var sum2 = 0L;
            for (int i = 0; i < 624; i++)
                sum2 += cached.GetRand();

            Assert.Equal(sum1, sum2);

            if (Util.IsDebug)
            {
                Assert.Throws<Exception>(() => cached.GetRand());
            }
            else
            {
                // 実際にエラーを吐くのは先読みしてある領域を食いつぶしてから
                mt.Advance(624);
                cached.Advance(624);
                Assert.NotEqual(mt.GetRand(), cached.GetRand());
            }
        }
    }
}

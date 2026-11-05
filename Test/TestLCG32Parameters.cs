using PokemonPRNG.LCG32;
using Alternative = PokemonPRNG.LCG32.AlternativeLCG.AlternativeLCGExtension;
using GC = PokemonPRNG.LCG32.GCLCG.GCLCGExtension;
using Standard = PokemonPRNG.LCG32.StandardLCG.StandardLCGExtension;
using Static = PokemonPRNG.LCG32.StaticLCG.StaticLCGExtension;

namespace Test
{
    public class TestLCG32Parameters
    {
        private delegate uint Step(ref uint seed);
        private delegate uint Jump(ref uint seed, uint count);

        private record Operations(
            LCG32 Core,
            Func<uint, uint> Next,
            Func<uint, uint> Previous,
            Step Advance,
            Step Back,
            Step Random,
            Jump RandomModulo,
            Func<uint, uint, uint> NextMany,
            Func<uint, uint, uint> PreviousMany,
            Jump AdvanceMany,
            Jump BackMany,
            Func<uint, uint, uint> GetSeed,
            Func<uint, uint> GetIndex,
            Func<uint, uint, uint> GetIndexFrom);

        private static readonly Operations[] Generators =
        {
            new(new LCG32(0x6C078965, 1, 0x9638806D, 0x69C77F93),
                Alternative.NextSeed, Alternative.PrevSeed, Alternative.Advance, Alternative.Back,
                Alternative.GetRand, Alternative.GetRand, Alternative.NextSeed, Alternative.PrevSeed,
                Alternative.Advance, Alternative.Back, Alternative.GetSeed, Alternative.GetIndex, Alternative.GetIndex),
            new(new LCG32(0x343FD, 0x269EC3, 0xB9B33155, 0xA170F641),
                GC.NextSeed, GC.PrevSeed, GC.Advance, GC.Back,
                GC.GetRand, GC.GetRand, GC.NextSeed, GC.PrevSeed,
                GC.Advance, GC.Back, GC.GetSeed, GC.GetIndex, GC.GetIndex),
            new(new LCG32(0x41C64E6D, 0x6073, 0xEEB9EB65, 0xA3561A1),
                Standard.NextSeed, Standard.PrevSeed, Standard.Advance, Standard.Back,
                Standard.GetRand, Standard.GetRand, Standard.NextSeed, Standard.PrevSeed,
                Standard.Advance, Standard.Back, Standard.GetSeed, Standard.GetIndex, Standard.GetIndex),
            new(new LCG32(0x41C64E6D, 0x3039, 0xEEB9EB65, 0xFC77A683),
                Static.NextSeed, Static.PrevSeed, Static.Advance, Static.Back,
                Static.GetRand, Static.GetRand, Static.NextSeed, Static.PrevSeed,
                Static.Advance, Static.Back, Static.GetSeed, Static.GetIndex, Static.GetIndex)
        };

        private static readonly uint[] Seeds = { 0, 1, 0xBEEFFACE, 0x80000000, uint.MaxValue };
        private static readonly uint[] Counts =
        {
            0, 1, 2, 255, 256, 257, 65535, 65536, 65537,
            0xFFFFFF, 0x1000000, 0x1000001, 0x80000000, uint.MaxValue
        };

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void TestSingleStep(int kind)
        {
            var operations = Generators[kind];
            foreach (var initial in Seeds)
            {
                var seed = initial;
                for (int i = 0; i < 1024; i++)
                {
                    var next = operations.Core.NextSeed(seed);
                    var previous = operations.Core.PrevSeed(seed);
                    Assert.Equal(next, operations.Next(seed));
                    Assert.Equal(previous, operations.Previous(seed));
                    Assert.Equal(seed, operations.Previous(operations.Next(seed)));
                    Assert.Equal(seed, operations.Next(operations.Previous(seed)));

                    var advanced = seed;
                    Assert.Equal(next, operations.Advance(ref advanced));
                    Assert.Equal(next, advanced);
                    var backed = seed;
                    Assert.Equal(previous, operations.Back(ref backed));
                    Assert.Equal(previous, backed);

                    var randomSeed = seed;
                    Assert.Equal(next >> 16, operations.Random(ref randomSeed));
                    Assert.Equal(next, randomSeed);
                    foreach (var modulo in new uint[] { 1, 2, 100, 65536, uint.MaxValue })
                    {
                        randomSeed = seed;
                        Assert.Equal((next >> 16) % modulo, operations.RandomModulo(ref randomSeed, modulo));
                        Assert.Equal(next, randomSeed);
                    }
                    seed = next;
                }
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void TestJumpAndIndex(int kind)
        {
            var operations = Generators[kind];
            foreach (var initial in Seeds)
            foreach (var count in Counts)
            {
                var next = operations.NextMany(initial, count);
                var previous = operations.PreviousMany(initial, count);
                Assert.Equal(operations.Core.NextSeed(initial, count), next);
                Assert.Equal(operations.Core.PrevSeed(initial, count), previous);
                Assert.Equal(next, operations.GetSeed(initial, count));
                Assert.Equal(initial, operations.PreviousMany(next, count));
                Assert.Equal(initial, operations.NextMany(previous, count));
                Assert.Equal(count, operations.GetIndexFrom(next, initial));
                Assert.Equal(count, operations.GetIndex(operations.GetSeed(0, count)));

                var seed = initial;
                Assert.Equal(next, operations.AdvanceMany(ref seed, count));
                Assert.Equal(next, seed);
                seed = initial;
                Assert.Equal(previous, operations.BackMany(ref seed, count));
                Assert.Equal(previous, seed);

                Assert.Equal(next, operations.Core.GenerateAdvanceFunction(count)(initial));
                Assert.Equal(previous, operations.Core.GenerateBackFunction(count)(initial));
            }

            foreach (var initial in Seeds)
            {
                var seed = initial;
                for (uint count = 0; count < 1024; count++)
                {
                    Assert.Equal(seed, operations.NextMany(initial, count));
                    seed = operations.Next(seed);
                }
            }
        }

        [Fact]
        public void TestGCRandomFloat()
        {
            foreach (var initial in Seeds)
            {
                var seed = initial;
                var expectedSeed = GC.NextSeed(initial);
                Assert.Equal((expectedSeed >> 16) / 65536.0f, GC.GetRand_f(ref seed));
                Assert.Equal(expectedSeed, seed);
            }
        }
    }
}

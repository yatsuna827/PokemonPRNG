
namespace PokemonPRNG.Xoroshiro128p.BDSP
{
    public static class BDSPExtension
    {
        public static (ulong S0, ulong S1) Initialize(this uint seed)
            => (seed.Temper(0x9E3779B97F4A7C15), seed.Temper(0x3C6EF372FE94F82A));
        private static ulong Temper(this uint seed, ulong state)
        {
            var value = seed + state;
            value = 0xBF58476D1CE4E5B9 * (value ^ (value >> 30));
            value = 0x94D049BB133111EB * (value ^ (value >> 27));
            return value ^ (value >> 31);
        }

        // maxなのにmod取ってるの馬鹿です。
        public static uint GetRand(ref this (ulong S0, ulong S1) state, uint max = 0xFFFFFFFF)
            => (uint)(Xoroshiro128p.GetRand(ref state) >> 32) % max;
    }
}

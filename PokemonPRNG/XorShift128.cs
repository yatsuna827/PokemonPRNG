
namespace PokemonPRNG.XorShift128
{
    public static class XorShift128Ext
    {
        public static uint GetRand(ref this (uint s0, uint s1, uint s2, uint s3) state)
        {
            var t1 = state.s0 ^ (state.s0 << 11);
            var t2 = state.s3 ^ (state.s3 >> 19) ^ t1 ^ (t1 >> 8);

            state = (state.s1, state.s2, state.s3, t2);

            return t2;
        }

        public static (uint s0, uint s1, uint s2, uint s3) Advance(ref this (uint s0, uint s1, uint s2, uint s3) state)
        {
            var t1 = state.s0 ^ (state.s0 << 11);
            var t2 = state.s3 ^ (state.s3 >> 19) ^ t1 ^ (t1 >> 8);

            return state = (state.s1, state.s2, state.s3, t2);
        }

        public static (uint s0, uint s1, uint s2, uint s3) Back(ref this (uint s0, uint s1, uint s2, uint s3) state)
        {
            var t11 = state.s2 ^ state.s2 >> 19;
            var t12 = t11 ^ (t11 >> 8) ^ (t11 >> 16) ^ (t11 >> 24);
            var t13 = t12 ^ (t12 << 11) ^ (t12 << 22);

            var t21 = state.s3 ^ (state.s3 >> 8) ^ (state.s3 >> 16) ^ (state.s3 >> 24);
            var t22 = t21 ^ (t21 << 11) ^ (t21 << 22);

            return state = (t13 ^ t22, state.s1, state.s2, state.s3);
        }
    }
}

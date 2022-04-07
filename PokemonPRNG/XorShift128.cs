
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
        public static uint GetRand(ref this (uint s0, uint s1, uint s2, uint s3) state, uint n)
        {
            var t1 = state.s0 ^ (state.s0 << 11);
            var t2 = state.s3 ^ (state.s3 >> 19) ^ t1 ^ (t1 >> 8);

            state = (state.s1, state.s2, state.s3, t2);

            return t2 % n;
        }

        public static float GetRand_f(ref this (uint s0, uint s1, uint s2, uint s3) state)
        {
            var t1 = state.s0 ^ (state.s0 << 11);
            var t2 = state.s3 ^ (state.s3 >> 19) ^ t1 ^ (t1 >> 8);

            state = (state.s1, state.s2, state.s3, t2);

            return (t2 & 0x7F_FFFF) / 8388607.0f;
        }
        public static float GetRand_f(ref this (uint s0, uint s1, uint s2, uint s3) state, float min, float max)
        {
            var t1 = state.s0 ^ (state.s0 << 11);
            var t2 = state.s3 ^ (state.s3 >> 19) ^ t1 ^ (t1 >> 8);

            state = (state.s1, state.s2, state.s3, t2);

            var r = (t2 & 0x7F_FFFF) / 8388607.0f;

            return r * min + (1 - r) * max;
        }

        public static (uint s0, uint s1, uint s2, uint s3) Next(this (uint s0, uint s1, uint s2, uint s3) state)
        {
            var t1 = state.s0 ^ (state.s0 << 11);
            var t2 = state.s3 ^ (state.s3 >> 19) ^ t1 ^ (t1 >> 8);

            return (state.s1, state.s2, state.s3, t2);
        }
        public static (uint s0, uint s1, uint s2, uint s3) Next(this (uint s0, uint s1, uint s2, uint s3) state, uint n)
        {
            for (int i = 0; i < n; i++)
                state.Advance();

            return state;
        }
        public static (uint s0, uint s1, uint s2, uint s3) Advance(ref this (uint s0, uint s1, uint s2, uint s3) state)
        {
            var t1 = state.s0 ^ (state.s0 << 11);
            var t2 = state.s3 ^ (state.s3 >> 19) ^ t1 ^ (t1 >> 8);

            return state = (state.s1, state.s2, state.s3, t2);
        }
        public static (uint s0, uint s1, uint s2, uint s3) Advance(ref this (uint s0, uint s1, uint s2, uint s3) state, uint n)
        {
            for (int i = 0; i < n; i++)
                state.Advance();

            return state;
        }

        public static (uint s0, uint s1, uint s2, uint s3) Prev(this (uint s0, uint s1, uint s2, uint s3) state)
        {
            var t = state.s3 ^ state.s2 ^ (state.s2 >> 19);
            t = t ^ (t >> 8) ^ (t >> 16) ^ (t >> 24);
            t = t ^ (t << 11) ^ (t << 22);

            return (t, state.s0, state.s1, state.s2);
        }
        public static (uint s0, uint s1, uint s2, uint s3) Prev(this (uint s0, uint s1, uint s2, uint s3) state, uint n)
        {
            for (int i = 0; i < n; i++)
                state.Back();

            return state;
        }
        public static (uint s0, uint s1, uint s2, uint s3) Back(ref this (uint s0, uint s1, uint s2, uint s3) state)
        {
            var t = state.s3 ^ state.s2 ^ (state.s2 >> 19);
            t = t ^ (t >> 8) ^ (t >> 16) ^ (t >> 24);
            t = t ^ (t << 11) ^ (t << 22);

            return state = (t, state.s0, state.s1, state.s2);
        }
        public static (uint s0, uint s1, uint s2, uint s3) Back(ref this (uint s0, uint s1, uint s2, uint s3) state, uint n)
        {
            for (int i = 0; i < n; i++)
                state.Back();

            return state;
        }
    }
}

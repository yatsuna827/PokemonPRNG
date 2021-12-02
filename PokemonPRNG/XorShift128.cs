
namespace PokemonPRNG.XorShift128
{
    public static class XorShift128Ext
    {
        public static uint GetRand(this ref (uint s1, uint s2, uint s3, uint s4) state)
        {
            var t1 = state.s1 ^ (state.s1 << 11);
            var t2 = state.s4 ^ (state.s4 >> 19) ^ t1 ^ (t1 >> 8);

            state = (state.s2, state.s3, state.s4, t2);

            return t2;
        }
    }
}

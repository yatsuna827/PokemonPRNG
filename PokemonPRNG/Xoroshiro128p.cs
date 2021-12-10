using System;
using System.Collections.Generic;
using System.Text;

namespace PokemonPRNG.Xoroshiro128p
{
    public static class Xoroshiro128p
    {
        public const ulong FIXSEED = 0x82A2B175229D6A5B;

        public static ulong GetRand(ref this (ulong s0, ulong s1) state)
        {
            var (_s0, _s1) = (state.s0, state.s0 ^ state.s1);
            var res = state.s0 + state.s1;

            state = (((_s0 << 24) | (_s0 >> 40)) ^ _s1 ^ (_s1 << 16), (_s1 << 37) | (_s1 >> 27));

            return res;
        }
        public static ulong GetRand(ref this (ulong s0, ulong s1) state, uint range)
        {
            var ceil2 = GetRandPow2(range);

            while (true)
            {
                var result = state.GetRand() & ceil2;
                if (result <= range) return result;
            }
        }
        private static ulong GetRandPow2(uint num)
        {
            if ((num & (num - 1)) == 0) return num - 1;

            ulong res = 1;
            while (res < num) res <<= 1;
            return res - 1;
        }

        public static (ulong s0, ulong s1) Advance(ref this (ulong s0, ulong s1) state)
        {
            var (s0, s1) = (state.s0, state.s0 ^ state.s1);

            return state = (((s0 << 24) | (s0 >> 40)) ^ s1 ^ (s1 << 16), (s1 << 37) | (s1 >> 27));
        }
        public static (ulong s0, ulong s1) Back(ref this (ulong s0, ulong s1) state)
        {
            var (s0, s1) = state;

            var s1_rotl27 = (s1 << 27) | (s1 >> 37);
            var t = s0 ^ (s1_rotl27 << 16);
            t = ((t << 40) | (t >> 24)) ^ ((s1 << 3) | (s1 >> 61));

            return state = (t, t ^ s1_rotl27);
        }

        public static string ToU128String(this (ulong s0, ulong s1) state) => $"{state.s1:X16}{state.s0:X16}";
        public static (ulong s0, ulong s1) FromU128String(this string hex)
        {
            if (hex.Length != 32) throw new ArgumentException("bad argument");

            var t0 = hex.Substring(16);
            var t1 = hex.Substring(0, 16);

            var s0 = Convert.ToUInt64(t0, 16);
            var s1 = Convert.ToUInt64(t1, 16);

            return (s0, s1);
        }
        }

}

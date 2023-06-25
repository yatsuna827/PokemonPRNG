using System;
using System.Linq;

namespace PokemonPRNG.TinyMT
{
    public class TinyMT
    {
        public TinyMT(uint initialSeed)
        {
            this.stateVector = new uint[4]
            {
                initialSeed,
                MAT1,
                MAT2,
                TMAT
            };

            for (uint i = 1; i < 8; i++)
                stateVector[i & 3] ^= i + 0x6C078965u * (stateVector[(i - 1) & 3] ^ (stateVector[(i - 1) & 3] >> 30));

            for (int i = 0; i < 8; i++)
                Advance();
        }

        public TinyMT(uint[] vec)
            => this.stateVector = vec.ToArray();

        public TinyMT(TinyMT parent)
            => this.stateVector = parent.stateVector.ToArray();

        public TinyMT Clone() => new TinyMT(this);

        public void Advance()
        {
            uint y = stateVector[3];
            uint x = (stateVector[0] & TINYMT32_MASK) ^ stateVector[1] ^ stateVector[2];
            x ^= (x << TINYMT32_SH0);
            y ^= (y >> TINYMT32_SH0) ^ x;
            stateVector[0] = stateVector[1];
            stateVector[1] = stateVector[2];
            stateVector[2] = x ^ (y << TINYMT32_SH1);
            stateVector[3] = y;

            if ((y & 1) == 1)
            {
                stateVector[1] ^= MAT1;
                stateVector[2] ^= MAT2;
            }
        }

        private uint Temper()
        {
            uint t0 = stateVector[3];
            uint t1 = stateVector[0] + (stateVector[2] >> TINYMT32_SH8);

            t0 ^= t1;
            if ((t1 & 1) == 1) t0 ^= TMAT;

            return t0;
        }

        public uint GetRand()
        {
            Advance();
            return Temper();
        }

        private const uint TINYMT32_MASK = 0x7FFFFFFF;
        private const int TINYMT32_SH0 = 1;
        private const int TINYMT32_SH1 = 10;
        private const int TINYMT32_SH8 = 8;
        private const uint MAT1 = 0x8f7011ee;
        private const uint MAT2 = 0xfc78ff1f;
        private const uint TMAT = 0x3793fdff;

        private readonly uint[] stateVector;
    }

    public static class TinyMTExt
    {
        public static (uint S0, uint S1, uint S2, uint S3) TinyMT(this uint initialSeed)
        {
            var stateVector = new uint[4]
            {
                initialSeed,
                0x8f7011ee,
                0xfc78ff1f,
                0x3793fdff,
            };

            for (uint i = 1; i < 8; i++)
                stateVector[i & 3] ^= i + 0x6C078965u * (stateVector[(i - 1) & 3] ^ (stateVector[(i - 1) & 3] >> 30));

            var state = (stateVector[0], stateVector[1], stateVector[2], stateVector[3]);

            for (int i = 0; i < 8; i++)
                state.Advance();

            return state;
        }

        public static uint GetRand(ref this (uint S0, uint S1, uint S2, uint S3) state)
        {
            var (x, y) = ((state.S0 & 0x7FFFFFFF) ^ state.S1 ^ state.S2, state.S3);
            x ^= (x << 1);
            y ^= (y >> 1) ^ x;

            state = (state.S1, state.S2, x ^ (y << 10), y);
            if ((y & 1) == 1)
            {
                state.S1 ^= 0x8f7011ee;
                state.S2 ^= 0xfc78ff1f;
            }

            var (t0, t1) = (state.S3, state.S0 + (state.S2 >> 8));

            t0 ^= t1;
            if ((t1 & 1) == 1) t0 ^= 0x3793fdff;

            return t0;
        }
        public static uint GetRand(ref this (uint S0, uint S1, uint S2, uint S3) state, uint n)
            => state.GetRand() % n;

        public static (uint S0, uint S1, uint S2, uint S3) Next(this (uint S0, uint S1, uint S2, uint S3) state)
        {
            var (x, y) = ((state.S0 & 0x7FFFFFFF) ^ state.S1 ^ state.S2, state.S3);
            x ^= (x << 1);
            y ^= (y >> 1) ^ x;

            var (s0, s1, s2, s3) = (state.S1, state.S2, x ^ (y << 10), y);
            if ((y & 1) == 1)
            {
                s1 ^= 0x8f7011ee;
                s2 ^= 0xfc78ff1f;
            }

            return (s0, s1, s2, s3);
        }
        public static (uint S0, uint S1, uint S2, uint S3) Next(this (uint S0, uint S1, uint S2, uint S3) state, uint n)
        {
            for (int i = 0; i < n; i++)
                state.Advance();

            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Advance(ref this (uint S0, uint S1, uint S2, uint S3) state)
        {
            var (x, y) = ((state.S0 & 0x7FFFFFFF) ^ state.S1 ^ state.S2, state.S3);
            x ^= (x << 1);
            y ^= (y >> 1) ^ x;

            var (s0, s1, s2, s3) = (state.S1, state.S2, x ^ (y << 10), y);
            if ((y & 1) == 1)
            {
                s1 ^= 0x8f7011ee;
                s2 ^= 0xfc78ff1f;
            }

            return state = (s0, s1, s2, s3);
        }
        public static (uint S0, uint S1, uint S2, uint S3) Advance(ref this (uint S0, uint S1, uint S2, uint S3) state, uint n)
        {
            for (int i = 0; i < n; i++)
                state.Advance();

            return state;
        }

    }
}

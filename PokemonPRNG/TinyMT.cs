using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonPRNG
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
}

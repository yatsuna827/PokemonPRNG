using System;
using System.Linq;
using System.Security.Cryptography;

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
        public static (uint S0, uint S1, uint S2, uint S3) Prev(this (uint S0, uint S1, uint S2, uint S3) state)
        {
            var odd = (state.S3 & 1) == 1;

            var s1 = state.S0;
            var s2 = odd ? state.S1 ^ 0x8f7011ee : state.S1;

            var x = (odd ? state.S2 ^ 0xfc78ff1f : state.S2) ^ (state.S3 << 10);
            var y = x ^ state.S3;

            var s0 = x ^ s1 ^ s2;
            var s3 = y;
            for (int i = 1; i < 32; i++)
            {
                s0 ^= x << i;
                s3 ^= y >> i;
            }

            return (s0, s1, s2, s3);
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
        public static (uint S0, uint S1, uint S2, uint S3) Back(ref this (uint S0, uint S1, uint S2, uint S3) state)
        {
            var odd = (state.S3 & 1) == 1;

            var s1 = state.S0;
            var s2 = odd ? state.S1 ^ 0x8f7011ee : state.S1;

            var x = (odd ? state.S2 ^ 0xfc78ff1f : state.S2) ^ (state.S3 << 10);
            var y = x ^ state.S3;

            var s0 = x ^ s1 ^ s2;
            var s3 = y;
            for (int i = 1; i < 32; i++)
            {
                s0 ^= x << i;
                s3 ^= y >> i;
            }

            return state = (s0, s1, s2, s3);
        }

    }

    public static class TinyMTJumpExt
    {
        private static readonly (uint S0, uint S1, uint S2, uint S3)[] tinyMTMatrix = new (uint, uint, uint, uint)[128]
        {
            (0x00000000u, 0x00000001u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000002u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000004u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000008u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000010u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000020u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000040u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000080u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000100u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000200u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000400u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000800u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00001000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00002000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00004000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00008000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00010000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00020000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00040000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00080000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00100000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00200000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00400000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00800000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x01000000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x02000000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x04000000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x08000000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x10000000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x20000000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x40000000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x80000000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000001u, 0x00000000u),
            (0x00000001u, 0x00000001u, 0x00000003u, 0x00000003u),
            (0x00000001u, 0x00000001u, 0x00000005u, 0x00000003u),
            (0x00000001u, 0x00000001u, 0x00000009u, 0x00000003u),
            (0x00000000u, 0x00000000u, 0x00000010u, 0x00000000u),
            (0x00000001u, 0x00000001u, 0x00000021u, 0x00000003u),
            (0x00000001u, 0x00000001u, 0x00000041u, 0x00000003u),
            (0x00000001u, 0x00000001u, 0x00000081u, 0x00000003u),
            (0x00000001u, 0x00000001u, 0x00000101u, 0x00000003u),
            (0x00000000u, 0x00000000u, 0x00000200u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000400u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000800u, 0x00000000u),
            (0x00000001u, 0x00000001u, 0x00001001u, 0x00000003u),
            (0x00000000u, 0x00000000u, 0x00002000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00004000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00008000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00010000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00020000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00040000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00080000u, 0x00000000u),
            (0x00000001u, 0x00000001u, 0x00100001u, 0x00000003u),
            (0x00000001u, 0x00000001u, 0x00200001u, 0x00000003u),
            (0x00000001u, 0x00000001u, 0x00400001u, 0x00000003u),
            (0x00000000u, 0x00000000u, 0x00800000u, 0x00000000u),
            (0x00000001u, 0x00000001u, 0x01000001u, 0x00000003u),
            (0x00000001u, 0x00000001u, 0x02000001u, 0x00000003u),
            (0x00000001u, 0x00000001u, 0x04000001u, 0x00000003u),
            (0x00000001u, 0x00000001u, 0x08000001u, 0x00000003u),
            (0x00000000u, 0x00000000u, 0x10000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x20000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x40000000u, 0x00000000u),
            (0x00000001u, 0x00000001u, 0x80000001u, 0x00000003u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00000003u),
            (0x00000002u, 0x00000002u, 0x00000002u, 0x00000003u),
            (0x00000007u, 0x00000007u, 0x00000007u, 0x00000003u),
            (0x0000000Du, 0x0000000Du, 0x0000000Du, 0x00000003u),
            (0x00000019u, 0x00000019u, 0x00000019u, 0x00000003u),
            (0x00000030u, 0x00000030u, 0x00000030u, 0x00000000u),
            (0x00000060u, 0x00000060u, 0x00000060u, 0x00000000u),
            (0x000000C0u, 0x000000C0u, 0x000000C0u, 0x00000000u),
            (0x00000181u, 0x00000181u, 0x00000181u, 0x00000003u),
            (0x00000301u, 0x00000301u, 0x00000301u, 0x00000003u),
            (0x00000600u, 0x00000600u, 0x00000600u, 0x00000000u),
            (0x00000C02u, 0x00000C02u, 0x00000C02u, 0x00000005u),
            (0x00001807u, 0x00001807u, 0x00001807u, 0x0000000Fu),
            (0x0000300Du, 0x0000300Du, 0x0000300Du, 0x0000001Bu),
            (0x00006019u, 0x00006019u, 0x00006019u, 0x00000033u),
            (0x0000C031u, 0x0000C031u, 0x0000C031u, 0x00000063u),
            (0x00018060u, 0x00018060u, 0x00018060u, 0x000000C0u),
            (0x000300C0u, 0x000300C0u, 0x000300C0u, 0x00000180u),
            (0x00060180u, 0x00060180u, 0x00060180u, 0x00000300u),
            (0x000C0301u, 0x000C0301u, 0x000C0301u, 0x00000603u),
            (0x00180601u, 0x00180601u, 0x00180601u, 0x00000C03u),
            (0x00300C01u, 0x00300C01u, 0x00300C01u, 0x00001803u),
            (0x00601801u, 0x00601801u, 0x00601801u, 0x00003003u),
            (0x00C03000u, 0x00C03000u, 0x00C03000u, 0x00006000u),
            (0x01806000u, 0x01806000u, 0x01806000u, 0x0000C000u),
            (0x0300C000u, 0x0300C000u, 0x0300C000u, 0x00018000u),
            (0x06018001u, 0x06018001u, 0x06018001u, 0x00030003u),
            (0x0C030001u, 0x0C030001u, 0x0C030001u, 0x00060003u),
            (0x18060001u, 0x18060001u, 0x18060001u, 0x000C0003u),
            (0x300C0001u, 0x300C0001u, 0x300C0001u, 0x00180003u),
            (0x60180001u, 0x60180001u, 0x60180001u, 0x00300003u),
            (0x40300001u, 0xC0300001u, 0xC0300001u, 0x00600003u),
            (0x00000001u, 0x00000001u, 0x00000001u, 0x00000003u),
            (0x00000003u, 0x00000003u, 0x00000003u, 0x00000006u),
            (0x00000006u, 0x00000006u, 0x00000006u, 0x0000000Cu),
            (0x0000000Cu, 0x0000000Cu, 0x0000000Cu, 0x00000018u),
            (0x00000018u, 0x00000018u, 0x00000018u, 0x00000030u),
            (0x00000030u, 0x00000030u, 0x00000030u, 0x00000060u),
            (0x00000060u, 0x00000060u, 0x00000060u, 0x000000C0u),
            (0x000000C0u, 0x000000C0u, 0x000000C0u, 0x00000180u),
            (0x00000180u, 0x00000180u, 0x00000180u, 0x00000300u),
            (0x00000300u, 0x00000300u, 0x00000300u, 0x00000600u),
            (0x00000600u, 0x00000600u, 0x00000600u, 0x00000C00u),
            (0x00000C00u, 0x00000C00u, 0x00000C00u, 0x00001800u),
            (0x00001800u, 0x00001800u, 0x00001800u, 0x00003000u),
            (0x00003000u, 0x00003000u, 0x00003000u, 0x00006000u),
            (0x00006000u, 0x00006000u, 0x00006000u, 0x0000C000u),
            (0x0000C000u, 0x0000C000u, 0x0000C000u, 0x00018000u),
            (0x00018000u, 0x00018000u, 0x00018000u, 0x00030000u),
            (0x00030000u, 0x00030000u, 0x00030000u, 0x00060000u),
            (0x00060000u, 0x00060000u, 0x00060000u, 0x000C0000u),
            (0x000C0000u, 0x000C0000u, 0x000C0000u, 0x00180000u),
            (0x00180000u, 0x00180000u, 0x00180000u, 0x00300000u),
            (0x00300000u, 0x00300000u, 0x00300000u, 0x00600000u),
            (0x00600000u, 0x00600000u, 0x00600000u, 0x00C00000u),
            (0x00C00000u, 0x00C00000u, 0x00C00000u, 0x01800000u),
            (0x01800000u, 0x01800000u, 0x01800000u, 0x03000000u),
            (0x03000000u, 0x03000000u, 0x03000000u, 0x06000000u),
            (0x06000000u, 0x06000000u, 0x06000000u, 0x0C000000u),
            (0x0C000000u, 0x0C000000u, 0x0C000000u, 0x18000000u),
            (0x18000000u, 0x18000000u, 0x18000000u, 0x30000000u),
            (0x30000000u, 0x30000000u, 0x30000000u, 0x60000000u),
            (0x60000000u, 0x60000000u, 0x60000000u, 0xC0000000u),
            (0x40000000u, 0xC0000000u, 0xC0000000u, 0x80000000u),
        };
        private static readonly (uint S0, uint S1, uint S2, uint S3)[] tinyMTBackMatrix = new (uint, uint, uint, uint)[128]
        {
            (0x00000001u, 0x00000001u, 0x00000001u, 0x00000001u),
            (0x00000002u, 0x00000002u, 0x00000003u, 0x00000001u),
            (0x00000004u, 0x00000004u, 0x00000007u, 0x00000000u),
            (0x00000008u, 0x00000008u, 0x0000000Fu, 0x00000001u),
            (0x00000010u, 0x00000010u, 0x0000001Fu, 0x00000001u),
            (0x00000020u, 0x00000020u, 0x0000003Fu, 0x00000000u),
            (0x00000040u, 0x00000040u, 0x0000007Fu, 0x00000000u),
            (0x00000080u, 0x00000080u, 0x000000FFu, 0x00000000u),
            (0x00000100u, 0x00000100u, 0x000001FFu, 0x00000001u),
            (0x00000200u, 0x00000200u, 0x000003FFu, 0x00000001u),
            (0x00000400u, 0x00000400u, 0x000007FFu, 0x00000001u),
            (0x00000800u, 0x00000800u, 0x00000FFFu, 0x00000002u),
            (0x00001000u, 0x00001000u, 0x00001FFFu, 0x00000006u),
            (0x00002000u, 0x00002000u, 0x00003FFFu, 0x0000000Eu),
            (0x00004000u, 0x00004000u, 0x00007FFFu, 0x0000001Fu),
            (0x00008000u, 0x00008000u, 0x0000FFFFu, 0x0000003Eu),
            (0x00010000u, 0x00010000u, 0x0001FFFFu, 0x0000007Eu),
            (0x00020000u, 0x00020000u, 0x0003FFFFu, 0x000000FEu),
            (0x00040000u, 0x00040000u, 0x0007FFFFu, 0x000001FEu),
            (0x00080000u, 0x00080000u, 0x000FFFFFu, 0x000003FFu),
            (0x00100000u, 0x00100000u, 0x001FFFFFu, 0x000007FFu),
            (0x00200000u, 0x00200000u, 0x003FFFFFu, 0x00000FFEu),
            (0x00400000u, 0x00400000u, 0x007FFFFFu, 0x00001FFFu),
            (0x00800000u, 0x00800000u, 0x00FFFFFFu, 0x00003FFEu),
            (0x01000000u, 0x01000000u, 0x01FFFFFFu, 0x00007FFFu),
            (0x02000000u, 0x02000000u, 0x03FFFFFFu, 0x0000FFFFu),
            (0x04000000u, 0x04000000u, 0x07FFFFFFu, 0x0001FFFEu),
            (0x08000000u, 0x08000000u, 0x0FFFFFFFu, 0x0003FFFFu),
            (0x10000000u, 0x10000000u, 0x1FFFFFFFu, 0x0007FFFFu),
            (0x20000000u, 0x20000000u, 0x3FFFFFFFu, 0x000FFFFEu),
            (0x40000000u, 0x40000000u, 0x7FFFFFFFu, 0x001FFFFFu),
            (0x80000000u, 0x80000000u, 0xFFFFFFFFu, 0x003FFFFFu),
            (0x00000001u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00000002u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00000004u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00000008u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00000010u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00000020u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00000040u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00000080u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00000100u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00000200u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00000400u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00000800u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00001000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00002000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00004000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00008000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00010000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00020000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00040000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00080000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00100000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00200000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00400000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00800000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x01000000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x02000000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x04000000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x08000000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x10000000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x20000000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x40000000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x80000000u, 0x00000000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000001u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000002u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x00000004u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x00000008u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x00000010u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000020u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x00000040u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x00000080u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x00000100u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x00000200u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000400u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00000800u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00001000u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x00002000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00004000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00008000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00010000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00020000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00040000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00080000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x00100000u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x00200000u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x00400000u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x00800000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x01000000u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x02000000u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x04000000u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x08000000u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x10000000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x20000000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x40000000u, 0x00000000u, 0x00000000u),
            (0x00000000u, 0x80000000u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x00000000u, 0xFFFFFFFFu, 0xFFC00001u),
            (0x00000000u, 0x00000000u, 0xFFFFFFFEu, 0xFFC00001u),
            (0x00000000u, 0x00000000u, 0xFFFFFFFCu, 0xFFC00002u),
            (0x00000000u, 0x00000000u, 0xFFFFFFF8u, 0xFFC00007u),
            (0x00000000u, 0x00000000u, 0xFFFFFFF0u, 0xFFC0000Eu),
            (0x00000000u, 0x00000000u, 0xFFFFFFE0u, 0xFFC0001Fu),
            (0x00000000u, 0x00000000u, 0xFFFFFFC0u, 0xFFC0003Fu),
            (0x00000000u, 0x00000000u, 0xFFFFFF80u, 0xFFC0007Fu),
            (0x00000000u, 0x00000000u, 0xFFFFFF00u, 0xFFC000FFu),
            (0x00000000u, 0x00000000u, 0xFFFFFE00u, 0xFFC001FEu),
            (0x00000000u, 0x00000000u, 0xFFFFFC00u, 0xFFC003FFu),
            (0x00000000u, 0x00000000u, 0xFFFFF800u, 0xFFC007FFu),
            (0x00000000u, 0x00000000u, 0xFFFFF000u, 0xFFC00FFCu),
            (0x00000000u, 0x00000000u, 0xFFFFE000u, 0xFFC01FF9u),
            (0x00000000u, 0x00000000u, 0xFFFFC000u, 0xFFC03FF0u),
            (0x00000000u, 0x00000000u, 0xFFFF8000u, 0xFFC07FE1u),
            (0x00000000u, 0x00000000u, 0xFFFF0000u, 0xFFC0FFC0u),
            (0x00000000u, 0x00000000u, 0xFFFE0000u, 0xFFC1FF80u),
            (0x00000000u, 0x00000000u, 0xFFFC0000u, 0xFFC3FF00u),
            (0x00000000u, 0x00000000u, 0xFFF80000u, 0xFFC7FE00u),
            (0x00000000u, 0x00000000u, 0xFFF00000u, 0xFFCFFC01u),
            (0x00000000u, 0x00000000u, 0xFFE00000u, 0xFFDFF800u),
            (0x00000000u, 0x00000000u, 0xFFC00000u, 0xFFFFF001u),
            (0x00000000u, 0x00000000u, 0xFF800000u, 0xFFBFE000u),
            (0x00000000u, 0x00000000u, 0xFF000000u, 0xFF3FC000u),
            (0x00000000u, 0x00000000u, 0xFE000000u, 0xFE3F8000u),
            (0x00000000u, 0x00000000u, 0xFC000000u, 0xFC3F0000u),
            (0x00000000u, 0x00000000u, 0xF8000000u, 0xF83E0001u),
            (0x00000000u, 0x00000000u, 0xF0000000u, 0xF03C0000u),
            (0x00000000u, 0x00000000u, 0xE0000000u, 0xE0380001u),
            (0x00000000u, 0x00000000u, 0xC0000000u, 0xC0300000u),
            (0x00000000u, 0x00000000u, 0x80000000u, 0x80200001u),
        };
        private readonly static (uint S0, uint S1, uint S2, uint S3)[][] jumpMatrixes, backJumpMatrixes;

        static TinyMTJumpExt()
        {
            jumpMatrixes = new (uint, uint, uint, uint)[64][];
            jumpMatrixes[0] = tinyMTMatrix;
            for (int i = 1; i < 64; i++)
                jumpMatrixes[i] = jumpMatrixes[i - 1].Products(jumpMatrixes[i - 1]);

            backJumpMatrixes = new (uint S0, uint S1, uint S2, uint S3)[64][];
            backJumpMatrixes[0] = tinyMTBackMatrix;
            for (int i = 1; i < 64; i++)
                backJumpMatrixes[i] = backJumpMatrixes[i - 1].Products(backJumpMatrixes[i - 1]);
        }

        public static (uint S0, uint S1, uint S2, uint S3) Next(this (uint S0, uint S1, uint S2, uint S3) state, uint n)
        {
            for (int i = 0; i < 32; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(jumpMatrixes[i]);

            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Next(this (uint S0, uint S1, uint S2, uint S3) state, ulong n)
        {
            for (int i = 0; i < 64; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(jumpMatrixes[i]);

            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Advance(ref this (uint S0, uint S1, uint S2, uint S3) state, uint n)
        {
            for (int i = 0; i < 32; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(jumpMatrixes[i]);

            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Advance(ref this (uint S0, uint S1, uint S2, uint S3) state, uint n, ref uint index)
        {
            for (int i = 0; i < 32; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(jumpMatrixes[i]);

            index += n;
            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Advance(ref this (uint S0, uint S1, uint S2, uint S3) state, ulong n)
        {
            for (int i = 0; i < 64; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(jumpMatrixes[i]);

            return state;
        }

        public static (uint S0, uint S1, uint S2, uint S3) Prev(this (uint S0, uint S1, uint S2, uint S31) state, uint n)
        {
            for (int i = 0; i < 32; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(backJumpMatrixes[i]);

            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Prev(this (uint S0, uint S1, uint S2, uint S3) state, ulong n)
        {
            for (int i = 0; i < 64; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(backJumpMatrixes[i]);

            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Back(ref this (uint S0, uint S1, uint S2, uint S3) state, uint n)
        {
            for (int i = 0; i < 32; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(backJumpMatrixes[i]);

            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Back(ref this (uint S0, uint S1, uint S2, uint S3) state, uint n, ref uint index)
        {
            for (int i = 0; i < 32; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(backJumpMatrixes[i]);

            index += n;
            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Back(ref this (uint S0, uint S1, uint S2, uint S3) state, ulong n)
        {
            for (int i = 0; i < 64; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(backJumpMatrixes[i]);

            return state;
        }

        // ベクトルに行列を作用させる。
        private static (uint S0, uint S1, uint S2, uint S3) Products(this (uint S0, uint S1, uint S2, uint S3) state, (uint S0, uint S1, uint S2, uint S3)[] matrix)
        {
            var r_s0 = 0U;
            var r_s1 = 0U;
            var r_s2 = 0U;
            var r_s3 = 0U;

            for (int i = 0; i < 32; i++)
            {
                {
                    var (s0, s1, s2, s3) = matrix[i];
                    var conv =
                        (s0 & state.S0).PopCount()
                        ^ (s1 & state.S1).PopCount()
                        ^ (s2 & state.S2).PopCount()
                        ^ (s3 & state.S3).PopCount();
                    r_s0 |= (uint)conv << i;
                }
                {
                    var (s0, s1, s2, s3) = matrix[i + 32];
                    var conv =
                        (s0 & state.S0).PopCount()
                        ^ (s1 & state.S1).PopCount()
                        ^ (s2 & state.S2).PopCount()
                        ^ (s3 & state.S3).PopCount();
                    r_s1 |= (uint)conv << i;
                }
                {
                    var (s0, s1, s2, s3) = matrix[i + 64];
                    var conv =
                        (s0 & state.S0).PopCount()
                        ^ (s1 & state.S1).PopCount()
                        ^ (s2 & state.S2).PopCount()
                        ^ (s3 & state.S3).PopCount();
                    r_s2 |= (uint)conv << i;
                }
                {
                    var (s0, s1, s2, s3) = matrix[i + 96];
                    var conv =
                        (s0 & state.S0).PopCount()
                        ^ (s1 & state.S1).PopCount()
                        ^ (s2 & state.S2).PopCount()
                        ^ (s3 & state.S3).PopCount();
                    r_s3 |= (uint)conv << i;
                }
            }

            return (r_s0, r_s1, r_s2, r_s3);
        }
        // 行列に行列を作用させる。
        private static (uint S0, uint S1, uint S2, uint S3)[] Products(this (uint S0, uint S1, uint S2, uint S3)[] matrix1, (uint S0, uint S1, uint S2, uint S3)[] matrix2)
        {
            var mat = new (uint S0, uint S1, uint S2, uint S3)[128];
            matrix2 = matrix2.Transpose();

            for (int i = 0; i < 128; i++)
            {
                for (int k = 0; k < 32; k++)
                {
                    var bit1 =
                        (matrix1[i].S0 & matrix2[k].S0).PopCount()
                        ^ (matrix1[i].S1 & matrix2[k].S1).PopCount()
                        ^ (matrix1[i].S2 & matrix2[k].S2).PopCount()
                        ^ (matrix1[i].S3 & matrix2[k].S3).PopCount();
                    var bit2 =
                        (matrix1[i].S0 & matrix2[k + 32].S0).PopCount()
                        ^ (matrix1[i].S1 & matrix2[k + 32].S1).PopCount()
                        ^ (matrix1[i].S2 & matrix2[k + 32].S2).PopCount()
                        ^ (matrix1[i].S3 & matrix2[k + 32].S3).PopCount();
                    var bit3 =
                        (matrix1[i].S0 & matrix2[k + 64].S0).PopCount()
                        ^ (matrix1[i].S1 & matrix2[k + 64].S1).PopCount()
                        ^ (matrix1[i].S2 & matrix2[k + 64].S2).PopCount()
                        ^ (matrix1[i].S3 & matrix2[k + 64].S3).PopCount();
                    var bit4 =
                        (matrix1[i].S0 & matrix2[k + 96].S0).PopCount()
                        ^ (matrix1[i].S1 & matrix2[k + 96].S1).PopCount()
                        ^ (matrix1[i].S2 & matrix2[k + 96].S2).PopCount()
                        ^ (matrix1[i].S3 & matrix2[k + 96].S3).PopCount();

                    mat[i].S0 |= (uint)bit1 << k;
                    mat[i].S1 |= (uint)bit2 << k;
                    mat[i].S2 |= (uint)bit3 << k;
                    mat[i].S3 |= (uint)bit4 << k;
                }
            }

            return mat;
        }
        private static (uint S0, uint S1, uint S2, uint S3)[] Transpose(this (uint S0, uint S1, uint S2, uint S3)[] matrix)
        {
            var mat = new (uint S0, uint S1, uint S2, uint S3)[128];
            for (int i = 0; i < 32; i++)
            {
                var (s0, s1, s2, s3) = matrix[i];
                for (int k = 0; k < 32; k++)
                {
                    mat[k].S0 |= ((s0 & (1u << k)) >> k) << i;
                    mat[k + 32].S0 |= ((s1 & (1u << k)) >> k) << i;
                    mat[k + 64].S0 |= ((s2 & (1u << k)) >> k) << i;
                    mat[k + 96].S0 |= ((s3 & (1u << k)) >> k) << i;
                }
            }
            for (int i = 0; i < 32; i++)
            {
                var (s0, s1, s2, s3) = matrix[i + 32];
                for (int k = 0; k < 32; k++)
                {
                    mat[k].S1 |= ((s0 & (1u << k)) >> k) << i;
                    mat[k + 32].S1 |= ((s1 & (1u << k)) >> k) << i;
                    mat[k + 64].S1 |= ((s2 & (1u << k)) >> k) << i;
                    mat[k + 96].S1 |= ((s3 & (1u << k)) >> k) << i;
                }
            }
            for (int i = 0; i < 32; i++)
            {
                var (s0, s1, s2, s3) = matrix[i + 64];
                for (int k = 0; k < 32; k++)
                {
                    mat[k].S2 |= ((s0 & (1u << k)) >> k) << i;
                    mat[k + 32].S2 |= ((s1 & (1u << k)) >> k) << i;
                    mat[k + 64].S2 |= ((s2 & (1u << k)) >> k) << i;
                    mat[k + 96].S2 |= ((s3 & (1u << k)) >> k) << i;
                }
            }
            for (int i = 0; i < 32; i++)
            {
                var (s0, s1, s2, s3) = matrix[i + 96];
                for (int k = 0; k < 32; k++)
                {
                    mat[k].S3 |= ((s0 & (1u << k)) >> k) << i;
                    mat[k + 32].S3 |= ((s1 & (1u << k)) >> k) << i;
                    mat[k + 64].S3 |= ((s2 & (1u << k)) >> k) << i;
                    mat[k + 96].S3 |= ((s3 & (1u << k)) >> k) << i;
                }
            }

            return mat;
        }

        // .NET 5以前はBitOperation.PopCountが提供されていないので。
        private static int PopCount(this uint x)
        {
            x = (x & 0x55555555u) + ((x & 0xAAAAAAAAu) >> 1);
            x = (x & 0x33333333u) + ((x & 0xCCCCCCCCu) >> 2);
            x = (x & 0x0F0F0F0Fu) + ((x & 0xF0F0F0F0u) >> 4);

            x += x >> 8;
            x += x >> 16;
            return (int)(x & 1);
        }
    }
}

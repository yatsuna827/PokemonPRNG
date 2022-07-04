using System;
using System.Collections.Generic;
using System.Linq;

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
        public static ulong GetRand(ref this (ulong s0, ulong s1) state, ref uint index)
        {
            var (_s0, _s1) = (state.s0, state.s0 ^ state.s1);
            var res = state.s0 + state.s1;

            state = (((_s0 << 24) | (_s0 >> 40)) ^ _s1 ^ (_s1 << 16), (_s1 << 37) | (_s1 >> 27));

            index++;
            return res;
        }
        public static ulong GetRand(ref this (ulong s0, ulong s1) state, uint range)
        {
            var ceil2 = GetRandPow2(range);

            while (true)
            {
                var result = state.GetRand() & ceil2;
                if (result < range) return result;
            }
        }
        public static ulong GetRand(ref this (ulong s0, ulong s1) state, uint range, ref uint index)
        {
            var ceil2 = GetRandPow2(range);

            while (true)
            {
                index++;
                var result = state.GetRand() & ceil2;
                if (result < range) return result;
            }
        }
        private static ulong GetRandPow2(uint num)
        {
            if ((num & (num - 1)) == 0) return num - 1;

            ulong res = 1;
            while (res < num) res <<= 1;
            return res - 1;
        }

        public static (ulong s0, ulong s1) Next(this (ulong s0, ulong s1) state)
        {
            var (s0, s1) = (state.s0, state.s0 ^ state.s1);

            return (((s0 << 24) | (s0 >> 40)) ^ s1 ^ (s1 << 16), (s1 << 37) | (s1 >> 27));
        }
        public static (ulong s0, ulong s1) Prev(this (ulong s0, ulong s1) state)
        {
            var (s0, s1) = state;

            var s1_rotl27 = (s1 << 27) | (s1 >> 37);
            var t = s0 ^ (s1_rotl27 << 16);
            t = ((t << 40) | (t >> 24)) ^ ((s1 << 3) | (s1 >> 61));

            return (t, t ^ s1_rotl27);
        }
        public static (ulong s0, ulong s1) Advance(ref this (ulong s0, ulong s1) state)
        {
            var (s0, s1) = (state.s0, state.s0 ^ state.s1);

            return state = (((s0 << 24) | (s0 >> 40)) ^ s1 ^ (s1 << 16), (s1 << 37) | (s1 >> 27));
        }
        public static (ulong s0, ulong s1) Advance(ref this (ulong s0, ulong s1) state, ref uint index)
        {
            var (s0, s1) = (state.s0, state.s0 ^ state.s1);

            index++;
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
        public static (ulong s0, ulong s1) Back(ref this (ulong s0, ulong s1) state, ref uint index)
        {
            var (s0, s1) = state;

            var s1_rotl27 = (s1 << 27) | (s1 >> 37);
            var t = s0 ^ (s1_rotl27 << 16);
            t = ((t << 40) | (t >> 24)) ^ ((s1 << 3) | (s1 >> 61));

            index--;
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
    
    public static class Xoroshiro128pJumpExt
    {
        // xoroshiro128pの更新関数を行列で表現したもの。
        // 普通に使う分には非効率。。。
        private readonly static (ulong s0, ulong s1)[] xoroshiroMatrix = new (ulong s0, ulong s1)[128]
        {
            (0x0000010000000001, 0x0000000000000001),
            (0x0000020000000002, 0x0000000000000002),
            (0x0000040000000004, 0x0000000000000004),
            (0x0000080000000008, 0x0000000000000008),
            (0x0000100000000010, 0x0000000000000010),
            (0x0000200000000020, 0x0000000000000020),
            (0x0000400000000040, 0x0000000000000040),
            (0x0000800000000080, 0x0000000000000080),
            (0x0001000000000100, 0x0000000000000100),
            (0x0002000000000200, 0x0000000000000200),
            (0x0004000000000400, 0x0000000000000400),
            (0x0008000000000800, 0x0000000000000800),
            (0x0010000000001000, 0x0000000000001000),
            (0x0020000000002000, 0x0000000000002000),
            (0x0040000000004000, 0x0000000000004000),
            (0x0080000000008000, 0x0000000000008000),
            (0x0100000000010001, 0x0000000000010001),
            (0x0200000000020002, 0x0000000000020002),
            (0x0400000000040004, 0x0000000000040004),
            (0x0800000000080008, 0x0000000000080008),
            (0x1000000000100010, 0x0000000000100010),
            (0x2000000000200020, 0x0000000000200020),
            (0x4000000000400040, 0x0000000000400040),
            (0x8000000000800080, 0x0000000000800080),
            (0x0000000001000101, 0x0000000001000100),
            (0x0000000002000202, 0x0000000002000200),
            (0x0000000004000404, 0x0000000004000400),
            (0x0000000008000808, 0x0000000008000800),
            (0x0000000010001010, 0x0000000010001000),
            (0x0000000020002020, 0x0000000020002000),
            (0x0000000040004040, 0x0000000040004000),
            (0x0000000080008080, 0x0000000080008000),
            (0x0000000100010100, 0x0000000100010000),
            (0x0000000200020200, 0x0000000200020000),
            (0x0000000400040400, 0x0000000400040000),
            (0x0000000800080800, 0x0000000800080000),
            (0x0000001000101000, 0x0000001000100000),
            (0x0000002000202000, 0x0000002000200000),
            (0x0000004000404000, 0x0000004000400000),
            (0x0000008000808000, 0x0000008000800000),
            (0x0000010001010000, 0x0000010001000000),
            (0x0000020002020000, 0x0000020002000000),
            (0x0000040004040000, 0x0000040004000000),
            (0x0000080008080000, 0x0000080008000000),
            (0x0000100010100000, 0x0000100010000000),
            (0x0000200020200000, 0x0000200020000000),
            (0x0000400040400000, 0x0000400040000000),
            (0x0000800080800000, 0x0000800080000000),
            (0x0001000101000000, 0x0001000100000000),
            (0x0002000202000000, 0x0002000200000000),
            (0x0004000404000000, 0x0004000400000000),
            (0x0008000808000000, 0x0008000800000000),
            (0x0010001010000000, 0x0010001000000000),
            (0x0020002020000000, 0x0020002000000000),
            (0x0040004040000000, 0x0040004000000000),
            (0x0080008080000000, 0x0080008000000000),
            (0x0100010100000000, 0x0100010000000000),
            (0x0200020200000000, 0x0200020000000000),
            (0x0400040400000000, 0x0400040000000000),
            (0x0800080800000000, 0x0800080000000000),
            (0x1000101000000000, 0x1000100000000000),
            (0x2000202000000000, 0x2000200000000000),
            (0x4000404000000000, 0x4000400000000000),
            (0x8000808000000000, 0x8000800000000000),

            (0x0000000008000000, 0x0000000008000000),
            (0x0000000010000000, 0x0000000010000000),
            (0x0000000020000000, 0x0000000020000000),
            (0x0000000040000000, 0x0000000040000000),
            (0x0000000080000000, 0x0000000080000000),
            (0x0000000100000000, 0x0000000100000000),
            (0x0000000200000000, 0x0000000200000000),
            (0x0000000400000000, 0x0000000400000000),
            (0x0000000800000000, 0x0000000800000000),
            (0x0000001000000000, 0x0000001000000000),
            (0x0000002000000000, 0x0000002000000000),
            (0x0000004000000000, 0x0000004000000000),
            (0x0000008000000000, 0x0000008000000000),
            (0x0000010000000000, 0x0000010000000000),
            (0x0000020000000000, 0x0000020000000000),
            (0x0000040000000000, 0x0000040000000000),
            (0x0000080000000000, 0x0000080000000000),
            (0x0000100000000000, 0x0000100000000000),
            (0x0000200000000000, 0x0000200000000000),
            (0x0000400000000000, 0x0000400000000000),
            (0x0000800000000000, 0x0000800000000000),
            (0x0001000000000000, 0x0001000000000000),
            (0x0002000000000000, 0x0002000000000000),
            (0x0004000000000000, 0x0004000000000000),
            (0x0008000000000000, 0x0008000000000000),
            (0x0010000000000000, 0x0010000000000000),
            (0x0020000000000000, 0x0020000000000000),
            (0x0040000000000000, 0x0040000000000000),
            (0x0080000000000000, 0x0080000000000000),
            (0x0100000000000000, 0x0100000000000000),
            (0x0200000000000000, 0x0200000000000000),
            (0x0400000000000000, 0x0400000000000000),
            (0x0800000000000000, 0x0800000000000000),
            (0x1000000000000000, 0x1000000000000000),
            (0x2000000000000000, 0x2000000000000000),
            (0x4000000000000000, 0x4000000000000000),
            (0x8000000000000000, 0x8000000000000000),
            (0x0000000000000001, 0x0000000000000001),
            (0x0000000000000002, 0x0000000000000002),
            (0x0000000000000004, 0x0000000000000004),
            (0x0000000000000008, 0x0000000000000008),
            (0x0000000000000010, 0x0000000000000010),
            (0x0000000000000020, 0x0000000000000020),
            (0x0000000000000040, 0x0000000000000040),
            (0x0000000000000080, 0x0000000000000080),
            (0x0000000000000100, 0x0000000000000100),
            (0x0000000000000200, 0x0000000000000200),
            (0x0000000000000400, 0x0000000000000400),
            (0x0000000000000800, 0x0000000000000800),
            (0x0000000000001000, 0x0000000000001000),
            (0x0000000000002000, 0x0000000000002000),
            (0x0000000000004000, 0x0000000000004000),
            (0x0000000000008000, 0x0000000000008000),
            (0x0000000000010000, 0x0000000000010000),
            (0x0000000000020000, 0x0000000000020000),
            (0x0000000000040000, 0x0000000000040000),
            (0x0000000000080000, 0x0000000000080000),
            (0x0000000000100000, 0x0000000000100000),
            (0x0000000000200000, 0x0000000000200000),
            (0x0000000000400000, 0x0000000000400000),
            (0x0000000000800000, 0x0000000000800000),
            (0x0000000001000000, 0x0000000001000000),
            (0x0000000002000000, 0x0000000002000000),
            (0x0000000004000000, 0x0000000004000000),
        };
        private readonly static (ulong s0, ulong s1)[] xoroshiroBackMatrix = new (ulong s0, ulong s1)[128]
        {
            (0x0000000001000000, 0x2000200000000000),
            (0x0000000002000000, 0x4000400000000000),
            (0x0000000004000000, 0x8000800000000000),
            (0x0000000008000000, 0x0001000000000001),
            (0x0000000010000000, 0x0002000000000002),
            (0x0000000020000000, 0x0004000000000004),
            (0x0000000040000000, 0x0008000000000008),
            (0x0000000080000000, 0x0010000000000010),
            (0x0000000100000000, 0x0020000000000020),
            (0x0000000200000000, 0x0040000000000040),
            (0x0000000400000000, 0x0080000000000080),
            (0x0000000800000000, 0x0100000000000100),
            (0x0000001000000000, 0x0200000000000200),
            (0x0000002000000000, 0x0400000000000400),
            (0x0000004000000000, 0x0800000000000800),
            (0x0000008000000000, 0x1000000000001000),
            (0x0000010000000000, 0x2000000000002000),
            (0x0000020000000000, 0x4000000000004000),
            (0x0000040000000000, 0x8000000000008000),
            (0x0000080000000000, 0x0000000000010001),
            (0x0000100000000000, 0x0000000000020002),
            (0x0000200000000000, 0x0000000000040004),
            (0x0000400000000000, 0x0000000000080008),
            (0x0000800000000000, 0x0000000000100010),
            (0x0001000000000000, 0x0000000000200020),
            (0x0002000000000000, 0x0000000000400040),
            (0x0004000000000000, 0x0000000000800080),
            (0x0008000000000000, 0x0000000001000100),
            (0x0010000000000000, 0x0000000002000200),
            (0x0020000000000000, 0x0000000004000400),
            (0x0040000000000000, 0x0000000008000800),
            (0x0080000000000000, 0x0000000010001000),
            (0x0100000000000000, 0x0000000020002000),
            (0x0200000000000000, 0x0000000040004000),
            (0x0400000000000000, 0x0000000080008000),
            (0x0800000000000000, 0x0000000100010000),
            (0x1000000000000000, 0x0000000200020000),
            (0x2000000000000000, 0x0000000400040000),
            (0x4000000000000000, 0x0000000800080000),
            (0x8000000000000000, 0x0000001000100000),

            (0x0000000000000001, 0x0000002000000000),
            (0x0000000000000002, 0x0000004000000000),
            (0x0000000000000004, 0x0000008000000000),
            (0x0000000000000008, 0x0000010000000000),
            (0x0000000000000010, 0x0000020000000000),
            (0x0000000000000020, 0x0000040000000000),
            (0x0000000000000040, 0x0000080000000000),
            (0x0000000000000080, 0x0000100000000000),
            (0x0000000000000100, 0x0000200000000000),
            (0x0000000000000200, 0x0000400000000000),
            (0x0000000000000400, 0x0000800000000000),
            (0x0000000000000800, 0x0001000000000000),
            (0x0000000000001000, 0x0002000000000000),
            (0x0000000000002000, 0x0004000000000000),
            (0x0000000000004000, 0x0008000000000000),
            (0x0000000000008000, 0x0010000000000000),
            (0x0000000000010000, 0x0020002000000000),
            (0x0000000000020000, 0x0040004000000000),
            (0x0000000000040000, 0x0080008000000000),
            (0x0000000000080000, 0x0100010000000000),
            (0x0000000000100000, 0x0200020000000000),
            (0x0000000000200000, 0x0400040000000000),
            (0x0000000000400000, 0x0800080000000000),
            (0x0000000000800000, 0x1000100000000000),
            (0x0000000001000000, 0x2000202000000000),
            (0x0000000002000000, 0x4000404000000000),
            (0x0000000004000000, 0x8000808000000000),
            (0x0000000008000000, 0x0001010000000001),
            (0x0000000010000000, 0x0002020000000002),
            (0x0000000020000000, 0x0004040000000004),
            (0x0000000040000000, 0x0008080000000008),
            (0x0000000080000000, 0x0010100000000010),
            (0x0000000100000000, 0x0020200000000020),
            (0x0000000200000000, 0x0040400000000040),
            (0x0000000400000000, 0x0080800000000080),
            (0x0000000800000000, 0x0101000000000100),
            (0x0000001000000000, 0x0202000000000200),
            (0x0000002000000000, 0x0404000000000400),
            (0x0000004000000000, 0x0808000000000800),
            (0x0000008000000000, 0x1010000000001000),
            (0x0000010000000000, 0x2020000000002000),
            (0x0000020000000000, 0x4040000000004000),
            (0x0000040000000000, 0x8080000000008000),
            (0x0000080000000000, 0x0100000000010001),
            (0x0000100000000000, 0x0200000000020002),
            (0x0000200000000000, 0x0400000000040004),
            (0x0000400000000000, 0x0800000000080008),
            (0x0000800000000000, 0x1000000000100010),
            (0x0001000000000000, 0x2000000000200020),
            (0x0002000000000000, 0x4000000000400040),
            (0x0004000000000000, 0x8000000000800080),
            (0x0008000000000000, 0x0000000001000101),
            (0x0010000000000000, 0x0000000002000202),
            (0x0020000000000000, 0x0000000004000404),
            (0x0040000000000000, 0x0000000008000808),
            (0x0080000000000000, 0x0000000010001010),
            (0x0100000000000000, 0x0000000020002020),
            (0x0200000000000000, 0x0000000040004040),
            (0x0400000000000000, 0x0000000080008080),
            (0x0800000000000000, 0x0000000100010100),
            (0x1000000000000000, 0x0000000200020200),
            (0x2000000000000000, 0x0000000400040400),
            (0x4000000000000000, 0x0000000800080800),
            (0x8000000000000000, 0x0000001000101000),
            (0x0000000000000001, 0x0000002000002000),
            (0x0000000000000002, 0x0000004000004000),
            (0x0000000000000004, 0x0000008000008000),
            (0x0000000000000008, 0x0000010000010000),
            (0x0000000000000010, 0x0000020000020000),
            (0x0000000000000020, 0x0000040000040000),
            (0x0000000000000040, 0x0000080000080000),
            (0x0000000000000080, 0x0000100000100000),
            (0x0000000000000100, 0x0000200000200000),
            (0x0000000000000200, 0x0000400000400000),
            (0x0000000000000400, 0x0000800000800000),
            (0x0000000000000800, 0x0001000001000000),
            (0x0000000000001000, 0x0002000002000000),
            (0x0000000000002000, 0x0004000004000000),
            (0x0000000000004000, 0x0008000008000000),
            (0x0000000000008000, 0x0010000010000000),
            (0x0000000000010000, 0x0020002020000000),
            (0x0000000000020000, 0x0040004040000000),
            (0x0000000000040000, 0x0080008080000000),
            (0x0000000000080000, 0x0100010100000000),
            (0x0000000000100000, 0x0200020200000000),
            (0x0000000000200000, 0x0400040400000000),
            (0x0000000000400000, 0x0800080800000000),
            (0x0000000000800000, 0x1000101000000000),
        };
        private readonly static (ulong s0, ulong s1)[][] jumpMatrixes, backJumpMatrixes;

        static Xoroshiro128pJumpExt()
        {
            jumpMatrixes = new (ulong, ulong)[64][];
            jumpMatrixes[0] = xoroshiroMatrix;
            for (int i = 1; i < 64; i++) 
                jumpMatrixes[i] = jumpMatrixes[i - 1].Products(jumpMatrixes[i - 1]);

            backJumpMatrixes = new (ulong, ulong)[64][];
            backJumpMatrixes[0] = xoroshiroBackMatrix;
            for (int i = 1; i < 64; i++)
                backJumpMatrixes[i] = backJumpMatrixes[i - 1].Products(backJumpMatrixes[i - 1]);
        }

        public static (ulong s0, ulong s1) Next(this (ulong s0, ulong s1) state, uint n)
        {
            for (int i = 0; i < 32; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(jumpMatrixes[i]);

            return state;
        }
        public static (ulong s0, ulong s1) Next(this (ulong s0, ulong s1) state, ulong n)
        {
            for (int i = 0; i < 64; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(jumpMatrixes[i]);

            return state;
        }
        public static (ulong s0, ulong s1) Advance(ref this (ulong s0, ulong s1) state, uint n)
        {
            for (int i = 0; i < 32; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(jumpMatrixes[i]);

            return state;
        }
        public static (ulong s0, ulong s1) Advance(ref this (ulong s0, ulong s1) state, uint n, ref uint index)
        {
            for (int i = 0; i < 32; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(jumpMatrixes[i]);

            index += n;
            return state;
        }
        public static (ulong s0, ulong s1) Advance(ref this (ulong s0, ulong s1) state, ulong n)
        {
            for (int i = 0; i < 64; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(jumpMatrixes[i]);

            return state;
        }

        public static (ulong s0, ulong s1) Prev(this (ulong s0, ulong s1) state, uint n)
        {
            for (int i = 0; i < 32; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(backJumpMatrixes[i]);

            return state;
        }
        public static (ulong s0, ulong s1) Prev(this (ulong s0, ulong s1) state, ulong n)
        {
            for (int i = 0; i < 64; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(backJumpMatrixes[i]);

            return state;
        }
        public static (ulong s0, ulong s1) Back(ref this (ulong s0, ulong s1) state, uint n)
        {
            for (int i = 0; i < 32; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(backJumpMatrixes[i]);

            return state;
        }
        public static (ulong s0, ulong s1) Back(ref this (ulong s0, ulong s1) state, uint n, ref uint index)
        {
            for (int i = 0; i < 32; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(backJumpMatrixes[i]);

            index += n;
            return state;
        }
        public static (ulong s0, ulong s1) Back(ref this (ulong s0, ulong s1) state, ulong n)
        {
            for (int i = 0; i < 64; i++)
                if ((n & (1UL << i)) != 0) state = state.Products(backJumpMatrixes[i]);

            return state;
        }

        // ベクトルに行列を作用させる。
        private static (ulong s0, ulong s1) Products(this (ulong s0, ulong s1) state, (ulong s0, ulong s1)[] matrix)
        {
            var r_s0 = 0UL;
            var r_s1 = 0UL;

            for (int i = 0; i < 64; i++)
            {
                {
                    var (s0, s1) = matrix[i];
                    var conv = (ulong)((s0 & state.s0).PopCount() ^ (s1 & state.s1).PopCount());
                    r_s0 |= (conv << i);
                }
                {
                    var (s0, s1) = matrix[i + 64];
                    var conv = (ulong)((s0 & state.s0).PopCount() ^ (s1 & state.s1).PopCount());
                    r_s1 |= (conv << i);
                }
            }

            return (r_s0, r_s1);
        }

        // 行列に行列を作用させる。
        private static (ulong s0, ulong s1)[] Products(this (ulong s0, ulong s1)[] matrix1, (ulong s0, ulong s1)[] matrix2)
        {
            var mat = new (ulong s0, ulong s1)[128];
            matrix2 = matrix2.Transpose();

            for (int i = 0; i < 128; i++)
            {
                for (int k = 0; k < 64; k++)
                {
                    var bit1 = (matrix1[i].s0 & matrix2[k].s0).PopCount() ^ (matrix1[i].s1 & matrix2[k].s1).PopCount();
                    var bit2 = (matrix1[i].s0 & matrix2[k + 64].s0).PopCount() ^ (matrix1[i].s1 & matrix2[k + 64].s1).PopCount();
                    mat[i].s0 |= (ulong)bit1 << k;
                    mat[i].s1 |= (ulong)bit2 << k;
                }
            }

            return mat;
        }

        private static (ulong s0, ulong s1)[] Transpose(this (ulong s0, ulong s1)[] matrix)
        {
            var mat = new (ulong s0, ulong s1)[128];
            for (int i = 0; i < 64; i++)
            {
                var (s0, s1) = matrix[i];
                for (int k = 0; k < 64; k++)
                {
                    mat[k].s0 |= ((s0 & (1UL << k)) >> k) << i;
                    mat[k + 64].s0 |= ((s1 & (1UL << k)) >> k) << i;
                }
            }
            for (int i = 0; i < 64; i++)
            {
                var (s0, s1) = matrix[i + 64];
                for (int k = 0; k < 64; k++)
                {
                    mat[k].s1 |= ((s0 & (1UL << k)) >> k) << i;
                    mat[k + 64].s1 |= ((s1 & (1UL << k)) >> k) << i;
                }
            }

            return mat;
        }


        // .NET 5以前はBitOperation.PopCountが提供されていないので。
        private static int PopCount(this ulong x)
        {
            x = (x & 0x5555555555555555UL) + ((x & 0xAAAAAAAAAAAAAAAAUL) >> 1);
            x = (x & 0x3333333333333333UL) + ((x & 0xCCCCCCCCCCCCCCCCUL) >> 2);
            x = (x & 0x0F0F0F0F0F0F0F0FUL) + ((x & 0xF0F0F0F0F0F0F0F0UL) >> 4);

            x += x >> 8;
            x += x >> 16;
            x += x >> 32;
            return (int)(x & 1);
        }
    }

    public interface IGeneratable<out TResult>
    {
        TResult Generate((ulong s0, ulong s1) seed);
    }
    public interface IGeneratable<out TResult, in TArg1>
    {
        TResult Generate((ulong s0, ulong s1) seed, TArg1 arg1);
    }

    public interface ISideEffectiveGeneratable<out TResult>
    {
        TResult Generate(ref (ulong s0, ulong s1) seed);
    }
    public interface ISideEffectiveGeneratable<out TResult, in TArg1>
    {
        TResult Generate(ref (ulong s0, ulong s1) seed, TArg1 arg1);
    }

    public static class XoroshiroExt
    {
        public static TResult Generate<TResult>(this (ulong s0, ulong s1) seed, IGeneratable<TResult> generatable)
            => generatable.Generate(seed);
        public static TResult Generate<TResult, TArg>(this (ulong s0, ulong s1) seed, IGeneratable<TResult, TArg> generatable, TArg arg)
            => generatable.Generate(seed, arg);
        public static TResult Generate<TResult>(ref this (ulong s0, ulong s1) seed, ISideEffectiveGeneratable<TResult> generatable)
            => generatable.Generate(ref seed);
        public static TResult Generate<TResult, TArg>(ref this (ulong s0, ulong s1) seed, ISideEffectiveGeneratable<TResult, TArg> generatable, TArg arg)
            => generatable.Generate(ref seed, arg);

        public static IEnumerable<(int index, T element)> WithIndex<T>(this IEnumerable<T> enumerator)
            => enumerator.Select((_, i) => (i, _));

        public static IEnumerable<(ulong s0, ulong s1)> Enumerate(this (ulong s0, ulong s1) seed)
        {
            yield return seed;
            while (true)
                yield return seed.Advance();
        }
        public static IEnumerable<TResult> Enumerate<TResult>
            (this IEnumerable<(ulong s0, ulong s1)> seedEnumerator, IGeneratable<TResult> igenerator)
            => seedEnumerator.Select(igenerator.Generate);

        public static IEnumerable<TResult> Enumerate<TResult, TArg1>
            (this IEnumerable<(ulong s0, ulong s1)> seedEnumerator, IGeneratable<TResult, TArg1> igenerator, TArg1 arg1)
            => seedEnumerator.Select(_ => igenerator.Generate(_, arg1));
    }
}

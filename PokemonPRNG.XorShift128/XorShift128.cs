using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonPRNG.XorShift128
{
    public static class XorShift128Ext
    {
        public static uint GetRand(ref this (uint s0, uint s1, uint s2, uint s3) state)
        {
            var t1 = state.s0 ^ (state.s0 << 11);
            var t2 = state.s3 ^ (state.s3 >> 19) ^ t1 ^ (t1 >> 8);

            state = (state.s1, state.s2, state.s3, t2);

            return (t2 % 0xFFFFFFFF) + 0x80000000;
        }
        public static uint GetRand(ref this (uint s0, uint s1, uint s2, uint s3) state, uint n)
        {
            var t1 = state.s0 ^ (state.s0 << 11);
            var t2 = state.s3 ^ (state.s3 >> 19) ^ t1 ^ (t1 >> 8);

            state = (state.s1, state.s2, state.s3, t2);

            return ((t2 % 0xFFFFFFFF) + 0x80000000) % n;
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
        public static (uint s0, uint s1, uint s2, uint s3) Advance(ref this (uint s0, uint s1, uint s2, uint s3) state)
        {
            var t1 = state.s0 ^ (state.s0 << 11);
            var t2 = state.s3 ^ (state.s3 >> 19) ^ t1 ^ (t1 >> 8);

            return state = (state.s1, state.s2, state.s3, t2);
        }

        public static (uint s0, uint s1, uint s2, uint s3) Prev(this (uint s0, uint s1, uint s2, uint s3) state)
        {
            var t = state.s3 ^ state.s2 ^ (state.s2 >> 19);
            t = t ^ (t >> 8) ^ (t >> 16) ^ (t >> 24);
            t = t ^ (t << 11) ^ (t << 22);

            return (t, state.s0, state.s1, state.s2);
        }
        public static (uint s0, uint s1, uint s2, uint s3) Back(ref this (uint s0, uint s1, uint s2, uint s3) state)
        {
            var t = state.s3 ^ state.s2 ^ (state.s2 >> 19);
            t = t ^ (t >> 8) ^ (t >> 16) ^ (t >> 24);
            t = t ^ (t << 11) ^ (t << 22);

            return state = (t, state.s0, state.s1, state.s2);
        }

        public static string ToU128String(this (uint s0, uint s1, uint s2, uint s3) state) => $"{state.s0:X8}{state.s1:X8}{state.s2:X8}{state.s3:X8}";
        public static (uint s0, uint s1, uint s2, uint s3) FromU128String(this string hex)
        {
            if (hex.Length > 32 || hex.Length == 0) throw new ArgumentException("bad argument");

            hex = hex.PadLeft(32, '0');

            var t0 = hex.Substring(0, 8);
            var t1 = hex.Substring(8, 8);
            var t2 = hex.Substring(16, 8);
            var t3 = hex.Substring(24, 8);

            var s0 = Convert.ToUInt32(t0, 16);
            var s1 = Convert.ToUInt32(t1, 16);
            var s2 = Convert.ToUInt32(t2, 16);
            var s3 = Convert.ToUInt32(t3, 16);

            return (s0, s1, s2, s3);
        }
    }

    public static class XorShift128JumpExt
    {
        private static readonly (uint S0, uint S1, uint S2, uint S3)[] xorshiftMatrix = new(uint, uint, uint, uint)[128]
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
            (0x00000000u, 0x00000000u, 0x00000002u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000004u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000008u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000010u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000020u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000040u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000080u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000100u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000200u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000400u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000800u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00001000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00002000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00004000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00008000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00010000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00020000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00040000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00080000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00100000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00200000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00400000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00800000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x01000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x02000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x04000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x08000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x10000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x20000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x40000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x80000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00000001u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00000002u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00000004u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00000008u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00000010u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00000020u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00000040u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00000080u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00000100u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00000200u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00000400u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00000800u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00001000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00002000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00004000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00008000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00010000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00020000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00040000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00080000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00100000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00200000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00400000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x00800000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x01000000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x02000000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x04000000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x08000000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x10000000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x20000000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x40000000u),
            (0x00000000u, 0x00000000u, 0x00000000u, 0x80000000u),
            (0x00000101u, 0x00000000u, 0x00000000u, 0x00080001u),
            (0x00000202u, 0x00000000u, 0x00000000u, 0x00100002u),
            (0x00000404u, 0x00000000u, 0x00000000u, 0x00200004u),
            (0x00000809u, 0x00000000u, 0x00000000u, 0x00400008u),
            (0x00001012u, 0x00000000u, 0x00000000u, 0x00800010u),
            (0x00002024u, 0x00000000u, 0x00000000u, 0x01000020u),
            (0x00004048u, 0x00000000u, 0x00000000u, 0x02000040u),
            (0x00008090u, 0x00000000u, 0x00000000u, 0x04000080u),
            (0x00010120u, 0x00000000u, 0x00000000u, 0x08000100u),
            (0x00020240u, 0x00000000u, 0x00000000u, 0x10000200u),
            (0x00040480u, 0x00000000u, 0x00000000u, 0x20000400u),
            (0x00080901u, 0x00000000u, 0x00000000u, 0x40000800u),
            (0x00101202u, 0x00000000u, 0x00000000u, 0x80001000u),
            (0x00202404u, 0x00000000u, 0x00000000u, 0x00002000u),
            (0x00404808u, 0x00000000u, 0x00000000u, 0x00004000u),
            (0x00809010u, 0x00000000u, 0x00000000u, 0x00008000u),
            (0x01012020u, 0x00000000u, 0x00000000u, 0x00010000u),
            (0x02024040u, 0x00000000u, 0x00000000u, 0x00020000u),
            (0x04048080u, 0x00000000u, 0x00000000u, 0x00040000u),
            (0x08090100u, 0x00000000u, 0x00000000u, 0x00080000u),
            (0x10120200u, 0x00000000u, 0x00000000u, 0x00100000u),
            (0x20240400u, 0x00000000u, 0x00000000u, 0x00200000u),
            (0x40480800u, 0x00000000u, 0x00000000u, 0x00400000u),
            (0x80901000u, 0x00000000u, 0x00000000u, 0x00800000u),
            (0x01002000u, 0x00000000u, 0x00000000u, 0x01000000u),
            (0x02004000u, 0x00000000u, 0x00000000u, 0x02000000u),
            (0x04008000u, 0x00000000u, 0x00000000u, 0x04000000u),
            (0x08010000u, 0x00000000u, 0x00000000u, 0x08000000u),
            (0x10020000u, 0x00000000u, 0x00000000u, 0x10000000u),
            (0x20040000u, 0x00000000u, 0x00000000u, 0x20000000u),
            (0x40080000u, 0x00000000u, 0x00000000u, 0x40000000u),
            (0x80100000u, 0x00000000u, 0x00000000u, 0x80000000u),
        };
        private static readonly (uint S0, uint S1, uint S2, uint S3)[] xorshiftBackMatrix = new (uint, uint, uint, uint)[128]
        {
            (0x00000000u, 0x00000000u, 0x09090101u, 0x01010101u),
            (0x00000000u, 0x00000000u, 0x12120202u, 0x02020202u),
            (0x00000000u, 0x00000000u, 0x24240404u, 0x04040404u),
            (0x00000000u, 0x00000000u, 0x48480808u, 0x08080808u),
            (0x00000000u, 0x00000000u, 0x90901010u, 0x10101010u),
            (0x00000000u, 0x00000000u, 0x21202020u, 0x20202020u),
            (0x00000000u, 0x00000000u, 0x42404040u, 0x40404040u),
            (0x00000000u, 0x00000000u, 0x84808080u, 0x80808080u),
            (0x00000000u, 0x00000000u, 0x09010100u, 0x01010100u),
            (0x00000000u, 0x00000000u, 0x12020200u, 0x02020200u),
            (0x00000000u, 0x00000000u, 0x24040400u, 0x04040400u),
            (0x00000000u, 0x00000000u, 0x41010901u, 0x09090901u),
            (0x00000000u, 0x00000000u, 0x82021202u, 0x12121202u),
            (0x00000000u, 0x00000000u, 0x04042404u, 0x24242404u),
            (0x00000000u, 0x00000000u, 0x08084808u, 0x48484808u),
            (0x00000000u, 0x00000000u, 0x10109010u, 0x90909010u),
            (0x00000000u, 0x00000000u, 0x20212020u, 0x21212020u),
            (0x00000000u, 0x00000000u, 0x40424040u, 0x42424040u),
            (0x00000000u, 0x00000000u, 0x80848080u, 0x84848080u),
            (0x00000000u, 0x00000000u, 0x01090100u, 0x09090100u),
            (0x00000000u, 0x00000000u, 0x02120200u, 0x12120200u),
            (0x00000000u, 0x00000000u, 0x04240400u, 0x24240400u),
            (0x00000000u, 0x00000000u, 0x01410901u, 0x49490901u),
            (0x00000000u, 0x00000000u, 0x02821202u, 0x92921202u),
            (0x00000000u, 0x00000000u, 0x05042404u, 0x25242404u),
            (0x00000000u, 0x00000000u, 0x0A084808u, 0x4A484808u),
            (0x00000000u, 0x00000000u, 0x14109010u, 0x94909010u),
            (0x00000000u, 0x00000000u, 0x28212020u, 0x29212020u),
            (0x00000000u, 0x00000000u, 0x50424040u, 0x52424040u),
            (0x00000000u, 0x00000000u, 0xA0848080u, 0xA4848080u),
            (0x00000000u, 0x00000000u, 0x41090100u, 0x49090100u),
            (0x00000000u, 0x00000000u, 0x82120200u, 0x92120200u),
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
            (0x00000000u, 0x00000000u, 0x00000002u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000004u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000008u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000010u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000020u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000040u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000080u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000100u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000200u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000400u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00000800u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00001000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00002000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00004000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00008000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00010000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00020000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00040000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00080000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00100000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00200000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00400000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x00800000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x01000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x02000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x04000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x08000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x10000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x20000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x40000000u, 0x00000000u),
            (0x00000000u, 0x00000000u, 0x80000000u, 0x00000000u),
        };
        private readonly static (uint S0, uint S1, uint S2, uint S3)[][] jumpMatrixes, backJumpMatrixes;

        static XorShift128JumpExt()
        {
            jumpMatrixes = new (uint, uint, uint, uint)[64][];
            jumpMatrixes[0] = xorshiftMatrix;
            for (int i = 1; i < 64; i++)
                jumpMatrixes[i] = jumpMatrixes[i - 1].Products(jumpMatrixes[i - 1]);

            backJumpMatrixes = new (uint S0, uint S1, uint S2, uint S3)[64][];
            backJumpMatrixes[0] = xorshiftBackMatrix;
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

    public interface IGeneratable<out TResult>
    {
        TResult Generate((uint s0, uint s1, uint s2, uint s3) seed);
    }
    public interface IGeneratable<out TResult, in TArg1>
    {
        TResult Generate((uint s0, uint s1, uint s2, uint s3) seed, TArg1 arg1);
    }

    public static class CommonEnumerator
    {
        public static IEnumerable<(int index, T element)> WithIndex<T>(this IEnumerable<T> enumerator)
            => enumerator.Select((_, i) => (i, _));

        public static IEnumerable<(uint s0, uint s1, uint s2, uint s3)> EnumerateSeed(this (uint s0, uint s1, uint s2, uint s3) seed)
        {
            yield return seed;
            while (true) yield return seed.Advance();
        }

        public static IEnumerable<TResult> EnumerateGeneration<TResult>
            (this IEnumerable<(uint s0, uint s1, uint s2, uint s3)> seedEnumerator, IGeneratable<TResult> igenerator)
            => seedEnumerator.Select(igenerator.Generate);

        public static IEnumerable<TResult> EnumerateGeneration<TResult, TArg1>
            (this IEnumerable<(uint s0, uint s1, uint s2, uint s3)> seedEnumerator, IGeneratable<TResult, TArg1> igenerator, TArg1 arg1)
            => seedEnumerator.Select(_ => igenerator.Generate(_, arg1));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonPRNG.XorShift128
{
    public static class XorShift128Ext
    {
        public static uint GetRand(ref this (uint S0, uint S1, uint S2, uint S3) state)
        {
            var t1 = state.S0 ^ (state.S0 << 11);
            var t2 = state.S3 ^ (state.S3 >> 19) ^ t1 ^ (t1 >> 8);

            state = (state.S1, state.S2, state.S3, t2);

            return (t2 % 0xFFFFFFFF) + 0x80000000;
        }
        public static uint GetRand(ref this (uint S0, uint S1, uint S2, uint S3) state, uint n)
        {
            var t1 = state.S0 ^ (state.S0 << 11);
            var t2 = state.S3 ^ (state.S3 >> 19) ^ t1 ^ (t1 >> 8);

            state = (state.S1, state.S2, state.S3, t2);

            return ((t2 % 0xFFFFFFFF) + 0x80000000) % n;
        }

        public static float GetRand_f(ref this (uint S0, uint S1, uint S2, uint S3) state)
        {
            var t1 = state.S0 ^ (state.S0 << 11);
            var t2 = state.S3 ^ (state.S3 >> 19) ^ t1 ^ (t1 >> 8);

            state = (state.S1, state.S2, state.S3, t2);

            return (t2 & 0x7F_FFFF) / 8388607.0f;
        }
        public static float GetRand_f(ref this (uint S0, uint S1, uint S2, uint S3) state, float min, float max)
        {
            var t1 = state.S0 ^ (state.S0 << 11);
            var t2 = state.S3 ^ (state.S3 >> 19) ^ t1 ^ (t1 >> 8);

            state = (state.S1, state.S2, state.S3, t2);

            var r = (t2 & 0x7F_FFFF) / 8388607.0f;

            return r * min + (1 - r) * max;
        }

        public static (uint S0, uint S1, uint S2, uint S3) Next(this (uint S0, uint S1, uint S2, uint S3) state)
        {
            var t1 = state.S0 ^ (state.S0 << 11);
            var t2 = state.S3 ^ (state.S3 >> 19) ^ t1 ^ (t1 >> 8);

            return (state.S1, state.S2, state.S3, t2);
        }
        public static (uint S0, uint S1, uint S2, uint S3) Advance(ref this (uint S0, uint S1, uint S2, uint S3) state)
        {
            var t1 = state.S0 ^ (state.S0 << 11);
            var t2 = state.S3 ^ (state.S3 >> 19) ^ t1 ^ (t1 >> 8);

            return state = (state.S1, state.S2, state.S3, t2);
        }

        public static (uint S0, uint S1, uint S2, uint S3) Prev(this (uint S0, uint S1, uint S2, uint S3) state)
        {
            var t = state.S3 ^ state.S2 ^ (state.S2 >> 19);
            t = t ^ (t >> 8) ^ (t >> 16) ^ (t >> 24);
            t = t ^ (t << 11) ^ (t << 22);

            return (t, state.S0, state.S1, state.S2);
        }
        public static (uint S0, uint S1, uint S2, uint S3) Back(ref this (uint S0, uint S1, uint S2, uint S3) state)
        {
            var t = state.S3 ^ state.S2 ^ (state.S2 >> 19);
            t = t ^ (t >> 8) ^ (t >> 16) ^ (t >> 24);
            t = t ^ (t << 11) ^ (t << 22);

            return state = (t, state.S0, state.S1, state.S2);
        }

        public static string ToU128String(this (uint S0, uint S1, uint S2, uint S3) state) => $"{state.S0:X8}{state.S1:X8}{state.S2:X8}{state.S3:X8}";
        public static (uint S0, uint S1, uint S2, uint S3) FromU128String(this string hex)
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
        private readonly static (uint S0, uint S1, uint S2, uint S3)[] _backJumpCoefficient = new (uint, uint, uint, uint)[56]
        {
            (0x69607894, 0xE249927D, 0x12FFD385, 0x1521F6BC),
            (0x9D55FA4C, 0x73C06EB1, 0xD2A9C1AE, 0x6D019307),
            (0xAA34A991, 0x4ECA4F16, 0xBDCD2D91, 0x770230E1),
            (0x7AB3358E, 0x686069C4, 0x70643BCA, 0xB926E940),
            (0x16553531, 0x00C0EF45, 0x8A322A25, 0x94A38798),
            (0x57EE12B6, 0x9301C6FE, 0x66E98829, 0xC38F4280),
            (0x928572AA, 0x71848785, 0x136EBE05, 0x7B16E0FB),
            (0x9F5EDDA9, 0x1E073E5E, 0x261BCBC6, 0x9F1037A3),
            (0xC3C877BB, 0xC302384D, 0xE999BBE8, 0x877AF079),
            (0x9C95A484, 0x331AD4DE, 0xEC4F2F77, 0x59D8CFBD),
            (0x75D5A042, 0xEE0CC026, 0x46F197E0, 0x3DA2EB2C),
            (0xB7456303, 0x36246567, 0xB24E0548, 0x85D618FA),
            (0xE451C756, 0x0E5A2B47, 0x3057BA16, 0xB12BADE8),
            (0xCB85F11C, 0x677F7C2F, 0x6AB2362F, 0xFB22A95A),
            (0x99037CDC, 0xD1AE76BF, 0xEE98D574, 0x3820D154),
            (0x97BFF2DE, 0x9C57F8FB, 0xE5911FF3, 0xA42EC31E),
            (0xB27C41A3, 0xB765CBF9, 0xB3A486B5, 0x4E63D2A2),
            (0xE2A886A1, 0xDD9B02B4, 0xD44A9F36, 0x8FFDE03A),
            (0x035806C3, 0x862A47B9, 0xB8E79821, 0x07F669C7),
            (0xC20EECF4, 0x802B07DD, 0x469081E0, 0xC88C7F15),
            (0x56A2E2B2, 0xA99F0007, 0xE261A200, 0x79112327),
            (0x76B779D0, 0x4D2D3672, 0xB95E0549, 0x470F8293),
            (0xA5BED579, 0x6A984B22, 0xE4D14EEE, 0xC66BDE38),
            (0x5A56E902, 0xC20A2B05, 0x8BF93126, 0xDC727493),
            (0x7EB1D4E4, 0x2B78EECC, 0x3A837DEF, 0xC1C17DA3),
            (0x27F5356D, 0xF722F908, 0xE542B700, 0xE1A59471),
            (0xC667095A, 0xB0F0F4C0, 0x76516DAA, 0x0A9C29E1),
            (0x1F376D50, 0x678C372D, 0x97C6A654, 0x69BDD8FD),
            (0xF6BF192D, 0x358B8C58, 0x655EB49A, 0xA1DDDDEB),
            (0xE819FEDF, 0x8DD27D6A, 0xA587AF13, 0xDD1D60DA),
            (0x00846353, 0x6E99D258, 0x3C9701E1, 0x8A414940),
            (0xF9C31F8F, 0x0A205D4F, 0xD6C62F73, 0x444948E5),
            (0x1581263C, 0xE7B657A1, 0x68A36767, 0x98224558),
            (0x1A18EE4D, 0x9CDDEFA5, 0xF9986761, 0xB7F51254),
            (0xB308C6FD, 0x41935FF6, 0x139461B1, 0x17213A22),
            (0x283D0B50, 0xA68D509D, 0x99190899, 0x9D0DF3A4),
            (0xD15CE435, 0xC90417A1, 0x596F5D1F, 0xA2769E2C),
            (0xD81C08EE, 0x4DE13B76, 0x73B5FBEB, 0x0CD39B29),
            (0x04AE1A7D, 0x5F2F3960, 0x1EACC4C0, 0x8C401C1A),
            (0x4DF633A9, 0x2F3C6B9C, 0x1A16603B, 0x2DA1D1F5),
            (0xDA256387, 0xB1316566, 0x583327EE, 0x7B9F3AA1),
            (0x567C1517, 0xA161A3B1, 0x8B434052, 0x0849221B),
            (0xB830DBC2, 0x883166E1, 0x864E6FC5, 0x7221FE8F),
            (0x938979CA, 0x4A9063D6, 0x16555B55, 0x7B8364CC),
            (0x01CB8747, 0xC820CAAB, 0x88E3C645, 0x2556EDA1),
            (0x66617D56, 0xF7AA082B, 0xD12FC0C5, 0x1C1D6A74),
            (0xD11D59F6, 0xB3711249, 0x1F0CEC70, 0x4550AF70),
            (0x23345725, 0x24C8B952, 0x02458BB4, 0x7BD5F9F4),
            (0xE8D1B843, 0x2829F736, 0xA650CDEC, 0xE07F34D7),
            (0x820C6A7D, 0xCC4DBF12, 0x0CCC5229, 0xD8C5C02F),
            (0x3D4D46F9, 0x44532B42, 0x06E9EF72, 0xCFC77FFB),
            (0xF721E29C, 0x7128531F, 0xFDD2BFA7, 0x41697BDE),
            (0x1186F8DF, 0x7CC2BC5D, 0xAF5A5BDF, 0x91D768A3),
            (0xCF7E9804, 0x307D0698, 0xCBE781B5, 0xFCDDCC91),
            (0xA634AE9D, 0x6AA131A6, 0xDA247606, 0xE440BE93),
            (0x45D9E0FC, 0x330CBE6A, 0x6B1B8424, 0xB4E9AC26),
        };
        private readonly static (uint S0, uint S1, uint S2, uint S3)[] _jumpCoefficient = new (uint, uint, uint, uint)[56]
        {
            (0xB1E10DA6, 0xA42CA9AE, 0xFA6B67E9, 0x956C89FB),
            (0x88F8A56E, 0x1A0988E9, 0x47EC17C7, 0xFF7AA97C),
            (0x443B16F0, 0xFB6668FF, 0x9BD01948, 0x9DFF3367),
            (0x1DC83CE2, 0x46A4759B, 0xE3B212DA, 0xBD36A1D3),
            (0xA0CBAA6C, 0x9640BC4C, 0x8B0E3C0B, 0x6D2F354B),
            (0x2B4D52FB, 0x947096C7, 0xCA4F108F, 0xECF6383D),
            (0x4DDCA12E, 0x0DAF32F0, 0x7177890A, 0xE1054E81),
            (0xB78641A5, 0xB9FA05AA, 0x115107C6, 0x02AE1912),
            (0x95F950E3, 0x382FA5AA, 0xF81649BE, 0x59981D3D),
            (0xFC044FDB, 0xDBA31D29, 0x0F8CEE00, 0x6644B35F),
            (0x3C338C19, 0x3CA16B95, 0x169FD455, 0xECFF213C),
            (0x6A60ECBE, 0x3FFDCB09, 0x0A094939, 0xA9DFD9FB),
            (0xF8C0B5FA, 0xFD6AEF50, 0xB16C479F, 0x079D7462),
            (0x8269B55D, 0x9148889B, 0xD707B6B6, 0x03896736),
            (0xB91EF36A, 0x4C6AC659, 0x99DBBEAA, 0xDEA22E88),
            (0xCDDB0649, 0x67CCF586, 0x5AE7D320, 0xC1150DDD),
            (0x6B2CC0F0, 0x33C8177D, 0xC7E9C381, 0x5F0BE91A),
            (0x104E47B9, 0x4A5F78FC, 0xA212E573, 0x0CD15D2B),
            (0xE8A0B936, 0xD69063E6, 0x147DEC3E, 0xAB586674),
            (0xF22D34F5, 0x7071114A, 0xED372866, 0x4BFD9D67),
            (0x2B5CD38C, 0x68628730, 0xB4EF5C18, 0xDAF387CA),
            (0x547CCA1E, 0xBB7D371F, 0x5790AF3E, 0xFFAF8274),
            (0xC88829F9, 0xEB96ACD6, 0xFE573AFA, 0x7B932849),
            (0x73BF7047, 0xB4FD2C65, 0xE2D6E821, 0x8CEDF8DF),
            (0x3A883107, 0x3532E5F3, 0xD3739051, 0xB067CC93),
            (0x023090E7, 0x9682C4C3, 0x36419BAA, 0xFE1A817D),
            (0x269DB445, 0xAE6D35E0, 0x4555ED5F, 0xAFDCBF8B),
            (0x49A2536B, 0x74A47EC9, 0x540609A1, 0xCD0DC146),
            (0xE9E69F80, 0x1809F899, 0xDC59EBB0, 0xF0BF2D2C),
            (0xEFB4F4DC, 0x15F1121F, 0xABC7D64D, 0x6F82DDB5),
            (0x44CBBBE6, 0xE6CBA911, 0x81574220, 0xDC0B5082),
            (0xD95AEB31, 0xA2C0A5A1, 0xD5BA396E, 0xB04BED1C),
            (0x9186D420, 0xCCB635F8, 0x65FCB394, 0x862F99D7),
            (0x5BFAEF90, 0xA5B59887, 0xDE5AAE15, 0xDBB253DA),
            (0x1F94AA7C, 0x06F04C1D, 0xA527D5F1, 0x14136DE0),
            (0xDC17108B, 0xC06B983C, 0xA2E84C2E, 0xB9ACC1C3),
            (0x6E61C7B7, 0x795D4288, 0x24D0B048, 0x286C7105),
            (0x4F66EF2B, 0x85658FA7, 0x1D047238, 0x96949337),
            (0xD636BEFB, 0x4E89AF13, 0x0D53FBC8, 0x228DC471),
            (0xD50DF99D, 0x9D022830, 0xA295DA66, 0x0CCEB170),
            (0x97E0579A, 0xF88C1CF6, 0x3E0A2DA1, 0xDDC43558),
            (0x76ED1362, 0x6EBD25B3, 0xCB3043DA, 0xB3360EC1),
            (0xEBCF8D60, 0xA6486F63, 0x04E9DD37, 0x665D04AD),
            (0x1533B855, 0x11DA0104, 0x3437FB2A, 0xA9FDD1C7),
            (0x35A3A0F0, 0x8AECE6B8, 0x814A9925, 0x12659E0E),
            (0x2CD1F179, 0x74DD61DB, 0x67109D09, 0xBC2B6ECD),
            (0xBC64A64E, 0xA3382FAD, 0xB9F024C6, 0x5596F764),
            (0x4A3A51EB, 0x36B3D3A3, 0x5C61C40F, 0x2E619BBD),
            (0x547D1FB2, 0xF9220668, 0xF61DDB9A, 0xAD17C26F),
            (0x89F8CF2D, 0xBA453412, 0xD95D4A24, 0x8768B12B),
            (0x413894B1, 0x660B2988, 0x87B5D735, 0xDA050F7D),
            (0x14157C23, 0x5577ABCE, 0xC6FC9C79, 0x98D18BF7),
            (0x9A46F6DC, 0x95EA3AB4, 0xCBFA874B, 0xBA46C07C),
            (0x68DBBB93, 0x449DA582, 0x5D848E85, 0xB3F66A17),
            (0xBC0EE1BE, 0xF973F85E, 0x9069C270, 0x29CB6828),
            (0xE7DEFF39, 0xEA3BCF8E, 0x2349BF61, 0xC5D52213),
        };

        public static (uint S0, uint S1, uint S2, uint S3) Next(this (uint S0, uint S1, uint S2, uint S3) state, uint n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Advance();
            n >>= 8;

            for (var k = 0; k < _jumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1, s2, s3) = (0u, 0u, 0u, 0u);
                var (c0, c1, c2, c3) = _jumpCoefficient[k];
                for (var i = 0; i < 32; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c2 >>= 1)
                {
                    if ((c2 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c3 >>= 1)
                {
                    if ((c3 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }

                state = (s0, s1, s2, s3);
            }

            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Next(this (uint S0, uint S1, uint S2, uint S3) state, ulong n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Advance();
            n >>= 8;

            for (var k = 0; k < _jumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1, s2, s3) = (0u, 0u, 0u, 0u);
                var (c0, c1, c2, c3) = _jumpCoefficient[k];
                for (var i = 0; i < 32; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c2 >>= 1)
                {
                    if ((c2 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c3 >>= 1)
                {
                    if ((c3 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }

                state = (s0, s1, s2, s3);
            }

            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Advance(ref this (uint S0, uint S1, uint S2, uint S3) state, uint n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Advance();
            n >>= 8;

            for (var k = 0; k < _jumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1, s2, s3) = (0u, 0u, 0u, 0u);
                var (c0, c1, c2, c3) = _jumpCoefficient[k];
                for (var i = 0; i < 32; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c2 >>= 1)
                {
                    if ((c2 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c3 >>= 1)
                {
                    if ((c3 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }

                state = (s0, s1, s2, s3);
            }

            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Advance(ref this (uint S0, uint S1, uint S2, uint S3) state, uint n, ref uint index)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Advance();
            n >>= 8;

            for (var k = 0; k < _jumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1, s2, s3) = (0u, 0u, 0u, 0u);
                var (c0, c1, c2, c3) = _jumpCoefficient[k];
                for (var i = 0; i < 32; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c2 >>= 1)
                {
                    if ((c2 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c3 >>= 1)
                {
                    if ((c3 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }

                state = (s0, s1, s2, s3);
            }

            index += n;
            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Advance(ref this (uint S0, uint S1, uint S2, uint S3) state, ulong n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Advance();
            n >>= 8;

            for (var k = 0; k < _jumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1, s2, s3) = (0u, 0u, 0u, 0u);
                var (c0, c1, c2, c3) = _jumpCoefficient[k];
                for (var i = 0; i < 32; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c2 >>= 1)
                {
                    if ((c2 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }
                for (var i = 0; i < 32; i++, c3 >>= 1)
                {
                    if ((c3 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Advance();
                }

                state = (s0, s1, s2, s3);
            }

            return state;
        }

        public static (uint S0, uint S1, uint S2, uint S3) Prev(this (uint S0, uint S1, uint S2, uint S3) state, uint n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Back();
            n >>= 8;

            for (var k = 0; k < _backJumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1, s2, s3) = (0u, 0u, 0u, 0u);
                var (c0, c1, c2, c3) = _backJumpCoefficient[k];
                for (var i = 0; i < 32; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c2 >>= 1)
                {
                    if ((c2 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c3 >>= 1)
                {
                    if ((c3 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }

                state = (s0, s1, s2, s3);
            }

            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Prev(this (uint S0, uint S1, uint S2, uint S3) state, ulong n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Back();
            n >>= 8;

            for (var k = 0; k < _backJumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1, s2, s3) = (0u, 0u, 0u, 0u);
                var (c0, c1, c2, c3) = _backJumpCoefficient[k];
                for (var i = 0; i < 32; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c2 >>= 1)
                {
                    if ((c2 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c3 >>= 1)
                {
                    if ((c3 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }

                state = (s0, s1, s2, s3);
            }

            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Back(ref this (uint S0, uint S1, uint S2, uint S3) state, uint n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Back();
            n >>= 8;

            for (var k = 0; k < _backJumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1, s2, s3) = (0u, 0u, 0u, 0u);
                var (c0, c1, c2, c3) = _backJumpCoefficient[k];
                for (var i = 0; i < 32; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c2 >>= 1)
                {
                    if ((c2 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c3 >>= 1)
                {
                    if ((c3 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }

                state = (s0, s1, s2, s3);
            }

            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Back(ref this (uint S0, uint S1, uint S2, uint S3) state, uint n, ref uint index)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Back();
            n >>= 8;

            for (var k = 0; k < _backJumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1, s2, s3) = (0u, 0u, 0u, 0u);
                var (c0, c1, c2, c3) = _backJumpCoefficient[k];
                for (var i = 0; i < 32; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c2 >>= 1)
                {
                    if ((c2 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c3 >>= 1)
                {
                    if ((c3 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }

                state = (s0, s1, s2, s3);
            }

            index += n;
            return state;
        }
        public static (uint S0, uint S1, uint S2, uint S3) Back(ref this (uint S0, uint S1, uint S2, uint S3) state, ulong n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Back();
            n >>= 8;

            for (var k = 0; k < _backJumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1, s2, s3) = (0u, 0u, 0u, 0u);
                var (c0, c1, c2, c3) = _backJumpCoefficient[k];
                for (var i = 0; i < 32; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c2 >>= 1)
                {
                    if ((c2 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }
                for (var i = 0; i < 32; i++, c3 >>= 1)
                {
                    if ((c3 & 1) != 0)
                        (s0, s1, s2, s3) = (s0 ^ state.S0, s1 ^ state.S1, s2 ^ state.S2, s3 ^ state.S3);

                    state.Back();
                }

                state = (s0, s1, s2, s3);
            }

            return state;
        }

    }

    public interface IGeneratable<out TResult>
    {
        TResult Generate((uint S0, uint S1, uint S2, uint S3) seed);
    }
    public interface IGeneratable<out TResult, in TArg1>
    {
        TResult Generate((uint S0, uint S1, uint S2, uint S3) seed, TArg1 arg1);
    }

    public interface IGeneratableEffectful<out TResult>
    {
        TResult Generate(ref (uint S0, uint S1, uint S2, uint S3) seed);
    }
    public interface IGeneratableEffectful<out TResult, in TArg1>
    {
        TResult Generate(ref (uint S0, uint S1, uint S2, uint S3) seed, TArg1 arg1);
    }

    public static class CommonEnumerator
    {
        public static TResult Generate<TResult>(this (uint S0, uint S1, uint S2, uint S3) seed, IGeneratable<TResult> generatable)
            => generatable.Generate(seed);
        public static TResult Generate<TResult, TArg>(this (uint S0, uint S1, uint S2, uint S3) seed, IGeneratable<TResult, TArg> generatable, TArg arg)
            => generatable.Generate(seed, arg);
        public static TResult Generate<TResult>(ref this (uint S0, uint S1, uint S2, uint S3) seed, IGeneratableEffectful<TResult> generatable)
            => generatable.Generate(ref seed);
        public static TResult Generate<TResult, TArg>(ref this (uint S0, uint S1, uint S2, uint S3) seed, IGeneratableEffectful<TResult, TArg> generatable, TArg arg)
            => generatable.Generate(ref seed, arg);

        public static IEnumerable<(int Index, T Element)> WithIndex<T>(this IEnumerable<T> enumerator)
            => enumerator.Select((_, i) => (i, _));

        public static IEnumerable<(uint S0, uint S1, uint S2, uint S3)> Enumerate(this (uint S0, uint S1, uint S2, uint S3) seed)
        {
            yield return seed;
            while (true) yield return seed.Advance();
        }
        public static IEnumerable<TResult> Enumerate<TResult>
            (this IEnumerable<(uint S0, uint S1, uint S2, uint S3)> seedEnumerator, IGeneratable<TResult> igenerator)
            => seedEnumerator.Select(igenerator.Generate);
        public static IEnumerable<TResult> Enumerate<TResult, TArg1>
            (this IEnumerable<(uint S0, uint S1, uint S2, uint S3)> seedEnumerator, IGeneratable<TResult, TArg1> igenerator, TArg1 arg1)
            => seedEnumerator.Select(_ => igenerator.Generate(_, arg1));
    }

    public static class ApplyExtension
    {
        public static IGeneratable<TResult> Apply<TResult, TArg1>(this IGeneratable<TResult, TArg1> generatable, TArg1 arg1)
            => new AppliedGeneratable<TResult, TArg1>(generatable, arg1);

        public static IGeneratableEffectful<TResult> Apply<TResult, TArg1>(this IGeneratableEffectful<TResult, TArg1> generatable, TArg1 arg1)
            => new AppliedRefGeneratable<TResult, TArg1>(generatable, arg1);
    }

    class AppliedGeneratable<TResult, TArg1> : IGeneratable<TResult>
    {
        private readonly TArg1 _arg;
        private readonly IGeneratable<TResult, TArg1> _generatable;
        public TResult Generate((uint S0, uint S1, uint S2, uint S3) seed) => _generatable.Generate(seed, _arg);
        public AppliedGeneratable(IGeneratable<TResult, TArg1> generatable, TArg1 arg)
            => (_generatable, _arg) = (generatable, arg);
    }
    class AppliedRefGeneratable<TResult, TArg1> : IGeneratableEffectful<TResult>
    {
        private readonly TArg1 _arg;
        private readonly IGeneratableEffectful<TResult, TArg1> _generatable;
        public TResult Generate(ref (uint S0, uint S1, uint S2, uint S3) seed) => _generatable.Generate(ref seed, _arg);
        public AppliedRefGeneratable(IGeneratableEffectful<TResult, TArg1> generatable, TArg1 arg)
            => (_generatable, _arg) = (generatable, arg);
    }

}

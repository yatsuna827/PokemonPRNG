using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonPRNG.Xoroshiro128p
{
    public static class Xoroshiro128p
    {
        public const ulong MAGIC_NUMBER = 0x82A2B175229D6A5B;

        public static ulong Advance(ref this ulong seed)
            => seed += MAGIC_NUMBER;
        public static ulong Advance(ref this ulong seed, uint n)
            => seed += MAGIC_NUMBER * n;
        public static ulong Back(ref this ulong seed)
            => seed -= MAGIC_NUMBER;
        public static ulong Back(ref this ulong seed, uint n)
            => seed -= MAGIC_NUMBER * n;

        public static ulong GetRand(ref this (ulong S0, ulong S1) state)
        {
            var (_s0, _s1) = (state.S0, state.S0 ^ state.S1);
            var res = state.S0 + state.S1;

            state = (((_s0 << 24) | (_s0 >> 40)) ^ _s1 ^ (_s1 << 16), (_s1 << 37) | (_s1 >> 27));

            return res;
        }
        public static ulong GetRand(ref this (ulong S0, ulong S1) state, ref uint index)
        {
            var (_s0, _s1) = (state.S0, state.S0 ^ state.S1);
            var res = state.S0 + state.S1;

            state = (((_s0 << 24) | (_s0 >> 40)) ^ _s1 ^ (_s1 << 16), (_s1 << 37) | (_s1 >> 27));

            index++;
            return res;
        }
        public static ulong GetRand(ref this (ulong S0, ulong S1) state, uint range)
        {
            var ceil2 = GetRandPow2(range);

            while (true)
            {
                var result = state.GetRand() & ceil2;
                if (result < range) return result;
            }
        }
        public static ulong GetRand(ref this (ulong S0, ulong S1) state, uint range, ref uint index)
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

        public static (ulong S0, ulong S1) Next(this (ulong S0, ulong S1) state)
        {
            var (s0, s1) = (state.S0, state.S0 ^ state.S1);

            return (((s0 << 24) | (s0 >> 40)) ^ s1 ^ (s1 << 16), (s1 << 37) | (s1 >> 27));
        }
        public static (ulong S0, ulong S1) Prev(this (ulong S0, ulong S1) state)
        {
            var (s0, s1) = state;

            var s1_rotl27 = (s1 << 27) | (s1 >> 37);
            var t = s0 ^ (s1_rotl27 << 16);
            t = ((t << 40) | (t >> 24)) ^ ((s1 << 3) | (s1 >> 61));

            return (t, t ^ s1_rotl27);
        }
        public static (ulong S0, ulong S1) Advance(ref this (ulong S0, ulong S1) state)
        {
            var (s0, s1) = (state.S0, state.S0 ^ state.S1);

            return state = (((s0 << 24) | (s0 >> 40)) ^ s1 ^ (s1 << 16), (s1 << 37) | (s1 >> 27));
        }
        public static (ulong S0, ulong S1) Advance(ref this (ulong S0, ulong S1) state, ref uint index)
        {
            var (s0, s1) = (state.S0, state.S0 ^ state.S1);

            index++;
            return state = (((s0 << 24) | (s0 >> 40)) ^ s1 ^ (s1 << 16), (s1 << 37) | (s1 >> 27));
        }
        public static (ulong S0, ulong S1) Back(ref this (ulong S0, ulong S1) state)
        {
            var (s0, s1) = state;

            var s1_rotl27 = (s1 << 27) | (s1 >> 37);
            var t = s0 ^ (s1_rotl27 << 16);
            t = ((t << 40) | (t >> 24)) ^ ((s1 << 3) | (s1 >> 61));

            return state = (t, t ^ s1_rotl27);
        }
        public static (ulong S0, ulong S1) Back(ref this (ulong S0, ulong S1) state, ref uint index)
        {
            var (s0, s1) = state;

            var s1_rotl27 = (s1 << 27) | (s1 >> 37);
            var t = s0 ^ (s1_rotl27 << 16);
            t = ((t << 40) | (t >> 24)) ^ ((s1 << 3) | (s1 >> 61));

            index--;
            return state = (t, t ^ s1_rotl27);
        }

        public static string ToU128String(this (ulong S0, ulong S1) state) => $"{state.S1:X16}{state.S0:X16}";
        public static (ulong S0, ulong S1) FromU128String(this string hex)
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
        // See: https://hackmd.io/@yatsuna827/Sy8tQETsh

        private readonly static (ulong S0, ulong S1)[] _jumpCoefficient = new (ulong, ulong)[56] {
            (0x162AD6EC01B26EAEul, 0x7A8FF5B1C465A931ul),
            (0xB4FBAA5C54EE8B8Ful, 0xB18B0D36CD81A8F5ul),
            (0x1207A1706BEBB202ul, 0x23AC5E0BA1CECB29ul),
            (0x2C88EF71166BC53Dul, 0xBB18E9C8D463BB1Bul),
            (0xC3865BB154E9BE10ul, 0xE3FBE606EF4E8E09ul),
            (0x1A9FC99FA7818274ul, 0x28FAAAEBB31EE2DBul),
            (0x588ABD4C2CE2BA80ul, 0x30A7C4EEF203C7EBul),
            (0x9C90DEBC053E8CEFul, 0xA425003F3220A91Dul),
            (0xB82CA99A09A4E71Eul, 0x81E1DD96586CF985ul),
            (0x35D69E118698A31Dul, 0x4F7FD3DFBB820BFBul),
            (0x49613606C466EFD3ul, 0xFEE2760EF3A900B3ul),
            (0xBD031D011900A9E5ul, 0xF0DF0531F434C57Dul),
            (0x235E761B3B378590ul, 0x442576715266740Cul),
            (0x3710A7AE7945DF77ul, 0x1E8BAE8F680D2B35ul),
            (0x75D8E7DBCEDA609Cul, 0xFD7027FE6D2F6764ul),
            (0xDE2CBA60CD3332B5ul, 0x28EFF231AD438124ul),
            (0x377E64C4E80A06FAul, 0x1808760D0A0909A1ul),
            (0x0CF0A2225DA7FB95ul, 0xB9A362FAFEDFE9D2ul),
            (0x2BAB58A3CADFC0A3ul, 0xF57881AB117349FDul),
            (0x8D51ECDB9ED82455ul, 0x849272241425C996ul),
            (0x521B29D0A57326C1ul, 0xF1CCB8898CBC07CDul),
            (0xFBE65017ABEC72DDul, 0x61179E44214CAAFAul),
            (0x6C446B9BC95C267Bul, 0xD9AA6B1E93FBB6E4ul),
            (0x64F80248D23655C6ul, 0x86E3772194563F6Dul),
            (0xFAD843622B252C78ul, 0xD4E95EEF9EDBDBC6ul),
            (0x598742BBFDDDE630ul, 0x05667023C584A68Aul),
            (0x3A9D7DCE072134A6ul, 0x401AACF87A5E21EEul),
            (0xF0CC32EAF522F0E0ul, 0xE114B1E65A950E43ul),
            (0xEB2BEAA80D3FD8A7ul, 0x905DFF85834FB8D1ul),
            (0x61F29536E1BB6B99ul, 0xC449C069734817CBul),
            (0x390CD235D35187DAul, 0x1E5BC0FE7032F3DFul),
            (0x744E5F1168BA3345ul, 0x3F399E6F1EA22DBCul),
            (0x8CC9AA88A153F5F8ul, 0xD47A02636F041CCAul),
            (0x08D037056C80B9E0ul, 0xF83C06B106D3B7ABul),
            (0x4CE3C123D196BF7Aul, 0x14223EEDAE116A83ul),
            (0xB1B206870DA4E89Aul, 0x24BFD164204335AEul),
            (0x207BB2453717CF67ul, 0x4A5953C8F4BC2A51ul),
            (0xA14E342BB11FF7E6ul, 0xF6B3F196DC551CCFul),
            (0x5422BCA5015DD3B7ul, 0x5B6233B76FA214D7ul),
            (0xEDE7341C00C65B85ul, 0xF20D7136458BD924ul),
            (0xD769CFC9028DEB78ul, 0x9B19BA6B3752065Aul),
            (0xC7B0E531ABE7E4BDul, 0x4F27796502238C48ul),
            (0x1C6D3BA4BB94182Aul, 0xB7B17DCD25003305ul),
            (0x3AE9471D0E2D0BCFul, 0xAAAE579366147D07ul),
            (0x8F9CD3794CA46FBFul, 0x0D56BB288C661CCFul),
            (0xDB2AD4E9C15A9D4Eul, 0x0402342EEDFF424Cul),
            (0x79E061AF5BE21395ul, 0x4E71559E6D0E7F00ul),
            (0x96E7D88C0794E785ul, 0x8367AF1C9D6C1406ul),
            (0xCCDDA809DB64B3E7ul, 0x0DBFCD2453D1D33Ful),
            (0x6C64681C21CD0286ul, 0x3309E57F180D4FF6ul),
            (0xACB8D4C6BA67113Eul, 0xB439F330AB3B9715ul),
            (0xBAD04CA5D96E2CD3ul, 0xC58F079D0205BCF3ul),
            (0xEBFBC2723A906760ul, 0x09417D8C80A37AA7ul),
            (0x38AC01316167183Dul, 0x52F51AC639E09712ul),
            (0x7A134006D4EFA484ul, 0xF37EAD6EA53B96BAul),
            (0x351561E58F8572D4ul, 0xDC1C01799CB8D734ul),
        };
        private readonly static (ulong S0, ulong S1)[] _backJumpCoefficient = new (ulong, ulong)[56] {
            (0x1BE6F9EF0F7C6E72, 0x713C20D6F62F069D),
            (0x81D4D1AD4124AD40, 0xE1E7124BE311E263),
            (0xD15B0AEA27EE0B84, 0x198183F248C62FE4),
            (0x7FDB253CD3096372, 0x6640BC58C22E6E09),
            (0x750E2B697D980C77, 0xAA9FEA82B71AA7A7),
            (0x11F4DD5DEA9DE81F, 0x49F5912C1FBBE16C),
            (0x59163666B6BBF2F9, 0xF7ED93095E928283),
            (0xD9D96F69A74992A7, 0x40F239DD31D0D8E8),
            (0x99E0A82A95DA3EC7, 0xFD8556C65CDDF5C8),
            (0xD123AA298D21DF09, 0x6E71206DB7C16667),
            (0xCC7253FD4B2EF7BF, 0xFFA952DC24A7658F),
            (0xC60CBD520EB5E2A1, 0x897527B00CC1314C),
            (0x33D679DADA618EFE, 0x91624CFCC02AFDD6),
            (0x6DD5D8D50A429DB0, 0xEAE5E2E8ECA75AB8),
            (0x8901AFC01EC0FF26, 0xA68746DD5DBB25C3),
            (0x09264D3799225BEB, 0x825ACF5BA97363B5),
            (0x44DB2B454B668AB3, 0xC76532622D5B9762),
            (0xB31193F714276910, 0xCDA5A8792D8AFD1F),
            (0x6552ECF7859D4F74, 0xDA849D662974C84A),
            (0xBBC997D2F0259955, 0xA94BF29A7E13E7C2),
            (0x344E4336614B4B7B, 0x0AE8D73BB1133DFD),
            (0x9382D5FB077E20D5, 0x90ECDC7830D08754),
            (0x31888C99EFF94254, 0xF6BB2B9C55D6D638),
            (0x66A482668E80BDE4, 0x9926D3AE212DC932),
            (0xB11CECFF5F558438, 0xB189246299BB5165),
            (0x887A70C100930644, 0xD7167AC343A3B34F),
            (0x6CCFD506D591494D, 0x38098B44B078537B),
            (0x8E5056FD972590ED, 0xE7090180AE85B56D),
            (0x088281764715E7C6, 0x7D4021E85C233EEA),
            (0xBE898F008F69795E, 0xBA8AD190266695C1),
            (0xB2BA2813E26719EC, 0x528E5128CDABC1EC),
            (0x0384719BA606E358, 0x86CDF41D512A0407),
            (0x1C21AAF104EDB8AA, 0x0E85CFDF62CD4874),
            (0xF36340BA95F8E193, 0xCDA6EC3A032A1052),
            (0xE43663C17C780509, 0xA6F4763279872361),
            (0x7C9BCAF53F3C4BA4, 0x4D52AD0C69A0139A),
            (0x113B6A0E658BAC62, 0x1034D520F48190FF),
            (0x93C6C017455B9486, 0x97307B6AA6522426),
            (0xD250D4C573DFF2E6, 0x1D3568C93025774A),
            (0xBE498FF6C91A5227, 0x49B6682FB7C90E70),
            (0x79E57BFC77E950D2, 0x4E2278414A756F47),
            (0xB3864BA0B1779E24, 0x607120B3F2E34C60),
            (0x63F0C3822EED2927, 0x626962C3483E93EA),
            (0xB13B0A779544E2E9, 0x919DAFDB2D9FB36F),
            (0x59A7586FC8C97F16, 0xD537E0C62E819478),
            (0x36F763923B504F22, 0xCCB0C187041F7897),
            (0x5E86974000483C40, 0x2F5E5E56BC8F8A80),
            (0x2460C19B5931EC74, 0xE190886E69191E57),
            (0x322067408D606672, 0xDBABB894530F388A),
            (0xA04CD3E3505E447C, 0x1019B8416864DAB1),
            (0xEE0F6BD815D24BFB, 0xB2D3CFA401466CEA),
            (0xE59618EE55E8590C, 0x41276E3EEE3EE06F),
            (0xED3C639654E29425, 0x64E8390434B2D488),
            (0x417FE57F81453A6D, 0x423CCC1BEAB36D0E),
            (0x7744E71A2AE1C50E, 0x5330BEDA050B2058),
            (0xBDD70B441E3B7EB0, 0xCC5E8389E96C1AC0),
            };

        public static (ulong S0, ulong S1) Next(this (ulong S0, ulong S1) state, uint n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Advance();
            n >>= 8;

            for (var k = 0; k < _jumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1) = (0ul, 0ul);
                var (c0, c1) = _jumpCoefficient[k];
                for (var i = 0; i < 64; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Advance();
                }
                for (var i = 0; i < 64; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Advance();
                }

                state = (s0, s1);
            }

            return state;
        }
        public static (ulong S0, ulong S1) Next(this (ulong S0, ulong S1) state, ulong n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Advance();
            n >>= 8;

            for (var k = 0; k < _jumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1) = (0ul, 0ul);
                var (c0, c1) = _jumpCoefficient[k];
                for (var i = 0; i < 64; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Advance();
                }
                for (var i = 0; i < 64; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Advance();
                }

                state = (s0, s1);
            }

            return state;
        }
        public static (ulong S0, ulong S1) Advance(ref this (ulong S0, ulong S1) state, uint n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Advance();
            n >>= 8;

            for (var k = 0; k < _jumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1) = (0ul, 0ul);
                var (c0, c1) = _jumpCoefficient[k];
                for (var i = 0; i < 64; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Advance();
                }
                for (var i = 0; i < 64; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Advance();
                }

                state = (s0, s1);
            }

            return state;
        }
        public static (ulong S0, ulong S1) Advance(ref this (ulong S0, ulong S1) state, uint n, ref uint index)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Advance();
            n >>= 8;

            for (var k = 0; k < _jumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1) = (0ul, 0ul);
                var (c0, c1) = _jumpCoefficient[k];
                for (var i = 0; i < 64; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Advance();
                }
                for (var i = 0; i < 64; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Advance();
                }

                state = (s0, s1);
            }

            index += n;
            return state;
        }
        public static (ulong S0, ulong S1) Advance(ref this (ulong S0, ulong S1) state, ulong n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Advance();
            n >>= 8;

            for (var k = 0; k < _jumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1) = (0ul, 0ul);
                var (c0, c1) = _jumpCoefficient[k];
                for (var i = 0; i < 64; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Advance();
                }
                for (var i = 0; i < 64; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Advance();
                }

                state = (s0, s1);
            }

            return state;
        }

        public static (ulong S0, ulong S1) Prev(this (ulong S0, ulong S1) state, uint n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Back();
            n >>= 8;

            for (var k = 0; k < _backJumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1) = (0ul, 0ul);
                var (c0, c1) = _backJumpCoefficient[k];
                for (var i = 0; i < 64; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Back();
                }
                for (var i = 0; i < 64; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Back();
                }

                state = (s0, s1);
            }

            return state;
        }
        public static (ulong S0, ulong S1) Prev(this (ulong S0, ulong S1) state, ulong n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Back();
            n >>= 8;

            for (var k = 0; k < _backJumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1) = (0ul, 0ul);
                var (c0, c1) = _backJumpCoefficient[k];
                for (var i = 0; i < 64; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Back();
                }
                for (var i = 0; i < 64; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Back();
                }

                state = (s0, s1);
            }

            return state;
        }
        public static (ulong S0, ulong S1) Back(ref this (ulong S0, ulong S1) state, uint n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Back();
            n >>= 8;

            for (var k = 0; k < _backJumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1) = (0ul, 0ul);
                var (c0, c1) = _backJumpCoefficient[k];
                for (var i = 0; i < 64; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Back();
                }
                for (var i = 0; i < 64; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Back();
                }

                state = (s0, s1);
            }

            return state;
        }
        public static (ulong S0, ulong S1) Back(ref this (ulong S0, ulong S1) state, uint n, ref uint index)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Back();
            n >>= 8;

            for (var k = 0; k < _backJumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1) = (0ul, 0ul);
                var (c0, c1) = _backJumpCoefficient[k];
                for (var i = 0; i < 64; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Back();
                }
                for (var i = 0; i < 64; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Back();
                }

                state = (s0, s1);
            }

            index += n;
            return state;
        }
        public static (ulong S0, ulong S1) Back(ref this (ulong S0, ulong S1) state, ulong n)
        {
            for (uint i = 0; i < (n & 0xFF); i++)
                state.Back();
            n >>= 8;

            for (var k = 0; k < _backJumpCoefficient.Length && n != 0; k++, n >>= 1)
            {
                if ((n & 1) == 0) continue;

                var (s0, s1) = (0ul, 0ul);
                var (c0, c1) = _backJumpCoefficient[k];
                for (var i = 0; i < 64; i++, c0 >>= 1)
                {
                    if ((c0 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Back();
                }
                for (var i = 0; i < 64; i++, c1 >>= 1)
                {
                    if ((c1 & 1) != 0)
                        (s0, s1) = (s0 ^ state.S0, s1 ^ state.S1);

                    state.Back();
                }

                state = (s0, s1);
            }

            return state;
        }

    }

    public interface IGeneratable<out TResult>
    {
        TResult Generate((ulong S0, ulong S1) seed);
    }
    public interface IGeneratable<out TResult, in TArg1>
    {
        TResult Generate((ulong S0, ulong S1) seed, TArg1 arg1);
    }

    public interface IGeneratableEffectful<out TResult>
    {
        TResult Generate(ref (ulong S0, ulong S1) seed);
    }
    public interface IGeneratableEffectful<out TResult, in TArg1>
    {
        TResult Generate(ref (ulong S0, ulong S1) seed, TArg1 arg1);
    }

    public static class XoroshiroExt
    {
        public static TResult Generate<TResult>(this (ulong S0, ulong S1) seed, IGeneratable<TResult> generatable)
            => generatable.Generate(seed);
        public static TResult Generate<TResult, TArg>(this (ulong S0, ulong S1) seed, IGeneratable<TResult, TArg> generatable, TArg arg)
            => generatable.Generate(seed, arg);
        public static TResult Generate<TResult>(ref this (ulong S0, ulong S1) seed, IGeneratableEffectful<TResult> generatable)
            => generatable.Generate(ref seed);
        public static TResult Generate<TResult, TArg>(ref this (ulong S0, ulong S1) seed, IGeneratableEffectful<TResult, TArg> generatable, TArg arg)
            => generatable.Generate(ref seed, arg);

        public static IEnumerable<(int Index, T Element)> WithIndex<T>(this IEnumerable<T> enumerator)
            => enumerator.Select((_, i) => (i, _));

        public static IEnumerable<(ulong S0, ulong S1)> Enumerate(this ulong seed)
        {
            yield return (seed, Xoroshiro128p.MAGIC_NUMBER);
            while (true)
                yield return (seed += Xoroshiro128p.MAGIC_NUMBER, Xoroshiro128p.MAGIC_NUMBER);
        }

        public static IEnumerable<(ulong S0, ulong S1)> Enumerate(this (ulong S0, ulong S1) seed)
        {
            yield return seed;
            while (true)
                yield return seed.Advance();
        }
        public static IEnumerable<TResult> Enumerate<TResult>
            (this IEnumerable<(ulong S0, ulong S1)> seedEnumerator, IGeneratable<TResult> igenerator)
            => seedEnumerator.Select(igenerator.Generate);
        public static IEnumerable<TResult> Enumerate<TResult, TArg1>
            (this IEnumerable<(ulong S0, ulong S1)> seedEnumerator, IGeneratable<TResult, TArg1> igenerator, TArg1 arg1)
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
        public TResult Generate((ulong S0, ulong S1) seed) => _generatable.Generate(seed, _arg);
        public AppliedGeneratable(IGeneratable<TResult, TArg1> generatable, TArg1 arg)
            => (_generatable, _arg) = (generatable, arg);
    }
    class AppliedRefGeneratable<TResult, TArg1> : IGeneratableEffectful<TResult>
    {
        private readonly TArg1 _arg;
        private readonly IGeneratableEffectful<TResult, TArg1> _generatable;
        public TResult Generate(ref (ulong S0, ulong S1) seed) => _generatable.Generate(ref seed, _arg);
        public AppliedRefGeneratable(IGeneratableEffectful<TResult, TArg1> generatable, TArg1 arg)
            => (_generatable, _arg) = (generatable, arg);
    }

}

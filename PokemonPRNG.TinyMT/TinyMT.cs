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

            var s2 = odd ? state.S1 ^ 0x8f7011ee : state.S1;

            var x = (odd ? state.S2 ^ 0xfc78ff1f : state.S2) ^ (state.S3 << 10);
            var y = x ^ state.S3;

            x ^= x << 1;
            x ^= x << 2;
            x ^= x << 4;
            x ^= x << 8;
            x ^= x << 16;

            y ^= y >> 1;
            y ^= y >> 2;
            y ^= y >> 4;
            y ^= y >> 8;
            y ^= y >> 16;

            var (s0, s1) = (state.S0 & 0x7FFFFFFF, x ^ s2);
            return ((s0 ^ s1) & 0x7FFFFFFF, s0 ^ (s1 & 0x80000000), s2, y);
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

            var s2 = odd ? state.S1 ^ 0x8f7011ee : state.S1;

            var x = (odd ? state.S2 ^ 0xfc78ff1f : state.S2) ^ (state.S3 << 10);
            var y = x ^ state.S3;

            x ^= x << 1;
            x ^= x << 2;
            x ^= x << 4;
            x ^= x << 8;
            x ^= x << 16;

            y ^= y >> 1;
            y ^= y >> 2;
            y ^= y >> 4;
            y ^= y >> 8;
            y ^= y >> 16;

            var (s0, s1) = (state.S0 & 0x7FFFFFFF, x ^ s2);
            return state = ((s0 ^ s1) & 0x7FFFFFFF, s0 ^ (s1 & 0x80000000), s2, y);
        }

    }

    public static class TinyMTJumpExt
    {
        private readonly static (uint S0, uint S1, uint S2, uint S3)[] _jumpCoefficient = new (uint, uint, uint, uint)[56]
        {
            (0x5F8D586B, 0xB9E15CAA, 0x03DBBF73, 0x3ACF5521),
            (0x56708B7E, 0x9951A064, 0xACAA7823, 0xFD7A37B1),
            (0x49A28E4D, 0x43AA2374, 0xFE4126B0, 0x82EA5FEF),
            (0x5A895878, 0x090D98E4, 0x7AEE0EEA, 0xE7D0C590),
            (0x4EC4AB34, 0x61DEF496, 0x3D093826, 0x2E1BD647),
            (0x7AB182DE, 0x334A0FE7, 0xD2005A71, 0x33AE14E5),
            (0x3D24301D, 0xBA589CE4, 0xE69F0174, 0x0E06F5B1),
            (0x4B86205D, 0x0B7359BE, 0x5FEAF53F, 0xDDD4A1F4),
            (0x5DB83196, 0xE2D08EC7, 0x345B1A3F, 0x8D859B2A),
            (0x2403495B, 0x55BF4C86, 0x28F32370, 0x457372C8),
            (0x7C1E3386, 0x2EBB4136, 0x1E112335, 0xA5C8C0D5),
            (0x180CCF6D, 0x12DD2371, 0xA5B17E10, 0xA40ED9C8),
            (0x33BCB628, 0x41FBD202, 0x21D18C04, 0x658CD2F4),
            (0x485CC220, 0xC8FC1F0F, 0x9783DB46, 0x69489879),
            (0x09724EB6, 0x8E695F01, 0xC4826E0B, 0x4CF6C5EC),
            (0x65C67175, 0xF9B4E14B, 0x194065DC, 0x2A5EAF3A),
            (0x75EFDA16, 0x7FEB70C4, 0x452BC110, 0x9F0DE9FE),
            (0x7A6C610A, 0x1AD541A0, 0x0FCBE635, 0xABF913E2),
            (0x7D356342, 0x203777CA, 0x716CA869, 0x20999170),
            (0x026763DF, 0xFFEEA095, 0x5E643545, 0x85996D5A),
            (0x730F081A, 0x6DD7A644, 0x9569A8B4, 0x197365AC),
            (0x153ED5CC, 0x80BFD2B6, 0xB37E61BE, 0xF2156D44),
            (0x640734F4, 0x227DF3DE, 0xF43B15A9, 0xAC7A0AB2),
            (0x462B6975, 0xD5885DD2, 0x0420B466, 0x98FDAAB3),
            (0x0A47EF2D, 0x75B09C04, 0x28B34AEB, 0xBF8B5E8A),
            (0x12900918, 0xEB165D1A, 0x910F33E8, 0xEADBDFD3),
            (0x0A85350B, 0x1048B2DA, 0xE5AFF8CA, 0xB8D3313A),
            (0x759F4C32, 0xDA996196, 0x6771A41B, 0xF3F4C915),
            (0x75BD230C, 0xE53B60CE, 0x669915F0, 0x12E70CFF),
            (0x69DB2E2E, 0xDDF5DCC0, 0xDCA69D74, 0x55F7ABC7),
            (0x5D682176, 0xC3FC0133, 0xC46696CC, 0x4D082E8A),
            (0x3F312A13, 0x174DE5DE, 0x94B4C2AD, 0xC9925EB4),
            (0x54FB35A1, 0x0B661E76, 0x69EB10EA, 0x27FE8D16),
            (0x73E761A6, 0xA9A13E6E, 0x142AC79E, 0x1D597217),
            (0x5A6BD7DD, 0x5A257650, 0x1228FC6E, 0x5B622994),
            (0x16367017, 0x5BDB7133, 0xDC1D1866, 0x42ADC5AE),
            (0x55E87B10, 0x7878F992, 0x0915D86A, 0xF03383D9),
            (0x767F778F, 0x44D77A12, 0xEC246728, 0x5ED41176),
            (0x23A9354A, 0x47048255, 0x0C493203, 0x4559986D),
            (0x2B01779F, 0xC484732D, 0x6C2AD6CA, 0x98795550),
            (0x13081146, 0x4E985BA9, 0x9DFE42FD, 0xB225D3ED),
            (0x68F2E4E4, 0x250A0CE5, 0x3B7B0F4B, 0x8CF6EAC5),
            (0x11236AFC, 0x75A7AECB, 0x13802730, 0xBD2A30F3),
            (0x3CF7F89F, 0x2DAB2D07, 0x66769E78, 0x423CE927),
            (0x1E33E943, 0xEE077674, 0xEA1B510D, 0xCB4C7EF9),
            (0x0EC97D83, 0x60D66B8E, 0x6965B38C, 0x70DD2810),
            (0x7708A50C, 0xAE907BFD, 0x64DEBE8F, 0x4E1503A6),
            (0x5FA90EF8, 0xCBDDD807, 0xBF48F09E, 0x38B88E42),
            (0x7E0CCD40, 0x51602138, 0x695D57F3, 0xB189DEBA),
            (0x1C1AD24F, 0xF9DDCB04, 0x7B47D766, 0x61C24A10),
            (0x5669C209, 0x56581589, 0xB51244BD, 0x4B120444),
            (0x378D6CE3, 0xB3513731, 0x06F70B3F, 0x3C6BDB68),
            (0x02B66808, 0x1DD34BD3, 0xE02158FA, 0xD534D9FD),
            (0x6741396A, 0x2E8D4E92, 0xD5D275DB, 0xDE0759D5),
            (0x29466695, 0x17562F68, 0xA4A5DE4A, 0x61CC2F72),
            (0x0A11FEA0, 0xDDA01585, 0xBFB29456, 0x9C4A2173),
        };
        private readonly static (uint S0, uint S1, uint S2, uint S3)[] _backJumpCoefficient = new (uint, uint, uint, uint)[56]
        {
            (0x968B0413, 0xFCACD0B5, 0xC54C6DD3, 0xBA0BAC8B),
            (0x44681086, 0xB096B031, 0x8151D31D, 0x3394E132),
            (0x301CCC1C, 0xD2AF4094, 0x9B512A74, 0x43FD8F52),
            (0x683871F2, 0x16D05F29, 0xAE68521D, 0xFD7FC725),
            (0xDE2F7001, 0xDEF65AA3, 0xF67AEE16, 0x1C73EFDF),
            (0x60916774, 0x88FE7A3E, 0xB22E0358, 0x925C9646),
            (0xCE446B99, 0x8C56160E, 0x714BA819, 0x47BF50EF),
            (0xAD799359, 0xA1A4B2C5, 0x107BB59A, 0x2FFCB49F),
            (0x6F0FC706, 0x139BF04F, 0xEADF1B6E, 0x629D613E),
            (0x05071C9E, 0x9EB949C5, 0xB11CDC47, 0xC0148623),
            (0x8BAD895D, 0x8A020D06, 0x300412EF, 0x0AD136E9),
            (0xFFA0C943, 0x09184414, 0x9EE05207, 0x8BFDDE82),
            (0x81D01611, 0x09027309, 0xB5005DD8, 0x83FD9F79),
            (0x07245C92, 0x8CFF83C3, 0xEB865FB0, 0x1E3B213F),
            (0x20965982, 0xE58F95FA, 0x0C40A0B5, 0xE8F3B304),
            (0x45D976CC, 0x282B743E, 0xF4D682C3, 0x783A33C3),
            (0x2D4BAAA6, 0x67CAF035, 0xA1569DC3, 0xC832E79E),
            (0x956A1A3B, 0x6E3F1D62, 0x70A16C36, 0xE092FB16),
            (0x7DC21DF6, 0x3BE06DA8, 0xBB3B47B4, 0xA0A78E16),
            (0x8A4B6EF7, 0x75881817, 0xEB914847, 0xF2A29B52),
            (0x5726F6AA, 0x68524A67, 0x172A70AB, 0x147E725A),
            (0xA9F6340B, 0x57290F16, 0x62EC42A7, 0xB99152E7),
            (0x25F467D4, 0x95A40976, 0xA4A7DD2F, 0x930D4A5A),
            (0x4936A528, 0xCC054742, 0xFB1D7B18, 0x2ABFD2A2),
            (0x4E62FA5E, 0xEE4D9619, 0x1BE59776, 0xC015C399),
            (0x38628758, 0x1CAC7ED9, 0x55C6F6AE, 0x9A3B1D1A),
            (0x66A2F15C, 0x1174FB3A, 0x935F7D4F, 0x7772F754),
            (0xAAB3ACA1, 0x808A87CA, 0x9A37B9E0, 0x018CEF50),
            (0x9929A0E3, 0x6121B6A5, 0xB9C6A26D, 0xA96012CE),
            (0xBF1CC17D, 0x510F0B3E, 0x470F3E2E, 0x275C0956),
            (0x25806AD8, 0x02B29311, 0x86090B44, 0x806E8BF6),
            (0xECD05119, 0xA2BA1308, 0x685A743B, 0x955EB359),
            (0x56C99FE4, 0x43D6C0CC, 0xAF63679F, 0xC9FABCE7),
            (0x57A35564, 0x7E9455B6, 0xD2A24E5F, 0x6F0FEB6B),
            (0xE2AEA9FF, 0xA497870B, 0x1C8DF7B6, 0xB977DFF9),
            (0x6693054A, 0x49B2A99B, 0xED07DAA9, 0xC386F1B4),
            (0xCD01CD27, 0xB2E4C5CF, 0xC3786519, 0x8A916635),
            (0x199AEECC, 0x43593139, 0xFFA90918, 0xD5E2B09F),
            (0x978EDF63, 0x4E1B8E32, 0x65500D72, 0x219F6C14),
            (0xCC6F0A97, 0xA93E810D, 0xFE263BA7, 0x90E47B5A),
            (0xEED9427D, 0x48205C5F, 0x16212E1E, 0x71C50F13),
            (0x6850D82E, 0xA262135D, 0x6FB35F17, 0x6F0BA506),
            (0x982DB3ED, 0x151B4D60, 0xAD005007, 0x9867287C),
            (0x3C49D148, 0x68FC5928, 0x3F075CE4, 0x2CBAEB09),
            (0xFAFDECBF, 0x7E0D5DA4, 0xB1D7851E, 0x69FB24DA),
            (0x2DDD41B2, 0x834175E6, 0xF661DD41, 0xDFC37408),
            (0x5E17E258, 0xA6111EA3, 0xF14D356B, 0xB9A2E128),
            (0x78ED5DC4, 0x9E3D9B88, 0x4842CDA9, 0x22E505AA),
            (0x6E39679A, 0xAEE813BB, 0xE591F44D, 0xDAF0735A),
            (0x27FEFEB0, 0xAE562374, 0xC2516F57, 0x90ADA8F0),
            (0xC4D522CD, 0x8DB6A028, 0xD7FE8FED, 0x268D5AD3),
            (0xFEB7ACAD, 0xF5E6D34F, 0xC5C3EB8B, 0x42362906),
            (0x8A206D29, 0xCE43C5ED, 0xF605C4FA, 0xE0DF83A2),
            (0x2A96FB44, 0x227FA420, 0xF9C19D68, 0x35D94E23),
            (0xCD8BC91B, 0x2E0232C5, 0x3AFF79BB, 0x09C74188),
            (0xB1E62443, 0x14DDD93D, 0xD252C96D, 0xDCEC879A),
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

}

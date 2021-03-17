/*
 * Copyright (C) Rei HOBARA 2007
 * 
 * Name:
 *     SFMT.cs
 * Class:
 *     Rei.Random.SFMT
 *     Rei.Random.MTPeriodType
 * Purpose:
 *     A random number generator using SIMD-oriented Fast Mersenne Twister(SFMT).
 * Remark:
 *     This code is C# implementation of SFMT.
 *     SFMT was introduced by Mutsuo Saito and Makoto Matsumoto.
 *     See http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/SFMT/index.html for detail of SFMT.
 * History:
 *     2007/10/6 initial release.
 * 
 */

using System.Linq;
namespace PokemonPRNG.SFMT
{
    /// <summary>
    /// SFMTの擬似乱数ジェネレータークラス。
    /// </summary>
    public class SFMT
    {
        /// <summary>
        /// 周期を表す指数。
        /// </summary>
        public const int MEXP = 19937;
        public const int POS1 = 122;
        public const int SL1 = 18;
        public const int SL2 = 1;
        public const int SR1 = 11;
        public const int SR2 = 1;
        public const uint MSK1 = 0xdfffffef;
        public const uint MSK2 = 0xddfecb7f;
        public const uint MSK3 = 0xbffaffff;
        public const uint MSK4 = 0xbffffff6;
        public const uint PARITY1 = 0x00000001;
        public const uint PARITY2 = 0x00000000;
        public const uint PARITY3 = 0x00000000;
        public const uint PARITY4 = 0x13c9e684;

        /// <summary>
        /// 各要素を128bitとしたときの内部状態ベクトルの個数。
        /// </summary>
        public const int N = MEXP / 128 + 1;
        /// <summary>
        /// 各要素を32bitとしたときの内部状態ベクトルの個数。
        /// </summary>
        public const int N32 = N * 4;

        private int randIndex;
        private readonly uint[] stateVector;

        public ulong Index32 { get; private set; }
        public ulong Index64 { get => Index32 / 2; }
        
        private SFMT(SFMT parent) { this.Index32 = parent.Index32; randIndex = parent.randIndex; stateVector = parent.stateVector.ToArray(); }
        public SFMT(uint seed)
        {
            //内部状態配列確保
            stateVector = new uint[N32];

            //内部状態配列初期化
            stateVector[0] = seed;
            for (int i = 1; i < N32; i++) stateVector[i] = (uint)(1812433253 * (stateVector[i - 1] ^ (stateVector[i - 1] >> 30)) + i);

            //確認
            PeriodCertification();

            //初期位置設定
            randIndex = N32;

            Index32 = 0;
        }

        public SFMT Clone() => new SFMT(this);

        /// <summary>
        /// 符号なし32bitの擬似乱数を取得します。
        /// </summary>
        public uint GetRand32()
        {
            if (randIndex >= N32)
            {
                GenerateRandAll();
                randIndex = 0;
            }

            Index32++;
            return stateVector[randIndex++];
        }
        public uint GetRand32(uint m) => GetRand32() % m;

        public ulong GetRand64() => GetRand32() | ((ulong)GetRand32() << 32);
        public ulong GetRand64(uint m) => GetRand64() % m;

        /// <summary>
        /// 内部状態ベクトルが適切か確認し、必要であれば調節します。
        /// </summary>
        private void PeriodCertification()
        {
            uint[] PARITY = new uint[] { PARITY1, PARITY2, PARITY3, PARITY4 };

            uint inner = 0;
            for (int i = 0; i < 4; i++) inner ^= stateVector[i] & PARITY[i];
            for (int i = 16; i > 0; i >>= 1) inner ^= inner >> i;

            // check OK
            if ((inner & 1) == 1) return;

            // check NG, and modification
            for (int i = 0; i < 4; i++)
            {
                uint work = 1;
                for (int j = 0; j < 32; j++, work <<= 1)
                {
                    if ((work & PARITY[i]) != 0)
                    {
                        stateVector[i] ^= work;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// gen_rand_allの(2^19937-1)周期用。
        /// </summary>
        private void GenerateRandAll()
        {
            uint[] p = this.stateVector;

            const int cMEXP = 19937;
            const int cPOS1 = 122;
            const uint cMSK1 = 0xdfffffefU;
            const uint cMSK2 = 0xddfecb7fU;
            const uint cMSK3 = 0xbffaffffU;
            const uint cMSK4 = 0xbffffff6U;
            const int cSL1 = 18;
            const int cSR1 = 11;
            const int cN = cMEXP / 128 + 1;
            const int cN32 = cN * 4;

            int a = 0;
            int b = cPOS1 * 4;
            int c = (cN - 2) * 4;
            int d = (cN - 1) * 4;
            do
            {
                p[a + 3] = p[a + 3] ^ (p[a + 3] << 8) ^ (p[a + 2] >> 24) ^ (p[c + 3] >> 8) ^ ((p[b + 3] >> cSR1) & cMSK4) ^ (p[d + 3] << cSL1);
                p[a + 2] = p[a + 2] ^ (p[a + 2] << 8) ^ (p[a + 1] >> 24) ^ (p[c + 3] << 24) ^ (p[c + 2] >> 8) ^ ((p[b + 2] >> cSR1) & cMSK3) ^ (p[d + 2] << cSL1);
                p[a + 1] = p[a + 1] ^ (p[a + 1] << 8) ^ (p[a + 0] >> 24) ^ (p[c + 2] << 24) ^ (p[c + 1] >> 8) ^ ((p[b + 1] >> cSR1) & cMSK2) ^ (p[d + 1] << cSL1);
                p[a + 0] = p[a + 0] ^ (p[a + 0] << 8) ^ (p[c + 1] << 24) ^ (p[c + 0] >> 8) ^ ((p[b + 0] >> cSR1) & cMSK1) ^ (p[d + 0] << cSL1);
                c = d; d = a; a += 4; b += 4;
                if (b >= cN32) b = 0;
            } while (a < cN32);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PokemonPRNG.SFMT
{
    public abstract class SFMTBase
    {
        /// <summary>
        /// 周期を表す指数。
        /// </summary>
        protected const int MEXP = 19937;

        /// <summary>
        /// 各要素を128bitとしたときの内部状態ベクトルの個数。
        /// </summary>
        protected const int N = MEXP / 128 + 1;

        /// <summary>
        /// 各要素を32bitとしたときの内部状態ベクトルの個数。
        /// </summary>
        protected const int N32 = N * 4;

        protected readonly uint[] _stateVector;

        /// <summary>
        /// 内部状態ベクトルが適切か確認し、必要であれば調節します。
        /// </summary>
        private void PeriodCertification()
        {
            var PARITY = new uint[] { 1, 0, 0, 0x13c9e684 };

            var inner = 0u;
            for (int i = 0; i < 4; i++) inner ^= _stateVector[i] & PARITY[i];
            for (int i = 16; i > 0; i >>= 1) inner ^= inner >> i;

            // check OK
            if ((inner & 1) == 1) return;

            // check NG, and modification
            for (int i = 0; i < 4; i++)
            {
                var work = 1u;
                for (int j = 0; j < 32; j++, work <<= 1)
                {
                    if ((work & PARITY[i]) != 0)
                    {
                        _stateVector[i] ^= work;
                        return;
                    }
                }
            }
        }

        protected void GenerateRandAll()
        {
            // コピーではなく別名参照
            var p = _stateVector;

            const int cPOS1 = 122;
            const uint cMSK1 = 0xdfffffefU;
            const uint cMSK2 = 0xddfecb7fU;
            const uint cMSK3 = 0xbffaffffU;
            const uint cMSK4 = 0xbffffff6U;
            const int cSL1 = 18;
            const int cSR1 = 11;

            int a = 0;
            int b = cPOS1 * 4;
            int c = (N - 2) * 4;
            int d = (N - 1) * 4;
            do
            {
                p[a + 3] = p[a + 3] ^ (p[a + 3] << 8) ^ (p[a + 2] >> 24) ^ (p[c + 3] >> 8) ^ ((p[b + 3] >> cSR1) & cMSK4) ^ (p[d + 3] << cSL1);
                p[a + 2] = p[a + 2] ^ (p[a + 2] << 8) ^ (p[a + 1] >> 24) ^ (p[c + 3] << 24) ^ (p[c + 2] >> 8) ^ ((p[b + 2] >> cSR1) & cMSK3) ^ (p[d + 2] << cSL1);
                p[a + 1] = p[a + 1] ^ (p[a + 1] << 8) ^ (p[a + 0] >> 24) ^ (p[c + 2] << 24) ^ (p[c + 1] >> 8) ^ ((p[b + 1] >> cSR1) & cMSK2) ^ (p[d + 1] << cSL1);
                p[a + 0] = p[a + 0] ^ (p[a + 0] << 8) ^ (p[c + 1] << 24) ^ (p[c + 0] >> 8) ^ ((p[b + 0] >> cSR1) & cMSK1) ^ (p[d + 0] << cSL1);
                c = d; d = a; a += 4; b += 4;
                if (b >= N32) b = 0;
            } while (a < N32);
        }

        public SFMTBase(SFMTBase source)
        {
            _stateVector = new uint[N32];
            Array.Copy(source._stateVector, _stateVector, N32);
        }
        public SFMTBase(uint initialSeed)
        {
            //内部状態配列確保
            _stateVector = new uint[N32];

            //内部状態配列初期化
            _stateVector[0] = initialSeed;
            for (uint i = 1; i < _stateVector.Length; i++)
                _stateVector[i] = 0x6C078965u * (_stateVector[i - 1] ^ (_stateVector[i - 1] >> 30)) + i;

            //確認
            PeriodCertification();
        }

        public abstract ulong Index32 { get; }
        public ulong Index64 { get => Index32 / 2; }

        /// <summary>
        /// 符号なし32bitの擬似乱数を取得します。
        /// </summary>
        public abstract uint GetRand32();

        /// <summary>
        /// 符号なし32bitの擬似乱数をmで割った余りを取得します。
        /// </summary>
        public uint GetRand32(uint m) => GetRand32() % m;

        /// <summary>
        /// 符号なし64bitの擬似乱数を取得します。
        /// </summary>
        public ulong GetRand64() => GetRand32() | ((ulong)GetRand32() << 32);

        /// <summary>
        /// 符号なし64bitの擬似乱数をmで割った余りを取得します。
        /// </summary>
        public ulong GetRand64(uint m) => GetRand64() % m;

        /// <summary>
        /// 乱数を進めるユーティリティです。
        /// 効率的なジャンプ関数は実装していないため、線形時間がかかります。
        /// </summary>
        public void Advance(uint n = 1)
        {
            for (int i = 0; i < n; i++) GetRand32();
        }
    }
}

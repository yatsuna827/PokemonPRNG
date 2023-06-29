using System;
using System.Diagnostics;

namespace PokemonPRNG.SFMT
{
    public sealed class CachedSFMT : SFMTBase
    {
        private readonly int _cacheSegments;
        private readonly int _capacity;

        private int _head;
        private ulong _headIndex;
        private int _tempIndex;
        private int _nextPeriod;
        private readonly uint[] _cache;

        /// <summary>
        /// 基準となる消費数を1進めます.
        /// </summary>
        public void MoveNext()
        {
            _head++;
            _headIndex++;
            _tempIndex = 0;

            if (_head == _nextPeriod)
            {
                GenerateRandAll();
                Array.Copy(_stateVector, 0, _cache, _head - N32, N32);
                _nextPeriod += N32;
                if (_head == _capacity) _head = 0;
                if (_nextPeriod > _capacity) _nextPeriod = N32;
            }
        }

        public override ulong Index32 { get => _headIndex + (ulong)_tempIndex; }

        [Conditional("DEBUG")]
        private void CheckCacheBoundary()
        {
            if (!(_tempIndex < _capacity - N32))
                throw new Exception("The number of calls to GetRand has exceeded the cache capacity.");
        }
        
        public override uint GetRand32()
        {
            // Conditional("DEBUG")属性を付与してあるので、リリースビルド時はメソッド呼出が省略される
            // 毎回チェックが入ると実行速度に大きく影響が出るので…。
            CheckCacheBoundary();

            return _cache[(_head + _tempIndex++) % _cache.Length];
        }

        public void Advance(int n)
            => _tempIndex += n;

        /// <summary>
        /// </summary>
        /// <param name="initialSeed">SFMTの初期seed</param>
        /// <param name="cacheSegments">SFMTの状態ベクトル何回分をキャッシュするか. 1以上100以下である必要があります.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public CachedSFMT(uint initialSeed, int cacheSegments) : base(initialSeed)
        {
            if (cacheSegments < 1 || 100 < cacheSegments) throw new ArgumentOutOfRangeException(nameof(cacheSegments));

            _cacheSegments = cacheSegments + 1;
            _capacity = N32 * _cacheSegments;

            _head = 0;
            _cache = new uint[_capacity];

            for (int i = 0; i < _cacheSegments; i++)
            {
                GenerateRandAll();
                Array.Copy(_stateVector, 0, _cache, N32 * i, N32);
            }

            _nextPeriod = N32;
        }

    }
}

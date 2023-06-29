using System;
using System.Diagnostics;

namespace PokemonPRNG.MT
{
    public sealed class CachedMT : MTBase
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
                Update();

                var source = _stateVector.AsSpan();
                var span = _cache.AsSpan(_head - N, source.Length);
                for (int k = 0; k < span.Length; k++)
                    span[k] = Temper(source[k]);

                _nextPeriod += N;
                if (_head == _capacity) _head = 0;
                if (_nextPeriod > _capacity) _nextPeriod = N;
            }
        }

        public override ulong Index { get => _headIndex + (ulong)_tempIndex; }

        [Conditional("DEBUG")]
        private void CheckCacheBoundary()
        {
            if (!(_tempIndex < _capacity - N))
                throw new Exception("The number of calls to GetRand has exceeded the cache capacity.");
        }

        public override uint GetRand()
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
        public CachedMT(uint initialSeed, int cacheSegments) : base(initialSeed)
        {
            if (cacheSegments < 1 || 100 < cacheSegments) throw new ArgumentOutOfRangeException(nameof(cacheSegments));

            _cacheSegments = cacheSegments + 1;
            _capacity = N * _cacheSegments;

            _head = 0;
            _cache = new uint[_capacity];

            for (int i = 0; i < _cacheSegments; i++)
            {
                Update();

                var source = _stateVector.AsSpan();
                var span = _cache.AsSpan(N * i, source.Length);
                for (int k = 0; k < span.Length; k++)
                    span[k] = Temper(source[k]);
            }

            _nextPeriod = N;
        }

    }

}

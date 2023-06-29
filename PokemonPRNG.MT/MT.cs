
namespace PokemonPRNG.MT
{
    public sealed class MT : MTBase
    {
        private ulong _index;
        private int _randIndex;

        public override ulong Index => _index;

        public override uint GetRand()
        {
            if (_randIndex >= N)
            {
                Update();
                _randIndex = 0;
            }

            _index++;

            return Temper(_stateVector[_randIndex++]);
        }

        /// <summary>
        /// 乱数を進めるユーティリティです。
        /// 効率的なジャンプ関数は実装していないため、線形時間がかかります。
        /// </summary>
        public void Advance(uint n = 1)
        {
            for (int i = 0; i < n; i++) GetRand();
        }

        public MT Clone() => new MT(this);
        public MT(MT source) : base(source)
        {
            _randIndex = source._randIndex;
        }
        public MT(uint initialSeed) : base(initialSeed)
        {
            _randIndex = N;
        }
    }
}

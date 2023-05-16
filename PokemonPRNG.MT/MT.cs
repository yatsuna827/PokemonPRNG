using System.Linq;
namespace PokemonPRNG.MT
{
    public class MT
    {
        private const int N = 624;
        private const int M = 397;
        private const uint MATRIX_A = 0x9908b0df;
        private const uint UPPER_MASK = 0x80000000;
        private const uint LOWER_MASK = 0x7fffffff;

        private readonly uint[] _stateVector;
        private int _randIndex;

        public uint GetRand()
        {
            if (_randIndex >= N) Update();

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


        private static uint Temper(uint y)
        {
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680;
            y ^= (y << 15) & 0xefc60000;
            y ^= (y >> 18);
            return y;
        }
        private void Update()
        {
            for (var k = 0; k < N - M; k++)
            {
                var temp = (_stateVector[k] & UPPER_MASK) | (_stateVector[k + 1] & LOWER_MASK);
                _stateVector[k] = _stateVector[k + M] ^ (temp >> 1);
                if ((temp & 1) == 1) _stateVector[k] ^= MATRIX_A;
            }
            for (var k = N - M; k < N - 1; k++)
            {
                var temp = (_stateVector[k] & UPPER_MASK) | (_stateVector[k + 1] & LOWER_MASK);
                _stateVector[k] = _stateVector[k + (M - N)] ^ (temp >> 1);
                if ((temp & 1) == 1) _stateVector[k] ^= MATRIX_A;
            }
            {
                var temp = (_stateVector[N - 1] & UPPER_MASK) | (_stateVector[0] & LOWER_MASK);
                _stateVector[N - 1] = _stateVector[M - 1] ^ (temp >> 1);
                if ((temp & 1) == 1) _stateVector[N - 1] ^= MATRIX_A;
            }

            _randIndex = 0;
        }

        public MT Clone() => new MT(this);

        private MT(MT parent) 
        { 
            _randIndex = parent._randIndex;
            _stateVector = parent._stateVector.ToArray();
        }
        public MT(uint seed)
        {
            // 配列初期化
            _stateVector = new uint[N];
            
            // 内部状態の初期化
            _stateVector[0] = seed;
            for (uint i = 1; i < _stateVector.Length; i++)
                _stateVector[i] = 0x6C078965u * (_stateVector[i - 1] ^ (_stateVector[i - 1] >> 30)) + i;

            _randIndex = N;
        }
    }
}
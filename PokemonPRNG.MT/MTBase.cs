using System;

namespace PokemonPRNG.MT
{
    public abstract class MTBase
    {
        protected const int N = 624;
        protected const int M = 397;
        protected const uint MATRIX_A = 0x9908b0df;
        protected const uint UPPER_MASK = 0x80000000;
        protected const uint LOWER_MASK = 0x7fffffff;

        protected readonly uint[] _stateVector;

        public abstract uint GetRand();

        protected static uint Temper(uint y)
        {
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680;
            y ^= (y << 15) & 0xefc60000;
            y ^= (y >> 18);
            return y;
        }

        protected void Update()
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
        }

        public abstract ulong Index { get; }

        public MTBase(MTBase source)
        {
            _stateVector = new uint[N];
            Array.Copy(source._stateVector, _stateVector, N);
        }
        public MTBase(uint initialSeed)
        {
            _stateVector = new uint[N];

            _stateVector[0] = initialSeed;
            for (uint i = 1; i < _stateVector.Length; i++)
                _stateVector[i] = 0x6C078965u * (_stateVector[i - 1] ^ (_stateVector[i - 1] >> 30)) + i;
        }
    }
}

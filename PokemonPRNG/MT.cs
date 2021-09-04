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

        private readonly uint[] stateVector;
        private int randIndex;

        private static uint Temper(uint y)
        {
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680;
            y ^= (y << 15) & 0xefc60000;
            y ^= (y >> 18);
            return y;
        }

        public uint GetRand()
        {
            if (randIndex >= N) Update();

            return Temper(stateVector[randIndex++]);
        }
        private void Update()
        {
            uint temp;
            for (var k = 0; k < N - M; k++)
            {
                temp = (stateVector[k] & UPPER_MASK) | (stateVector[k + 1] & LOWER_MASK);
                stateVector[k] = stateVector[k + M] ^ (temp >> 1);
                if ((temp & 1) == 1) stateVector[k] ^= MATRIX_A;
            }
            for (var k = N - M; k < N - 1; k++)
            {
                temp = (stateVector[k] & UPPER_MASK) | (stateVector[k + 1] & LOWER_MASK);
                stateVector[k] = stateVector[k + (M - N)] ^ (temp >> 1);
                if ((temp & 1) == 1) stateVector[k] ^= MATRIX_A;
            }

            temp = (stateVector[N - 1] & UPPER_MASK) | (stateVector[0] & LOWER_MASK);
            stateVector[N - 1] = stateVector[M - 1] ^ (temp >> 1);
            if ((temp & 1) == 1) stateVector[N - 1] ^= MATRIX_A;

            randIndex = 0;
        }

        public uint GetSeed()
        {
            return Temper(stateVector[randIndex]);
        }

        public MT Clone() => new MT(this);

        private MT(MT parent) { this.randIndex = parent.randIndex; this.stateVector = parent.stateVector.ToArray(); }
        public MT(uint seed)
        {
            // 配列初期化
            stateVector = new uint[N];
            
            // 内部状態の初期化
            stateVector[0] = seed;
            for (uint i = 1; i < N; i++)
                stateVector[i] = 0x6C078965u * (stateVector[i - 1] ^ (stateVector[i - 1] >> 30)) + i;

            randIndex = N;
        }
    }
}
using System.Runtime.Intrinsics.X86;

namespace PokemonPRNG.SFMT.SIMD
{
    public sealed class SIMDSFMT : SFMTBase
    {
        private int _randIndex;
        private ulong _index32;

        public override ulong Index32 { get => _index32; }

        private SIMDSFMT(SIMDSFMT source) : base(source)
        {
            _index32 = source.Index32;
            _randIndex = source._randIndex;
        }
        public SIMDSFMT(uint initialSeed) : base(initialSeed)
        {
            _index32 = 0;
            _randIndex = N32;
        }

        public SIMDSFMT Clone() => new(this);

        /// <summary>
        /// 符号なし32bitの擬似乱数を取得します。
        /// </summary>
        public override uint GetRand32()
        {
            if (_randIndex >= N32)
            {
                if (Avx2.IsSupported)
                    SIMDSFMTFunction.GenerateRandAll_Avx2(_stateVector, _stateVector);
                else if (Sse2.IsSupported)
                    SIMDSFMTFunction.GenerateRandAll_Sse2(_stateVector, _stateVector);
                else
                    GenerateRandAll();
                _randIndex = 0;
            }

            _index32++;
            return _stateVector[_randIndex++];
        }

    }

}

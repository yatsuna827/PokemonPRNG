using System;
using System.Linq;

namespace PokemonPRNG.SFMT
{
    /// <summary>
    /// SFMTの擬似乱数ジェネレータークラス。
    /// </summary>
    public sealed class SFMT : SFMTBase
    {
        private int _randIndex;
        private ulong _index32;

        public override ulong Index32 { get => _index32; }
        
        private SFMT(SFMT source) : base(source)
        { 
            _index32 = source.Index32; 
            _randIndex = source._randIndex;
        }
        public SFMT(uint initialSeed) : base(initialSeed)
        {
            _index32 = 0;
            _randIndex = N32;
        }

        public SFMT Clone() => new SFMT(this);

        /// <summary>
        /// 符号なし32bitの擬似乱数を取得します。
        /// </summary>
        public override uint GetRand32()
        {
            if (_randIndex >= N32)
            {
                GenerateRandAll();
                _randIndex = 0;
            }

            _index32++;
            return _stateVector[_randIndex++];
        }

    }
}

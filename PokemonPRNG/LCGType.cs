using System;
namespace PokemonPRNG.LCG32
{
    class LCGType
    {
        internal readonly uint MultiplecationConst, AdditionConst, Reverse_MultiplecationConst, Reverse_AdditionConst;
        private LCGType(uint mc, uint ac, uint rmc, uint rac)
        {
            MultiplecationConst = mc;
            AdditionConst = ac;
            Reverse_MultiplecationConst = rmc;
            Reverse_AdditionConst = rac;
        }

        internal static readonly LCGType StandardLCG = new LCGType(0x41c64e6d, 0x6073, 0xEEB9EB65, 0xA3561A1);
        internal static readonly LCGType GCLCG = new LCGType(0x343FD, 0x269EC3, 0xB9B33155, 0xA170F641);
        internal static readonly LCGType StaticLCG = new LCGType(0x41C64E6D, 0x3039, 0xEEB9EB65, 0xFC77A683);
        internal static readonly LCGType ReverseStdLCG = new LCGType(0xEEB9EB65, 0xA3561A1, 0x41c64e6d, 0x6073);
    }
}
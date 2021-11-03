using System;
using System.Collections.Generic;
using System.Text;

namespace PokemonPRNG.LCG32
{
    public static class LCG32Params
    {
        public static (uint, uint, uint, uint) StandardLCG = (0x41c64e6d, 0x6073, 0xEEB9EB65, 0xA3561A1);
        public static (uint, uint, uint, uint) GCLCG = (0x343FD, 0x269EC3, 0xB9B33155, 0xA170F641);
        public static (uint, uint, uint, uint) StaticLCG = (0x41C64E6D, 0x3039, 0xEEB9EB65, 0xFC77A683);
        public static (uint, uint, uint, uint) Gen4DailyLCG = (0x6C078965, 1, 0x9638806D, 0x69C77F93);
    }
}

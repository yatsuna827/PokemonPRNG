﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonPRNG.LCG32.StaticLCG
{
    public static class Enumerator
    {
        /// <summary>
        /// 無限にseedを返し続けます. SkipやTakeと組み合わせてください.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static IEnumerable<uint> EnumerateSeed(this uint seed)
        {
            yield return seed;
            while (true) yield return seed.Advance();
        }

        /// <summary>
        /// 無限にseedを返し続けます. SkipやTakeと組み合わせてください.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static IEnumerable<uint> EnumerateSeed(this IEnumerator<uint> e)
        {
            do { yield return e.Current; } while (e.MoveNext());
        }

        /// <summary>
        /// 無限に乱数値を返し続けます. SkipやTakeと組み合わせてください.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static IEnumerable<uint> EnumerateRand(this uint seed)
        {
            while (true) yield return seed.GetRand();
        }

    }
}
// Copyright 2007-2008 Rory Plaire (codekaizen@gmail.com)

// Adapted from:

/* C# Version Copyright (C) 2001-2004 Akihilo Kramot (Takel).  */
/* C# porting from a C-program for MT19937, originaly coded by */
/* Takuji Nishimura and Makoto Matsumoto, considering the suggestions by */
/* Topher Cooper and Marc Rieffel in July-Aug. 1997.           */
/* This library is free software under the Artistic license:   */
/*                                                             */
/* You can find the original C-program at                      */
/*     http://www.math.keio.ac.jp/~matumoto/mt.html            */
/*                                                             */

// and:

/////////////////////////////////////////////////////////////////////////////
// C# Version Copyright (c) 2003 CenterSpace Software, LLC                 //
//                                                                         //
// This code is free software under the Artistic license.                  //
//                                                                         //
// CenterSpace Software                                                    //
// 2098 NW Myrtlewood Way                                                  //
// Corvallis, Oregon, 97330                                                //
// USA                                                                     //
// http://www.centerspace.net                                              //
/////////////////////////////////////////////////////////////////////////////

// and, of course:
/* 
   A C-program for MT19937, with initialization improved 2002/2/10.
   Coded by Takuji Nishimura and Makoto Matsumoto.
   This is a faster version by taking Shawn Cokus's optimization,
   Matthe Bellew's simplification, Isaku Wada's real version.

   Before using, initialize the state by using init_genrand(seed) 
   or init_by_array(init_key, key_length).

   Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
   All rights reserved.                          

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:

     1. Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.

     2. Redistributions in binary form must reproduce the above copyright
        notice, this list of conditions and the following disclaimer in the
        documentation and/or other materials provided with the distribution.

     3. The names of its contributors may not be used to endorse or promote 
        products derived from this software without specific prior written 
        permission.

   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
   CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.


   Any feedback is very welcome.
   http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
   email: m-mat @ math.sci.hiroshima-u.ac.jp (remove space)
*/

using System.Linq;
namespace PokemonPRNG.MT
{
    /// <summary>
    ///     Generates pseudo-random numbers using the Mersenne Twister algorithm.
    /// </summary>
    /// <remarks>
    ///     See
    ///     <a href="http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html">
    ///         http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
    ///     </a>
    ///     for details
    ///     on the algorithm.
    /// </remarks>
    public class MT
    {
        private const int N = 624;
        private const int M = 397;
        private const uint MATRIX_A = 0x9908b0df;   /* constant vector a */
        private const uint UPPER_MASK = 0x80000000; /* most significant w-r bits */
        private const uint LOWER_MASK = 0x7fffffff; /* least significant r bits */

        private readonly uint[] stateVector; /* the array for the state vector  */
        private int randIndex; /* mti==N+1 means mt[N] is not initialized */

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
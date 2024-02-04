using System;

namespace PokemonPRNG.LCG32
{
    // アルゴリズム参考
    // https://oupo.hatenadiary.com/entry/20100112/1263306151
    // https://oupo.hatenadiary.com/entry/20150128/1422413218
    public class LCG32
    {
        private readonly uint mulConst, addConst, mulInvConst, addInvConst;
        private readonly uint[,] mulConstCache, addConstCache, mulInvConstCache, addInvConstCache;

        public LCG32(uint mulConst, uint addConst, uint invMulConst, uint invAddConst)
        {
            this.mulConst = mulConst;
            this.addConst = addConst;
            this.mulInvConst = invMulConst;
            this.addInvConst = invAddConst;

            var At = new uint[32]; 
            var Bt = new uint[32]; 
            var Ct = new uint[32]; 
            var Dt = new uint[32];
            At[0] = mulConst;
            Bt[0] = addConst;
            Ct[0] = mulInvConst;
            Dt[0] = addInvConst;
            for (int i = 1; i < 32; i++)
            {
                At[i] = At[i - 1] * At[i - 1];
                Bt[i] = Bt[i - 1] * (1 + At[i - 1]);
                Ct[i] = Ct[i - 1] * Ct[i - 1];
                Dt[i] = Dt[i - 1] * (1 + Ct[i - 1]);
            }

            mulConstCache = new uint[4, 256]; 
            addConstCache = new uint[4, 256]; 
            mulInvConstCache = new uint[4, 256]; 
            addInvConstCache = new uint[4, 256];
            for (int i = 0; i < 4; i++)
            {
                var t = i << 3;
                mulConstCache[i, 0] = 1;
                addConstCache[i, 0] = 0;
                mulInvConstCache[i, 0] = 1;
                addInvConstCache[i, 0] = 0;

                mulConstCache[i, 1] = At[t];
                addConstCache[i, 1] = Bt[t];
                mulInvConstCache[i, 1] = Ct[t];
                addInvConstCache[i, 1] = Dt[t];
                for (int k = 2; k < 256; k++)
                {
                    mulConstCache[i, k] = mulConstCache[i, k - 1] * At[t];
                    addConstCache[i, k] = addConstCache[i, k - 1] * At[t] + Bt[t];
                    mulInvConstCache[i, k] = mulInvConstCache[i, k - 1] * Ct[t];
                    addInvConstCache[i, k] = addInvConstCache[i, k - 1] * Ct[t] + Dt[t];
                }
            }
        }

        public uint NextSeed(uint seed) => seed * mulConst + addConst;
        public uint NextSeed(uint seed, uint n)
        {
            for (int i = 0; i < 4; i++) seed = seed * mulConstCache[i, (n >> (i * 8)) & 0xff] + addConstCache[i, (n >> (i * 8)) & 0xff];
            return seed;
        }
        public uint PrevSeed(uint seed) => seed * mulInvConst + addInvConst;
        public uint PrevSeed(uint seed, uint n)
        {
            for (int i = 0; i < 4; i++) seed = seed * mulInvConstCache[i, (n >> (i * 8)) & 0xff] + addInvConstCache[i, (n >> (i * 8)) & 0xff];
            return seed;
        }

        /// <summary>
        /// seedをn進める関数を生成します.
        /// 消費数固定のAdvanceを大量に呼び出す場合はこちらを使う方が速くなります.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public Func<uint,uint> GenerateAdvanceFunction(uint n)
        {
            uint a = 1, b = 0;
            for (int i = 0; i < 4; i++)
            {
                b += a * addConstCache[i, (n >> (i * 8)) & 0xff];
                a *= mulConstCache[i, (n >> (i * 8)) & 0xff];
            }
            return _ => a * _ + b;
        }

        /// <summary>
        /// seedをn戻す関数を生成します.
        /// 消費数固定のAdvanceを大量に呼び出す場合はこちらを使う方が速くなります.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public Func<uint, uint> GenerateBackFunction(uint n)
        {
            uint a = 1, b = 0;
            for (int i = 0; i < 4; i++)
            {
                b += a * addInvConstCache[i, (n >> (i * 8)) & 0xff];
                a *= mulInvConstCache[i, (n >> (i * 8)) & 0xff];
            }
            return _ => a * _ + b;
        }

        public uint CalcIndex(uint seed)
        {
            var a = mulConst;
            var b = addConst;

            var result = 0x0u;

            seed = seed * a + b;
            for (uint i = 0, mask = 1; i < 32; i++, mask <<= 1)
            {
                if ((seed & mask) == 0)
                    result |= mask;
                else
                    seed = seed * a + b;

                // f^iを二乗する
                (a, b) = (a * a, (a + 1) * b);
            }

            return result;
        }

        public uint CalcIndex(uint seed, uint initialSeed)
        {
            var a = mulConst;
            var b = addConst;

            var result = 0x0u;

            seed = seed * a + b;
            for (uint i = 0, mask = 1; i < 32; i++, mask <<= 1)
            {
                if (((seed ^ initialSeed) & mask) == 0)
                    result |= mask;
                else
                    seed = seed * a + b;

                // f^iを二乗する
                (a, b) = (a * a, (a + 1) * b);
            }

            return result;
        }
    }

}
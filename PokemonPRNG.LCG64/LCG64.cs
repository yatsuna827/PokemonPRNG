namespace PokemonPRNG.LCG64
{
    public static class LCG64Extension
    {
        private const ulong MulConst = 0x5D588B656C078965u; // 乗法定数
        private const ulong AddConst = 0x269EC3u; // 加法定数
        private const ulong MulInvConst = 0xDEDCEDAE9638806Dul;
        private const ulong AddInvConst = 0x9B1AE6E9A384E6F9ul;

        private static readonly ulong[] At, Bt, Ct, Dt;
        static LCG64Extension()
        {
            At = new ulong[64]; 
            Bt = new ulong[64];
            At[0] = MulConst;
            Bt[0] = AddConst;
            for (int i = 1; i < 64; i++)
            {
                At[i] = At[i - 1] * At[i - 1];
                Bt[i] = Bt[i - 1] * (1 + At[i - 1]);
            }

            Ct = new ulong[64]; 
            Dt = new ulong[64];
            Ct[0] = MulInvConst;
            Dt[0] = AddInvConst;
            for (int i = 1; i < 64; i++)
            {
                Ct[i] = Ct[i - 1] * Ct[i - 1];
                Dt[i] = Dt[i - 1] * (1 + Ct[i - 1]);
            }
        }

        /// <summary>
        /// 次のseedを取得します. 渡したseedは変更されません.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static ulong NextSeed(this ulong seed) => seed * MulConst + AddConst;

        /// <summary>
        /// n先のseedを取得します. 渡したseedは変更されません.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static ulong NextSeed(this ulong seed, ulong n)
        {
            for (int i = 0; n != 0; i++, n >>= 1)
                if ((n & 1) != 0) seed = seed * At[i] + Bt[i];

            return seed;
        }

        /// <summary>
        /// 前のseedを取得します. 渡したseedは変更されません.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static ulong PrevSeed(this ulong seed) => seed * MulInvConst + AddInvConst;

        /// <summary>
        /// n前のseedを取得します. 渡したseedは変更されません.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static ulong PrevSeed(this ulong seed, uint n)
        {
            for (int i = 0; n != 0; i++, n >>= 1)
                if ((n & 1) != 0) seed = seed * Ct[i] + Dt[i];

            return seed;
        }


        /// <summary>
        /// seedを1つ進めます.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns>更新後のseed</returns>
        public static ulong Advance(ref this ulong seed) => seed = seed * MulConst + AddConst;

        /// <summary>
        /// seedをn進めます.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="n"></param>
        /// <returns>更新後のseed</returns>
        public static ulong Advance(ref this ulong seed, ulong n)
        {
            for (int i = 0; n != 0; i++, n >>= 1)
                if ((n & 1) != 0) seed = seed * At[i] + Bt[i];

            return seed;
        }


        /// <summary>
        /// seedを1つ戻します. 
        /// </summary>
        /// <param name="seed"></param>
        /// <returns>更新後のseed</returns>
        public static ulong Back(ref this ulong seed) => seed = seed * MulInvConst + AddInvConst;

        /// <summary>
        /// seedをn戻します.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="n"></param>
        /// <returns>更新後のseed</returns>
        public static ulong Back(ref this ulong seed, uint n)
        {
            for (int i = 0; n != 0; i++, n >>= 1)
                if ((n & 1) != 0) seed = seed * Ct[i] + Dt[i];

            return seed;
        }


        /// <summary>
        /// seedから16bitの乱数値を取得します. 渡したseedは更新されます.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns>16bit乱数値</returns>
        public static ulong GetRand(ref this ulong seed) => (seed = seed * MulConst + AddConst) >> 32;

        /// <summary>
        /// 指定したseedの0x0からの消費数を取得します.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static ulong GetIndex(this ulong seed) => CalcIndex(seed, MulConst, AddConst, 64);

        /// <summary>
        /// 指定したseedの指定した初期seedから消費数を取得します.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="InitialSeed"></param>
        /// <returns></returns>
        public static ulong GetIndex(this ulong seed, ulong InitialSeed) => GetIndex(seed) - GetIndex(InitialSeed);

        private static ulong CalcIndex(ulong seed, ulong A, ulong B, uint order)
        {
            if (order == 0) return 0;
            else if ((seed & 1) == 0) return CalcIndex(seed / 2, A * A, (A + 1) * B / 2, order - 1) * 2;
            else return CalcIndex((A * seed + B) / 2, A * A, (A + 1) * B / 2, order - 1) * 2 - 1;
        }
    }
}

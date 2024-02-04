namespace PokemonPRNG.LCG32.StandardLCG
{
    public static class StandardLCGExtension
    {
        private static readonly LCG32 lcg = new LCG32(0x41c64e6d, 0x6073, 0xEEB9EB65, 0xA3561A1);
        /// <summary>
        /// 初期seedと消費数を指定してseedを取得します.
        /// </summary>
        /// <param name="initialSeed">初期seed</param>
        /// <param name="n">消費数</param>
        /// <returns></returns>
        public static uint GetSeed(this uint initialSeed, uint n) => lcg.NextSeed(initialSeed, n);

        /// <summary>
        /// 次のseedを取得します. 渡したseedは変更されません.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static uint NextSeed(this uint seed) => lcg.NextSeed(seed);

        /// <summary>
        /// n先のseedを取得します. 渡したseedは変更されません.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static uint NextSeed(this uint seed, uint n) => lcg.NextSeed(seed, n);

        /// <summary>
        /// 前のseedを取得します. 渡したseedは変更されません.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static uint PrevSeed(this uint seed) => lcg.PrevSeed(seed);

        /// <summary>
        /// n前のseedを取得します. 渡したseedは変更されません.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static uint PrevSeed(this uint seed, uint n) => lcg.PrevSeed(seed, n);

        /// <summary>
        /// seedを1つ進めます.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns>更新後のseed</returns>
        public static uint Advance(ref this uint seed) => seed = lcg.NextSeed(seed);

        /// <summary>
        /// seedをn進めます.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="n"></param>
        /// <returns>更新後のseed</returns>
        public static uint Advance(ref this uint seed, uint n) => seed = lcg.NextSeed(seed, n);

        /// <summary>
        /// seedを1つ戻します. 
        /// </summary>
        /// <param name="seed"></param>
        /// <returns>更新後のseed</returns>
        public static uint Back(ref this uint seed) => seed = lcg.PrevSeed(seed);

        /// <summary>
        /// seedをn戻します.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="n"></param>
        /// <returns>更新後のseed</returns>
        public static uint Back(ref this uint seed, uint n) => seed = lcg.PrevSeed(seed, n);

        /// <summary>
        /// seedから16bitの乱数値を取得します. 渡したseedは更新されます.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns>16bit乱数値</returns>
        public static uint GetRand(ref this uint seed) => seed.Advance() >> 16;

        /// <summary>
        /// seedから0 ~ (m-1)の乱数を取得します.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="modulo"></param>
        /// <returns>o~(module-1)の値</returns>
        public static uint GetRand(ref this uint seed, uint m) => (seed.Advance() >> 16) % m;

        /// <summary>
        /// 指定したseedの0x0からの消費数を取得します.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static uint GetIndex(this uint seed) => lcg.CalcIndex(seed);

        /// <summary>
        /// 指定したseedの指定した初期seedから消費数を取得します.
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="InitialSeed"></param>
        /// <returns></returns>
        public static uint GetIndex(this uint seed, uint initialSeed) => lcg.CalcIndex(seed, initialSeed);

    }
}

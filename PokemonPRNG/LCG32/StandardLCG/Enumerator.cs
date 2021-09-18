using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonPRNG.LCG32.StandardLCG
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
        /// 無限にseedと消費数のTupleを返し続けます. SkipやTakeと組み合わせてください.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static IEnumerable<(int index, uint seed)> EnumerateSeedWithIndex(this uint seed)
        {
            int i = 0;
            yield return (i++, seed);
            while (true) yield return (i++, seed.Advance());
        }

        /// <summary>
        /// 無限にseedと経過フレームのTupleを返し続けます. SkipやTakeと組み合わせてください.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static IEnumerable<(int Index, uint seed)> EnumerateSeedWithIndex(this IEnumerator<uint> e)
        {
            int i = 0;
            do { yield return (i++, e.Current); } while (e.MoveNext());
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

        /// <summary>
        /// seedEnumeratorから受け取ったseedから生成処理を行い、得られるTResultを返し続けます.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="seed"></param>
        /// <param name="igenerator"></param>
        /// <returns></returns>
        public static IEnumerable<(int index, uint seed, TResult result)> EnumerateGeneration<TResult>(this IEnumerable<uint> seedEnumerator, IGeneratable<TResult> igenerator)
        {
            int idx = 0;
            foreach (var seed in seedEnumerator) yield return (idx++, seed, igenerator.Generate(seed));
        }
        /// <summary>
        /// seedEnumeratorから受け取ったseedから生成処理を行い、得られるTResultを返し続けます.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="seed"></param>
        /// <param name="igenerator"></param>
        /// <returns></returns>
        public static IEnumerable<(int index, uint seed, TResult result)> EnumerateGeneration<TResult, TArg1>
            (this IEnumerable<uint> seedEnumerator, IGeneratable<TResult, TArg1> igenerator, TArg1 arg1)
        {
            int idx = 0;
            foreach (var seed in seedEnumerator) yield return (idx++, seed, igenerator.Generate(seed, arg1));
        }
        /// <summary>
        /// seedEnumeratorから受け取ったseedから生成処理を行い、得られるTResultを返し続けます.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="seed"></param>
        /// <param name="igenerator"></param>
        /// <returns></returns>
        public static IEnumerable<(int index, uint seed, TResult result)> EnumerateGeneration<TResult, TArg1, TArg2>
            (this IEnumerable<uint> seedEnumerator, IGeneratable<TResult, TArg1, TArg2> igenerator, TArg1 arg1, TArg2 arg2)
        {
            int idx = 0;
            foreach (var seed in seedEnumerator) yield return (idx++, seed, igenerator.Generate(seed, arg1, arg2));
        }
        /// <summary>
        /// seedEnumeratorから受け取ったseedから生成処理を行い、得られるTResultを返し続けます.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="seed"></param>
        /// <param name="igenerator"></param>
        /// <returns></returns>
        public static IEnumerable<(int index, uint seed, TResult result)> EnumerateGeneration<TResult, TArg1, TArg2, TArg3>
            (this IEnumerable<uint> seedEnumerator, IGeneratable<TResult, TArg1, TArg2, TArg3> igenerator, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            int idx = 0;
            foreach (var seed in seedEnumerator) yield return (idx++, seed, igenerator.Generate(seed, arg1, arg2, arg3));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonPRNG.LCG64
{
    public static class Enumerator
    {
        /// <summary>
        /// 無限にseedを返し続けます. SkipやTakeと組み合わせてください.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> EnumerateSeed(this ulong seed)
        {
            yield return seed;
            while (true) yield return seed.Advance();
        }

        /// <summary>
        /// 無限にseedを返し続けます. SkipやTakeと組み合わせてください.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> EnumerateSeed(this IEnumerator<ulong> e)
        {
            do { yield return e.Current; } while (e.MoveNext());
        }

        /// <summary>
        /// 無限に乱数値を返し続けます. SkipやTakeと組み合わせてください.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> EnumerateRand(this ulong seed)
        {
            while (true) yield return seed.GetRand();
        }

        /// <summary>
        /// 0-indexedな添字とのタプルを返し続けます.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static IEnumerable<(int Index, T Element)> WithIndex<T>(this IEnumerable<T> enumerator, int offset = 0)
            => enumerator.Select((_, i) => (i + offset, _));

        /// <summary>
        /// seedEnumeratorから受け取ったseedから生成処理を行い、得られるTResultを返し続けます.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="seed"></param>
        /// <param name="igenerator"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> EnumerateGeneration<TResult>
            (this IEnumerable<ulong> seedEnumerator, IGeneratable<TResult> igenerator)
            => seedEnumerator.Select(igenerator.Generate);

        /// <summary>
        /// seedEnumeratorから受け取ったseedから生成処理を行い、得られるTResultを返し続けます.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="seed"></param>
        /// <param name="igenerator"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> EnumerateGeneration<TResult, TArg1>
            (this IEnumerable<ulong> seedEnumerator, IGeneratable<TResult, TArg1> igenerator, TArg1 arg1)
            => seedEnumerator.Select(_ => igenerator.Generate(_, arg1));

        /// <summary>
        /// seedEnumeratorから受け取ったseedから生成処理を行い、得られるTResultを返し続けます.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="seed"></param>
        /// <param name="igenerator"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> EnumerateGeneration<TResult, TArg1, TArg2>
            (this IEnumerable<ulong> seedEnumerator, IGeneratable<TResult, TArg1, TArg2> igenerator, TArg1 arg1, TArg2 arg2)
            => seedEnumerator.Select(_ => igenerator.Generate(_, arg1, arg2));

        /// <summary>
        /// seedEnumeratorから受け取ったseedから生成処理を行い、得られるTResultを返し続けます.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="seed"></param>
        /// <param name="igenerator"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> EnumerateGeneration<TResult, TArg1, TArg2, TArg3>
            (this IEnumerable<ulong> seedEnumerator, IGeneratable<TResult, TArg1, TArg2, TArg3> igenerator, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => seedEnumerator.Select(_ => igenerator.Generate(_, arg1, arg2, arg3));
    }
}

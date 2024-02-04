using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonPRNG.LCG32
{
    public static class CommonEnumerator
    {
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
            (this IEnumerable<uint> seedEnumerator, IGeneratable<TResult> igenerator)
            => seedEnumerator.Select(igenerator.Generate);

        /// <summary>
        /// seedEnumeratorから受け取ったseedから生成処理を行い、得られるTResultを返し続けます.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="seed"></param>
        /// <param name="igenerator"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> EnumerateGeneration<TResult, TArg1>
            (this IEnumerable<uint> seedEnumerator, IGeneratable<TResult, TArg1> igenerator, TArg1 arg1)
            => seedEnumerator.Select(_ => igenerator.Generate(_, arg1));

        /// <summary>
        /// seedEnumeratorから受け取ったseedから生成処理を行い、得られるTResultを返し続けます.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="seed"></param>
        /// <param name="igenerator"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> EnumerateGeneration<TResult, TArg1, TArg2>
            (this IEnumerable<uint> seedEnumerator, IGeneratable<TResult, TArg1, TArg2> igenerator, TArg1 arg1, TArg2 arg2)
            => seedEnumerator.Select(_ => igenerator.Generate(_, arg1, arg2));

        /// <summary>
        /// seedEnumeratorから受け取ったseedから生成処理を行い、得られるTResultを返し続けます.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="seed"></param>
        /// <param name="igenerator"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> EnumerateGeneration<TResult, TArg1, TArg2, TArg3>
            (this IEnumerable<uint> seedEnumerator, IGeneratable<TResult, TArg1, TArg2, TArg3> igenerator, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => seedEnumerator.Select(_ => igenerator.Generate(_, arg1, arg2, arg3));
    }
}

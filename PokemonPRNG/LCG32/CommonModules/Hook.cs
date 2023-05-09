using System;
using System.Collections.Generic;
using System.Text;

namespace PokemonPRNG.LCG32
{
    class HookGenerator<TResult, TOption> : IGeneratable<RNGResult<TResult, TOption>>
    {
        private readonly IGeneratable<RNGResult<TResult>> _generator;
        private readonly IGeneratableEffectful<TOption> _option;

        public RNGResult<TResult, TOption> Generate(uint seed)
        {
            var res = _generator.Generate(seed);
            seed = res.TailSeed;
            var opt = _option.Generate(ref seed);

            return new RNGResult<TResult, TOption>(res.Content, opt, res.HeadSeed, seed);
        }

        public HookGenerator(IGeneratable<RNGResult<TResult>> generator, IGeneratableEffectful<TOption> option)
            => (_generator, _option) = (generator, option);
    }

    public static class HookExtensions
    {
        public static IGeneratable<RNGResult<T, O>> Hook<T, O>(this IGeneratable<RNGResult<T>> generator, IGeneratableEffectful<O> opt)
            => new HookGenerator<T, O>(generator, opt);
    }
}

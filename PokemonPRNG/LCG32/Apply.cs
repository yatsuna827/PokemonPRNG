using System;
using System.Collections.Generic;
using System.Text;

namespace PokemonPRNG.LCG32
{
    public static class ApplyExtension
    {
        public static IGeneratable<TResult> Apply<TResult, TArg1>(this IGeneratable<TResult, TArg1> generatable, TArg1 arg1)
            => new AppliedGeneratable<TResult, TArg1>(generatable, arg1);
        public static IGeneratable<TResult> Apply<TResult, TArg1, TArg2>(this IGeneratable<TResult, TArg1, TArg2> generatable, TArg1 arg1, TArg2 arg2)
            => new AppliedGeneratable<TResult, TArg1, TArg2>(generatable, arg1, arg2);
        public static IGeneratable<TResult> Apply<TResult, TArg1, TArg2, Targ3>(this IGeneratable<TResult, TArg1, TArg2, Targ3> generatable, TArg1 arg1, TArg2 arg2, Targ3 arg3)
            => new AppliedGeneratable<TResult, TArg1, TArg2, Targ3>(generatable, arg1, arg2, arg3);

        public static ISideEffectiveGeneratable<TResult> Apply<TResult, TArg1>(this ISideEffectiveGeneratable<TResult, TArg1> generatable, TArg1 arg1)
            => new AppliedRefGeneratable<TResult, TArg1>(generatable, arg1);
        public static ISideEffectiveGeneratable<TResult> Apply<TResult, TArg1, TArg2>(this ISideEffectiveGeneratable<TResult, TArg1, TArg2> generatable, TArg1 arg1, TArg2 arg2)
            => new AppliedRefGeneratable<TResult, TArg1, TArg2>(generatable, arg1, arg2);
        public static ISideEffectiveGeneratable<TResult> Apply<TResult, TArg1, TArg2, Targ3>(this ISideEffectiveGeneratable<TResult, TArg1, TArg2, Targ3> generatable, TArg1 arg1, TArg2 arg2, Targ3 arg3)
            => new AppliedRefGeneratable<TResult, TArg1, TArg2, Targ3>(generatable, arg1, arg2, arg3);
    }


    class AppliedGeneratable<TResult, TArg1> : IGeneratable<TResult>
    {
        private readonly TArg1 arg;
        private readonly IGeneratable<TResult, TArg1> generatable;
        public TResult Generate(uint seed) => generatable.Generate(seed, arg);
        public AppliedGeneratable(IGeneratable<TResult, TArg1> generatable, TArg1 arg)
            => (this.generatable, this.arg) = (generatable, arg);
    }
    class AppliedGeneratable<TResult, TArg1, TArg2> : IGeneratable<TResult>
    {
        private readonly TArg1 arg1;
        private readonly TArg2 arg2;
        private readonly IGeneratable<TResult, TArg1, TArg2> generatable;
        public TResult Generate(uint seed) => generatable.Generate(seed, arg1, arg2);
        public AppliedGeneratable(IGeneratable<TResult, TArg1, TArg2> generatable, TArg1 arg1, TArg2 arg2)
            => (this.generatable, this.arg1, this.arg2) = (generatable, arg1, arg2);
    }
    class AppliedGeneratable<TResult, TArg1, TArg2, TArg3> : IGeneratable<TResult>
    {
        private readonly TArg1 arg1;
        private readonly TArg2 arg2;
        private readonly TArg3 arg3;
        private readonly IGeneratable<TResult, TArg1, TArg2, TArg3> generatable;
        public TResult Generate(uint seed) => generatable.Generate(seed, arg1, arg2, arg3);
        public AppliedGeneratable(IGeneratable<TResult, TArg1, TArg2, TArg3> generatable, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => (this.generatable, this.arg1, this.arg2, this.arg3) = (generatable, arg1, arg2, arg3);
    }

    class AppliedRefGeneratable<TResult, TArg1> : ISideEffectiveGeneratable<TResult>
    {
        private readonly TArg1 arg;
        private readonly ISideEffectiveGeneratable<TResult, TArg1> generatable;
        public TResult Generate(ref uint seed) => generatable.Generate(ref seed, arg);
        public AppliedRefGeneratable(ISideEffectiveGeneratable<TResult, TArg1> generatable, TArg1 arg)
            => (this.generatable, this.arg) = (generatable, arg);
    }
    class AppliedRefGeneratable<TResult, TArg1, TArg2> : ISideEffectiveGeneratable<TResult>
    {
        private readonly TArg1 arg1;
        private readonly TArg2 arg2;
        private readonly ISideEffectiveGeneratable<TResult, TArg1, TArg2> generatable;
        public TResult Generate(ref uint seed) => generatable.Generate(ref seed, arg1, arg2);
        public AppliedRefGeneratable(ISideEffectiveGeneratable<TResult, TArg1, TArg2> generatable, TArg1 arg1, TArg2 arg2)
            => (this.generatable, this.arg1, this.arg2) = (generatable, arg1, arg2);
    }
    class AppliedRefGeneratable<TResult, TArg1, TArg2, TArg3> : ISideEffectiveGeneratable<TResult>
    {
        private readonly TArg1 arg1;
        private readonly TArg2 arg2;
        private readonly TArg3 arg3;
        private readonly ISideEffectiveGeneratable<TResult, TArg1, TArg2, TArg3> generatable;
        public TResult Generate(ref uint seed) => generatable.Generate(ref seed, arg1, arg2, arg3);
        public AppliedRefGeneratable(ISideEffectiveGeneratable<TResult, TArg1, TArg2, TArg3> generatable, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => (this.generatable, this.arg1, this.arg2, this.arg3) = (generatable, arg1, arg2, arg3);
    }
}

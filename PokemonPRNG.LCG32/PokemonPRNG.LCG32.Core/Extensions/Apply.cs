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

        public static IGeneratableEffectful<TResult> Apply<TResult, TArg1>(this IGeneratableEffectful<TResult, TArg1> generatable, TArg1 arg1)
            => new AppliedRefGeneratable<TResult, TArg1>(generatable, arg1);
        public static IGeneratableEffectful<TResult> Apply<TResult, TArg1, TArg2>(this IGeneratableEffectful<TResult, TArg1, TArg2> generatable, TArg1 arg1, TArg2 arg2)
            => new AppliedRefGeneratable<TResult, TArg1, TArg2>(generatable, arg1, arg2);
        public static IGeneratableEffectful<TResult> Apply<TResult, TArg1, TArg2, Targ3>(this IGeneratableEffectful<TResult, TArg1, TArg2, Targ3> generatable, TArg1 arg1, TArg2 arg2, Targ3 arg3)
            => new AppliedRefGeneratable<TResult, TArg1, TArg2, Targ3>(generatable, arg1, arg2, arg3);

        public static ILcgUser Apply<TArg1>(this ILcgUser<TArg1> user, TArg1 arg1)
            => new AppliedUser<TArg1>(user, arg1);
        public static ILcgUser Apply<TArg1, TArg2>(this ILcgUser<TArg1, TArg2> user, TArg1 arg1, TArg2 arg2)
            => new AppliedUser<TArg1, TArg2>(user, arg1, arg2);
        public static ILcgUser Apply<TArg1, TArg2, Targ3>(this ILcgUser<TArg1, TArg2, Targ3> user, TArg1 arg1, TArg2 arg2, Targ3 arg3)
            => new AppliedUser<TArg1, TArg2, Targ3>(user, arg1, arg2, arg3);

        public static ILcgConsumer Apply<TArg1>(this ILcgConsumer<TArg1> consumer, TArg1 arg1)
            => new AppliedConsumer<TArg1>(consumer, arg1);
        public static ILcgConsumer Apply<TResult, TArg1, TArg2>(this ILcgConsumer<TArg1, TArg2> consumer, TArg1 arg1, TArg2 arg2)
            => new AppliedConsumer<TArg1, TArg2>(consumer, arg1, arg2);
        public static ILcgConsumer Apply<TResult, TArg1, TArg2, Targ3>(this ILcgConsumer<TArg1, TArg2, Targ3> consumer, TArg1 arg1, TArg2 arg2, Targ3 arg3)
            => new AppliedConsumer<TArg1, TArg2, Targ3>(consumer, arg1, arg2, arg3);
    }


    class AppliedGeneratable<TResult, TArg1> : IGeneratable<TResult>
    {
        private readonly TArg1 _arg;
        private readonly IGeneratable<TResult, TArg1> _generatable;
        public TResult Generate(uint seed) => _generatable.Generate(seed, _arg);
        public AppliedGeneratable(IGeneratable<TResult, TArg1> generatable, TArg1 arg)
            => (_generatable, _arg) = (generatable, arg);
    }
    class AppliedGeneratable<TResult, TArg1, TArg2> : IGeneratable<TResult>
    {
        private readonly TArg1 _arg1;
        private readonly TArg2 _arg2;
        private readonly IGeneratable<TResult, TArg1, TArg2> _generatable;
        public TResult Generate(uint seed) => _generatable.Generate(seed, _arg1, _arg2);
        public AppliedGeneratable(IGeneratable<TResult, TArg1, TArg2> generatable, TArg1 arg1, TArg2 arg2)
            => (_generatable, _arg1, _arg2) = (generatable, arg1, arg2);
    }
    class AppliedGeneratable<TResult, TArg1, TArg2, TArg3> : IGeneratable<TResult>
    {
        private readonly TArg1 _arg1;
        private readonly TArg2 _arg2;
        private readonly TArg3 _arg3;
        private readonly IGeneratable<TResult, TArg1, TArg2, TArg3> _generatable;
        public TResult Generate(uint seed) => _generatable.Generate(seed, _arg1, _arg2, _arg3);
        public AppliedGeneratable(IGeneratable<TResult, TArg1, TArg2, TArg3> generatable, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => (_generatable, _arg1, _arg2, _arg3) = (generatable, arg1, arg2, arg3);
    }

    class AppliedRefGeneratable<TResult, TArg1> : IGeneratableEffectful<TResult>
    {
        private readonly TArg1 _arg;
        private readonly IGeneratableEffectful<TResult, TArg1> _generatable;
        public TResult Generate(ref uint seed) => _generatable.Generate(ref seed, _arg);
        public AppliedRefGeneratable(IGeneratableEffectful<TResult, TArg1> generatable, TArg1 arg)
            => (_generatable, _arg) = (generatable, arg);
    }
    class AppliedRefGeneratable<TResult, TArg1, TArg2> : IGeneratableEffectful<TResult>
    {
        private readonly TArg1 _arg1;
        private readonly TArg2 _arg2;
        private readonly IGeneratableEffectful<TResult, TArg1, TArg2> _generatable;
        public TResult Generate(ref uint seed) => _generatable.Generate(ref seed, _arg1, _arg2);
        public AppliedRefGeneratable(IGeneratableEffectful<TResult, TArg1, TArg2> generatable, TArg1 arg1, TArg2 arg2)
            => (_generatable, _arg1, _arg2) = (generatable, arg1, arg2);
    }
    class AppliedRefGeneratable<TResult, TArg1, TArg2, TArg3> : IGeneratableEffectful<TResult>
    {
        private readonly TArg1 _arg1;
        private readonly TArg2 _arg2;
        private readonly TArg3 _arg3;
        private readonly IGeneratableEffectful<TResult, TArg1, TArg2, TArg3> _generatable;
        public TResult Generate(ref uint seed) => _generatable.Generate(ref seed, _arg1, _arg2, _arg3);
        public AppliedRefGeneratable(IGeneratableEffectful<TResult, TArg1, TArg2, TArg3> generatable, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => (_generatable, _arg1, _arg2, _arg3) = (generatable, arg1, arg2, arg3);
    }

    class AppliedUser<TArg1> : ILcgUser
    {
        private readonly TArg1 _arg;
        private readonly ILcgUser<TArg1> _user;
        public void Use(ref uint seed) => _user.Use(ref seed, _arg);
        public AppliedUser(ILcgUser<TArg1> user, TArg1 arg)
            => (_user, _arg) = (user, arg);
    }
    class AppliedUser<TArg1, TArg2> : ILcgUser
    {
        private readonly TArg1 _arg1;
        private readonly TArg2 _arg2;
        private readonly ILcgUser<TArg1, TArg2> _user;
        public void Use(ref uint seed) => _user.Use(ref seed, _arg1, _arg2);
        public AppliedUser(ILcgUser<TArg1, TArg2> user, TArg1 arg1, TArg2 arg2)
            => (_user, _arg1, _arg2) = (user, arg1, arg2);
    }
    class AppliedUser<TArg1, TArg2, TArg3> : ILcgUser
    {
        private readonly TArg1 _arg1;
        private readonly TArg2 _arg2;
        private readonly TArg3 _arg3;
        private readonly ILcgUser<TArg1, TArg2, TArg3> _user;
        public void Use(ref uint seed) => _user.Use(ref seed, _arg1, _arg2, _arg3);
        public AppliedUser(ILcgUser<TArg1, TArg2, TArg3> user, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => (_user, _arg1, _arg2, _arg3) = (user, arg1, arg2, arg3);
    }

    class AppliedConsumer<TArg1> : ILcgConsumer
    {
        private readonly TArg1 _arg;
        private readonly ILcgConsumer<TArg1> _consumer;
        public void Use(ref uint seed) => _consumer.Use(ref seed, _arg);
        public uint ComputeConsumption(uint seed) => _consumer.ComputeConsumption(seed, _arg);

        public AppliedConsumer(ILcgConsumer<TArg1> consumer, TArg1 arg)
            => (_consumer, _arg) = (consumer, arg);
    }
    class AppliedConsumer<TArg1, TArg2> : ILcgConsumer
    {
        private readonly TArg1 _arg1;
        private readonly TArg2 _arg2;
        private readonly ILcgConsumer<TArg1, TArg2> _consumer;
        public void Use(ref uint seed) => _consumer.Use(ref seed, _arg1, _arg2);
        public uint ComputeConsumption(uint seed) => _consumer.ComputeConsumption(seed, _arg1, _arg2);

        public AppliedConsumer(ILcgConsumer<TArg1, TArg2> consumer, TArg1 arg1, TArg2 arg2)
            => (_consumer, _arg1, _arg2) = (consumer, arg1, arg2);
    }
    class AppliedConsumer<TArg1, TArg2, TArg3> : ILcgConsumer
    {
        private readonly TArg1 _arg1;
        private readonly TArg2 _arg2;
        private readonly TArg3 _arg3;
        private readonly ILcgConsumer<TArg1, TArg2, TArg3> _consumer;
        public void Use(ref uint seed) => _consumer.Use(ref seed, _arg1, _arg2, _arg3);
        public uint ComputeConsumption(uint seed) => _consumer.ComputeConsumption(seed, _arg1, _arg2, _arg3);

        public AppliedConsumer(ILcgConsumer<TArg1, TArg2, TArg3> consumer, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => (_consumer, _arg1, _arg2, _arg3) = (consumer, arg1, arg2, arg3);
    }
}

namespace PokemonPRNG.LCG64
{
    public interface IGeneratable<out TResult>
    {
        TResult Generate(ulong seed);
    }
    public interface IGeneratable<out TResult, in TArg1>
    {
        TResult Generate(ulong seed, TArg1 arg1);
    }
    public interface IGeneratable<out TResult, in TArg1, in TArg2>
    {
        TResult Generate(ulong seed, TArg1 arg1, TArg2 arg2);
    }
    public interface IGeneratable<out TResult, in TArg1, in TArg2, in TArg3>
    {
        TResult Generate(ulong seed, TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }

    public interface IGeneratableEffectful<out TResult>
    {
        TResult Generate(ref ulong seed);
    }
    public interface IGeneratableEffectful<out TResult, in TArg1>
    {
        TResult Generate(ref ulong seed, TArg1 arg1);
    }
    public interface IGeneratableEffectful<out TResult, in TArg1, in TArg2>
    {
        TResult Generate(ref ulong seed, TArg1 arg1, TArg2 arg2);
    }
    public interface IGeneratableEffectful<out TResult, in TArg1, in TArg2, in TArg3>
    {
        TResult Generate(ref ulong seed, TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }

    public static class GeneratorExt
    {
        public static TResult Generate<TResult>(this ulong seed, IGeneratable<TResult> generator)
            => generator.Generate(seed);
        public static TResult Generate<TResult, TArg1>(this ulong seed, IGeneratable<TResult, TArg1> generator, TArg1 arg1)
            => generator.Generate(seed, arg1);
        public static TResult Generate<TResult, TArg1, TArg2>(this ulong seed, IGeneratable<TResult, TArg1, TArg2> generator, TArg1 arg1, TArg2 arg2)
            => generator.Generate(seed, arg1, arg2);
        public static TResult Generate<TResult, TArg1, TArg2, TArg3>(this ulong seed, IGeneratable<TResult, TArg1, TArg2, TArg3> generator, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => generator.Generate(seed, arg1, arg2, arg3);

        public static TResult Generate<TResult>(ref this ulong seed, IGeneratableEffectful<TResult> generator)
            => generator.Generate(ref seed);
        public static TResult Generate<TResult, TArg1>(ref this ulong seed, IGeneratableEffectful<TResult, TArg1> generator, TArg1 arg1)
            => generator.Generate(ref seed, arg1);
        public static TResult Generate<TResult, TArg1, TArg2>(ref this ulong seed, IGeneratableEffectful<TResult, TArg1, TArg2> generator, TArg1 arg1, TArg2 arg2)
            => generator.Generate(ref seed, arg1, arg2);
        public static TResult Generate<TResult, TArg1, TArg2, TArg3>(ref this ulong seed, IGeneratableEffectful<TResult, TArg1, TArg2, TArg3> generator, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => generator.Generate(ref seed, arg1, arg2, arg3);
    }
}

namespace PokemonPRNG.LCG32
{
    public interface IGeneratable<out TResult>
    {
        TResult Generate(uint seed);
    }
    public interface IGeneratable<out TResult, in TArg1>
    {
        TResult Generate(uint seed, TArg1 arg1);
    }
    public interface IGeneratable<out TResult, in TArg1, in TArg2>
    {
        TResult Generate(uint seed, TArg1 arg1, TArg2 arg2);
    }
    public interface IGeneratable<out TResult, in TArg1, in TArg2, in TArg3>
    {
        TResult Generate(uint seed, TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }

    public interface ISideEffectiveGeneratable<out TResult>
    {
        TResult Generate(ref uint seed);
    }
    public interface ISideEffectiveGeneratable<out TResult, in TArg1>
    {
        TResult Generate(ref uint seed, TArg1 arg1);
    }
    public interface ISideEffectiveGeneratable<out TResult, in TArg1, in TArg2>
    {
        TResult Generate(ref uint seed, TArg1 arg1, TArg2 arg2);
    }
    public interface ISideEffectiveGeneratable<out TResult, in TArg1, in TArg2, in TArg3>
    {
        TResult Generate(ref uint seed, TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }

    public static class GeneratorExt
    {
        public static TResult Generate<TResult>(this uint seed, IGeneratable<TResult> generator)
            => generator.Generate(seed);
        public static TResult Generate<TResult, TArg1>(this uint seed, IGeneratable<TResult, TArg1> generator, TArg1 arg1)
            => generator.Generate(seed, arg1);
        public static TResult Generate<TResult, TArg1, TArg2>(this uint seed, IGeneratable<TResult, TArg1, TArg2> generator, TArg1 arg1, TArg2 arg2)
            => generator.Generate(seed, arg1, arg2);
        public static TResult Generate<TResult, TArg1, TArg2, TArg3>(this uint seed, IGeneratable<TResult, TArg1, TArg2, TArg3> generator, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => generator.Generate(seed, arg1, arg2, arg3);

        public static TResult Generate<TResult>(ref this uint seed, ISideEffectiveGeneratable<TResult> generator)
            => generator.Generate(ref seed);
        public static TResult Generate<TResult, TArg1>(ref this uint seed, ISideEffectiveGeneratable<TResult, TArg1> generator, TArg1 arg1)
            => generator.Generate(ref seed, arg1);
        public static TResult Generate<TResult, TArg1, TArg2>(ref this uint seed, ISideEffectiveGeneratable<TResult, TArg1, TArg2> generator, TArg1 arg1, TArg2 arg2)
            => generator.Generate(ref seed, arg1, arg2);
        public static TResult Generate<TResult, TArg1, TArg2, TArg3>(ref this uint seed, ISideEffectiveGeneratable<TResult, TArg1, TArg2, TArg3> generator, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => generator.Generate(ref seed, arg1, arg2, arg3);
    }
}

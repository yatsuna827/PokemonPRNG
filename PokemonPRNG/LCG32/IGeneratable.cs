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

}

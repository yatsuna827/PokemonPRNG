namespace PokemonPRNG.LCG32
{
    public interface IGeneratable<TResult>
    {
        TResult Generate(uint seed);
    }
}

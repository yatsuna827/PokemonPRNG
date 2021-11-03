using System;
using System.Collections.Generic;
using System.Text;

namespace PokemonPRNG.LCG32
{
    public readonly struct RNGResult<T>
    {
        public T Content { get; }
        public uint HeadSeed { get; }
        public uint TailSeed { get; }

        public RNGResult(T content, uint head, uint tail)
            => (Content, HeadSeed, TailSeed) = (content, head, tail);
    }

    public readonly struct RNGResult<TContent, TOption>
    {
        public TContent Content { get; }
        public TOption Option { get; }
        public uint HeadSeed { get; }
        public uint TailSeed { get; }

        public RNGResult(TContent content, TOption option, uint head, uint tail)
            => (Content, Option, HeadSeed, TailSeed) = (content, option, head, tail);
    }
}

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
        public void Deconstruct(out T content, out uint seed) => (seed, content) = (HeadSeed, Content);
        public void Deconstruct(out T content, out uint head, out uint tail) => (content, head, tail) = (Content, HeadSeed, TailSeed);
    }

    public readonly struct RNGResult<TContent, TOption>
    {
        public TContent Content { get; }
        public TOption Option { get; }
        public uint HeadSeed { get; }
        public uint TailSeed { get; }

        public RNGResult(TContent content, TOption option, uint head, uint tail)
            => (Content, Option, HeadSeed, TailSeed) = (content, option, head, tail);
        public void Deconstruct(out TContent content, out TOption option, out uint seed) => (seed, content, option) = (HeadSeed, Content, Option);
        public void Deconstruct(out TContent content, out TOption option, out uint head, out uint tail) => (content, option, head, tail) = (Content, Option, HeadSeed, TailSeed);
    }
}

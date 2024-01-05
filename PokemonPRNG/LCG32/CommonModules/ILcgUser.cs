using System;
using System.Collections.Generic;
using System.Text;

namespace PokemonPRNG.LCG32
{
    public interface ILcgUser
    {
        void Use(ref uint seed);
    }
    public interface ILcgUser<in TArg1>
    {
        void Use(ref uint seed, TArg1 arg1);
    }
    public interface ILcgUser<in TArg1, in TArg2>
    {
        void Use(ref uint seed, TArg1 arg1, TArg2 arg2);
    }
    public interface ILcgUser<in TArg1, in TArg2, in TArg3>
    {
        void Use(ref uint seed, TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }

    public interface ILcgConsumer : ILcgUser
    {
        uint ComputeConsumption(uint seed);
    }
    public interface ILcgConsumer<in TArg1> : ILcgUser<TArg1>
    {
        uint ComputeConsumption(uint seed, TArg1 arg1);
    }
    public interface ILcgConsumer<in TArg1, in TArg2> : ILcgUser<TArg1, TArg2>
    {
        uint ComputeConsumption(uint seed, TArg1 arg1, TArg2 arg2);
    }
    public interface ILcgConsumer<in TArg1, in TArg2, in TArg3> : ILcgUser<TArg1, TArg2, TArg3>
    {
        uint ComputeConsumption(uint seed, TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }

    public static class LcgUserExt
    {
        public static void Used(ref this uint seed, ILcgUser user)
            => user.Use(ref seed);
        public static void Used<TArg1>(ref this uint seed, ILcgUser<TArg1> user, TArg1 arg1)
            => user.Use(ref seed, arg1);
        public static void Used<TArg1, TArg2>(ref this uint seed, ILcgUser<TArg1, TArg2> user, TArg1 arg1, TArg2 arg2)
            => user.Use(ref seed, arg1, arg2);
        public static void Used<TArg1, TArg2, TArg3>(ref this uint seed, ILcgUser<TArg1, TArg2, TArg3> user, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => user.Use(ref seed, arg1, arg2, arg3);

        public static uint NextSeed(this uint seed, ILcgConsumer consumer)
            => consumer.ComputeConsumption(seed);
        public static uint NextSeed<TArg1>(this uint seed, ILcgConsumer<TArg1> consumer, TArg1 arg1)
            => consumer.ComputeConsumption(seed, arg1);
        public static uint NextSeed<TArg1, TArg2>(this uint seed, ILcgConsumer<TArg1, TArg2> consumer, TArg1 arg1, TArg2 arg2)
            => consumer.ComputeConsumption(seed, arg1, arg2);
        public static uint NextSeed<TArg1, TArg2, TArg3>(this uint seed, ILcgConsumer<TArg1, TArg2, TArg3> consumer, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => consumer.ComputeConsumption(seed, arg1, arg2, arg3);

        public static uint Advance(ref this uint seed, ILcgConsumer consumer)
            => (seed = consumer.ComputeConsumption(seed));
        public static uint Advance<TArg1>(ref this uint seed, ILcgConsumer<TArg1> consumer, TArg1 arg1)
            => (seed = consumer.ComputeConsumption(seed, arg1));
        public static uint Advance<TArg1, TArg2>(ref this uint seed, ILcgConsumer<TArg1, TArg2> consumer, TArg1 arg1, TArg2 arg2)
            => (seed = consumer.ComputeConsumption(seed, arg1, arg2));
        public static uint Advance<TArg1, TArg2, TArg3>(ref this uint seed, ILcgConsumer<TArg1, TArg2, TArg3> consumer, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => (seed = consumer.ComputeConsumption(seed, arg1, arg2, arg3));
    }
}

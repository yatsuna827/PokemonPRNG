﻿namespace PokemonPRNG.LCG64
{
    public interface ILcgUser
    {
        void Use(ref ulong seed);
    }
    public interface ILcgUser<in TArg1>
    {
        void Use(ref ulong seed, TArg1 arg1);
    }
    public interface ILcgUser<in TArg1, in TArg2>
    {
        void Use(ref ulong seed, TArg1 arg1, TArg2 arg2);
    }
    public interface ILcgUser<in TArg1, in TArg2, in TArg3>
    {
        void Use(ref ulong seed, TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }

    public interface ILcgUtilizer<out TResult>
    {
        TResult Utilize(ref ulong seed);
    }
    public interface ILcgUtilizer<out TResult, in TArg1>
    {
        TResult Utilize(ref ulong seed, TArg1 arg1);
    }
    public interface ILcgUtilizer<out TResult, in TArg1, in TArg2>
    {
        TResult Utilize(ref ulong seed, TArg1 arg1, TArg2 arg2);
    }
    public interface ILcgUtilizer<out TResult, in TArg1, in TArg2, in TArg3>
    {
        TResult Utilize(ref ulong seed, TArg1 arg1, TArg2 arg2, TArg3 arg3);
    }


    public static class LcgUserExt
    {
        public static void Used(ref this ulong seed, ILcgUser user)
            => user.Use(ref seed);
        public static void Used<TArg1>(ref this ulong seed, ILcgUser<TArg1> user, TArg1 arg1)
            => user.Use(ref seed, arg1);
        public static void Used<TArg1, TArg2>(ref this ulong seed, ILcgUser<TArg1, TArg2> user, TArg1 arg1, TArg2 arg2)
            => user.Use(ref seed, arg1, arg2);
        public static void Used<TArg1, TArg2, TArg3>(ref this ulong seed, ILcgUser<TArg1, TArg2, TArg3> user, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => user.Use(ref seed, arg1, arg2, arg3);

        public static TResult Used<TResult>(ref this ulong seed, ILcgUtilizer<TResult> utilizer)
            => utilizer.Utilize(ref seed);
        public static TResult Used<TResult, TArg1>(ref this ulong seed, ILcgUtilizer<TResult, TArg1> utilizer, TArg1 arg1)
            => utilizer.Utilize(ref seed, arg1);
        public static TResult Used<TResult, TArg1, TArg2>(ref this ulong seed, ILcgUtilizer<TResult, TArg1, TArg2> utilizer, TArg1 arg1, TArg2 arg2)
            => utilizer.Utilize(ref seed, arg1, arg2);
        public static TResult Used<TResult, TArg1, TArg2, TArg3>(ref this ulong seed, ILcgUtilizer<TResult, TArg1, TArg2, TArg3> utilizer, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => utilizer.Utilize(ref seed, arg1, arg2, arg3);
    }
}

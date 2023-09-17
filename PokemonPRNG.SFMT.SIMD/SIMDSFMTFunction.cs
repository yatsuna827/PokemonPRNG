using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace PokemonPRNG.SFMT.SIMD
{
    internal static class SIMDSFMTFunction
    {
        const int N = 156;
        const int POS1 = 122;

        private static readonly Vector128<uint> MASK = Vector128.LoadUnsafe(ref MemoryMarshal.GetReference(new[] { 0xdfffffefU, 0xddfecb7fU, 0xbffaffffU, 0xbffffff6U }.AsSpan()));
        public static void GenerateRandAll_Sse2(in Span<uint> vecSpan, in Span<uint> storeSpan)
        {
            ref var vecSpanRef = ref MemoryMarshal.GetReference(vecSpan);
            var r1 = Vector128.LoadUnsafe(ref vecSpanRef, 4 * (N - 2));
            var r2 = Vector128.LoadUnsafe(ref vecSpanRef, 4 * (N - 1));

            ref var storeSpanRef = ref MemoryMarshal.GetReference(storeSpan);

            ref var itr = ref MemoryMarshal.GetReference(storeSpan);
            nuint i = 0;
            for (; i < N - POS1; i++, itr = ref Unsafe.AddByteOffset(ref itr, 16))
            {
                var a = Vector128.LoadUnsafe(ref vecSpanRef, 4 * i);
                var b = Vector128.LoadUnsafe(ref vecSpanRef, 4 * (i + POS1));

                var x = a 
                    ^ Sse2.ShiftLeftLogical128BitLane(a, 1)
                    ^ (Sse2.ShiftRightLogical(b, 11) & MASK)
                    ^ Sse2.ShiftRightLogical128BitLane(r1, 1)
                    ^ Sse2.ShiftLeftLogical(r2, 18);
                
                x.StoreUnsafe(ref itr);

                r1 = r2;
                r2 = x;
            }
            for (; i < N; i++, itr = ref Unsafe.AddByteOffset(ref itr, 16))
            {
                var a = Vector128.LoadUnsafe(ref vecSpanRef, 4 * i);
                var b = Vector128.LoadUnsafe(ref storeSpanRef, 4 * (i + POS1 - N));

                var x = a 
                    ^ Sse2.ShiftLeftLogical128BitLane(a, 1)
                    ^ (Sse2.ShiftRightLogical(b, 11) & MASK)
                    ^ Sse2.ShiftRightLogical128BitLane(r1, 1)
                    ^ Sse2.ShiftLeftLogical(r2, 18);

                x.StoreUnsafe(ref itr);

                r1 = r2;
                r2 = x;
            }
        }
        
        public static void GenerateRandAll_Avx2(in Span<uint> vecSpan, in Span<uint> storeSpan)
        {
            ref var vecSpanRef = ref MemoryMarshal.GetReference(vecSpan);
            var r1 = Vector128.LoadUnsafe(ref vecSpanRef, 4 * (N - 2));
            var r2 = Vector128.LoadUnsafe(ref vecSpanRef, 4 * (N - 1));

            ref var storeSpanRef = ref MemoryMarshal.GetReference(storeSpan);

            ref var itr = ref MemoryMarshal.GetReference(storeSpan);
            nuint i = 0;
            for (; i < N - POS1; i++, itr = ref Unsafe.AddByteOffset(ref itr, 16))
            {
                var a = Vector128.LoadUnsafe(ref vecSpanRef, 4 * i);
                var b = Vector128.LoadUnsafe(ref vecSpanRef, 4 * (i + POS1));

                var x = a 
                    ^ Avx2.ShiftLeftLogical128BitLane(a, 1)
                    ^ (Avx2.ShiftRightLogical(b, 11) & MASK)
                    ^ Avx2.ShiftRightLogical128BitLane(r1, 1)
                    ^ Avx2.ShiftLeftLogical(r2, 18);

                x.StoreUnsafe(ref itr);

                r1 = r2;
                r2 = x;
            }
            for (; i < N; i++, itr = ref Unsafe.AddByteOffset(ref itr, 16))
            {
                var a = Vector128.LoadUnsafe(ref vecSpanRef, 4 * i);
                var b = Vector128.LoadUnsafe(ref storeSpanRef, 4 * (i + POS1 - N));

                var x = a 
                    ^ Avx2.ShiftLeftLogical128BitLane(a, 1)
                    ^ (Avx2.ShiftRightLogical(b, 11) & MASK)
                    ^ Avx2.ShiftRightLogical128BitLane(r1, 1)
                    ^ Avx2.ShiftLeftLogical(r2, 18);

                x.StoreUnsafe(ref itr);

                r1 = r2;
                r2 = x;
            }
        }

    }
}

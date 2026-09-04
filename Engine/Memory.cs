using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Engine
{
    internal unsafe static class Memory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* Alloc<T>(nuint alignment) where T : unmanaged
        {
            return (T*)NativeMemory.AlignedAlloc((nuint)sizeof(T), alignment);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* Alloc<T>(int count, nuint alignment) where T : unmanaged
        {
            ArgumentOutOfRangeException.ThrowIfNegative(count);
            nuint byteCount = checked((nuint)count * (nuint)sizeof(T));
            return (T*)NativeMemory.AlignedAlloc(byteCount, alignment);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free<T>(T* ptr) where T : unmanaged
        {
            NativeMemory.AlignedFree(ptr);
        }

        public static void Memcpy<T>(T* source, T* destination, int count) where T : unmanaged
        {
            Buffer.MemoryCopy(
                source,
                destination,
                destinationSizeInBytes: (nuint)(sizeof(T) * count),
                sourceBytesToCopy: (nuint)(sizeof(T) * count)
            );
        }
    }
}

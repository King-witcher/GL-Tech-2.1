using System.Runtime.InteropServices;

namespace Engine
{
    // TODO: Use NativeMemory
    internal unsafe static class Memory
    {
        public static T* HeapAlloc<T>() where T : unmanaged
        {
            return (T*)Marshal.AllocHGlobal(sizeof(T));
        }

        public static void HeapFree<T>(T* ptr) where T : unmanaged
        {
            Marshal.FreeHGlobal((nint)ptr);
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

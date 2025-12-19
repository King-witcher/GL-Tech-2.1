using System.Runtime.InteropServices;

namespace Engine
{
    internal unsafe static class Memory
    {
        public static T* Alloc<T>() where T : unmanaged
        {
            return (T*)Marshal.AllocHGlobal(sizeof(T));
        }

        public static void Free<T>(T* ptr) where T : unmanaged
        {
            Marshal.FreeHGlobal((nint)ptr);
        }

        public static void Copy<T>(T* source, T* destination, int count) where T : unmanaged
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

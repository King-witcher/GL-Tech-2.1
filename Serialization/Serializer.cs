
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Engine.Serialization
{
    /// <summary>
    /// Represents a file that stores a scene.
    /// </summary>
    internal class Serializer
    {
        Stream fileStream;

        public Serializer(Stream fs)
        {
            this.fileStream = fs;
        }

        public unsafe void SaveStruct<Struct>(Struct s) where Struct : unmanaged
        {
            int size = sizeof(Struct);
            byte[] byteArray = new byte[size];

            Marshal.Copy((IntPtr)(&s), byteArray, 0, size);
            fileStream.Write(byteArray, 0, size);
        }

        public unsafe Struct LoadStruct<Struct>() where Struct : unmanaged
        {
            Struct* s = stackalloc Struct[1];
            int size = sizeof(Struct);
            byte[] byteArray = new byte[size];

            fileStream.Read(byteArray, 0, size);
            Marshal.Copy(byteArray, 0, (IntPtr)s, size);
            return *s;
        }

        public void Close()
        {
            fileStream.Close();
            fileStream.Dispose();
        }
    }
}

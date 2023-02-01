using System;
using System.IO;

using Engine.World;
using Engine.Serialization;
using Engine.Structs;

namespace Engine.Development
{
    internal class Program
    {
        static unsafe void Main()
        {
            Scene scene = new Engine.Demos.SuperBlockMap.Map();
        }
    }
}

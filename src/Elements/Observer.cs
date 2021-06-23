using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ObserverData
    {
        internal Vector position;
        internal float rotation; //MUST be 0 <= x < 360

        static internal ObserverData* Create(Vector position, float rotation) // a little bit optimizable
        {
            ObserverData* result = (ObserverData*)Marshal.AllocHGlobal(sizeof(ObserverData));
            result->position = position;
            result->rotation = rotation;
            return result;
        }

        static internal void Delete(ObserverData* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }

    /// <summary>
    /// Represents a point of view from which the Renderer can render a scene.
    /// </summary>
    public unsafe class Observer : Element, IDisposable
    {
        internal ObserverData* unmanaged;

        public Observer(Vector position, float rotation)
        {
            unmanaged = ObserverData.Create(position, rotation);
        }

        private protected override Vector AbsolutePosition
        {
            get
            {
                return unmanaged->position;
            }
            set => unmanaged->position = value;
        }

        private protected override Vector AbsoluteNormal
        {
            get => new Vector(unmanaged->rotation);
            set => unmanaged->rotation = value.Angle;
        }

        public override void Dispose()
        {
            ObserverData.Delete(unmanaged);
            unmanaged = null;
        }
    }
}

using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SCamera
    {
        internal Vector position;
        internal float rotation; //MUST be 0 <= x < 360

        static internal SCamera* Create(Vector position, float rotation) // a little bit optimizable
        {
            SCamera* result = (SCamera*)Marshal.AllocHGlobal(sizeof(SCamera));
            result->position = position;
            result->rotation = rotation;
            return result;
        }

        static internal void Delete(SCamera* item)
        {
            Marshal.FreeHGlobal((IntPtr)item);
        }
    }

    /// <summary>
    /// Represents a point of view from which the Renderer can render a scene.
    /// </summary>
    public unsafe class Camera : Element, IDisposable
    {
        internal SCamera* unmanaged;

        public Camera(Vector position, float rotation = 0f)
        {
            unmanaged = SCamera.Create(position, rotation);
        }

        public override Vector WorldPosition
        {
            get
            {
                return unmanaged->position;
            }
            set => unmanaged->position = value;
        }

        public override Vector WorldRotation
        {
            get => new Vector(unmanaged->rotation);
            set => unmanaged->rotation = value.Angle;
        }

        public override void Dispose()
        {
            SCamera.Delete(unmanaged);
            unmanaged = null;
        }

        internal sealed override unsafe void AddToSScene(SScene* data)
        {
            if (data->camera == null)
                data->camera = unmanaged;
        }
    }
}

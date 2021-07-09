using System;

namespace GLTech2
{
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

        private protected override Vector PositionData
        {
            get => unmanaged->position;
            set => unmanaged->position = value;
        }

        private protected override Vector DirectionData
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

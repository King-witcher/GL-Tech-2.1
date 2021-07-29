using System;

namespace GLTech2
{
    /// <summary>
    /// Represents a camera view from which the Renderer can render a scene.
    /// </summary>
    public unsafe class Camera : Element, IDisposable
    {
        #region What should happen to the unmanaged data if its position/direction changes? Here's where the class answers it.
        internal SCamera* unmanaged;

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
        #endregion

        /// <summary>
        /// Gets a new instance of Camera.
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="rotation">The rotation</param>
        public Camera(Vector position, float rotation = 0f)
        {
            unmanaged = SCamera.Create(position, rotation);
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

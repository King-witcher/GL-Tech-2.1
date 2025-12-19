using Struct = GLTech.Structs.CameraStruct;

namespace GLTech.World
{
    public unsafe sealed class Camera : Entity, IDisposable
    {
        #region What should happen to the unmanaged data if its position/direction changes? Here's where the class answers it.
        internal Struct* raw;

        private protected override Vector PositionData
        {
            get => raw->position;
            set => raw->position = value;
        }

        private protected override Vector DirectionData
        {
            get => raw->direction;
            set => raw->direction = value;
        }
        #endregion

        public float Z
        {
            get => raw->z;
            set => raw->z = value;
        }

        internal Camera()
        {
            raw = Struct.Create(Vector.Zero, Vector.North);
        }

        public override void Dispose()
        {
            Struct.Delete(raw);
            raw = null;
        }
    }
}

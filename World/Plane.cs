using Engine.Imaging;
using Engine.Data;

namespace Engine.World
{
    public unsafe class Floor : Entity
    {
        #region What should happen to the unmanaged data if its position/direction changes? Here's where the class answers it.
        internal SFloor* unmanaged;

        private protected override Vector PositionData
        {
            get => unmanaged->oa;
            set => unmanaged->oa = value;
        }

        private protected override Vector DirectionData
        {
            get => Vector.Zero;
            set { }
        }
        #endregion

        public Vector Position
        {
            // Just a bit spaguetti
            get => unmanaged->oa;
            set => unmanaged->oa = value;
        }

        public Texture Texture
        {
            get => unmanaged->texture;
            set
            {
                unmanaged->texture = value;
            }
        }

        public Floor(Vector a, Vector b, Vector c, Texture texture)
        {
            unmanaged = SFloor.Create(a, b, c, texture);
        }

        public override void Dispose()
        {
            //STriFloor.Delete(unmanaged);
            unmanaged = null;
        }

        public override string ToString()
        {
            return $"<Floor: { Position }> ";
        }
    }
}

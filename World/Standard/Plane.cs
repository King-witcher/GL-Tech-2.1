using Engine.Imaging;
using Struct = Engine.Structs.Plane;

namespace Engine.World
{
    public unsafe class Plane : Entity
    {
        #region What should happen to the unmanaged data if its position/direction changes? Here's where the class answers it.
        internal Struct* unmanaged;

        private protected override Vector PositionData
        {
            get => unmanaged->segment.start;
            set => unmanaged->segment.start = value;
        }

        private protected override Vector DirectionData
        {
            get => unmanaged->segment.direction;
            set => unmanaged->segment.direction = value;
        }
        #endregion

        public Vector Start
        {
            // Just a bit spaguetti
            get => unmanaged->segment.start;
            set => unmanaged->segment.start = value;
        }

        public Vector End
        {
            // Spaguetti?
            get => unmanaged->segment.start + unmanaged->segment.direction;
            set => unmanaged->segment.direction = value - unmanaged->segment.start;
        }

        public float Length
        {
            get => unmanaged->segment.direction.Module;
            set => unmanaged->segment.direction *= value / unmanaged->segment.direction.Module;
        }

        public Texture Texture
        {
            get => unmanaged->texture;
            set
            {
                unmanaged->texture = value;
            }
        }

        public Plane(Vector start, Vector end, Texture texture)
        {
            unmanaged = Struct.Create(start, end, texture);
        }

        public override void Dispose()
        {
            Struct.Delete(unmanaged);
            unmanaged = null;
        }

        public override string ToString()
        {
            return $"|{ Start } -- { End }| ";
        }
    }
}

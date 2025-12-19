using Struct = GLTech.Structs.PlaneStruct;

namespace GLTech.World
{
    public unsafe class Plane : Entity
    {
        #region What should happen to the unmanaged data if its position/direction changes? Here's where the class answers it.
        internal Struct* unmanaged;

        private protected override Vector PositionData
        {
            get => unmanaged->segment.Start;
            set => unmanaged->segment.Start = value;
        }

        private protected override Vector DirectionData
        {
            get => unmanaged->segment.Direction;
            set => unmanaged->segment.Direction = value;
        }
        #endregion

        public Vector Start
        {
            // Just a bit spaguetti
            get => unmanaged->segment.Start;
            set => unmanaged->segment.Start = value;
        }

        public Vector End
        {
            // Spaguetti?
            get => unmanaged->segment.Start + unmanaged->segment.Direction;
            set => unmanaged->segment.Direction = value - unmanaged->segment.Start;
        }

        public float Length
        {
            get => unmanaged->segment.Direction.Module;
            set => unmanaged->segment.Direction *= value / unmanaged->segment.Direction.Module;
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

        ~Plane()
        {
            if (unmanaged != null) Dispose();
        }

        public override void Dispose()
        {
            Struct.Delete(unmanaged);
            unmanaged = null;
        }

        public override string ToString()
        {
            return $"|{Start} -- {End}| ";
        }
    }
}

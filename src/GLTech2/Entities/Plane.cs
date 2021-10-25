using GLTech2.Drawing;

namespace GLTech2.Entities
{
    public unsafe class Plane : Entity
    {
        #region What should happen to the unmanaged data if its position/direction changes? Here's where the class answers it.
        internal SPlane* unmanaged;

        private protected override Vector PositionData
        {
            get => unmanaged->start;
            set => unmanaged->start = value;
        }

        private protected override Vector DirectionData
        {
            get => unmanaged->direction;
            set => unmanaged->direction = value;
        }
        #endregion

        public Vector Start
        {
            // Just a bit spaguetti
            get => unmanaged->start;
            set => unmanaged->start = value;
        }

        public Vector End
        {
            // Spaguetti?
            get => unmanaged->start + unmanaged->direction;
            set => unmanaged->direction = value - unmanaged->start;
        }

        public float Length
        {
            get => unmanaged->direction.Module;
            set => unmanaged->direction *= value / unmanaged->direction.Module;
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
            unmanaged = SPlane.Create(start, end, texture);
        }


        public override void Dispose()
        {
            SPlane.Delete(unmanaged);
            unmanaged = null;
        }

        public override string ToString()
        {
            return $"|{ Start } -- { End }| ";
        }
    }
}

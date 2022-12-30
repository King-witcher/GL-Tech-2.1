using Engine.Imaging;
using Engine.Data;
using System;

namespace Engine.World
{
    public unsafe class Floor : Entity
    {
        #region What should happen to the unmanaged data if its position/direction changes? Here's where the class answers it.
        internal FloorStruct* unmanaged;

        private protected override Vector PositionData
        {
            get => unmanaged->tl;
            set => unmanaged->tl = value;
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
            get => unmanaged->tl;
            set => unmanaged->tl = value;
        }

        public Texture Texture
        {
            get => unmanaged->texture;
            set
            {
                unmanaged->texture = value;
            }
        }

        public Floor(Vector start, Vector end, Texture texture)
        {
            var bottom = Math.Min(start.y, end.y);
            var top = Math.Max(start.y, end.y);
            var left = Math.Min(start.x, end.x);
            var right = Math.Max(start.x, end.x);

            unmanaged = FloorStruct.Create((left, top), (right, bottom), texture);
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

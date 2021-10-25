using System.Runtime.CompilerServices;
using GLTech2.Unmanaged;

namespace GLTech2.Entities
{
    public unsafe class Collider : Entity
    {
        #region What should happen to the unmanaged data if its position/direction changes? Here's where the class answers it.
        internal SCollider* unmanaged;

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

        public Collider(Vector start, Vector end) =>
            unmanaged = SCollider.Alloc(start, end);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Test(Ray ray, out float distance) =>
            unmanaged->Test(ray, out distance);

        public override void Dispose()
        {
            SCollider.Delete(unmanaged);
            unmanaged = null;
        }
    }
}

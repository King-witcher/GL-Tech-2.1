using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GLTech2
{
    /// <summary>
    /// Represents an invisible plane that can be dettected via collision dettection.
    /// </summary>
    public unsafe class Collider : Element
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

        /// <summary>
        /// Gets a new instance of Collider.
        /// </summary>
        /// <param name="start">Inicial point</param>
        /// <param name="end">Final point</param>
        public Collider(Vector start, Vector end) =>
            unmanaged = SCollider.Alloc(start, end);

        /// <summary>
        /// Tests whether a a ray collides or not with this collider.
        /// </summary>
        /// <param name="ray">A ray to test collision</param>
        /// <param name="distance">The distance beetween (idk how to write) the ray's initial point and the collision point</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Test(Ray ray, out float distance) =>
            unmanaged->Test(ray, out distance);

        public override void Dispose()
        {
            SCollider.Delete(unmanaged);
            unmanaged = null;
        }

        internal override unsafe void ChangeSScene(SScene* data) =>
            data->Add(unmanaged);
    }
}

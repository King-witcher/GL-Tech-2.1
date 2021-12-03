﻿using System.Runtime.CompilerServices;
using GLTech2.Unmanaged;

namespace GLTech2.Entities
{
    public unsafe sealed class Collider : Entity
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

        private Entity visibleBody = null;
        public Entity VisibleBody => visibleBody;

        public Collider(Vector start, Vector end)
        {
            unmanaged = SCollider.Alloc(start, end);
        }

        public Collider(Plane visiblePlane)
        {
            unmanaged = SCollider.Alloc(visiblePlane.Start, visiblePlane.End);
            visibleBody = visiblePlane;
        }

        public Collider(Vector start, Vector end, Entity visibleBody)
        {
            unmanaged = SCollider.Alloc(start, end);
            this.visibleBody = visibleBody;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Test(Ray ray, out float distance)
        {
            return unmanaged->Test(ray, out distance);
        }

        public override void Dispose()
        {
            SCollider.Delete(unmanaged);
            unmanaged = null;
        }
    }
}
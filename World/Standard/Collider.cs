using System.Runtime.CompilerServices;
using Engine.Structs;

namespace Engine.World
{
    public unsafe sealed class Collider : Entity
    {
        #region What should happen to the unmanaged data if its position/direction changes? Here's where the class answers it.
        internal ColliderStruct* unmanaged;

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
            unmanaged = ColliderStruct.Create(start, end);
        }

        public Collider(Plane visiblePlane)
        {
            unmanaged = ColliderStruct.Create(visiblePlane.Start, visiblePlane.End);
            visibleBody = visiblePlane;
        }

        public Collider(Vector start, Vector end, Entity visibleBody)
        {
            unmanaged = ColliderStruct.Create(start, end);
            this.visibleBody = visibleBody;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Test(Segment ray, out float distance)
        {
            return unmanaged->Test(ray, out distance);
        }

        public override void Dispose()
        {
            ColliderStruct.Delete(unmanaged);
            unmanaged = null;
        }
    }
}

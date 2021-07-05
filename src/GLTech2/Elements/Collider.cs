using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GLTech2
{
    public unsafe class Collider : Element
    {
        internal SCollider* unmanaged;

        public Collider(Vector start, Vector end) =>
            unmanaged = SCollider.UnmanagedAlloc(start, end);

        public override Vector WorldPosition
        {
            get => unmanaged->start;
            set => unmanaged->start = value;
        }

        public override Vector WorldRotation
        {
            get => unmanaged->direction;
            set => unmanaged->direction = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Test(Ray ray, out float distance) =>
            unmanaged->Test(ray, out distance);

        public override void Dispose()
        {
            SCollider.Delete(unmanaged);
            unmanaged = null;
        }

        internal override unsafe void AddToSScene(SScene* data) =>
            data->Add(unmanaged);
    }
}

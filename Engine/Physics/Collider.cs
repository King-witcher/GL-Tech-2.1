using GLTech.World;
using System.Runtime.CompilerServices;

namespace Engine.Physics;

public unsafe sealed class Collider : Entity
{
    #region What should happen to the unmanaged data if its position/direction changes? Here's where the class answers it.
    internal RawCollider* raw;

    private protected override Vector PositionData
    {
        get => raw->Start;
        set => raw->Start = value;
    }

    private protected override Vector DirectionData
    {
        get => raw->Direction;
        set => raw->Direction = value;
    }
    #endregion

    public Collider(Vector start, Vector end)
    {
        raw = Memory.Alloc<RawCollider>();
        raw->Start = start;
        raw->Direction = end - start;
    }

    public Collider(Plane plane)
    {
        raw = Memory.Alloc<RawCollider>();
        raw->Start = plane.Start;
        raw->Direction = plane.End - plane.Start;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Test(Segment ray, out float distance)
    {
        return raw->Test(ray, out distance);
    }

    public override void Dispose()
    {
        Memory.Free(raw);
        raw = null;
    }
}

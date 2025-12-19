namespace Engine.Physics;

public struct CollisionSummary
{
    public float ray_factor;
    public Vector normal;

    public CollisionSummary()
    {
        ray_factor = float.MaxValue;
        normal = Vector.Zero;
    }
}

public struct RayCastInfo
{
    public Segment segment;
    float radius;

    public RayCastInfo()
    {
        radius = 0f;
    }
}

unsafe class ColliderSystem
{
    StructList<RawCollider> list = new StructList<RawCollider>();

    public ColliderSystem() { }

    public unsafe CollisionSummary RayCast(RayCastInfo info)
    {
        CollisionSummary summary = new();
        var iterator = list.Iter();

        while (iterator.Next(out RawCollider* pcollider))
        {
            var collider = *pcollider;

            // Ignore back faces
            if (info.segment.direction.y * collider.segment.direction.x - info.segment.direction.x * collider.segment.direction.y < 0f)
                continue;

            var rs = info.segment.GetRS(collider.segment);

            if (
                rs.y >= 0f && rs.y < 1f &&
                rs.x > 0 && rs.x < summary.ray_factor
            )
            {
                summary.ray_factor = rs.x;
                summary.normal = collider.Direction.Right / collider.Direction.Module;
            }
        }

        return summary;
    }

    public unsafe void Add(RawCollider* pcollider) => list.Add(pcollider);

    public unsafe void Add(Collider collider) => Add(collider.raw);

    ~ColliderSystem()
    {
        list.Clear();
    }
}

unsafe struct ListNode<T> where T : unmanaged
{
    public T* value;
    public ListNode<T>* pnext;
}

unsafe struct StructList<T> where T : unmanaged
{
    private ListNode<T>* first;

    public StructList()
    {
        first = null;
    }

    public void Add(T* item)
    {
        var node = Memory.Alloc<ListNode<T>>();
        node->value = item;
        node->pnext = first;
        first = node;
    }

    public void Clear()
    {
        ListNode<T>* current = first;
        while (current != null)
        {
            var next = current->pnext;
            Memory.Free(current);
            current = next;
        }
        first = null;
    }

    public ListIterator<T> Iter()
    {
        return new ListIterator<T>(first);
    }
}

unsafe struct ListIterator<T> where T : unmanaged
{
    private ListNode<T>* current;

    public ListIterator(ListNode<T>* start)
    {
        current = start;
    }

    public bool Next(out T* value)
    {
        value = current->value;
        current = current->pnext;
        return current != null;
    }
}
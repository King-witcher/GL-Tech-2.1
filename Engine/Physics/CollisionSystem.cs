namespace Engine.Physics;

public struct RayCastInfo
{
    public Segment displacement;

    public RayCastInfo() { }
}

public struct RayCastResult
{
    /// <summary>Fração t ∈ [0,1] ao longo do deslocamento até o contato.</summary>
    public float fraction;
    public Vector surfaceNormal;

    public RayCastResult()
    {
        fraction = float.MaxValue;
        surfaceNormal = Vector.Zero;
    }
}

public struct CircleCastResult
{
    /// <summary>Fração t ∈ [0,1] ao longo do deslocamento até o contato.</summary>
    public float fraction;
    /// <summary>Normal unitária do contato — aponta da parede para o centro do círculo.</summary>
    public Vector surfaceNormal;
    /// <summary>Ponto de contato sobre a geometria.</summary>
    public Vector contactPoint;
}

public struct MoveResult
{
    public Vector newPosition;
    public Vector newVelocity;
    public bool collided;
}

public unsafe sealed class CollisionSystem
{
    private const float Epsilon = 1e-6f;
    private const float Skin = 1e-3f;
    private const int MaxSlides = 4;

    private readonly List<Collider> colliders = new();

    internal CollisionSystem() { }

    internal void Add(Collider collider) => colliders.Add(collider);

    internal void Remove(Collider collider) => colliders.Remove(collider);

    public int ColliderCount => colliders.Count;

    public RayCastResult GetCollision(RayCastInfo info)
    {
        RayCastResult summary = new();

        foreach (Collider collider in colliders)
        {
            Segment wall = collider.raw->segment;

            if (info.displacement.direction.y * wall.direction.x - info.displacement.direction.x * wall.direction.y < 0f)
                continue;

            Vector rs = info.displacement.GetRS(wall);

            if (rs.y >= 0f && rs.y < 1f &&
                rs.x > 0f && rs.x < summary.fraction)
            {
                summary.fraction = rs.x;
                summary.surfaceNormal = wall.direction.Right / wall.direction.Module;
            }
        }

        return summary;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Varredura de círculo: primeiro contato de um círculo de raio `radius`
    //  movendo-se de `origin` por `displacement`.
    //
    //  Para cada parede testamos dois tipos de contato (soma de Minkowski do
    //  segmento com o círculo = uma "cápsula"):
    //    1) FACE  — o segmento deslocado por `radius` ao longo da normal; é um
    //               raycast do CENTRO contra essa reta deslocada.
    //    2) QUINA — círculo do corpo contra cada extremo do segmento; é um
    //               raycast do CENTRO contra um círculo de raio `radius` no extremo.
    //               É isto que faz o corpo "dobrar" suavemente em cantos e pilares.
    //  Fica com o contato de menor t (o mais cedo) entre todas as paredes.
    // ─────────────────────────────────────────────────────────────────────────
    public bool CircleCast(Vector origin, Vector displacement, float radius, out CircleCastResult hit)
    {
        hit = default;

        float bestT = float.PositiveInfinity;
        Vector bestNormal = Vector.Zero;
        Vector bestPoint = Vector.Zero;
        bool found = false;

        foreach (Collider collider in colliders)
        {
            Segment wall = collider.raw->segment;
            Vector a = wall.start;
            Vector d = wall.direction;

            float len2 = d.x * d.x + d.y * d.y;
            if (len2 < Epsilon) continue;

            Vector n = d.Right / MathF.Sqrt(len2);

            float denom = Vector.DotProduct(displacement, n);
            if (denom < -Epsilon)
            {
                float dist = Vector.DotProduct(origin - a, n);
                float t = (radius - dist) / denom;

                if (t >= 0f && t <= 1f)
                {
                    Vector center = origin + t * displacement;
                    float s = Vector.DotProduct(center - a, d) / len2;

                    if (s >= 0f && s <= 1f && t < bestT)
                    {
                        bestT = t;
                        bestNormal = n;
                        bestPoint = center - radius * n;
                        found = true;
                    }
                }
            }

            TestEndpoint(origin, displacement, radius, a, ref bestT, ref bestNormal, ref bestPoint, ref found);
            TestEndpoint(origin, displacement, radius, a + d, ref bestT, ref bestNormal, ref bestPoint, ref found);
        }

        if (found)
        {
            hit.fraction = bestT;
            hit.surfaceNormal = bestNormal;
            hit.contactPoint = bestPoint;
        }

        return found;
    }

    private static void TestEndpoint(Vector origin, Vector displacement, float radius, Vector e,
        ref float bestT, ref Vector bestNormal, ref Vector bestPoint, ref bool found)
    {
        Vector m = origin - e;
        float a = Vector.DotProduct(displacement, displacement);
        if (a < Epsilon) return;

        float b = 2f * Vector.DotProduct(m, displacement);
        float c = Vector.DotProduct(m, m) - radius * radius;

        float disc = b * b - 4f * a * c;
        if (disc < 0f) return;

        float t = (-b - MathF.Sqrt(disc)) / (2f * a);
        if (t < 0f || t > 1f) return;

        Vector center = origin + t * displacement;
        Vector normal = center - e;
        float nl = normal.Module;
        if (nl < Epsilon) return;
        normal /= nl;

        if (Vector.DotProduct(displacement, normal) >= -Epsilon) return;

        if (t < bestT)
        {
            bestT = t;
            bestNormal = normal;
            bestPoint = e;
            found = true;
        }
    }

    public MoveResult MoveAndSlide(Vector origin, Vector displacement, Vector velocity, float radius)
    {
        Vector position = Depenetrate(origin, radius);
        Vector remaining = displacement;
        bool collided = false;

        for (int slide = 0; slide < MaxSlides; slide++)
        {
            if (Vector.DotProduct(remaining, remaining) < Epsilon)
                break;

            if (!CircleCast(position, remaining, radius, out CircleCastResult hit))
            {
                // remaining path is free
                position += remaining;
                break;
            }

            position += hit.fraction * remaining;
            collided = true;

            Vector leftover = (1f - hit.fraction) * remaining;
            remaining = leftover - hit.surfaceNormal * Vector.DotProduct(leftover, hit.surfaceNormal);
            velocity -= hit.surfaceNormal * Vector.DotProduct(velocity, hit.surfaceNormal);
            position = Depenetrate(position, radius);
        }

        return new MoveResult
        {
            newPosition = position,
            newVelocity = velocity,
            collided = collided,
        };
    }

    private Vector Depenetrate(Vector position, float radius)
    {
        float target = radius + Skin;

        for (int pass = 0; pass < 2; pass++)
        {
            bool moved = false;

            foreach (Collider collider in colliders)
            {
                Segment wall = collider.raw->segment;
                Vector wall_s = wall.start;
                Vector wall_d = wall.direction;
                Vector wall_s_to_pos = position - wall_s;

                // Avoid div by 0
                float len_sqr = Vector.DotProduct(wall_d, wall_d);
                if (len_sqr < Epsilon) continue;

                Vector n = wall_d.Right / MathF.Sqrt(len_sqr);
                float s = Vector.DotProduct(wall_s_to_pos, wall_d) / len_sqr;

                if (*(uint*)&s <= 0x3f80_0000) // 0f <= s <= 1f
                {
                    float dist = Vector.DotProduct(wall_s_to_pos, n);
                    if (*(uint*)&dist < *(uint*)&target)
                    {
                        position += n * (target - dist);
                        moved = true;
                    }
                }
                else
                {
                    Vector e = s < 0f ? wall_s : wall_s + wall_d;
                    Vector diff = position - e;
                    float dist = diff.Module;
                    if (dist < target && dist > Epsilon && Vector.DotProduct(diff, n) >= 0f)
                    {
                        position += diff / dist * (target - dist);
                        moved = true;
                    }
                }
            }

            if (!moved) break;
        }

        return position;
    }
}

using Engine.Rendering;
using GLTech.Structs;
using GLTech.World;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech;

// TODO: RendererInner como readonly struct
public unsafe class Renderer
{
    // One source of truth for the frustum constants; WallPass reads the same struct.
    Projection projection;
    int cached_width, cached_height;

    // Wall scratch, reused across frames so that a frame allocates nothing. Both arrays live in
    // the pinned object heap, so they never move and the pointers below stay valid for good.
    Segment[] wall_segments = [];
    Texture[] wall_textures = [];
    Segment* wall_segments_ptr;
    Texture* wall_textures_ptr;
    Stopwatch sw = new();

    public bool Parallel { get; set; } = true;
    public float HFov { get; set; }

    public Renderer(float hFov, bool parallel)
    {
        HFov = hFov;
        Parallel = parallel;
        SetProjection(800, 600, hFov);
    }

    internal void Render(Scene managed_scene, SwapchainImage target)
    {
        EnsureCompatibility(target);

        RawScene* scene = managed_scene.raw;
        PlayerStruct* player = scene->player;

        var col_start_table = stackalloc int[target.width];
        var col_end_table = stackalloc int[target.width];

        CameraInfo camera = new(player->position, player->direction, player->height);

        sw.Restart();
        EnsureWallCapacity(managed_scene.planes.Count);
        int wall_count = FlattenWalls(managed_scene.planes);
        WallPass.GatherViewspaceWalls(
            camera, projection,
            wall_count, wall_segments_ptr, wall_textures_ptr,
            out int visible_count, wall_segments_ptr, wall_textures_ptr);
        sw.Stop();

        Debug.VisibleWalls = visible_count;
        Debug.WallCullTime = (float)sw.Elapsed.TotalSeconds;

        Frame frame = new(
            camera: camera,
            projection: projection,
            spans: new ColumnSpans(col_start_table, col_end_table),
            image: target,
            walls: new WallsInfo(visible_count, wall_segments_ptr, wall_textures_ptr));

        // Render walls
        sw.Restart();
        if (Parallel)
            System.Threading.Tasks.Parallel.ForEach(Partitioner.Create(0, target.width),
                range => WallPass.Render(frame, range.Item1, range.Item2));
        else
            WallPass.Render(frame, 0, target.width);
        sw.Stop();
        Debug.WallRenderTime = (float)sw.Elapsed.TotalSeconds;

        //if (Parallel)
        //{
        //    System.Threading.Tasks.Parallel.For(fromInclusive: 0, toExclusive: target.height >> 1, body: drawCeilingLine);
        //    System.Threading.Tasks.Parallel.For(fromInclusive: target.height >> 1, toExclusive: target.height, body: drawFloorLine);
        //}
        //else
        //{
        //    for (int i = 0; i < target.height >> 1; i++)
        //        drawCeilingLine(i);
        //    for (int i = target.height >> 1; i < target.height; i++)
        //        drawFloorLine(i);
        //}

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        void drawFloorLine(int line)
        {
            unchecked
            {
                float hit_dist = scene->player->height / ((line - projection.horizon) * projection.columnSlope);
                float step = projection.columnSlope * hit_dist;

                Vector camera_dir = scene->player->direction;
                Vector right = new Vector(camera_dir.y, -camera_dir.x);
                Vector center_hit = scene->player->position + camera_dir * hit_dist;

                Vector left_bound_hit = center_hit - right * step * (target.width >> 1);
                using HorizontalList list = scene->floor_list.GetIntersections(left_bound_hit, left_bound_hit + right);
                var step_vec = right * step;
                for (int col = 0; col < target.width; col++)
                {
                    if (col_end_table[col] > line) continue;

                    Vector floor_hit = left_bound_hit + col * step_vec;
                    HorizontalStruct* floor = list.FindAndRaise(floor_hit);

                    if (floor != null)
                        target[col, line] = floor->MapTexture(floor_hit);
                    else
                        target[col, line] = 0;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        void drawCeilingLine(int line)
        {
            unchecked
            {
                float hit_dist = (1 - scene->player->height) / ((projection.horizon - line) * projection.columnSlope);
                float step = projection.columnSlope * hit_dist;

                Vector camera_dir = scene->player->direction;
                Vector right = new Vector(camera_dir.y, -camera_dir.x);
                Vector center_hit = scene->player->position + camera_dir * hit_dist;

                Vector left_bound_hit = center_hit - right * step * (target.width >> 1);
                using HorizontalList list = scene->ceiling_list.GetIntersections(left_bound_hit, left_bound_hit + right);

                var step_vec = right * step;
                for (int col = 0; col < target.width; col++)
                {
                    if (col_start_table[col] <= line) continue;

                    Vector hit = left_bound_hit + step_vec * col;
                    HorizontalStruct* ceiling = list.FindAndRaise(hit);

                    if (ceiling != null)
                        target[col, line] = ceiling->MapTexture(hit);
                    else
                        target[col, line] = 0;
                }
            }
        }
    }

    public void Update(int width, int height, float hFov)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive integers.");

        if (hFov <= 0 || hFov >= 180)
            throw new ArgumentException("Horizontal field of view must be between 0 and 180 degrees.");

        SetProjection(width, height, hFov);
    }

    private int FlattenWalls(List<Plane> planes)
    {
        Segment* segments = wall_segments_ptr;
        Texture* textures = wall_textures_ptr;
        int count = 0;

        foreach (Plane plane in CollectionsMarshal.AsSpan(planes))
        {
            PlaneInner* inner = plane.unmanaged;
            segments[count] = inner->segment;
            textures[count] = inner->texture;
            count++;
        }

        return count;
    }

    private void EnsureWallCapacity(int plane_count)
    {
        if (plane_count <= wall_segments.Length)
            return;

        // Pinned object heap: never relocated, so a pointer taken once here stays valid.
        wall_segments = GC.AllocateArray<Segment>(plane_count, pinned: true);
        wall_textures = GC.AllocateArray<Texture>(plane_count, pinned: true);
        wall_segments_ptr = (Segment*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(wall_segments));
        wall_textures_ptr = (Texture*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(wall_textures));
    }

    private void EnsureCompatibility(SwapchainImage target)
    {
        // TODO: Check hFov
        if (target.width != cached_width || target.height != cached_height)
            SetProjection(target.width, target.height, HFov);
    }

    private void SetProjection(int width, int height, float hFov)
    {
        cached_width = width;
        cached_height = height;
        projection = Projection.From(width, height, hFov);
    }
}

using Engine.Rendering;
using Engine.Window;
using GLTech.Structs;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace GLTech;

// TODO: RendererInner como readonly struct
public unsafe class Renderer
{
    RendererState state;
    //RenderTarget target;
    readonly HorizontalPass floorPass = new(isFloor: true);
    readonly HorizontalPass ceilingPass = new(isFloor: false);

    public bool Parallel { get; set; } = true;
    public float HFov { get; set; }

    public Renderer(float hFov, bool parallel)
    {
        state = new RendererState(800, 600, hFov);
        HFov = hFov;
        Parallel = parallel;
    }

    internal void Render(RawScene* scene, RenderTarget target)
    {
        EnsureCompatibility(target);

        var col_start_table = stackalloc int[target.width];
        var col_end_table = stackalloc int[target.width];

        // Cull only the planes that appear in the field of view.
        var left = new Vector(-scene->player->direction.y, scene->player->direction.x);
        var rtl = left * state.step0 * target.widthf;
        View view = new(
            center: scene->player->position,
            left: scene->player->direction + rtl * 0.5f,
            right: scene->player->direction - rtl * 0.5f
        );

        // TODO: Culling is not renderer's responsibility
        using var surface_culled = scene->plane_list.CullBySurface(scene->player->position);
        using var plane_list = surface_culled.CullByFrustum(view);

        {
            if (Parallel)
                //System.Threading.Tasks.Parallel.For(fromInclusive: 0, toExclusive: target.width, body: DrawColumn);
                System.Threading.Tasks.Parallel.ForEach(Partitioner.Create(0, target.width), range =>
                {
                    for (int col = range.Item1; col < range.Item2; col++) DrawColumn(col);
                });
            else
                for (int i = 0; i < target.width; i++)
                    DrawColumn(i);
        }

        // Floors and ceilings: snapshot and bucket the visible triangles once per
        // frame, then rasterize spans per scanline (see HorizontalPass).
        {
            var cam_pos = scene->player->position;
            var cam_dir = scene->player->direction;
            var cam_right = new Vector(cam_dir.y, -cam_dir.x);
            float player_height = scene->player->height;

            ceilingPass.Prepare(scene->ceiling_list, cam_pos, cam_dir, cam_right, view.left, view.right,
                1f - player_height, state.step0, target.width, target.height);
            floorPass.Prepare(scene->floor_list, cam_pos, cam_dir, cam_right, view.left, view.right,
                player_height, state.step0, target.width, target.height);
        }

        int half_height = target.height >> 1;

        if (Parallel)
            System.Threading.Tasks.Parallel.ForEach(Partitioner.Create(0, target.height), range =>
            {
                for (int line = range.Item1; line < range.Item2; line++)
                    DrawHorizontalLine(line);
            });
        else
            for (int line = 0; line < target.height; line++)
                DrawHorizontalLine(line);

        void DrawHorizontalLine(int line)
        {
            if (line < half_height)
                ceilingPass.DrawLine<AboveWalls>(line, target, col_start_table);
            else
                floorPass.DrawLine<BelowWalls>(line, target, col_end_table);
        }

        // Render a vertical column of the screen.
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        void DrawColumn(int col_idx)
        {
            unchecked
            {
                int width = target.width;
                int height = target.height;
                float height_f = target.heightf;

                // Caching frequently used variables
                var delta = width / 2 - col_idx;
                var dir = scene->player->direction + left * (state.step0 * delta);
                Segment ray = new Segment(scene->player->position, dir);
                //Texture bg = scene->background;

                // Cast the ray towards every plane.
                PlaneStruct* plane = plane_list.NearestPlane(ray, out Vector rs);

                if (plane == null)
                {
                    col_start_table[col_idx] = height;
                    col_end_table[col_idx] = 0;
                    return;
                }

                // Found out that optimizing this part by separing the case when it hits and not a wall is unecessary.
                #region Render the plane

                // Height that the current column should have on the screen.
                float colision_depth = Vector.DotProduct(ray.direction, scene->player->direction) * rs.x;
                float col_height_f = state.colHeight1 / colision_depth; // Wall column size in pixels

                // Where the column starts and ends relative to the screen.
                float col_start_f = (height_f - 1f - col_height_f) * 0.5f + col_height_f * (scene->player->height - 0.5f);
                float col_end_f = (height_f - 1f + col_height_f) * 0.5f + col_height_f * (scene->player->height - 0.5f);

                // Wall rendering bounds on the screen...
                int col_start_i = height - (int)(height_f - col_start_f);    // Inclusive
                if (col_start_i < 0)
                    col_start_i = 0;

                int col_end_i = height - (int)(height_f - col_end_f);        // Exclusive
                if (col_end_i > height)
                    col_end_i = height;

                col_start_table[col_idx] = col_start_i;
                col_end_table[col_idx] = col_end_i;

                // Draw the wall
                // Critical performance impact.
                float i_col_h = 1f / col_height_f;
                // Resolve the texture column once for the entire column (rs.y is constant),
                // hoisting both x and the column base pointer out of the per-pixel loop.
                Texture tex = plane->texture;
                uint* texcol = tex.ResolveColumn(rs.y);
                for (int line_idx = col_start_i; line_idx < col_end_i; line_idx++)
                {
                    float v = (line_idx - col_start_f) * i_col_h;
                    Color color = tex.MapColumn(texcol, v);
                    target[col_idx, line_idx] = color;
                }
                #endregion
            }
        }
    }

    public void Update(int width, int height, float hFov)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive integers.");

        if (hFov <= 0 || hFov >= 180)
            throw new ArgumentException("Horizontal field of view must be between 0 and 180 degrees.");

        state = new RendererState(width, height, hFov);
    }

    private void EnsureCompatibility(RenderTarget target)
    {
        // TODO: Check hFov
        if (target.width != state.width || target.height != state.height)
            state = new RendererState(target.width, target.height, HFov);
    }
}

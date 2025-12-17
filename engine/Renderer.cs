using GLTech.Imaging;
using GLTech.Structs;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GLTech;

internal unsafe class Renderer
{
    RenderCache* cache;
    Image target;

    public bool ParallelRendering { get; set; } = true;
    public float HFov
    {
        get;
        set
        {
            field = value;
            RecalculateCache();
        }
    } = 90f;

    public Renderer(Image target)
    {
        this.target = target;
        cache = RenderCache.Create(HFov, target.widthf);
    }

    public unsafe void Draw(SceneStruct* scene)
    {
        var col_start_table = new int[target.width];
        var col_end_table = new int[target.width];

        // Cull only the planes that appear in the field of view.
        var left = new Vector(-scene->camera->direction.y, scene->camera->direction.x);
        var rtl = left * cache->step0 * target.widthf;
        View view = new(
            center: scene->camera->position,
            left: scene->camera->direction + rtl * 0.5f,
            right: scene->camera->direction - rtl * 0.5f
        );
        using var surface_culled = scene->plane_list.CullBySurface(scene->camera->position);
        using var plane_list = surface_culled.CullByFrustum(view);

        {
            if (ParallelRendering)
                Parallel.For(fromInclusive: 0, toExclusive: target.Width, body: DrawColumn);
            else
                for (int i = 0; i < target.Width; i++)
                    DrawColumn(i);
        }

        // Checks if the code should be run in all cores or just one.
        if (ParallelRendering)
            Parallel.For(fromInclusive: 0, toExclusive: target.Height >> 1, body: drawCeilingLine);
        else
            for (int i = 0; i < target.Height >> 1; i++)
                drawCeilingLine(i);

        // Checks if the code should be run in all cores or just one.
        if (ParallelRendering)
            Parallel.For(fromInclusive: target.Height >> 1, toExclusive: target.Height, body: drawFloorLine);
        else
            for (int i = target.Height >> 1; i < target.Height; i++)
                drawFloorLine(i);

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        void drawFloorLine(int line)
        {
            unchecked
            {
                float hit_dist = scene->camera->z / ((line - target.heightf * 0.5f) * cache->step0);
                float step = cache->step0 * hit_dist;

                Vector camera_dir = scene->camera->direction;
                Vector right = new Vector(camera_dir.Y, -camera_dir.X);
                Vector center_hit = scene->camera->position + camera_dir * hit_dist;

                Vector left_bound_hit = center_hit - right * step * (target.Width >> 1);
                using HorizontalList list = scene->floor_list.GetIntersections(left_bound_hit, left_bound_hit + right);
                var step_vec = right * step;
                for (int col = 0; col < target.Width; col++)
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
                float hit_dist = (1 - scene->camera->z) / ((target.heightf * 0.5f - line) * cache->step0);
                float step = cache->step0 * hit_dist;

                Vector camera_dir = scene->camera->direction;
                Vector right = new Vector(camera_dir.Y, -camera_dir.X);
                Vector center_hit = scene->camera->position + camera_dir * hit_dist;

                Vector left_bound_hit = center_hit - right * step * (target.Width >> 1);
                using HorizontalList list = scene->ceiling_list.GetIntersections(left_bound_hit, left_bound_hit + right);

                var step_vec = right * step;
                for (int col = 0; col < target.Width; col++)
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
                var dir = scene->camera->direction + left * (cache->step0 * delta);
                Segment ray = new Segment(scene->camera->position, dir);
                //Texture bg = scene->background;

                // Cast the ray towards every plane.
                PlaneStruct* plane = plane_list.NearestPlane(ray, out Vector rs);
                //if (plane == null) return;

                // Found out that optimizing this part by separing the case when it hits and not a wall is unecessary.
                #region Render the plane

                // Height that the current column should have on the screen.
                float colision_depth = Vector.DotProduct(ray.direction, scene->camera->direction) * rs.x;
                float col_height_f = cache->colHeight1 / colision_depth; // Wall column size in pixels

                // Where the column starts and ends relative to the screen.
                float col_start_f = (height_f - 1f - col_height_f) * 0.5f + col_height_f * (scene->camera->z - 0.5f);
                float col_end_f = (height_f - 1f + col_height_f) * 0.5f + col_height_f * (scene->camera->z - 0.5f);

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
                for (int line_idx = col_start_i; line_idx < col_end_i; line_idx++)
                {
                    float v = (line_idx - col_start_f) * i_col_h;
                    Color color = plane->texture.MapNearest(rs.y, v);
                    target[col_idx, line_idx] = color;
                }
                #endregion
            }
        }
    }

    ~Renderer()
    {
        RenderCache.Delete(cache);
    }

    private void RecalculateCache()
    {
        RenderCache.Delete(cache);
        cache = RenderCache.Create(HFov, target.widthf);
    }
}

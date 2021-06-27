using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SceneData
    {
        internal SpriteData** sprities; //not implemented
        internal int sprite_count;
        internal int sprite_max;
        internal VisualPlaneData** planes;
        internal int plane_count;
        internal int plane_max;
        internal Texture background;
        internal ObserverData* activeObserver;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SceneData* Create(int maxWalls, int maxSprities, Texture background)
        {
            int total_positions = maxWalls + maxSprities + 1;
            SceneData* result = (SceneData*)Marshal.AllocHGlobal(sizeof(SceneData));
            result->sprities = null;
            //result->sprities = (Sprite_**)Marshal.AllocHGlobal(maxSprities * sizeof(Sprite_*)); // Not implemented yet
            result->planes = (VisualPlaneData**)Marshal.AllocHGlobal(total_positions * sizeof(void*));
            *result->planes = null;
            result->sprite_count = 0;
            result->sprite_max = maxSprities;
            result->plane_count = 0;
            result->plane_max = maxWalls;
            result->background = background;
            return result;
        }

        public static void Delete(SceneData* item)
        {
            Marshal.FreeHGlobal((IntPtr)item->sprities);
            Marshal.FreeHGlobal((IntPtr)item->planes);
            Marshal.FreeHGlobal((IntPtr)item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(VisualPlaneData* wall)
        {
            planes[plane_count++] = wall;
            planes[plane_count] = null;
        }

        internal VisualPlaneData* PlaneRayCast(Ray ray, out float nearest_dist, out float nearest_ratio)
        {
            // 0 < distance <= infinity
            // 0 <= split < 1
            // split = 2f means no collision
            void GetCollisionData(VisualPlaneData* wall, out float cur_dist, out float cur_split)
            {
                // Medium performance impact.
                float
                    drx = wall->geom_direction.x,
                    dry = wall->geom_direction.y;

                float det = ray.direction.x * dry - ray.direction.y * drx; // Caching can only be used here

                if (det == 0) // Parallel
                {
                    cur_dist = float.PositiveInfinity;
                    cur_split = 2f;
                    return;
                }

                float spldet = ray.direction.x * (ray.start.y - wall->geom_start.y) - ray.direction.y * (ray.start.x - wall->geom_start.x);
                float dstdet = wall->geom_direction.x * (ray.start.y - wall->geom_start.y) - wall->geom_direction.y * (ray.start.x - wall->geom_start.x);
                float spltmp = spldet / det;
                float dsttmp = dstdet / det;
                if (spltmp < 0 || spltmp >= 1 || dsttmp <= 0) // dsttmp = 0 means column height = x/0.
                {
                    cur_dist = float.PositiveInfinity;
                    cur_split = 2f;
                    return;
                }
                cur_split = spltmp;
                cur_dist = dsttmp;
            }

            nearest_dist = float.PositiveInfinity;
            nearest_ratio = 2f;
            int wallcount = plane_count;
            VisualPlaneData** cur = planes;
            VisualPlaneData* nearest = null;

            for (int i = 0; i < wallcount; i++)
            {
                //walls cannot be null.
                GetCollisionData(*cur, out float cur_dist, out float cur_ratio);
                if (cur_dist < nearest_dist)
                {
                    nearest_ratio = cur_ratio;
                    nearest_dist = cur_dist;
                    nearest = *cur;
                }
                cur++;
            }
            return nearest;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(void* sprite) => throw new NotImplementedException();
    }
}

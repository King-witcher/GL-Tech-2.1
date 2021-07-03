using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Implementar culling para otimizar a detecção de muros

namespace GLTech2
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SceneData
    {
        internal SpriteData* first_spritie; //not implemented
        internal SpriteData* lasat_spritie;
        internal int sprite_count;
        internal VisualPlaneData* first_plane;
        internal VisualPlaneData* last_plane;
        internal int plane_count;
        internal Texture background;
        internal ObserverData* activeObserver;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SceneData* Create(Texture background)
        {
            SceneData* result = (SceneData*)Marshal.AllocHGlobal(sizeof(SceneData));
            result->first_spritie = null;
            result->first_plane = null;
            result->sprite_count = 0;
            result->plane_count = 0;
            result->background = background;
            return result;
        }

        // Must delete element list!
        public static void Delete(SceneData* item)
        {
            // Marshal.FreeHGlobal((IntPtr)item->sprities);
            // Marshal.FreeHGlobal((IntPtr)item->planes);
            Marshal.FreeHGlobal((IntPtr)item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(VisualPlaneData* wall)
        {
            if (first_plane == null)    // Has no elements
                first_plane = last_plane = wall;
            else
            {
                last_plane->link_next = wall;
                last_plane = wall;
            }
            plane_count++;
        }

        internal VisualPlaneData* VisualRayCast(Ray ray, out float nearest_dist, out float nearest_ratio)
        {
            // 0 < distance <= infinity
            // 0 <= split < 1
            // split = 2f means no collision
            void GetCollisionData(VisualPlaneData* plane, out float cur_dist, out float cur_split)
            {
                float prod = plane->geom_direction.x * ray.direction.y - plane->geom_direction.y * ray.direction.x;
                if (prod <= 0)
                {
                    cur_dist = float.PositiveInfinity;
                    cur_split = 2f;
                    return;
                }

                // Medium performance impact.
                float
                    drx = plane->geom_direction.x,
                    dry = plane->geom_direction.y;

                float det = ray.direction.x * dry - ray.direction.y * drx; // Caching can only be used here

                if (det == 0) // Parallel
                {
                    cur_dist = float.PositiveInfinity;
                    cur_split = 2f;
                    return;
                }

                float spldet = ray.direction.x * (ray.start.y - plane->geom_start.y) - ray.direction.y * (ray.start.x - plane->geom_start.x);
                float dstdet = drx * (ray.start.y - plane->geom_start.y) - dry * (ray.start.x - plane->geom_start.x);
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

            VisualPlaneData* nearest = null;
            nearest_dist = float.PositiveInfinity;
            nearest_ratio = 2f;
            VisualPlaneData* cur = first_plane;

            while(cur != null)
            {
                GetCollisionData(cur, out float cur_dist, out float cur_ratio);
                if (cur_dist < nearest_dist)
                {
                    nearest_ratio = cur_ratio;
                    nearest_dist = cur_dist;
                    nearest = cur;
                }
                cur = cur->link_next;
            }
            return nearest;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(void* sprite) => throw new NotImplementedException();
    }
}

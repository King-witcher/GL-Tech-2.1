using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using GLTech2.Drawing;
using GLTech2.Entities;
using GLTech2.Unmanaged;

namespace GLTech2.Unmanaged
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SScene
    {
        internal SSprite* first_sprite; //not implemented
        internal SSprite* last_sprite;
        internal int sprite_count;
        internal SPlane* first_plane;
        internal SPlane* last_plane;
        internal int plane_count;
        internal SCollider* first_collider;
        internal SCollider* last_collider;
        internal int collider_count;
        internal Texture background;
        internal SCamera* camera;  // Talvez eu mude isso

        internal static SScene* Create(Texture background)
        {
            SScene* result = (SScene*)Marshal.AllocHGlobal(sizeof(SScene));
            result->first_sprite = null;
            result->first_plane = null;
            result->first_collider = null;
            result->last_sprite = null;
            result->last_plane = null;
            result->last_collider = null;
            result->sprite_count = 0;
            result->plane_count = 0;
            result->collider_count = 0;
            result->background = background;
            result->camera = null;
            return result;
        }

        // Must delete plane list!
        public static void Delete(SScene* item)
        {
            // Marshal.FreeHGlobal((IntPtr)item->sprities);
            // Marshal.FreeHGlobal((IntPtr)item->planes);
            Marshal.FreeHGlobal((IntPtr)item);
        }

        public void Add(Entity entity)
        {
            if (entity is Camera camera)
            {
                if (this.camera != null)    // Must be changed.
                    return;
                this.camera = camera.unmanaged;
            }
            else if (entity is Plane plane)
            {
                Add(plane.unmanaged);
            }
            else if (entity is Collider collider)
            {
                Add(collider.unmanaged);
            }
            else if (entity is Sprite sprite)
            {
                Add(sprite.unmanaged);
            }
        }

        private void Add(SPlane* plane)
        {
            if (first_plane == null)    // Has no entities
                first_plane = last_plane = plane;
            else
            {
                last_plane->list_next = plane;
                last_plane = plane;
            }
            plane_count++;
        }

        [Obsolete]
        private void Remove(SPlane* plane)
        {
            fixed (SPlane** csharpisbad = &first_plane)
            {
                SPlane** pptr = csharpisbad;

                while (*pptr != plane)
                    pptr = &(*pptr)->list_next;

                *pptr = plane->list_next;
                plane_count--;

                if (*pptr == null) // Maybe it's wrong
                    last_plane = null;
            }
        }

        private void Add(SSprite* sprite)
        {
            if (first_sprite == null)
                first_sprite = last_sprite = sprite;
            else
            {
                last_sprite->list_next = sprite;
                last_sprite = sprite;
            }
            plane_count++;
        }

        [Obsolete]
        private void Remove(SSprite* sprite)
        {
            fixed (SSprite** csharpisbad = &first_sprite)
            {
                SSprite** pptr = csharpisbad;

                while (*pptr != sprite)
                    pptr = &(*pptr)->list_next;

                *pptr = sprite->list_next;
                sprite_count--;

                if (*pptr == null)
                    last_sprite = null;
            }
        }

        private void Add(SCollider* collider)
        {
            if (first_collider == null)    // Has no elements
                first_collider = last_collider = collider;
            else
            {
                last_collider->list_next = collider;
                last_collider = collider;
            }
            collider_count++;
        }

        [Obsolete]
        private void Remove(SCollider* collider)
        {
            fixed (SCollider** csharpisbad = &first_collider)
            {
                SCollider** pptr = csharpisbad;

                while (*pptr != collider)
                    pptr = &(*pptr)->list_next;

                *pptr = collider->list_next;
                collider_count--;

                if (*pptr == null)
                    last_collider = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SPlane* NearestPlane(Ray ray, out float nearest_dist, out float nearest_ratio)
        {
            SPlane* nearest = null;
            nearest_dist = float.PositiveInfinity;
            nearest_ratio = 2f;
            SPlane* cur = first_plane;

            while (cur != null)
            {
                cur->Test(ray, out float cur_dist, out float cur_ratio);
                if (cur_dist < nearest_dist)
                {
                    nearest_ratio = cur_ratio;
                    nearest_dist = cur_dist;
                    nearest = cur;
                }
                cur = cur->list_next;
            }
            return nearest;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SCollider* Cllsn_rcast(Ray ray, out float distance)
        {
            // 0 < distance <= infinity
            // 0 <= split < 1
            // split = 2f means no collision
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void GetCollisionData(SCollider* collider, out float cur_dist, out float cur_split)
            {
                // Culling
                if (collider->direction.x * ray.direction.y - collider->direction.y * ray.direction.x <= 0)
                {
                    cur_dist = float.PositiveInfinity;
                    cur_split = 2f;
                    return;
                }

                // Medium performance impact.
                float
                    drx = collider->direction.x,
                    dry = collider->direction.y;

                float det = ray.direction.x * dry - ray.direction.y * drx; // Caching can only be used here

                if (det == 0) // Parallel
                {
                    cur_dist = float.PositiveInfinity;
                    cur_split = 2f;
                    return;
                }

                float spldet = ray.direction.x * (ray.start.y - collider->start.y) - ray.direction.y * (ray.start.x - collider->start.x);
                float dstdet = drx * (ray.start.y - collider->start.y) - dry * (ray.start.x - collider->start.x);
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

            SCollider* nearest = null;
            distance = float.PositiveInfinity;
            SCollider* cur = first_collider;

            while (cur != null)
            {
                GetCollisionData(cur, out float cur_dist, out float cur_ratio);
                if (cur_dist < distance)
                {
                    distance = cur_dist;
                    nearest = cur;
                }
                cur = cur->list_next;
            }
            return nearest;
        }
    }
}

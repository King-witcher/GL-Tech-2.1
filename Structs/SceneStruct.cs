using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Engine.Imaging;

using Entity = Engine.World.Entity;
using PlaneClass = Engine.World.Plane;
using ColliderClass = Engine.World.Collider;
using SpriteClass = Engine.World.Sprite;
using FloorClass = Engine.World.Floor;
using CeilingClass = Engine.World.Ceiling;
using CameraClass = Engine.World.Camera;

namespace Engine.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SceneStruct : IDisposable
    {
        internal static int count;
        internal SpriteStruct* first_sprite; //not implemented
        internal SpriteStruct* last_sprite;
        internal int sprite_count;
        internal PlaneList plane_list;
        internal ColliderStruct* first_collider;
        internal ColliderStruct* last_collider;
        internal int collider_count;
        internal Texture background;
        internal CameraStruct* camera;  // Talvez eu mude isso
        internal HorizontalList floor_list;
        internal HorizontalList ceiling_list;

        internal static SceneStruct* Create()
        {
            count++;
            SceneStruct* result = (SceneStruct*)Marshal.AllocHGlobal(sizeof(SceneStruct));
            result->first_sprite = null;
            result->plane_list = new();
            result->first_collider = null;
            result->last_sprite = null;
            result->last_collider = null;
            result->sprite_count = 0;
            result->collider_count = 0;
            result->background = Texture.NullTexture;
            result->camera = null;
            result->floor_list = new();
            result->ceiling_list = new();

            return result;
        }

        public void Dispose()
        {
            plane_list.Dispose();
            floor_list.Dispose();
            ceiling_list.Dispose();
        }

        // TODO Must delete plane list!
        public static void Delete(SceneStruct* item)
        {
            // Marshal.FreeHGlobal((IntPtr)item->sprities);
            // Marshal.FreeHGlobal((IntPtr)item->planes);
            item->Dispose();
            Marshal.FreeHGlobal((IntPtr)item);
            count--;
        }

        public void Add(Entity entity)
        {
            if (entity is CameraClass camera)
            {
                if (this.camera != null)    // Must be changed.
                    return;
                this.camera = camera.unmanaged;
            }
            else if (entity is PlaneClass plane)
            {
                Add(plane.unmanaged);
            }
            else if (entity is ColliderClass collider)
            {
                Add(collider.unmanaged);
            }
            else if (entity is SpriteClass sprite)
            {
                Add(sprite.unmanaged);
            }
            else if (entity is FloorClass floor)
            {
                floor_list.Add(floor.unmanaged);
            }
            else if (entity is CeilingClass ceiling)
            {
                ceiling_list.Add(ceiling.unmanaged);
            }
        }

        private void Add(PlaneStruct* plane)
        {
            plane_list.Add(plane);
        }

        private void Add(SpriteStruct* sprite)
        {
            if (first_sprite == null)
                first_sprite = last_sprite = sprite;
            else
            {
                last_sprite->list_next = sprite;
                last_sprite = sprite;
            }
            sprite_count++;
        }

        private void Add(ColliderStruct* collider)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PlaneStruct* NearestPlane(Segment ray, out float nearest_dist, out float nearest_ratio)
        {
            unchecked
            {
                PlaneStruct* nearest = null;
                nearest_dist = float.PositiveInfinity;
                nearest_ratio = 2f;
                PlaneList.Node* cur = plane_list.first;

                while (cur != null)
                {
                    cur->data->Test(ray, out float cur_dist, out float cur_ratio);
                    if (cur_dist < nearest_dist)
                    {
                        nearest_ratio = cur_ratio;
                        nearest_dist = cur_dist;
                        nearest = cur->data;
                    }
                    cur = cur->next;
                }

                return nearest;
            }
        }

        internal HorizontalStruct* FloorAt(Vector point)
        {
            return floor_list.FindAndRaise(point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ColliderStruct* Cllsn_rcast(Segment ray, out float distance)
        {
            // 0 < distance <= infinity
            // 0 <= split < 1
            // split = 2f means no collision
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void GetCollisionData(ColliderStruct* collider, out float cur_dist, out float cur_split)
            {
                unchecked
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
            }

            ColliderStruct* nearest = null;
            distance = float.PositiveInfinity;
            ColliderStruct* cur = first_collider;

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

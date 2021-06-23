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
        internal WallData** walls;
        internal int wall_count;
        internal int wall_max;
        internal Texture background;
        internal ObserverData* activeObserver;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SceneData* Create(int maxWalls, int maxSprities, Texture background)
        {
            int total_positions = maxWalls + maxSprities + 1;
            SceneData* result = (SceneData*)Marshal.AllocHGlobal(sizeof(SceneData));
            result->sprities = null;
            //result->sprities = (Sprite_**)Marshal.AllocHGlobal(maxSprities * sizeof(Sprite_*)); // Not implemented yet
            result->walls = (WallData**)Marshal.AllocHGlobal(total_positions * sizeof(void*));
            *result->walls = null;
            result->sprite_count = 0;
            result->sprite_max = maxSprities;
            result->wall_count = 0;
            result->wall_max = maxWalls;
            result->background = background;
            return result;
        }

        public static void Delete(SceneData* item)
        {
            Marshal.FreeHGlobal((IntPtr)item->sprities);
            Marshal.FreeHGlobal((IntPtr)item->walls);
            Marshal.FreeHGlobal((IntPtr)item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(WallData* wall)
        {
            walls[wall_count++] = wall;
            walls[wall_count] = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(void* sprite) => throw new NotImplementedException();
    }
}

using Engine;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct RawScene : IDisposable
    {
        public static int count;
        public SpriteStruct* first_sprite; //not implemented
        public SpriteStruct* last_sprite;
        public int sprite_count;
        public Texture background;
        public PlayerStruct* player;  // Talvez eu mude isso
        public HorizontalList floor_list;
        public HorizontalList ceiling_list;

        public static RawScene* Create()
        {
            count++;
            RawScene* result = Memory.Alloc<RawScene>(alignment: 16);
            result->first_sprite = null;
            result->last_sprite = null;
            result->sprite_count = 0;
            result->background = Texture.NullTexture;
            result->player = null;
            result->floor_list = new();
            result->ceiling_list = new();

            return result;
        }

        public void Dispose()
        {
            floor_list.Dispose();
            ceiling_list.Dispose();
        }

        // TODO Must delete plane list!
        public static void Delete(RawScene* item)
        {
            item->Dispose();
            Memory.Free(item);
            count--;
        }

        public void Add(PlayerStruct* camera)
        {
            this.player = camera;
        }

        public void Add(SpriteStruct* sprite)
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

        public void AddFloor(HorizontalStruct* floor)
        {
            floor_list.Add(floor);
        }

        public void AddCeiling(HorizontalStruct* ceiling)
        {
            ceiling_list.Add(ceiling);
        }

        internal HorizontalStruct* FloorAt(Vector point)
        {
            return floor_list.FindAndRaise(point);
        }
    }
}

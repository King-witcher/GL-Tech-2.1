using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GLTech.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SceneStruct : IDisposable
    {
        public static int count;
        public SpriteStruct* first_sprite; //not implemented
        public SpriteStruct* last_sprite;
        public int sprite_count;
        public PlaneList plane_list;
        public Texture background;
        public CameraStruct* camera;  // Talvez eu mude isso
        public HorizontalList floor_list;
        public HorizontalList ceiling_list;

        public static SceneStruct* Create()
        {
            count++;
            SceneStruct* result = (SceneStruct*)Marshal.AllocHGlobal(sizeof(SceneStruct));
            result->first_sprite = null;
            result->plane_list = new();
            result->last_sprite = null;
            result->sprite_count = 0;
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
            item->Dispose();
            Marshal.FreeHGlobal((IntPtr)item);
            count--;
        }

        public void Add(PlaneStruct* plane)
        {
            plane_list.Add(plane);
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

        public void Add(CameraStruct* camera)
        {
            this.camera = camera;
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

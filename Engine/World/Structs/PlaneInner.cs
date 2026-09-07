using Engine;

namespace GLTech.Structs
{
    internal unsafe struct PlaneInner
    {
        public Segment segment;
        public Texture texture;               // Yes, by value.

        public static PlaneInner* Create(Vector start, Vector end, Texture texture)
        {
            PlaneInner* result = Memory.Alloc<PlaneInner>(alignment: 16);
            result->texture = texture;
            result->segment = new(start, end - start);
            return result;
        }

        public static PlaneInner* Create(Vector start, float angle, float length, Texture texture)
        {
            PlaneInner* result = Memory.Alloc<PlaneInner>(alignment: 16);
            Vector direction = new Vector(angle) * length;
            result->texture = texture;
            result->segment = new(start, direction);
            return result;
        }

        public static void Delete(PlaneInner* item)
        {
            Memory.Free(item);
        }
    }
}

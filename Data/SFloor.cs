using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Engine.Imaging;

namespace Engine.Data
{
    internal unsafe struct SFloorList
    {
        public Node* first;

        public SFloorList()
        {
            first = null;
        }

        public void Add(Node* node)
        {
            node->next = first;
            first = node;
        }

        public void Add(SFloor* floor)
        {
            Node* node = Node.Create(floor);
            Add(node);
        }

        void Raise(Node* prev)
        {
            Node* cur = prev->next;
            prev->next = cur->next;
            cur->next = first;
            first = cur;
        }

        internal SFloor* Locate(Vector point)
        {
            Node* prev = null;
            Node* cur = first;

            while (cur != null)
            {
                SFloor* data = cur->data;
                if (data->Contains(point))
                {
                    if (prev != null)
                        Raise(prev);
                    return cur->data;
                }
                prev = cur;
                cur = cur->next;
            }
            return null;
        }

        [NativeCppClass]
        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct Node
        {
            public SFloor* data;
            public Node* next;

            public static Node* Create(SFloor* sfloor)
            {
                Node* result = (Node*)Marshal.AllocHGlobal(sizeof(Node));
                result->data = sfloor;
                return result;
            }
        }
    }


    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SFloor
    {
        internal Vector tl;
        internal Vector br;

        public Texture texture;

        internal static SFloor* Create(Vector topLeft, Vector bottomRight, Texture texture)
        {
            SFloor* result = (SFloor*)Marshal.AllocHGlobal(sizeof(SFloor));
            result->tl = topLeft;
            result->br = bottomRight;
            result->texture = texture;
            return result;
        }

        public bool Contains(Vector point)
        {
            if (point.x > tl.x && point.x < br.x && point.y > tl.y && point.y < br.y)
                return true;
            return false;
        }

        public Color MapTexture(Vector coordinates)
        {
            float xratio = (coordinates.x) % 1f;
            float yratio = (coordinates.y) % 1f;
            return texture.MapPixel(xratio, yratio);
        }
    }
}

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Engine.Imaging;

namespace Engine.Data
{
    internal unsafe struct SFloorList : IDisposable
    {
        private Node* first;

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
            var cur = prev->next;
            prev->next = cur->next;
            cur->next = first;
            first = cur;
        }

        public SFloorList GetIntersections(Vector a, Vector b)
        {
            var cur = first;
            var list = new SFloorList();

            while(cur != null)
            {
                if (cur->data->Intersects(a, b))
                    list.Add(Node.Create(cur->data));
                cur = cur->next;
            }

            return list;
        }

        public SFloor* Locate(Vector point)
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

        public void Dispose()
        {
            var current = first;

            while (current != null)
            {
                var next = current->next;
                Node.Delete(current);
                current = next;
            }
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

            public static void Delete(Node* node)
            {
                Marshal.FreeHGlobal((IntPtr) node);
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

        public bool Intersects(Vector start, Vector end)
        {
            var bottom = Math.Min(start.y, end.y);
            var top = Math.Max(start.y, end.y);
            var left = Math.Min(start.x, end.x);
            var right = Math.Max(start.x, end.x);

            if (top > br.y && bottom < tl.y && left < br.x && right > tl.x)
                return true;

            if (Contains(start) || Contains(end))
                return true;

            return false;
        }

        public bool Contains(Vector point)
        {
            if (point.x > tl.x && point.x < br.x && point.y < tl.y && point.y > br.y)
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

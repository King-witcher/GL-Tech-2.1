using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Engine.Structs
{
    [NativeCppClass]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct GenericList<T> : IDisposable
        where T : unmanaged
    {
        Node* first;

        public void Add(T* item)
        {
            Node* node = Node.Create(item);
            node->next = first;
            first = node;
        }

        public GenericList<T> Filter(Predicate predicate)
        {
            Node* cur = first;
            GenericList<T> list = new();

            while (cur != null)
            {
                T* data = cur->data;
                if (predicate(data))
                    list.Add(data);
                cur = cur->next;
            }
            return list;
        }

        [MethodImpl(MethodImplOptions.Unmanaged)]
        public T* FindAndRaise(Predicate predicate)
        {
            Node* prev = null;
            Node* cur = first;

            while (cur != null)
            {
                if (predicate(cur->data))
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

        void Raise(Node* prev)
        {
            Node* current = prev->next;
            prev->next = current->next;
            current->next = first;
            first = current;
        }

        public delegate bool Predicate(T* item);


        /// <summary>
        /// Stores a pointer to the item but does not take ownership of it.
        /// </summary>
        [NativeCppClass]
        [StructLayout(LayoutKind.Sequential)]
        internal struct Node
        {
            public T* data;
            public Node* next;
            public static int count;

            public static Node* Create(T* item)
            {
                Interlocked.Increment(ref count);
                Node* result = (Node*)Marshal.AllocHGlobal(sizeof(Node));
                result->data = item;
                return result;
            }

            public static void Delete(Node* node)
            {
                Interlocked.Decrement(ref count);
                Marshal.FreeHGlobal((IntPtr)node);
            }
        }
    }
}

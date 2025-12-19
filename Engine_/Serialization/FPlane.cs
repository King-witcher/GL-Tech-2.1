namespace GLTech.Serialization
{
    unsafe struct FScene
    {
        internal int PlaneCount;
        internal int ColliderCount;
        internal int EmptyCount;

        internal int TextureCount;
        internal int ImageCount;
    }

    internal unsafe struct FPlane
    {
        public fixed char Name[64];
        public Vector Start;
        public Vector Direction;
        public int ParentIndex;
        public int TextureIndex;
    }

    internal unsafe struct FEmpty
    {
        public Vector Position;
        public Vector Direction;
        public int ParentIndex;
    }

    internal unsafe struct FCollider
    {
        public fixed char Name[64];
        public Vector Start;
        public Vector Direction;
        public int ParentIndex;
    }

    internal unsafe struct FTexture
    {
        public int ImageIndex;
        public float hoffset;
        public float hrepeat;
        public float voffset;
        public float vrepeat;
    }
    internal unsafe struct FImage
    {
        public int Height;
        public int Width;
    }
}

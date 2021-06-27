
using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    internal unsafe sealed class Sprite : Element
    {
        internal SpriteData* unmanaged;

        public override Vector AbsolutePosition
        {
            get => unmanaged->position;
            set => unmanaged->position = value;
        }

        public override Vector AbsoluteNormal { get; set; }

        public Sprite(Vector position, Texture material)
        {
            unmanaged = SpriteData.Create(position, material);
        }

        public override void Dispose()
        {
            SpriteData.Delete(unmanaged);
            unmanaged = null;
        }
    }
}

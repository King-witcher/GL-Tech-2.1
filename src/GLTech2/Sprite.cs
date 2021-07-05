
using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    internal unsafe sealed class Sprite : Element
    {
        internal SSprite* unmanaged;

        public override Vector WorldPosition
        {
            get => unmanaged->position;
            set => unmanaged->position = value;
        }

        public override Vector WorldRotation { get; set; }

        public Sprite(Vector position, Texture material)
        {
            unmanaged = SSprite.Create(position, material);
        }

        public override void Dispose()
        {
            SSprite.Delete(unmanaged);
            unmanaged = null;
        }
    }
}

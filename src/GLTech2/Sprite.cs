﻿
using System;
using System.Runtime.InteropServices;

namespace GLTech2
{
    internal unsafe sealed class Sprite : Element
    {
        internal SSprite* unmanaged;

        private protected override Vector PositionData
        {
            get => unmanaged->position;
            set => unmanaged->position = value;
        }

        private protected override Vector DirectionData { get; set; }

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
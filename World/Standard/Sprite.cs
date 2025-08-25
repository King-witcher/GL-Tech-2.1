﻿using Engine.Imaging;

using Struct = Engine.Structs.SpriteStruct;

namespace Engine.World
{
    internal unsafe sealed class Sprite : Entity
    {
        internal Struct* unmanaged;

        private protected override Vector PositionData
        {
            get => unmanaged->position;
            set => unmanaged->position = value;
        }

        private protected override Vector DirectionData { get; set; }

        public Sprite(Vector position, Texture material)
        {
            unmanaged = Struct.Create(position, material);
        }

        public override void Dispose()
        {
            Struct.Delete(unmanaged);
            unmanaged = null;
        }
    }
}

using Engine.Imaging;
using Engine.Structs;

namespace Engine.World
{
    internal unsafe sealed class Sprite : Entity
    {
        internal SpriteStruct* unmanaged;

        private protected override Vector PositionData
        {
            get => unmanaged->position;
            set => unmanaged->position = value;
        }

        private protected override Vector DirectionData { get; set; }

        public Sprite(Vector position, Texture material)
        {
            unmanaged = SpriteStruct.Create(position, material);
        }

        public override void Dispose()
        {
            SpriteStruct.Delete(unmanaged);
            unmanaged = null;
        }
    }
}

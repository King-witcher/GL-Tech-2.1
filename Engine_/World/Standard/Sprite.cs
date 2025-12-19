using Struct = GLTech.Structs.SpriteStruct;

namespace GLTech.World
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

        ~Sprite()
        {
            if (unmanaged != null) Dispose();
        }

        public override void Dispose()
        {
            Struct.Delete(unmanaged);
            unmanaged = null;
        }
    }
}

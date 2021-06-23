#pragma warning disable IDE0051

namespace GLTech2.PrefabBehaviours
{
    public sealed class Move : Behaviour
    {
        public Vector Direction { get; set; } = Vector.Forward;
        public float Speed { get; set; } = 0.5f;

        void Update()
        {
            Element.Translate(Direction * Speed * Time.DeltaTime);
        }
    }
}

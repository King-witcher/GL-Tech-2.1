#pragma warning disable IDE0051

namespace GLTech2.PrefabBehaviours
{
    public sealed class Rotate : Behaviour
    {
        public float Speed { get; set; } = 30f;
        void Update()
        {
            Element.Rotate(Speed * Time.DeltaTime);
        }
    }
}

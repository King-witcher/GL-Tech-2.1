#pragma warning disable IDE0051

namespace Engine.Scripting.Prefab
{
    public sealed class Rotate : Behaviour
    {
        public float AngularSpeed { get; set; } = 30f;

        void OnFrame() =>
            Entity.Rotate(AngularSpeed * Frame.DeltaTime);
    }
}

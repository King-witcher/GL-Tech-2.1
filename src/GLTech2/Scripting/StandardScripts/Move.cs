#pragma warning disable IDE0051

namespace GLTech2.Scripting.StandardScripts
{
    public sealed class Move : Behaviour
    {
        public Vector Direction { get; set; } = Vector.Forward;

        public float Speed { get; set; } = 0.5f;

        void OnFrame() =>
            Entity.RelativePosition += Direction * Speed * Frame.DeltaTime;
    }
}

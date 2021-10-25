
namespace GLTech2.Scripting.StandardScripts
{
    public sealed class MouseRotation : Behaviour
    {
        public bool Enabled { get; set; } = true;

        public float Sensitivity { get; set; } = 5f;

        public MouseRotation() { }

        public MouseRotation(float sensitivity)
        {
            Sensitivity = sensitivity;
        }

        void OnFrame()
        {
            if (Enabled)
                Entity.Rotate(Mouse.HShift * 0.022f * Sensitivity);
        }
    }
}


namespace GLTech2.Scripting.StandardScripts
{
    public sealed class MouseLook : Behaviour
    {
        public bool Enabled { get; set; } = true;

        public float Sensitivity { get; set; } = 2.2f;

        public MouseLook() { }

        public MouseLook(float sensitivity)
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

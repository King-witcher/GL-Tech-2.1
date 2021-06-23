
namespace GLTech2.PrefabBehaviours
{
    public sealed class MouseLook : Behaviour
    {
        public bool Enabled { get; set; } = true;
        public float Sensitivity { get; set; } = 5f;

        public MouseLook() { }

        public MouseLook(float sensitivity)
        {
            this.Sensitivity = sensitivity;
        }

        void Update()
        {
            if (Enabled)
                element.Rotate(Mouse.HShift * 0.022f * Sensitivity);
        }
    }
}

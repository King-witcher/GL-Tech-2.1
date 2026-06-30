namespace GLTech.Scripting.Prefab
{
    public sealed class MouseLook : Script
    {
        public bool Enabled { get; set; } = true;

        public float Sensitivity { get; set; } = 2.2f;

        public MouseLook() { }

        public MouseLook(float sensitivity)
        {
            Sensitivity = sensitivity;
        }

        void Update()
        {
            if (Enabled)
            {
                Entity.Rotate(Input.MouseRel.x * 0.022f * Sensitivity);
            }
        }
    }
}

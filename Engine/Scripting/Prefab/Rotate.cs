#pragma warning disable IDE0051

namespace GLTech.Scripting.Prefab
{
    public sealed class Rotate : Script
    {
        public Rotate()
        {

        }

        public Rotate(float angularSpeed)
        {
            AngularSpeed = angularSpeed;
        }

        public float AngularSpeed { get; set; } = 30f;


        [ScriptUpdate]
        void Update()
        {
            Entity?.Rotate(AngularSpeed * Time.TimeStep);
        }
    }
}

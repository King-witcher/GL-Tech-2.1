#pragma warning disable IDE0051

namespace GLTech.Scripting.Prefab
{
    public sealed class Move : Script
    {
        public Move()
        {

        }

        public Move(Vector velocity)
        {
            Direction = velocity / velocity.Module;
            Speed = velocity.Module;
        }

        public Vector Direction { get; set; } = Vector.Forward;

        public float Speed { get; set; } = 0.5f;

        void OnFrame()
        {
            Entity.RelativePosition += Direction * Speed * Time.TimeStep;
            System.Console.WriteLine(Entity as World.Plane);
        }
    }
}

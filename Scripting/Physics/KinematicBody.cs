
namespace Engine.Scripting.Physics
{
    public abstract class KinematicBody : Behaviour
    {
        internal KinematicBody() { }

        public Vector Velocity { get; set; }

        public float Speed => Velocity.Module;

        public float AngularVelocity { get; set; }

        public abstract void Accelerate(Vector acceleration);

        public abstract void AddVelocity(Vector velocity);

        public abstract void AngularAccelerate(float acceleration);

        public abstract void AddAngularVelocity(float momentum);
    }
}

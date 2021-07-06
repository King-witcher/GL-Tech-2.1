namespace GLTech2.Behaviours
{
    /// <summary>
    /// Provides an interface to control the position of an Element. 
    /// </summary>
    public abstract class KinematicBody : Behaviour
    {
        internal KinematicBody(){ }

        public Vector Velocity { get; set; }

        public float Speed => Velocity.Module;

        public float AngularVelocity { get; set; }

        public abstract void Accelerate(Vector acceleration);

        public abstract void AddVelocity(Vector velocity);

        public abstract void AngularAccelerate(float acceleration);

        public abstract void AddAngularVelocity(float momentum);
    }
}

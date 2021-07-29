namespace GLTech2.Behaviours
{
    /// <summary>
    /// Provides an interface to control the position of an Element. 
    /// </summary>
    public abstract class KinematicBody : Behaviour
    {
        internal KinematicBody(){ }

        /// <summary>
        /// Velocity vector of the object
        /// </summary>
        public Vector Velocity { get; set; }

        /// <summary>
        /// Speed of the object
        /// </summary>
        public float Speed => Velocity.Module;

        /// <summary>
        /// Angular velocity of the object
        /// </summary>
        public float AngularVelocity { get; set; }

        /// <summary>
        /// Applies an acceleration to the object
        /// </summary>
        /// <remarks>
        /// The speed will be updated based on how much did the frame last for.
        /// </remarks>
        /// <param name="acceleration"></param>
        public abstract void Accelerate(Vector acceleration);

        /// <summary>
        /// Directly adds a new velocity to the object.
        /// </summary>
        /// <remarks>
        /// Does not depend on frame time.
        /// </remarks>
        /// <param name="velocity"></param>
        public abstract void AddVelocity(Vector velocity);

        /// <summary>
        /// Applies an angular acceleration to the object.
        /// </summary>
        /// <remarks>
        /// The angular speed will be updated based on how much did the frame last for.
        /// </remarks>
        /// <param name="acceleration"></param>
        public abstract void AngularAccelerate(float acceleration);

        /// <summary>
        /// Directly adds a new angular velocity to the object. 
        /// </summary>
        /// <remarks>
        /// Does not depend on frame time.
        /// </remarks>
        public abstract void AddAngularVelocity(float momentum);
    }
}

using Engine.Physics;
using GLTech.World;

namespace GLTech.Scripting.Physics
{
    public class KinematicBody : Script
    {
        private Vector previousPosition;
        private Vector currentPosition;
        private Vector velocity;

        public float Radius { get; set; } = 0.1f;

        public bool CollisionEnabled { get; set; } = true;

        public Vector StartPosition { get; set; } = Vector.Zero;

        public Vector Velocity
        {
            get => velocity;
            set => velocity = value;
        }

        public float Speed => velocity.Module;

        public Vector Position => currentPosition;

        public void Accelerate(Vector acceleration) => velocity += acceleration * Time.TimeStep;

        public void AddVelocity(Vector deltaVelocity) => velocity += deltaVelocity;

        [ScriptStart]
        private void Start()
        {
            currentPosition = StartPosition;
            previousPosition = StartPosition;
            if (Entity is not null)
                Entity.WorldPosition = StartPosition;
        }

        [ScriptFixedUpdate]
        private void FixedUpdate()
        {
            previousPosition = currentPosition;

            Vector step = velocity * Time.TimeStep;

            if (CollisionEnabled)
            {
                MoveResult result = Scene.CollisionSystem.MoveAndSlide(currentPosition, step, velocity, Radius);
                currentPosition = result.newPosition;
                velocity = result.newVelocity;
            }
            else
            {
                currentPosition += step;
            }
        }

        [ScriptUpdate]
        private void Update()
        {
            if (Entity is not null)
            {
                Vector interpolated = previousPosition + (currentPosition - previousPosition) * Time.FixedRemainder;
                Entity.WorldPosition = interpolated;
            }
        }
    }
}

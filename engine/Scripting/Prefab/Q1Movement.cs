using Engine.Physics;
using GLTech.Input;
using GLTech.World;

namespace GLTech.Scripting.Prefab
{
    public sealed class Q1Movement : Script
    {
        static Logger logger = new(typeof(Q1Movement).Name);
        float zspeed = 0f;
        bool grounded = true;
        RigidBody rigidBody;

        public Q1Movement(RigidBody rigidBody)
        {
            this.rigidBody = rigidBody;
        }

        public bool AlwaysRun { get; set; } = true;
        public float TurnSpeed { get; set; } = 90f;
        public float JumpSpeed { get; set; } = 2.7f;
        public float Gravity { get; set; } = 8f;
        public float StopSpeed { get; set; } = 1f;
        public float MaxSpeed { get; set; } = 3.2f;
        public float Acceleration { get; set; } = 10f;
        public float AirAcceleration { get; set; } = 10f;
        public float Friction { get; set; } = 6f;
        public float Height { get; set; } = 0.45f;
        public ScanCode StepForward { get; set; } = ScanCode.W;
        public ScanCode StepBack { get; set; } = ScanCode.S;
        public ScanCode StepLeft { get; set; } = ScanCode.A;
        public ScanCode StepRight { get; set; } = ScanCode.D;
        public ScanCode TurnRight { get; set; } = ScanCode.Right;
        public ScanCode TurnLeft { get; set; } = ScanCode.Left;
        public ScanCode ChangeRun_Walk { get; set; } = ScanCode.LeftShift;
        public ScanCode Jump { get; set; } = ScanCode.Space;

        void Start()
        {
            if (Entity is Camera camera)
                camera.Z = Height;
            else
                logger.Error($"Entity {Entity} is not a camera.");
        }

        void Update()
        {
            if (Entity is Camera camera)
            {
                DetectJump();
                UpdateVelocity(GetMaxSpeed());
                UpdateZ(camera);

                if (Input.IsKeyDown(ScanCode.Left))
                    camera.RelativeRotation -= Time.TimeStep * TurnSpeed;
                if (Input.IsKeyDown(ScanCode.Right))
                    camera.RelativeRotation += Time.TimeStep * TurnSpeed;
            }
        }

        void DetectJump()
        {
            if ((Input.WasKeyPressed(Jump) || Input.IsKeyDown(Jump)) && grounded)
            {
                zspeed = JumpSpeed;
                grounded = false;
            }
        }

        void UpdateZ(Camera camera)
        {
            if (grounded) return;

            camera.Z += zspeed * Time.TimeStep;
            if (camera.Z < Height)
            {
                camera.Z = Height;
                grounded = true;
                zspeed = 0f;
            }

            if (camera.Z > 1f)
            {
                camera.Z = 0.9f;
                zspeed = 0f;
            }

            zspeed -= Gravity * Time.TimeStep;
        }

        float GetMaxSpeed()
        {
            bool run = AlwaysRun;
            if (Input.IsKeyDown(ChangeRun_Walk))
                run = !run;

            if (run)
                return MaxSpeed;
            else
                return MaxSpeed / 2;
        }

        void UpdateVelocity(float maxspeed)
        {
            Vector wishdir = GetWishDir();

            if (grounded)
                Accelerate(wishdir, maxspeed);
            else
                AirAccelerate(wishdir, maxspeed);

            if (rigidBody.Speed < .01f) rigidBody.Velocity = (0, 0);
        }

        void Accelerate(Vector wishdir, float wishspeed)
        {
            ApplyFriction();
            float currentspeed = Vector.DotProduct(rigidBody.Velocity, wishdir);
            float addspeed = wishspeed - currentspeed;
            if (addspeed < 0) return;
            float accelspeed = Acceleration * wishspeed * Time.TimeStep;
            if (accelspeed > addspeed)
                accelspeed = addspeed;

            rigidBody.Accelerate(accelspeed * wishdir);
        }

        void AirAccelerate(Vector wishdir, float wishspeed)
        {
            float wishspd = wishspeed;
            if (wishspd > .3f)
                wishspd = .3f;

            float currentspeed = Vector.DotProduct(rigidBody.Velocity, wishdir);
            float addspeed = wishspd - currentspeed;

            if (addspeed < 0) return;

            float accelspeed = AirAcceleration * wishspeed * Time.TimeStep;
            if (accelspeed > addspeed)
                accelspeed = addspeed;

            rigidBody.Velocity += accelspeed * wishdir;
        }

        Vector GetWishDir()
        {
            Vector result = Vector.Zero;

            if (Input.IsKeyDown(StepForward))
                result += Vector.North;
            if (Input.IsKeyDown(StepBack))
                result += Vector.South;
            if (Input.IsKeyDown(StepLeft))
                result += Vector.West;
            if (Input.IsKeyDown(StepRight))
                result += Vector.East;

            result *= Entity.RelativeDirection;

            if (result.Module == 0)
                return Vector.Zero;
            else
                return result / result.Module;
        }


        void ApplyFriction()
        {
            if (rigidBody.Speed < 0.01f)
            {
                rigidBody.Velocity = (0, 0);
                return;
            }

            float control = rigidBody.Speed < StopSpeed ? StopSpeed : rigidBody.Speed;
            float drop = control * Friction * Time.TimeStep;

            float newspeed = rigidBody.Speed - drop;
            if (newspeed < 0f)
                newspeed = 0f;
            newspeed /= rigidBody.Speed;

            rigidBody.Velocity *= newspeed;
        }
    }
}

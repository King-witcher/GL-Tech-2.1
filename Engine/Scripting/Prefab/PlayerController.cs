using GLTech;
using GLTech.Input;
using GLTech.Scripting;
using GLTech.Scripting.Physics;
using GLTech.World;

namespace Engine.Scripting.Prefab;

public abstract class PlayerController : Script
{
    private static Logger logger = new Logger(typeof(PlayerController).Name);

    private VerticalMotion vertical = new();

    protected KinematicBody? PlayerPhysics { get; private set; }

    public float MouseSensitivity { get; set; } = 2.2f;
    public bool AlwaysRun { get; set; } = true;
    public float RunSpeed { get; set; } = 3.2f;
    public float WalkSpeed { get; set; } = 1.6f;
    public float PlayerHeight { get; set; } = 0.55f;
    public float JumpSpeed { get => vertical.JumpSpeed; set => vertical.JumpSpeed = value; }
    public float Gravity { get => vertical.Gravity; set => vertical.Gravity = value; }

    public ScanCode StepForward { get; set; } = ScanCode.W;
    public ScanCode StepBack { get; set; } = ScanCode.S;
    public ScanCode StepLeft { get; set; } = ScanCode.A;
    public ScanCode StepRight { get; set; } = ScanCode.D;
    public ScanCode SlowWalk { get; set; } = ScanCode.LeftShift;
    public ScanCode Jump { get; set; } = ScanCode.Space;
    public bool Grounded => vertical.Grounded;

    [ScriptStart]
    protected void Start()
    {
        Player player = Scene.Player;
        var kinematicBody = player.GetScript<KinematicBody>();
        if (kinematicBody is not null)
        {
            PlayerPhysics = kinematicBody;
            player.Height = PlayerHeight;
        }
        else
        {
            logger.Error($"For PlayerController to work, the Scene Player must have a KinematicBody Script added.");
        }
    }

    [ScriptFixedUpdate]
    protected void FixedUpdate()
    {
        if (PlayerPhysics is null)
            return;

        vertical.FixedStep(Time.TimeStep, Input.IsKeyDown(Jump) || Input.WasKeyPressed(Jump));

        Vector velocity = PlayerPhysics.Velocity;

        bool onGround = vertical.Grounded && !vertical.Jumped;
        if (onGround)
            ApplyFriction(ref velocity);

        Vector wishDir = GetWishDir();
        float wishSpeed = GetWishSpeed();
        Vector wishVelocity = wishDir * wishSpeed;

        if (onGround)
            Accelerate(ref velocity, wishVelocity);
        else
            AirAccelerate(ref velocity, wishVelocity);

        PlayerPhysics.Velocity = velocity;
    }

    [ScriptUpdate]
    protected void Update()
    {
        Scene.Player.Rotate(Input.MouseRel.x * 0.022f * MouseSensitivity);
        Scene.Player.Height = PlayerHeight + vertical.InterpolatedDisplacement(Time.FixedRemainder);
    }

    private Vector GetWishDir()
    {
        Vector result = Vector.Zero;

        if (Input.IsKeyDown(StepForward)) result += Vector.North;
        if (Input.IsKeyDown(StepBack)) result += Vector.South;
        if (Input.IsKeyDown(StepLeft)) result += Vector.West;
        if (Input.IsKeyDown(StepRight)) result += Vector.East;

        result *= Scene.Player.WorldDirection;

        float module = result.Module;
        return module == 0f ? Vector.Zero : result / module;
    }

    private float GetWishSpeed() =>
        Input.IsKeyDown(SlowWalk) && !AlwaysRun ? WalkSpeed : RunSpeed;

    protected abstract void Accelerate(ref Vector velocity, Vector wishVelocity);
    protected abstract void AirAccelerate(ref Vector velocity, Vector wishVelocity);
    protected abstract void ApplyFriction(ref Vector velocity);
}

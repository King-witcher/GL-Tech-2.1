using GLTech;
using GLTech.Input;
using GLTech.Scripting;
using GLTech.World;

namespace Engine.Scripting.Prefab;

public abstract class PlayerController : Script
{
    static Logger logger = new Logger(typeof(PlayerController).Name);

    // When the player will be considered grounded again.
    float groundedTime = 0f;

    Vector truePosition;
    Vector trueVelocity;

    Vector lastPosition;

    Vector guessPosition;
    Vector guessVelocity;

    Vector positionError;
    Vector velocityError;

    public Vector StartPosition { get; set; } = Vector.Zero;
    public float MouseSensitivity { get; set; } = 2.2f;
    public bool AlwaysRun { get; set; } = true;
    public float TurnSpeed { get; set; } = 90f;
    public float JumpSpeed { get; set; } = 2.7f;
    public float Gravity { get; set; } = 8f;
    public float RunSpeed { get; set; } = 3.2f;
    public float WalkSpeed { get; set; } = 1.6f;
    public float Height { get; set; } = 0.45f;
    public ScanCode StepForward { get; set; } = ScanCode.W;
    public ScanCode StepBack { get; set; } = ScanCode.S;
    public ScanCode StepLeft { get; set; } = ScanCode.A;
    public ScanCode StepRight { get; set; } = ScanCode.D;
    public ScanCode TurnRight { get; set; } = ScanCode.Right;
    public ScanCode TurnLeft { get; set; } = ScanCode.Left;
    public ScanCode ChangeRun_Walk { get; set; } = ScanCode.LeftShift;
    public ScanCode Jump { get; set; } = ScanCode.Space;

    public bool IsGrounded => (Time.TotalTime - groundedTime) < 0.2f;

    [ScriptStart]
    protected void Start()
    {
        guessPosition = StartPosition;
        truePosition = StartPosition;
        lastPosition = StartPosition;

        Entity?.WorldPosition = StartPosition;

        if (Entity is Camera camera)
            camera.Z = Height;
        else
            logger.Error($"PlayerController must be added to a Camera, but was added to a {Entity}.");
    }

    [ScriptUpdate]
    protected void Update()
    {
        Entity?.Rotate(Input.MouseRel.x * 0.022f * MouseSensitivity);

        ApplyFriction(ref guessVelocity);

        var wishdir = GetWishdir();
        var wishspeed = GetWishspeed();
        Accelerate(ref guessVelocity, wishdir * wishspeed, true);
        Straighten(ref guessVelocity, ref velocityError);

        guessPosition += guessVelocity * Time.TimeStep;
        Straighten(ref guessPosition, ref positionError);

        Entity?.WorldPosition = guessPosition;
    }

    [ScriptFixedUpdate]
    protected void FixedUpdate()
    {
        ApplyFriction(ref trueVelocity);

        var wishdir = GetWishdir();
        var wishspeed = GetWishspeed();
        Accelerate(ref trueVelocity, wishdir * wishspeed, true);

        lastPosition = truePosition;
        truePosition += trueVelocity * Time.TimeStep;

        // Calculate differences
        positionError = guessPosition - truePosition;
        velocityError = guessVelocity - trueVelocity;

        Console.WriteLine(positionError.Module);
    }

    Vector GetWishdir()
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

        result *= Entity.WorldDirection;

        if (result.Module == 0)
            return Vector.Zero;
        else
            return result / result.Module;
    }

    float GetWishspeed()
    {
        return Input.IsKeyDown(ChangeRun_Walk) && !AlwaysRun ? WalkSpeed : RunSpeed;
    }

    void Straighten(ref Vector value, ref Vector error)
    {
        var distance = error.Module;
        var correctionFactor = 2f * MathF.Sqrt(2f / distance) * Time.TimeStep;
        correctionFactor = MathF.Min(correctionFactor, 1f);

        var correction = -error * correctionFactor;
        value += correction;
        error += correction;
    }

    protected abstract void Accelerate(ref Vector source, Vector wishvel, bool grounded);

    protected abstract void ApplyFriction(ref Vector source);
}

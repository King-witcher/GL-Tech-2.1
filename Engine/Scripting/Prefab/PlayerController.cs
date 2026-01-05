using GLTech;
using GLTech.Input;
using GLTech.Scripting;
using GLTech.World;

namespace Engine.Scripting.Prefab;

public abstract class PlayerController : Script
{
    static Logger logger = new Logger(typeof(PlayerController).Name);

    void Start()
    {
        if (!(Entity is Camera))
        {
            PlayerController.logger.Error($"PlayerController must be added to a Camera, but was added to a {Entity}.");
        }
    }

    void Update()
    {

    }

    void FixedUpdate()
    {

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

        result *= Entity.WorldDirection;

        if (result.Module == 0)
            return Vector.Zero;
        else
            return result / result.Module;
    }

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

    protected abstract void Accelerate(ref Vector source, Vector wishdir, float wishspeed);

    protected abstract void AirAccelerate(ref Vector source, Vector wishdir, float wishspeed);

    protected abstract void ApplyFriction(ref Vector source);
}

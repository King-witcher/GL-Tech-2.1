using GLTech;
using GLTech.Input;
using GLTech.Scripting;
using GLTech.Scripting.Physics;
using GLTech.World;

namespace Engine.Scripting.Prefab;

/// <summary>
/// Base de controle de jogador, modelada como PRODUTOR DE VELOCIDADE: lê o input,
/// aplica fricção/aceleração (deixadas para a subclasse) e escreve o resultado em
/// um <see cref="KinematicBody"/> que ele dirige por composição.
///
/// Não integra posição nem trata colisão — isso é papel do <see cref="KinematicBody"/>
/// e do serviço de colisão da engine. Aqui mora só a regra de jogo (input → desejo
/// de movimento). O olhar do mouse roda por frame (responsivo); a velocidade é
/// recalculada no passo físico fixo, logo antes de o corpo integrar.
/// </summary>
public abstract class PlayerController : Script
{
    private static Logger logger = new Logger(typeof(PlayerController).Name);

    protected KinematicBody Body { get; }

    public PlayerController(KinematicBody body)
    {
        Body = body;
    }

    public float MouseSensitivity { get; set; } = 2.2f;
    public bool AlwaysRun { get; set; } = true;
    public float RunSpeed { get; set; } = 3.2f;
    public float WalkSpeed { get; set; } = 1.6f;
    public float Height { get; set; } = 0.45f;
    public ScanCode StepForward { get; set; } = ScanCode.W;
    public ScanCode StepBack { get; set; } = ScanCode.S;
    public ScanCode StepLeft { get; set; } = ScanCode.A;
    public ScanCode StepRight { get; set; } = ScanCode.D;
    public ScanCode SlowWalk { get; set; } = ScanCode.LeftShift;

    [ScriptStart]
    protected void Start()
    {
        if (Entity is Camera camera)
            camera.Z = Height;
        else
            logger.Error($"PlayerController must be added to a Camera, but was added to a {Entity}.");
    }

    [ScriptUpdate]
    protected void Update()
    {
        // Olhar com o mouse: responsivo, atualizado a cada frame.
        Entity?.Rotate(Input.MouseRel.x * 0.022f * MouseSensitivity);
    }

    [ScriptFixedUpdate]
    protected void FixedUpdate()
    {
        // Passo determinístico: parte da velocidade atual do corpo, aplica a regra
        // de movimento e devolve. O corpo (próximo no fixed update) é quem integra
        // e resolve a colisão.
        Vector velocity = Body.Velocity;

        ApplyFriction(ref velocity);

        Vector wishDir = GetWishDir();
        float wishSpeed = GetWishSpeed();
        Accelerate(ref velocity, wishDir * wishSpeed);

        Body.Velocity = velocity;
    }

    private Vector GetWishDir()
    {
        Vector result = Vector.Zero;

        if (Input.IsKeyDown(StepForward)) result += Vector.North;
        if (Input.IsKeyDown(StepBack)) result += Vector.South;
        if (Input.IsKeyDown(StepLeft)) result += Vector.West;
        if (Input.IsKeyDown(StepRight)) result += Vector.East;

        // Leva do referencial da câmera para o mundo.
        result *= Entity?.WorldDirection ?? Vector.North;

        float module = result.Module;
        return module == 0f ? Vector.Zero : result / module;
    }

    private float GetWishSpeed() =>
        Input.IsKeyDown(SlowWalk) && !AlwaysRun ? WalkSpeed : RunSpeed;

    /// <summary>Como a subclasse acelera a velocidade rumo ao desejo de movimento.</summary>
    protected abstract void Accelerate(ref Vector velocity, Vector wishVelocity);

    /// <summary>Como a subclasse aplica fricção/desaceleração.</summary>
    protected abstract void ApplyFriction(ref Vector velocity);
}

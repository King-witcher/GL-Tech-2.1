using Engine.Scripting.Prefab;

namespace GLTech.Scripting.Prefab
{
    /// <summary>
    /// Movimentação "seca": velocidade igual ao desejo imediato, sem inércia nem
    /// fricção. Útil para debug/edição. A colisão continua a cargo do corpo.
    /// </summary>
    public sealed class FlatMovement : PlayerController
    {
        protected override void Accelerate(ref Vector velocity, Vector wishVelocity)
            => velocity = wishVelocity;

    protected override void AirAccelerate(ref Vector velocity, Vector wishVelocity) => Accelerate(ref velocity, wishVelocity);

        protected override void ApplyFriction(ref Vector velocity)
            => velocity = Vector.Zero;
    }
}

using Engine.Scripting.Prefab;
using GLTech.Scripting.Physics;

namespace GLTech.Scripting.Prefab
{
    /// <summary>
    /// Movimentação "seca": velocidade igual ao desejo imediato, sem inércia nem
    /// fricção. Útil para debug/edição. A colisão continua a cargo do corpo.
    /// </summary>
    public sealed class FlatMovement : PlayerController
    {
        public FlatMovement(KinematicBody body) : base(body) { }

        protected override void Accelerate(ref Vector velocity, Vector wishVelocity)
            => velocity = wishVelocity;

        protected override void ApplyFriction(ref Vector velocity)
            => velocity = Vector.Zero;
    }
}

using Engine.Scripting.Prefab;
using GLTech.Scripting.Physics;

namespace GLTech.Scripting.Prefab
{
    public sealed class FlatMovement : PlayerController
    {
        KinematicBody body;

        protected override void Accelerate(ref Vector source, Vector wishvel, bool grounded)
        {
            source = wishvel;
        }

        protected override void ApplyFriction(ref Vector source)
        {
            source = Vector.Zero;
        }
    }
}

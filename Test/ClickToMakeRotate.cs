using Engine;
using Engine.Input;
using Engine.World;
using Engine.Scripting;
using Engine.Scripting.Prefab;

namespace Test
{
    class ClickToMakeRotate : Script
    {
        const float accel = 5f;

        void OnFrame()
        {
            if (Keyboard.IsKeyDown(InputKey.X))
            {
                Ray ray = new Ray(Scene.Camera.WorldPosition, Scene.Camera.WorldDirection);
                Collider collider = Scene.CastRay(ray, out float distance);

                if (collider != null && collider.TryGetScript(out Move move))
                {
                    collider.VisibleBody.TryGetScript(out Move move2);
                    move.Speed += Frame.DeltaTime * accel / distance;
                    move2.Speed += Frame.DeltaTime * accel / distance;
                }
                else if (collider != null && !collider.TryGetScript<Move>(out _))
                {
                    Entity visible = collider.VisibleBody;
                    visible?.AddScript(new Move()
                    {
                        Direction = Scene.Camera.WorldDirection,
                        Speed = Frame.DeltaTime * accel / distance
                    });
                    collider.AddScript(new Move()
                    {
                        Direction = Scene.Camera.WorldDirection,
                        Speed = Frame.DeltaTime * accel / distance
                    });
                }
            }
        }
    }
}

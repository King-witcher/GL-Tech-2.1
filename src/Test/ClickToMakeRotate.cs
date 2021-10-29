using GLTech2;
using GLTech2.Entities;
using GLTech2.Scripting;
using GLTech2.Scripting.StandardScripts;

namespace Test
{
    class ClickToMakeRotate : Behaviour
    {
        const float accel = 5f;

        void OnFrame()
        {
            if (Keyboard.IsKeyDown(InputKey.X))
            {
                Ray ray = new Ray(Scene.Camera.WorldPosition, Scene.Camera.WorldDirection);
                Collider collider = Scene.RayCast(ray, out float distance);

                if (collider != null && collider.TryGetBehaviour(out Move move))
                {
                    collider.VisibleBody.TryGetBehaviour(out Move move2);
                    move.Speed += Frame.DeltaTime * accel / distance;
                    move2.Speed += Frame.DeltaTime * accel / distance;
                }
                else if (collider != null && !collider.TryGetBehaviour<Move>(out _))
                {
                    Entity visible = collider.VisibleBody;
                    visible?.AddBehaviour(new Move()
                    {
                        Direction = Scene.Camera.WorldDirection,
                        Speed = Frame.DeltaTime * accel / distance
                    });
                    collider.AddBehaviour(new Move()
                    {
                        Direction = Scene.Camera.WorldDirection,
                        Speed = Frame.DeltaTime * accel / distance
                    });
                }
            }
        }
    }
}

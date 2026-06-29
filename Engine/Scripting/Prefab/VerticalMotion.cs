namespace Engine.Scripting.Prefab;

internal sealed class VerticalMotion
{
    private float displacement = 0f;
    private float previousDisplacement = 0f;
    private float speed;

    public float Gravity { get; set; } = 8f;
    public float JumpSpeed { get; set; } = 2f;

    public bool Grounded => displacement <= 0f;

    public bool Jumped { get; private set; }

    public void FixedStep(float dt, bool jump)
    {
        previousDisplacement = displacement;
        displacement += speed * dt;
        Jumped = false;

        if (displacement <= 0f)
        {
            displacement = speed = 0f;

            if (jump)
            {
                speed = JumpSpeed;
                Jumped = true;
            }
        }
        else
        {
            speed -= Gravity * dt;
        }
    }

    public float InterpolatedDisplacement(float remainder) =>
        previousDisplacement + (displacement - previousDisplacement) * remainder;
}

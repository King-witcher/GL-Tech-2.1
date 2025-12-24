using GLTech.World;

namespace Engine.Physics;

public class RigidBody : Entity
{
    Vector truePosition;
    Vector trueVelocity;

    Vector lastPosition;

    Vector guessPosition;
    Vector guessVelocity;

    Vector positionError;
    Vector velocityError;

    public RigidBody(Vector position)
    {
        truePosition = position;
        guessPosition = position;
        lastPosition = position;

        positionError = Vector.Zero;
        trueVelocity = Vector.Zero;
        guessVelocity = Vector.Zero;
        velocityError = Vector.Zero;
    }

    public float Speed => guessVelocity.Module;

    private protected override Vector PositionData
    {
        get => guessPosition;
        set
        {
            truePosition = value;
            guessPosition = value;
            positionError = Vector.Zero;
        }
    }

    public Vector Velocity
    {
        get => guessVelocity;
        set
        {
            trueVelocity = value;
            guessVelocity = value;
            velocityError = Vector.Zero;
        }
    }

    internal void FrameUpdate(float timestep)
    {
        var velCorrection = -velocityError * MathF.Pow(2f, -timestep);
        guessVelocity += velCorrection;
        velocityError += velCorrection;

        var posCorrection = -positionError * MathF.Pow(2f, -timestep);
        guessPosition += posCorrection;
        positionError += posCorrection;
        WorldPosition = guessPosition;
    }

    internal void PhysicsUpdate(float timestep)
    {
        Console.WriteLine("here");
        // Update true position based on true velocity
        lastPosition = truePosition;
        truePosition += trueVelocity * timestep;

        // Calculate differences
        positionError = guessPosition - truePosition;
        velocityError = guessVelocity - trueVelocity;
    }

    public void Accelerate(Vector acceleration)
    {
        trueVelocity += acceleration;
    }

    public void AccelerateGuess(Vector acceleration)
    {
        guessVelocity += acceleration;
    }
}

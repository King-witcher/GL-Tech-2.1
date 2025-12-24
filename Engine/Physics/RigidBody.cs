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
        positionError = Vector.Zero;
    }

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
        throw new NotImplementedException();
    }

    internal void PhysicsUpdate(float timestep)
    {
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

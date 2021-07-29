#pragma warning disable IDE0051

namespace GLTech2.Behaviours
{
    /// <summary>
    /// A basic test script that makes the object rotate indefinitely.
    /// </summary>
    public sealed class Rotate : Behaviour
    {
        /// <summary>
        /// The angular speed the object turns in degrees per second
        /// </summary>
        public float AngularSpeed { get; set; } = 30f;

        void OnFrame() =>
            Element.Rotate(AngularSpeed * Frame.DeltaTime);
    }
}

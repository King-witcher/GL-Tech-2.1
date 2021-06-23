
namespace GLTech2.PostProcessing
{
    /// <summary>
    /// Represents a post processing effect that can be added to the Renderer.
    /// </summary>
    public abstract class Effect
    {
        public abstract void Process(PixelBuffer target);
    }
}


namespace GLTech2
{
    /// <summary>
    /// Represents a post processing effect that can be added to the Renderer.
    /// </summary>
    public abstract class PostProcessing
    {
        /// <summary>
        /// Applies the post processing effect to a target Pixelbuffer.
        /// </summary>
        /// <param name="target"></param>
        public abstract void Process(PixelBuffer target);
    }
}

using Engine.Rendering;

namespace Engine.Window
{
    public interface IWindow
    {
        (int width, int height) Size { get; set; }
        (int x, int y) Position { get; set; }
        bool RelativeMouseMode { get; set; }
        bool Fullscreen { get; set; }
        void Close();

        internal void PushDebugMsg(string msg) { }
        internal SwapchainImage AcquireImage();
        internal void Present(SwapchainImage target);
    }
}

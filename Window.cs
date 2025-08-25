
using Engine.Imaging;
using Engine.Input;
using System;
using static SDL2.SDL;

namespace Engine
{
    internal sealed class Window
    {
        Image buffer;
        IntPtr window;
        IntPtr renderer;
        IntPtr texture;
        bool fullscreen;
        (int width, int height) windowSize;

        public bool CaptureMouse { get; set; } = false;

        public Window(Image buffer, bool fullscreen = false)
        {
            this.buffer = buffer;
            this.fullscreen = fullscreen;
        }

        public void Spawn()
        {
            window = SDL_CreateWindow("GLTech 2.1", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, buffer.Width, buffer.Height, 0);
            SDL_GetWindowSize(window, out windowSize.width, out windowSize.height);
            renderer = SDL_CreateRenderer(window, -1, 0);
            texture = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STATIC, buffer.Width, buffer.Height);
            
            if (fullscreen)
            {
                SDL_SetWindowFullscreen(window, (int)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN);
            }
            SDL_SetRelativeMouseMode(SDL_bool.SDL_TRUE);
        }

        public void Update()
        {
            // SDL_RenderClear(renderer);
            SDL_UpdateTexture(texture, IntPtr.Zero, buffer.Buffer, buffer.Width * 4);
            SDL_RenderCopy(renderer, texture, IntPtr.Zero, IntPtr.Zero);
            SDL_RenderPresent(renderer);
        }

        public void PollEvents()
        {
            (int x, int y) mouseShift = (0, 0);

            while (SDL_PollEvent(out SDL_Event sdlEvent) != 0)
            {
                switch (sdlEvent.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        OnQuit?.Invoke();
                        break;
                    case SDL_EventType.SDL_KEYDOWN:
                        OnKeyDown?.Invoke((ScanCode)sdlEvent.key.keysym.scancode);
                        break;
                    case SDL_EventType.SDL_KEYUP:
                        OnKeyUp?.Invoke((ScanCode)sdlEvent.key.keysym.scancode);
                        break;
                }
            }
        }

        public (int x, int y) GetMouseShift()
        {
            SDL_GetRelativeMouseState(out int x, out int y);
            return (x, y);
        }

        public void Close()
        {
            SDL_DestroyTexture(texture);
            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);
        }

        public event Action<ScanCode> OnKeyDown;
        public event Action<ScanCode> OnKeyUp;

        public event Action OnQuit;
    }
}

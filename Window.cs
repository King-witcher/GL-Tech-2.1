
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

        public static bool CaptureMouse
        {
            get => SDL_GetRelativeMouseMode() == SDL_bool.SDL_TRUE;
            set => SDL_SetRelativeMouseMode(value ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
        }

        public Window(Image buffer, bool fullscreen = false)
        {
            this.buffer = buffer;
            this.fullscreen = fullscreen;
        }

        public void Spawn()
        {
            window = SDL_CreateWindow(
                title: "GLTech 2.1",
                x: SDL_WINDOWPOS_UNDEFINED,
                y: SDL_WINDOWPOS_UNDEFINED,
                w: buffer.Width,
                h: buffer.Height,
                flags: 0
            );
            SDL_GetWindowSize(window, out windowSize.width, out windowSize.height);
            renderer = SDL_CreateRenderer(window, -1, SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
            texture = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_RGB888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STATIC, buffer.Width, buffer.Height);

            if (fullscreen)
            {
                SDL_SetWindowFullscreen(window, (int)(SDL_WindowFlags.SDL_WINDOW_FULLSCREEN));
            }
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
            SDL_DestroyWindow(window);
            SDL_DestroyRenderer(renderer);
            SDL_DestroyTexture(texture);
        }

        public event Action<ScanCode> OnKeyDown;
        public event Action<ScanCode> OnKeyUp;

        public event Action OnQuit;
    }
}

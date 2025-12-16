
using Engine.Imaging;
using Engine.Input;
using System;
using static SDL2.SDL;

namespace Engine
{
    internal sealed class Window
    {
        Image buffer;
        nint window;
        nint renderer;
        nint texture;

        static Window()
        {
            SDL_Init(SDL_INIT_VIDEO);
            SDL_InitSubSystem(SDL_INIT_VIDEO);
        }

        internal Window(
            string title,
            int w,
            int h,
            int bufw,
            int bufh,
            bool fullscreen,
            out Image buffer
        )
        {
            this.buffer = new Image(bufw, bufh);
            buffer = this.buffer;
            Fullscreen = fullscreen;

            SDL_WindowFlags flags = SDL_WindowFlags.SDL_WINDOW_VULKAN;
            if (fullscreen) flags |= SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;

            window = SDL_CreateWindow(
                title,
                x: SDL_WINDOWPOS_UNDEFINED,
                y: SDL_WINDOWPOS_UNDEFINED,
                w,
                h,
                flags
            );

            renderer = SDL_CreateRenderer(window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            texture = SDL_CreateTexture(
                renderer,
                SDL_PIXELFORMAT_ARGB8888,
                (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STATIC,
                bufw,
                bufh
            );
        }

        public static bool CaptureMouse
        {
            get => SDL_GetRelativeMouseMode() == SDL_bool.SDL_TRUE;
            set => SDL_SetRelativeMouseMode(value ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
        }

        public bool Fullscreen
        {
            get => (SDL_GetWindowFlags(window) & (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0;
            set => SDL_SetWindowFullscreen(window, value ? (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN : 0);
        }

        internal void Update()
        {
            // SDL_RenderClear(renderer);
            SDL_UpdateTexture(texture, 0, buffer.Buffer, buffer.Width * 4);
            SDL_RenderCopy(renderer, texture, 0, 0);
            SDL_RenderPresent(renderer);
        }

        /** Processes all pending events from the event queue. */
        internal void ProcessEvents()
        {
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

        internal (int x, int y) GetMouseShift()
        {
            SDL_GetRelativeMouseState(out int x, out int y);
            return (x, y);
        }

        internal void Destroy()
        {
            SDL_DestroyTexture(texture);
            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);
            buffer.Dispose();
        }

        internal event Action<ScanCode>? OnKeyDown;
        internal event Action<ScanCode>? OnKeyUp;

        internal event Action? OnQuit;
    }
}

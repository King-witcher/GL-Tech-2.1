
using GLTech.Imaging;
using GLTech.Input;
using SDL;
using System;
using static SDL.SDL3;

namespace GLTech
{
    internal unsafe sealed class Window
    {
        Image buffer;
        SDL_Window* window;
        SDL_Renderer* renderer;
        SDL_Texture* texture;

        public Image Buffer { get => buffer; }
        public (int width, int height) Size
        {
            get
            {
                int width, height;
                SDL_GetWindowSize(window, &width, &height);
                return (width, height);
            }
            set
            {
                SDL_SetWindowSize(window, value.width, value.height);
            }
        }

        static Window()
        {
            SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO);
        }

        internal Window(
            string title,
            int width,
            int height,
            bool fullscreen
        )
        {
            buffer = new Image(width, height);
            //Fullscreen = fullscreen;

            SDL_WindowFlags flags = SDL_WindowFlags.SDL_WINDOW_MOUSE_GRABBED;
            if (fullscreen) flags |= SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;

            window = SDL_CreateWindow(
                title,
                width,
                height,
                flags
            );


            renderer = SDL_CreateRenderer(window, (byte*)null);

            texture = SDL_CreateTexture(
                renderer,
                SDL_PixelFormat.SDL_PIXELFORMAT_ARGB8888,
                SDL_TextureAccess.SDL_TEXTUREACCESS_STATIC,
                width,
                height
            );
        }

        public bool RelativeMouseMode
        {
            get => SDL_GetWindowRelativeMouseMode(window);
            set => SDL_SetWindowRelativeMouseMode(window, value);
        }

        public bool Fullscreen
        {
            get
            {
                var flags = SDL_GetWindowFlags(window);
                return (flags & SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0;
            }
            set => SDL_SetWindowFullscreen(window, value);
        }

        internal void Present()
        {
            SDL_UpdateTexture(texture, null, buffer.Buffer, buffer.Width * 4);
            SDL_RenderTexture(renderer, texture, null, null);
            SDL_RenderPresent(renderer);
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

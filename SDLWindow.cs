
using Engine.Imaging;
using System;
using static SDL2.SDL;

namespace Engine
{
    internal class SDLWindow
    {
        Image buffer;
        IntPtr window;
        IntPtr renderer;
        IntPtr texture;

        public bool CaptureMouse { get; set; } = false;

        public SDLWindow(Image buffer)
        {
            this.buffer = buffer;
        }

        public void Initialize()
        {
            window = SDL_CreateWindow("GLTech 2.1", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, buffer.Width, buffer.Height, 0);
            renderer = SDL_CreateRenderer(window, -1, 0);
            texture = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STATIC, buffer.Width, buffer.Height);
        }

        public void Update()
        {
            // SDL_RenderClear(renderer);
            SDL_UpdateTexture(texture, IntPtr.Zero, buffer.Buffer, buffer.Width * 4);
            SDL_RenderCopy(renderer, texture, IntPtr.Zero, IntPtr.Zero);
            SDL_RenderPresent(renderer);

            while (SDL_PollEvent(out SDL_Event sdlEvent) != 0)
            {
                if (sdlEvent.type == SDL_EventType.SDL_QUIT)
                {
                    OnQuit?.Invoke();
                }
                Console.WriteLine(sdlEvent.type);
            }
        }

        public void Close()
        {
            SDL_DestroyTexture(texture);
            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);
        }

        public event Action OnQuit;
    }
}

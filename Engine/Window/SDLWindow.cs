using Engine.Rendering;
using GLTech;
using GLTech.Input;
using SDL;
using static SDL.SDL3;

namespace Engine.Window
{
    public unsafe sealed class SDLWindow : IWindow
    {
        //SwapchainImage buffer;
        SDL_Window* window;
        SDL_Renderer* renderer;
        SDL_Texture* texture;
        Color* stagingBuffer;
        List<string> messages = new();

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

        public (int x, int y) Position
        {
            get
            {
                int x, y;
                SDL_GetWindowPosition(window, &x, &y);
                return (x, y);
            }
            set
            {
                SDL_SetWindowPosition(window, value.x, value.y);
            }
        }

        static SDLWindow()
        {
            SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO);
        }

        public SDLWindow(
            string title,
            int width,
            int height,
            bool fullscreen,
            bool vsync
        )
        {
            stagingBuffer = Memory.Alloc<Color>(width * height, 64);
            //Fullscreen = fullscreen;

            SDL_WindowFlags flags = SDL_WindowFlags.SDL_WINDOW_MOUSE_GRABBED;
            if (fullscreen) flags |= SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;

            window = SDL_CreateWindow(
                title,
                width,
                height,
                flags
            );

            // Find the best screen mode on the primary monitor
            var bestDisplayMode = FindDisplayMode();
            SDL_SetWindowFullscreenMode(window, &bestDisplayMode);

            renderer = SDL_CreateRenderer(window, (byte*)null);
            if (vsync) SDL_SetRenderVSync(renderer, 1);

            texture = SDL_CreateTexture(
                renderer,
                SDL_PixelFormat.SDL_PIXELFORMAT_ABGR8888,
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

        SwapchainImage IWindow.AcquireImage()
        {
            return new SwapchainImage(stagingBuffer, Size.width, Size.height);
        }

        void IWindow.PushDebugMsg(string msg)
        {
            messages.Add(msg);
        }

        void IWindow.Present(SwapchainImage rt)
        {
            // TODO: use SDL_LockTexture and SDL_UnlockTexture instead of memcpy
            SDL_UpdateTexture(texture, null, (nint)rt.buffer, rt.width * 4);
            SDL_RenderTexture(renderer, texture, null, null);
            RenderLogs();
            SDL_RenderPresent(renderer);
        }

        void RenderLogs()
        {
            var logs = Debug.LogEntries;
            for (int i = 0; i < logs.Length; i++)
            {
                var entry = logs[i];
                var rect = new SDL_FRect { x = 0, y = 12 * i, w = 8 * entry.message.Length + 4, h = 12 }; ;
                SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
                SDL_RenderFillRect(renderer, &rect);

                SDL_SetRenderDrawColor(renderer, entry.color.R, entry.color.G, entry.color.B, 255);
                SDL_RenderDebugText(renderer, 2, 12 * i + 2, entry.message);
            }
        }

        void IWindow.Close()
        {
            SDL_DestroyTexture(texture);
            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);
        }

        internal event Action<ScanCode>? OnKeyDown;
        internal event Action<ScanCode>? OnKeyUp;

        internal event Action? OnQuit;

        private SDL_DisplayMode FindDisplayMode()
        {
            int displaysCount;
            var pdisplay = SDL_GetDisplays(&displaysCount);

            if (pdisplay is null || displaysCount == 0)
            {
                SDL_free(pdisplay);
                var error = SDL_GetError();
                throw new Exception($"Couldn't find a display: {error}");
            }

#if DEBUG
            for (int i = 0; i < displaysCount; i++)
            {
                var cur = pdisplay[i];

                var name = SDL_GetDisplayName(cur);

                SDL_Rect bounds;
                if (!SDL_GetDisplayBounds(cur, &bounds))
                {
                    SDL_free(pdisplay);
                    var error = SDL_GetError();
                    throw new Exception($"Failed to get display bounds for display id {cur}: {error}");
                }

                Console.WriteLine($"Found display {cur}: {name} {bounds.w}x{bounds.h}");
            }
#endif

            // Only display 0 matters
            var display = *pdisplay;

            int count;
            var ppmode = SDL_GetFullscreenDisplayModes(display, &count);
            if (ppmode is null || count == 0)
            {
                SDL_free(pdisplay);
                var error = SDL_GetError();
                throw new Exception($"Failed to get display mode: {error}");
            }

            // Only mode 0 matters because they are sorted from best to worse.
            var pmode = *ppmode;
            if (pmode is null)
            {
                SDL_free(pdisplay);
                SDL_free(ppmode);
                var error = SDL_GetError();
                throw new Exception($"Failed to get display mode: {error}");
            }

            var result = *pmode;
            SDL_free(ppmode);
            SDL_free(pdisplay);

            return result;
        }
    }
}

using GLTech.Input;
using SDL;
using static SDL.SDL3;

namespace GLTech.Scripting
{
    partial class Script
    {
        protected internal static class Input
        {
            private static bool[] keysDown = new bool[512];
            private static bool[] keysPressed = new bool[512];
            private static byte mouseDown = 0x00;
            private static byte mousePressed = 0x00;
            public static Vector MousePosition { get; private set; } = Vector.Zero;
            public static Vector MouseRel { get; private set; } = Vector.Zero;
            public static bool ShouldExit { get; internal set; } = false;

            public static bool IsKeyDown(ScanCode scancode)
            {
                return keysDown[(nint)scancode];
            }

            public static bool WasKeyPressed(ScanCode scancode)
            {
                return keysPressed[(nint)scancode];
            }

            unsafe internal static void Update()
            {
                MouseRel = (0, 0);
                keysPressed = new bool[512];
                mousePressed = 0;

                SDL_Event sdlEvent;
                while (SDL_PollEvent(&sdlEvent))
                {
                    switch ((SDL_EventType)sdlEvent.type)
                    {
                        case SDL_EventType.SDL_EVENT_QUIT:
                            {
                                ShouldExit = true;
                                break;
                            }
                        case SDL_EventType.SDL_EVENT_KEY_DOWN:
                            {
                                var scancode = (nint)sdlEvent.key.scancode;
                                keysDown[scancode] = true;
                                keysPressed[scancode] = true;
                                break;
                            }
                        case SDL_EventType.SDL_EVENT_KEY_UP:
                            {
                                var scancode = (nint)sdlEvent.key.scancode;
                                keysDown[scancode] = false;
                                break;
                            }
                        case SDL_EventType.SDL_EVENT_MOUSE_MOTION:
                            {
                                MousePosition = new Vector(sdlEvent.motion.x, sdlEvent.motion.y);
                                MouseRel += new Vector(sdlEvent.motion.xrel, sdlEvent.motion.yrel);
                                break;
                            }
                    }
                }
            }

            internal static void Clear()
            {
                keysDown = new bool[512];
                keysPressed = new bool[512];
                mouseDown = 0x00;
                mousePressed = 0x00;
                MousePosition = Vector.Zero;
                MouseRel = Vector.Zero;
                ShouldExit = false;
            }
        }
    }
}

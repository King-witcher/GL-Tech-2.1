using SDL;
using static SDL.SDL3;

namespace Engine.Scripting
{
    public static class Input
    {
        private static bool[] keysDown = new bool[512];
        private static bool[] keysPressed = new bool[512];
        private static byte mouseDown = 0;
        private static byte mousePressed = 0;
        public static Vector MousePosition { get; private set; } = (0f, 0f);
        public static Vector MouseRel { get; private set; } = (0f, 0f);
        public static bool ShouldExit { get; internal set; } = false;

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
                            var scancode = (int)sdlEvent.key.scancode;
                            keysDown[scancode] = true;
                            keysPressed[scancode] = true;
                            break;
                        }
                    case SDL_EventType.SDL_EVENT_KEY_UP:
                        {
                            var scancode = (int)sdlEvent.key.scancode;
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
    }
}

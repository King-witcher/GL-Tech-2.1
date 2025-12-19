using static SDL2.SDL;

namespace GLTech.Input
{
    public static class Mouse
    {
        private readonly static int width;
        private readonly static int height;

        static (int x, int y) previousCursorPosition;

        static Mouse()
        {
            SDL_GetCurrentDisplayMode(0, out SDL_DisplayMode mode);
            width = mode.w;
            height = mode.h;
            int x = width / 2;
            int y = height / 2;
        }

        public static (int x, int y) MousePosition
        {
            get
            {
                SDL_GetMouseState(out int x, out int y);
                return (x, y);
            }
            set
            {
                //Cursor.Position = new Point(value.x, value.y);
            }
        }

        public static (int X, int Y) Shift
        {
            get;
            internal set;
        }

        static bool captureMouse = false;

        internal static void Enable()
        {
            if (!captureMouse)
            {
                /*Cursor.Hide();
                previousCursorPosition = (Cursor.Position.X, Cursor.Position.Y);
                Cursor.Position = new Point(MouseFixedPoint.x, MouseFixedPoint.y);
                captureMouse = true;*/
            }
        }

        internal static void Disable()
        {
            if (captureMouse)
            {
                MousePosition = previousCursorPosition;
                //Cursor.Show();
                captureMouse = false;
            }
        }
    }
}

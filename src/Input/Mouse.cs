
using System.Drawing;
using System.Windows.Forms;

namespace Engine.Input
{
    public static class Mouse
    {
        static (int x, int y) previousCursorPosition;

        static Mouse()
        {
            int x = Screen.PrimaryScreen.Bounds.Width / 2;
            int y = Screen.PrimaryScreen.Bounds.Height / 2;
            MouseFixedPoint = (x, y);
        }

        public static (int x, int y) CursorPosition
        {
            get => (Cursor.Position.X, Cursor.Position.Y);
            set => Cursor.Position = new Point(value.x, value.y);
        }

        static bool captureMouse = false;

        public static (int x, int y) MouseFixedPoint { get; set; }

        public static (int dx, int dy) Hook()
        {
            if (captureMouse)
            {
                int delta_x = CursorPosition.x - MouseFixedPoint.x;
                int delta_y = CursorPosition.y - MouseFixedPoint.y;

                CursorPosition = MouseFixedPoint;

                return (delta_x, delta_y);
            }
            else
            {
                return (0, 0);
            }
        }

        internal static void Enable()
        {
            if (!captureMouse)
            {
                Cursor.Hide();
                previousCursorPosition = (Cursor.Position.X, Cursor.Position.Y);
                Cursor.Position = new Point(MouseFixedPoint.x, MouseFixedPoint.y);
                captureMouse = true;
            }
        }

        internal static void Disable()
        {
            if (captureMouse)
            {
                CursorPosition = previousCursorPosition;
                Cursor.Show();
                captureMouse = false;
            }
        }
    }
}

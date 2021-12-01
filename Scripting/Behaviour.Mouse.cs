using System;
using System.Drawing;
using System.Windows.Forms;

namespace GLTech2.Scripting
{
    public partial class Behaviour
    {
        protected internal static class Cursor
        {
            static (int x, int y) previousCursorPosition;

            static Cursor()
            {
                int x = Screen.PrimaryScreen.Bounds.Width / 2;
                int y = Screen.PrimaryScreen.Bounds.Height / 2;
                MouseFixedPoint = (x, y);
            }

            public static (int x, int y) CursorPosition
            {
                get => (System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
                set => System.Windows.Forms.Cursor.Position = new Point(value.x, value.y);
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
                    System.Windows.Forms.Cursor.Hide();
                    previousCursorPosition = (System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y);
                    System.Windows.Forms.Cursor.Position = new Point(MouseFixedPoint.x, MouseFixedPoint.y);
                    captureMouse = true;
                }
            }

            internal static void Disable()
            {
                if (captureMouse)
                {
                    CursorPosition = previousCursorPosition;
                    System.Windows.Forms.Cursor.Show();
                    captureMouse = false;
                }
            }
        }
    }
}

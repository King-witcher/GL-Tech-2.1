using System.Drawing;
using System.Windows.Forms;

namespace GLTech2.Scripting
{
    public partial class Behaviour
    {
        // Spaguetti?
        protected internal static class Mouse
        {
            public static int HShift { get; private set; }

            public static int VShift { get; private set; }

            static bool enabled = false;
            static int centerH = Screen.PrimaryScreen.Bounds.Width / 2;
            static int centerV = Screen.PrimaryScreen.Bounds.Height / 2;
            static Point center = new Point(centerH, centerV);
            static Point previousCursorPosition;

            internal static void Enable()
            {
                if (!enabled)
                {
                    Cursor.Hide();
                    previousCursorPosition = Cursor.Position;
                    Cursor.Position = center;
                    enabled = true;
                }
            }

            internal static void Disable()
            {
                if (enabled)
                {
                    Cursor.Position = previousCursorPosition;
                    Cursor.Show();
                    enabled = false;
                    HShift = 0;
                    VShift = 0;
                }
            }

            internal static void Measure()
            {
                if (enabled)
                {
                    HShift = Cursor.Position.X - centerH;
                    VShift = Cursor.Position.Y - centerV;

                    Cursor.Position = center;
                }
            }
        }
    }
}

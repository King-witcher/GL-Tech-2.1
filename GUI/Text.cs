using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Engine;
using Engine.Imaging;

namespace Engine.GUI
{
    public class Text : GUIElement
    {
        public Text(string value, float size = 0.03f)
        {
            Value = value;
            FontSize = size;
        }

        public string Value { get; set; }
        public float FontSize { get; set; }
        public Engine.Imaging.Color Color { get; set; } = Engine.Imaging.Color.White;

        internal override void Render(Engine.Imaging.Image image)
        {
            base.Render(image);

            using Bitmap bmp = new Bitmap(image.Width, image.Height, image.Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, image.Buffer);
            using Graphics g = Graphics.FromImage(bmp);
            using Brush brush = new SolidBrush(System.Drawing.Color.FromArgb(Color.R, Color.G, Color.B));
            using Font font = new Font("Consolas", image.Height * FontSize / 1.5612f);

            RectangleF rectf = new RectangleF(WorldPosition.X * image.Width, WorldPosition.Y * image.Height, image.flt_width, image.flt_height);
            
            g.DrawString(Value, font, brush, rectf);

            g.Flush();
        }
    }
}

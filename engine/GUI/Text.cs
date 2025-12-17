using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using GLTech;
using GLTech.Imaging;

namespace GLTech.GUI
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
        public global::GLTech.Imaging.Color Color { get; set; } = global::GLTech.Imaging.Color.White;

        internal override void Render(global::GLTech.Imaging.Image image)
        {
            base.Render(image);

            using Bitmap bmp = new Bitmap(image.Width, image.Height, image.Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, image.Buffer);
            using Graphics g = Graphics.FromImage(bmp);
            using Brush brush = new SolidBrush(System.Drawing.Color.FromArgb(Color.R, Color.G, Color.B));
            using Font font = new Font("Consolas", image.Height * FontSize / 1.5612f);

            RectangleF rectf = new RectangleF(WorldPosition.X * image.Width, WorldPosition.Y * image.Height, image.widthf, image.heightf);
            
            g.DrawString(Value, font, brush, rectf);

            g.Flush();
        }
    }
}

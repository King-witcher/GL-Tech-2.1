using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Engine;
using Engine.Imaging;

namespace Engine.GUI
{
    public class Text : GUIElement
    {
        public string Value { get; set; }
        public float Size { get; set; } = 20;
        public Engine.Imaging.Color Color { get; set; } = Engine.Imaging.Color.White;

        internal override void Render(Engine.Imaging.Image image)
        {
            using Bitmap bmp = new Bitmap(image.Width, image.Height, image.Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, image.Buffer);
            using Graphics g = Graphics.FromImage(bmp);
            using Brush brush = new SolidBrush(System.Drawing.Color.FromArgb(Color.R, Color.G, Color.B));
            using Font font = new Font("Consolas", Size);

            RectangleF rectf = new RectangleF(WorldPosition.X, WorldPosition.Y, image.flt_width, image.flt_height);
            
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawString(Value, font, brush, rectf);

            g.Flush();
        }
    }
}

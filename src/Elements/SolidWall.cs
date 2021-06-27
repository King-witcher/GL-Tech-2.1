
namespace GLTech2.Elements
{
    class SolidWall : Element
    {
        private protected override Vector AbsolutePosition { get; set; }
        private protected override Vector AbsoluteNormal { get; set; }

        public SolidWall(Vector start, Vector end, Texture texture)
        {
            AbsolutePosition = start;
            AbsoluteNormal = end - start;
            new VisualPlane(start, end, texture).Parent = this;
            new PhysicalPlane(start, end).Parent = this;
        }
    }
}

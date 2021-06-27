
namespace GLTech2.Elements
{
    class SolidWall : Element
    {
        public override Vector AbsolutePosition { get; set; }
        public override Vector AbsoluteNormal { get; set; }

        public SolidWall(Vector start, Vector end, Texture texture)
        {
            AbsolutePosition = start;
            AbsoluteNormal = end - start;

            new VisualPlane(start, end, texture).ReferencePoint = this;
            new PhysicalPlane(start, end).ReferencePoint = this;
        }
    }
}

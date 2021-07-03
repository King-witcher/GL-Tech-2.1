namespace GLTech2
{
    public class PhysicalPlane : Element
    {
        public override Vector WorldPosition { get; set; }
        public override Vector WorldRotation { get; set; }

        public PhysicalPlane(Vector start, Vector end)
        {
            WorldPosition = start;
            WorldRotation = end - start;
        }
    }
}

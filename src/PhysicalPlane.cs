namespace GLTech2
{
    class PhysicalPlane : Element
    {
        public override Vector AbsolutePosition { get; set; }
        public override Vector AbsoluteNormal { get; set; }

        public PhysicalPlane(Vector start, Vector end)
        {
            AbsolutePosition = start;
            AbsoluteNormal = end - start;
        }
    }
}

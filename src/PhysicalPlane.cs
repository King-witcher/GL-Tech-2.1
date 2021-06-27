namespace GLTech2
{
    class PhysicalPlane : Element
    {
        private protected override Vector AbsolutePosition { get; set; }
        private protected override Vector AbsoluteNormal { get; set; }

        public PhysicalPlane(Vector start, Vector end)
        {
            AbsolutePosition = start;
            AbsoluteNormal = end - start;
        }
    }
}

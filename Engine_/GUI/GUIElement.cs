using GLTech.World;

namespace GLTech.GUI;

public enum VerticalAnchor : byte
{
    Top, Bottom
}

public enum HorizontalAnchor : byte
{
    Left, Right
}

public abstract class GUIElement : Entity
{
    internal GUIElement()
    {

    }

    public Vector Size { get; set; }

    public VerticalAnchor VerticalAnchor { get; set; }
    public HorizontalAnchor HorizontalAnchor { get; set; }

    internal virtual void Render(Image screen)
    {

    }
}

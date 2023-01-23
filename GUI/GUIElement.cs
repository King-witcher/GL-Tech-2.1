using Engine.World;
using Engine.Imaging;

namespace Engine.GUI;

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

    public float right;
    public float? Right { get; set; }

    public VerticalAnchor VerticalAnchor { get; set; }
    public HorizontalAnchor HorizontalAnchor { get; set; }

    internal abstract void Render(Image screen);
}

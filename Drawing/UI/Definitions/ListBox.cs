using Ion.Drawing.UI.Definitions.Renderables;

namespace Ion.Drawing.UI.Definitions;

public class ListBox : UIElement
{
    private Square _background;
    private bool Horizontal;
    private bool Scrollable = true;
    
    public ListBox(Vector2 position, Vector2 size, Vector4 color, bool horizontal = false)
    {
        this.Position = position;
        this.Size = size;
        this.Color = color;
        this.Horizontal = horizontal;
        Initialize();
    }
    
    private void Initialize()
    {
        _background = new Square(Position, Size, Color, Layer);
        _background.FlipY = true;
    }
    
    public override void DoRender()
    {
        List<UIElement> elements = new List<UIElement>();
        _background.Position = Position;
        _background.Size = Size;
        
        elements.Add(_background);
        
    }
}
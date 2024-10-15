using Ion.Common;
using Ion.Drawing.UI.Definitions.Renderables;

namespace Ion.Drawing.UI.Definitions;

public class ListBox : UIElement
{
    private Square _background;
    private bool Horizontal;
    private bool Scrollable = true;
    
    public ListBox(UIElement parent, Vector2 position, Vector2 size, Vector4 color, bool horizontal = false)
    {
        this.Position = position;
        this.Size = size;
        this.Color = color;
        this.Horizontal = horizontal;
        this.Parent = parent;
        this.Layer = parent.Layer - 1;
        Initialize();
        
    }

    public int Stride { get; set; } = 20;

    private void Initialize()
    {
        _background = new Square(Position, Size, Color, Layer);
        _background.FlipY = true;
    }
    
    public override void DoRender()
    {
        List<IRenderable> elements = new();

        Position = Parent!.Position + new Vector2(0, 20);
        _background.Position = Position;
        _background.Size = Size;
        _background.Layer = Layer - 1;
        
        elements.Add(_background);

        int index = 0 ;
        int xOffset = 0;
        int yOffset = 0;
        
        // Get the max width and height of the children
        foreach (var child in Children)
        {
            if (Horizontal)
            {
                child.Position = Position + new Vector2(xOffset, 0);
                xOffset += (int)child.Size.X + Stride;
            }
            else
            {
                child.Position = Position + new Vector2(0, yOffset);
                yOffset += (int)child.Size.Y + Stride;
            }
        }
        
        ClipRect = InteractionSystem.ViewToScreen(new Vector4(Position.X, Position.Y, Position.X + Size.X, Position.Y + Size.Y));
        InvertClipRect = true;
        foreach (var child in Children)
        {
            child.Layer = Layer - 2;
            child.Visible = true;
            
            child.InvertClipRect = true;
            child.ClipRect = ClipRect;
            
            elements.Add(child);
            
            index++;
        }
        
        if (Visible)
        {
            UIRenderer.Render(elements);
        }
    }

    public void AddElement(UIElement item)
    {
        item.Parent = this;
        item.Layer = Layer - 2;
        Children.Add(item);
    }
}
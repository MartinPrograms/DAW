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
    
    private void Initialize()
    {
        _background = new Square(Position, Size, Color, Layer);
        _background.FlipY = true;
    }
    
    public override void DoRender()
    {
        List<IRenderable> elements = new();

        Position = Parent!.Position + new Vector2(0, 20);
        this.ClipRect = InteractionSystem.ViewToScreen(new Vector4(0,0, Size.X, Size.Y));
        this.InvertClipRect = true; // makes it a mask
        _background.Position = Position;
        _background.Size = Size;
        _background.ClipRect = ClipRect;
        _background.InvertClipRect = true;
        
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
                xOffset += (int) child.Size.X;
            }
            else
            {
                child.Position = Position + new Vector2(0, yOffset);
                yOffset += (int) child.Size.Y;
            }
        }
        
        foreach (var child in Children)
        {
            child.ClipRect = ClipRect; // So they dont appear outside the listbox

            child.Layer = Layer - 1;
            child.Visible = true;
            
            child.Position = child.Position + new Vector2(0, index * xOffset);
            child.Size = new Vector2(Size.X, child.Size.Y);
            
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
        Children.Add(item);
    }
}
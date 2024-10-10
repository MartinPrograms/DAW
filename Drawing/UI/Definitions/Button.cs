using Ion.Drawing.UI.Definitions.Renderables;

namespace Ion.Drawing.UI.Definitions;

public class Button : UIElement
{
    public Vector2 Padding
    {
        get
        {
            return _text.Offset;
        }
        set
        {
            _text.Offset = value;
        }
    }

    private Square _background;
    private Text _text;

    public string Text;
    public Button(string text, Vector2 position, Vector2 size, Vector4 color, int layer)
    {
        this.Position = position;
        this.Size = size;
        this.Color = color;
        this.Text = text;
        this.Layer = layer;
        Initialize();
    }
    /*
    {
        this.Layer = 1;
        this.Visible = true;

        _background = (new Square(0, 0, 100, 100, 5f, 0.5f, Vector4.One, this.Layer)); // The button will be a square with a size of 100x100.
        _text = (new Text("Click me!", 50,50,100,100,HorizontalTextAlignment.Center, VerticalTextAlignment.Center, Vector4.One)); // The text will be centered in the button.
    }*/
    
    public void Initialize()
    {
        this.Visible = true;

        _background = (new Square(Position, Size,Vector4.One, this.Layer)); // The button will be a square with a size of 100x100.
        _text = (new Text(Text,1f,HorizontalAlignment.Center, VerticalAlignment.Center, new Vector4(0,0,0,1), this.Layer)); // The text will be centered in the button.
    }
    
    public override void DoRender()
    {
        // This also acts as an update method. Yes, I know it's bad practice.
        
        _background.Color = Color;
        _background.Position = this.Position2D;
        _background.Size = this.Size;
        _background.Layer = this.Layer - 1;
        _background.FlipY = false;
        
        _text.Position = this.Position2D; // includes padding
        _text.Layer = this.Layer - 2;
        _text.Content = this.Text;
        _text.ContentAlignmentHorizontal = HorizontalAlignment.Center;
        _text.ContentAlignmentVertical = VerticalAlignment.Center;
        _text.Parent = this;
        
        if (IsHovered)
        {
            // Reduce color brightness by 0.2
            _background.Color = new Vector4(Color.X - 0.2f, Color.Y - 0.2f, Color.Z - 0.2f, Color.W);
        }

        if (IsClicked)
        {
            _background.FlipY = true;
        }
        
        if (this.Visible)
        {
            UIRenderer.Render(new List<IRenderable>(){_background, _text});
        }
    }
}
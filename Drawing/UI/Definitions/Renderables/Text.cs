global using Font = Ion.Drawing.Rendering.Font;
using Ion.Drawing.Rendering;

namespace Ion.Drawing.UI.Definitions.Renderables;

public class Text : UIElement
{
    public string Content { get; set; }
    public float Scale { get; set; } = 1.0f;
    public HorizontalAlignment HorizontalAlignment { get; set; }
    public VerticalAlignment VerticalAlignment { get; set; }
    public float Spacing { get; set; } = 1f;
    public Font Font { get; set; }
    
    public Text(string content, float scale, HorizontalAlignment horizontalAlignment,
        VerticalAlignment verticalAlignment, Vector4 color, int layer, Vector2 padding = default, string font = "default")
    {
        this.Content = content;
        this.Scale = scale;
        this.HorizontalAlignment = horizontalAlignment;
        this.VerticalAlignment = verticalAlignment;
        this.Color = color;
        this.Layer = layer;
        this.Padding = padding;
        
        this.Font = TextManager.GetFont(font);
    }

    public Text(string content, Vector2 padding = default)
    {
        this.Content = content;
        this.Padding = padding;
        this.Color = new Vector4(0,0,0,1);
        this.Layer = 1;
        this.HorizontalAlignment = HorizontalAlignment.Left;
        this.VerticalAlignment = VerticalAlignment.Top;
        
        this.Font = TextManager.GetFont("default");
    }
    
    public void Render()
    {
        if (!Visible) return;
        this.DoRender();
    }

    public override void DoRender()
    {
        // Render the text.
        // This is a bit more complex than the square, but it's still doable.
        
        var size = Font.MeasureText(Content, Scale); // used for alignment
        
        Vector2 pos = Position;
        pos.X = Position2D.X;
        pos.Y = Position2D.Y;
        
        Vector4 AlignmentBox = new Vector4(0, 0, InteractionSystem.Resolution.X, InteractionSystem.Resolution.Y);
        
        if (Parent != null)
        {
            AlignmentBox = new Vector4(Parent.Position2D.X, Parent.Position2D.Y, Parent.Position2D.X + Parent.Size.X, Parent.Position2D.Y + Parent.Size.Y);
        }
        
        // We have to move to the middle/right/bottom of the alignment box.
        switch (ContentAlignmentHorizontal)
        {
            case HorizontalAlignment.Center:
                pos.X = AlignmentBox.X + (AlignmentBox.Z - AlignmentBox.X) / 2 - size.X / 2;
                break;
            case HorizontalAlignment.Right:
                pos.X = AlignmentBox.Z - size.X;
                break;
        }
        
        pos.X += Padding.X;
        
        switch (ContentAlignmentVertical)
        {
            case VerticalAlignment.Center:
                pos.Y = AlignmentBox.Y + (AlignmentBox.W - AlignmentBox.Y) / 2 - size.Y / 2;
                break;
            case VerticalAlignment.Bottom:
                pos.Y = AlignmentBox.W - size.Y;
                break;
        }
        
        
        pos.Y += Padding.Y;

        Font.RenderText(Content, pos, Color, Layer, Scale, Spacing, ClipRect);

    }
}

public enum HorizontalAlignment
{
    Left = 0,
    Center = 1,
    Right = 2
}

public enum VerticalAlignment
{
    Top = 0,
    Center = 1,
    Bottom = 2
}
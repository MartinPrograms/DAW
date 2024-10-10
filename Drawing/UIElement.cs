using Ion.Drawing.UI;
using Ion.Drawing.UI.Definitions.Renderables;

namespace Ion.Drawing;

public abstract class UIElement : IRenderable, IQueueItem
{
    public Guid ID { get; } = Guid.NewGuid();
    public int Layer { get; set; }
    public bool Visible { get; set; } = true;

    public Vector2 Position { get; set; }
    public Vector2 Position2D => new(Position.X + Offset.X, Position.Y + Offset.Y);

    /*
     /*
    aPos2.x = aPos2.x * size.x + position.x;
    aPos2.y = aPos2.y * -size.y + -position.y;

    aPos2.z = position.z - far / 2.0 + float(-layer) * far / 100.0;
* /
     */
    public Vector3 Position3D => new(Position.X, Position.Y, Layer * 0.1f);

    public float Rotation { get; set; }
    public Vector2 Size { get; set; }
    public Vector4 Color { get; set; }
    
    public Action OnClick { get; set; }
    public Action<Vector2> OnHover { get; set; } // passes in the mouse position, which is useful for hover effects or dragging
    public Action OnLeave { get; set; }
    
    public Action<Vector2, Vector2> OnDrag { get; set; }
    
    public Action<string[]> OnFileDrop { get; set; }
    public bool EnableFileDrop { get; set; } = false;
    public List<UIElement> Children { get; set; } = new();
    
    public bool IsClicked { get; set; }
    public bool IsHovered { get; set; }
    public Vector2 Offset { get; set; } = Vector2.Zero;
    
    public HorizontalAlignment? ContentAlignmentHorizontal { get; set; }
    public VerticalAlignment? ContentAlignmentVertical { get; set; }
    public UIElement? Parent { get; set; }
    public Vector2 Padding { get; set; } = Vector2.Zero; // Content offset from the edge of the element

    public void Update()
    {
        if (Visible)
        {
            bool isMouseOver = InteractionSystem.MouseOver3D(Position3D, Size);
            
            if (!isMouseOver && IsHovered)
            {
                OnLeave?.Invoke();
            }
            
            if (isMouseOver && InteractionSystem.MouseButtonPressed(MouseButton.Left))
            {
                IsClicked = true;
            }
            else if (IsClicked && InteractionSystem.MouseButtonReleased(MouseButton.Left))
            {
                IsClicked = false;
                OnClick?.Invoke();
            }
            
            IsHovered = isMouseOver;
            
            if (IsHovered)
            {
                OnHover?.Invoke(InteractionSystem.MousePosition);
            }
            
            if (IsClicked)
            {
                OnDrag?.Invoke(InteractionSystem.MousePosition, InteractionSystem.MouseDelta);
            }
        }
        
        this.DoRender();
    }
    
    public void Render()
    {
       
            this.Update(); // This is a bad practice, but it's fine for now
        
    }
    public abstract void DoRender();
}
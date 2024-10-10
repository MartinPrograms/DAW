using Ion.Common;
using Ion.Drawing.Tooling;
using Ion.Drawing.UI.Definitions.Renderables;

namespace Ion.Drawing.UI.Definitions;

public class Window : UIElement
{
    private Square _bar;
    private Square _content; // Will be hidden if minimized
    
    private Text _title;
    private Button _closeButton;
    private Button _minimizeButton;
    
    public string Title;
    
    public bool Minimized = false;

    public Action<Window> WantsFocus;
    public bool Focus = false;
    public void AddElement(UIElement element)
    {
        element.Layer = this.Layer - 2;
        Children.Add(element);
    }
    
    public void AddElementRange(List<UIElement> elements)
    {
        foreach (var element in elements)
        {
            AddElement(element);
        }
    }
    
    public void RemoveElement(UIElement element)
    {
        Children.Remove(element);
    }
    
    public Window(Vector2 position, Vector2 size, Vector4 color, string title)
    {
        this.Position = position;
        this.Size = size;
        this.Color = color;
        this.Title = title;
        _targetPosition = position;
        Initialize();
    }

    private Vector2 _targetPosition;
    public void Initialize()
    {
        this.Visible = true;
        
        _bar = new Square(Position, new Vector2(Size.X, 20), Color, this.Layer);
        _bar.FlipY = true;
        _bar.OnDrag = (mousePos, mouseDelta) =>
        {
            var pos = _targetPosition + mouseDelta;
            this._targetPosition = pos;
            
            WantsFocus?.Invoke(this);
        };
        
        InteractionSystem.AddInteractiveElement(_bar);
        
        _title = new Text(Title, 1f, HorizontalAlignment.Left, VerticalAlignment.Center, new Vector4(0,0,0,1), this.Layer, new Vector2(5,4));
        _closeButton = new Button("x",new Vector2(Position.X + Size.X - 20, 0), new Vector2(20, 20), new Vector4(1,0,0,1), this.Layer + 1);
        _closeButton.OnClick = () =>
        {
            this.Visible = false;
            
            WindowManager.UnregisterWindow(this);
            
            InteractionSystem.RemoveInteractiveElement(_bar);
            InteractionSystem.RemoveInteractiveElement(_closeButton);
            InteractionSystem.RemoveInteractiveElement(_minimizeButton);
            InteractionSystem.RemoveInteractiveElement(_content);
        };
        
        InteractionSystem.AddInteractiveElement(_closeButton);
        
        _minimizeButton = new Button("-",new Vector2(Position.X + Size.X - 40, -Position.Y), new Vector2(20, 20), new Vector4(0,1,0,1), this.Layer + 1);
        _minimizeButton.OnClick = () =>
        {
            Minimized = !Minimized;
        };
        
        InteractionSystem.AddInteractiveElement(_minimizeButton);
        
        // Content should be 20 pixels below the bar
        _content = new Square(new Vector2(Position.X, Position.Y + 20), new Vector2(Size.X, Size.Y - 20), new Vector4(1,1,1,1), this.Layer);
        
        _content.OnDrag = (mousePos, mouseDelta) =>
        {
            WantsFocus?.Invoke(this);
        };
        
        InteractionSystem.AddInteractiveElement(_content);
        
        WindowManager.RegisterWindow(this);
    }
    
    public override void DoRender()
    {
        if (Visible)
        {
            Position = EasingType.OutCubic.Ease(Position, _targetPosition, 6 * Time.DeltaTime);
            
            _bar.Color = Color;
            _bar.Position = this.Position;
            _bar.Size = new Vector2(this.Size.X, 20);
            _bar.Layer = this.Layer;
            _bar.FlipY = true;
            
            _title.Position = this.Position;
            _title.Layer = this.Layer - 1;
            _title.Content = this.Title;
            
            _closeButton.Position = new Vector2(Position.X + Size.X - 20, Position.Y); // 0 is top of the window
            _closeButton.Layer = this.Layer - 1;
            _closeButton.Color = new Vector4(1,0,0,1);
            
            _minimizeButton.Position = new Vector2(Position.X + Size.X - 40, Position.Y); // 0 is top of the window
            _minimizeButton.Layer = this.Layer - 1;
            _minimizeButton.Text = Minimized ? "+" : "-";
            _minimizeButton.Color = new Vector4(0, 1, 0, 1);
            
            _content.Position = new Vector2(Position.X, Position.Y + 20);
            _content.Size = new Vector2(Size.X, Size.Y - 20);
            _content.Layer = this.Layer - 1;
            _content.FlipY = false;


            if (!Focus)
            {
                _bar.Color = Color * 0.8f;
                _minimizeButton.Color = new Vector4(0,1,0,1) * 0.8f;
                _closeButton.Color = new Vector4(1,0,0,1) * 0.8f;
            }

            List<IRenderable> elements = new ();
            elements.Add(_bar);
            elements.Add(_title);
            elements.Add(_closeButton);
            elements.Add(_minimizeButton);
            
            if (!Minimized)
            {
                elements.Add(_content);
                foreach (var element in Children)
                {
                    element.Offset = new Vector2(Position.X, Position.Y + 20);
                    element.Layer = this.Layer - 2;
                    elements.Add(element);
                }
            }

            UIRenderer.Render(elements);
        }
    }
}
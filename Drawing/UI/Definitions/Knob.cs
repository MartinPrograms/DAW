using Ion.Drawing.UI.Definitions.Renderables;

namespace Ion.Drawing.UI.Definitions;

public class Knob : UIElement
{
    private Circle _background;
    private Circle _knob;
    private Text _text;
    
    private float _value;
    private float _min;
    private float _max;
    
    public string Text;
    
    private const float _maxRotation = 2.5f;
    private const float _minRotation = -2.5f;
    
    public Action<float> OnChange;
    
    public Knob(string text, Vector2 position, Vector2 size, Vector4 color, int layer, float min, float max)
    {
        this.Position = position;
        this.Size = size;
        this.Color = color;
        this.Layer = layer;
        this.Text = text;
        this._min = max;
        this._max = min;
        
        
        Initialize();
    }

    private void Initialize()
    {
        this.Visible = true;

        _background = new Circle(Position, Size, new Vector4(0.5f, 0.5f, 0.5f, 0.2f), this.Layer);
        _knob = new Circle(Position, new Vector2(Size.X / 1.3f, Size.Y / 1.3f), new Vector4(0.5f, 0.5f, 0.5f, 1), this.Layer - 2, "knob");
        OnDrag += (pos, delta) =>
        {
            Logger.Info("Dragging knob");
            
            _knob.Rotation += delta.Y * 0.01f;
            
            if (_knob.Rotation > _maxRotation)
            {
                _knob.Rotation = _maxRotation;
            }
            
            if (_knob.Rotation < _minRotation)
            {
                _knob.Rotation = _minRotation;
            }
            
            _value = (_knob.Rotation - _minRotation) / (_maxRotation - _minRotation);
            _value = _value * (_max - _min) + _min;
            
            OnChange?.Invoke(_value);
        };
        // If we don't add something as a child, it will NOT register for events.
        
        _text = new Text(Text, 1f, HorizontalAlignment.Center, VerticalAlignment.Bottom, new Vector4(0, 0, 0, 1), this.Layer - 1, new Vector2(0,7.5f), "small");
        
        InteractionSystem.AddInteractiveElement(this);
    }
    
    public override void DoRender()
    {
        _background.Color = Color;
        _background.Position = this.Position2D;
        _background.Size = this.Size;
        _background.Layer = this.Layer - 1;
        _background.FlipY = false;
        
        _knob.Color = Color - new Vector4(0.1f, 0.1f, 0.1f, 0);
        _knob.Position = this.Position2D;
        _knob.Size = new Vector2(Size.X / 1.3f, Size.Y / 1.3f);
        _knob.Layer = this.Layer - 2;
        _knob.FlipY = false;
        _knob.Parent = this;
        _knob.ContentAlignmentHorizontal = HorizontalAlignment.Center;
        _knob.ContentAlignmentVertical = VerticalAlignment.Center;
        
        _text.Position = this.Position2D; // includes padding
        _text.Layer = this.Layer - 1;
        _text.Content = this.Text;
        _text.ContentAlignmentHorizontal = HorizontalAlignment.Center;
        _text.ContentAlignmentVertical = VerticalAlignment.Bottom;
        _text.Parent = this;

        if (IsHovered)
        {
            _knob.Color = Color + new Vector4(0.1f, 0.1f, 0.1f, 0);
        }
        
        if (IsClicked)
        {
            _knob.Color = Color - new Vector4(0.3f, 0.3f, 0.3f, 0);
            _background.FlipY = true;
        }
        
        if (this.Visible)
        {
            UIRenderer.Render(new List<IRenderable>()
            {
                _background, 
                _knob, 
                _text
            });
        }
    }
}
global using Eto.Forms;
using System;
using System.Runtime.CompilerServices;
using Eto.Drawing;

namespace DAW.UI;

public class Knob : Panel
{
    public string Text { get; set; }
    public Knob()
    {
        MinValue = 0;
        MaxValue = 100;
        Step = 1;
        SnapToTick = true;
        Text = "Knob";
        
        Width = 50;
        Height = 50;
        
        ValueChanged += (sender, e) =>
        {
            
        };
        
        CreateAssets();
    }
    
    public Knob(float value)
    {
        Value = value;
        
        MinValue = 0;
        MaxValue = 100;
        Step = 1;
        SnapToTick = true;
        Text = "Knob";
        
        Width = 50;
        Height = 50;
        
        ValueChanged += (sender, e) =>
        {
            Console.WriteLine(Value);
        };
        
        CreateAssets();
    }

    void CreateAssets()
    {
        var drawable = new Drawable();
        drawable.Size = new Size(Width, Height);
        drawable.Paint += (sender, args) =>
        {
            // We need 2 circles
            var circle1 = new RectangleF(0, 0, Width, Height);
            var circle2 = new RectangleF(5, 5, Width - 10, Height - 10);
            
            // The knob
            args.Graphics.FillEllipse(Brushes.Black, circle1);
            args.Graphics.FillEllipse(Brushes.White, circle2);
            
            // The line
            var line = new Pen(Colors.Black, 2);
            
            var minAngle = -225;
            var maxAngle = 225;
            var angle = (Value - MinValue) / (MaxValue - MinValue) * (maxAngle - minAngle) + minAngle;
            
            var x = Width / 2;
            var y = Height / 2;
            var radius = Width / 2 - 5;
            var radians = angle * Math.PI / 180;
            var x2 = (float)(x + radius * Math.Cos(radians));
            var y2 = (float)(y + radius * Math.Sin(radians));
            args.Graphics.DrawLine(line, x, y, x2, y2);
            
            Console.WriteLine(angle);
        };
        
        drawable.MouseWheel += (sender, e) =>
        {
            OnMouseWheel(e);
            drawable.Invalidate();
        };
        
        drawable.MouseDown += (sender, e) =>
        {
            OnMouseDown(e);
        };
        
        drawable.MouseMove += (sender, e) =>
        {
            OnMouseMove(e);
            drawable.Invalidate();
        };
        
        drawable.MouseUp += (sender, e) =>
        {
            _dragging = false;
        };
        
        // The text
        var text = new Label()
        {
            Text = Text,
            VerticalAlignment = VerticalAlignment.Bottom,
            TextAlignment = TextAlignment.Center,
        };
        
        Content = new StackLayout()
        {
            Items =
            {
                drawable,
                text,
            },
        };
    }
    
    public float Value { get; set; }
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
    public float Step { get; set; }
    public bool SnapToTick { get; set; }
    
    public event EventHandler ValueChanged;
    
    protected virtual void OnValueChanged(EventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }
    
    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        
        if (SnapToTick)
        {
            Value = (float)Math.Round(Value / Step) * Step;
        }
        if (e.Delta.Height > 0)
        {
            Value += Step;
        }
        else
        {
            Value -= Step;
        }
        
        Console.WriteLine(Value);
        
        OnValueChanged(EventArgs.Empty);
    }
    
    private bool _dragging = false;
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        
        if (e.Buttons == MouseButtons.Primary)
        {
            _dragging = true;
        }
    }
    
    private float prevY = 0;
    private float delta = 0;
    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        
        if (e.Buttons == MouseButtons.Primary)
        {
            var y = e.Location.Y;
            
            delta = y - prevY;
            prevY = y;

            y /= 100;
            
            if (!_dragging)
            {
                return;
            }
            
            if (SnapToTick)
            {
                delta = (int)Math.Round(delta / Step) * Step;
            }
            
            Value += delta;
            OnValueChanged(EventArgs.Empty);
        }
    }
}
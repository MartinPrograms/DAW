using Eto.Forms;
using Ion.Audion;

namespace DAW.UI;

public class Channel : Panel
{
    public Channel(AudioChannel channel)
    {
        var slider = new Slider()
        {
            MinValue = 0,
            MaxValue = 100,
            Value = (int)channel.Volume,
            SnapToTick = false,
            TickFrequency = 10,
            Orientation = Orientation.Vertical,
            Width = 200,
            Height = 20,
            Enabled = true,
            ToolTip = "Volume",
        };
        
        slider.ValueChanged += (sender, e) =>
        {
            channel.Volume = slider.Value / 100f;
        };
        
        var knob = new Knob()
        {
            Value = channel.Pan,
            Width = 50,
            Height = 50,
            ToolTip = "Pan",
            MinValue = -1,
            MaxValue = 1,
            SnapToTick = false,
            Step = 0.1f,
        };
        
        Content = new StackLayout()
        {
            Items =
            {
                new Label()
                {
                    Text = channel.Name,
                },
                slider,
                knob,
            },
        };
    }
}
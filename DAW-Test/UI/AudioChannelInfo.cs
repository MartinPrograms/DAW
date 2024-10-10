using Ion.Audion;
using Ion.Drawing;
using Ion.Drawing.UI.Definitions;
using Ion.Drawing.UI.Definitions.Renderables;

namespace DAW_Test.UI;

public class AudioChannelInfo : UIElement
{
    private AudioChannel _channel;
    
    public AudioChannelInfo(AudioChannel channel)
    {
        this._channel = channel;
        
        Initialize();
    }

    private void Initialize()
    {
        var name = new Text(_channel.Name, new Vector2(10, 10));
        var volume = new Knob("Volume", new Vector2(10, 30), new Vector2(20, 20), Vector4.One, -1, 0, 1);
        var pan = new Knob("Pan", new Vector2(10, 60), new Vector2(20, 20), Vector4.One, -1, -1, 1);
        var pitch = new Knob("Pitch", new Vector2(10, 90), new Vector2(20, 20), Vector4.One, -1, 0.5f, 2);
        
        volume.OnChange += (value) =>
        {
            _channel.Volume = value;
        };
        
        pan.OnChange += (value) =>
        {
            _channel.Pan = value;
        };
        
        pitch.OnChange += (value) =>
        {
            _channel.Pitch = value;
        };
        
        Children.AddRange(new UIElement[] { name, volume, pan, pitch });
    }

    public override void DoRender()
    {
        // Do nothing
    }
}
global using OpenTK.Mathematics;
using Ion.Audion;
using Ion.Drawing;
using Ion.Drawing.UI.Definitions;

namespace DAW_Test.UI;

// Blatantly inspired by FL Studio's Channel Rack
public class SampleRack
{
    private Window? _window;
    
    private AudioChannel[] _channels;
    private AudioEngine _engine;
    
    public SampleRack(AudioEngine engine)
    {
        this._engine = engine;
        engine.GetChannels(out _channels);
        
        Initialize();
    }

    private void Initialize()
    {
        _window = new Window(new Vector2(0, 0), new Vector2(400, 200), new Vector4(0.9f, 0.9f, 0.9f, 1), "Sample Rack");

        _engine.ChannelAdded += (channel) =>
        {
            var channelInfo = new AudioChannelInfo(channel);
            _window.AddElementRange(channelInfo.Children);
        };
    }

    public UIElement GetWindow()
    {
        return _window;
    }
}
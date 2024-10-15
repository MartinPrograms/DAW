global using OpenTK.Mathematics;
using Ion.Audion;
using Ion.Drawing;
using Ion.Drawing.UI.Definitions;
using Ion.Drawing.UI.Definitions.Renderables;
using StupidSimpleLogger;

namespace DAW_Test.UI;

// Blatantly inspired by FL Studio's Channel Rack
public class SampleRack
{
    private Window? _window;
    private ListBox _listBox;
    
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
        _listBox = new ListBox(_window, new Vector2(10, 10), new Vector2(400, 180), new Vector4(0.9f, 0.9f, 0.9f, 1));

        var testText = new Text("CLIPPING RECT TEXT CLIPPING RECT TEXT CLIPPING RECT TEXT CLIPPING RECT TEXT CLIPPING RECT TEXT ");
        _listBox.AddElement(testText);
        
        _engine.ChannelAdded += ChannelAdded;
        
        _window.AddElement(_listBox);
    }

    private void ChannelAdded(AudioChannel obj)
    {
        _listBox.AddElement(new Knob("test", new Vector2(0, 0), new Vector2(50, 50), new Vector4(0.9f, 0.9f, 0.9f, 1), 0, -1,1));
    }

    public UIElement GetWindow()
    {
        return _window;
    }
}
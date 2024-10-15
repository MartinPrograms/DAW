using Ion.Audion;
using Ion.Drawing;
using Ion.Drawing.UI;
using Ion.Drawing.UI.Definitions;
using StupidSimpleLogger;

namespace DAW_Test.UI;

public class Playlist : UIElement
{
    private Window _window;
    private AudioEngine audioEngine;
    public Playlist(AudioEngine engine)
    {
        audioEngine = engine;
        
        
    }
    
    public override void DoRender()
    {
        throw new NotImplementedException();
    }
}
global using System.Collections.Generic;
using Eto.Forms;
using Ion.Audion;

namespace DAW.UI;

public class ChannelRack : Panel
{
    public ChannelRack(AudioEngine engine)
    {
        engine.ChannelAdded += (channel) =>
        {
            var channelPanel = new Channel(channel);
            StackLayout layout = (StackLayout)Content;
            layout.Items.Add(channelPanel);
            
        };
        
        Content = new StackLayout
        {
        };
    }
    
    
}
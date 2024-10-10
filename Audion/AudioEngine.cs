using StupidSimpleLogger;

namespace Ion.Audion;

public class AudioEngine
{
    public int Time { get; private set; }
    
    List<AudioChannel> channels = new List<AudioChannel>();
    public bool Loop { get; set; } = false;
    // Length in samples
    public int Length()
    {
        int length = 0;
        
        foreach (AudioChannel channel in channels)
        {
            int channelLength = channel.Length();
            
            if (channelLength > length)
            {
                length = channelLength;
            }
        }
        
        return length;
    }
    
    public void AddChannel(AudioChannel channel)
    {
        channels.Add(channel);
    }
    
    public void RemoveChannel(AudioChannel channel)
    {
        channels.Remove(channel);
    }
    
    public SampleSegment ProcessSample()
    {
        SampleSegment sample = SampleSegment.Zero;
        
        foreach (AudioChannel channel in channels)
        {
            sample += channel.GetSample(Time);
        }
        
        Time += 1;
        
        return sample;
    }
    
    public void Reset()
    {
        Time = 0;
    }

    public SampleSegment[] GetBuffer(int length, int time)
    {
        Time = time;
        var buffer = new SampleSegment[length];
        
        for (int i = 0; i < length; i++)
        {
            buffer[i] = ProcessSample();
        }
        
        return buffer;
    }
    
    public void TogglePlayback()
    {
        Audio.Instance.TogglePlayback();
    }

    public void GetChannels(out AudioChannel[] audioChannels)
    {
        audioChannels = channels.ToArray();
    }
}
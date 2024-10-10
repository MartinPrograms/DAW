namespace Ion.Audion;

public class AudioChannel
{
    public string Name;
    public float Volume;
    public float Pan;
    public float Pitch;
    
    public AudioChannel(string name)
    {
        Name = name;
        Volume = 1;
        Pan = 0;
        Pitch = 1;
    }
    
    private List<AudioEvent> events = new List<AudioEvent>();
    
    public void AddEvent(AudioEvent e)
    {
        events.Add(e);
    }
    
    public void RemoveEvent(AudioEvent e)
    {
        events.Remove(e);
    }
    
    public SampleSegment GetSample(int time)
    {
        SampleSegment sample = SampleSegment.Zero;
        
        time = (int)(time * Pitch);
        
        foreach (AudioEvent e in events)
        {
            sample += e.GetSample(time); // Does NOT need to be divided by the number of events, because this is audio mixing
        }
        
        AudioExtensions.SetVolumeLinear(ref sample, Volume);
        AudioExtensions.SetPan(ref sample, Pan);
        
        return sample;
    }

    // Length in samples
    public int Length()
    {
        int length = 0;
        
        foreach (AudioEvent e in events)
        {
            int eventLength = e.MaxEnd();
            
            if (eventLength > length)
            {
                length = eventLength;
            }
        }
        
        return length;
    }
}
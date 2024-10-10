namespace Ion.Audion;


public class AudioEvent
{
    public SampleSegment[] Data { get; private set; }
    public double StartTime { get; private set; } // in samples
    public double EndTime { get; private set; }
    public double Duration => EndTime - StartTime;
    
    public double Volume { get; set; } = 1.0;
    public double Pan { get; set; } = 0.0;

    public SampleSegment GetSample(int time)
    {
        if (time < StartTime || time > EndTime)
        {
            return SampleSegment.Zero;
        }
        
        // time is in samples
        int index = (int)((time - StartTime));
        
        // Volume and pan will be implemented later 
        // TODO: Implement volume and pan
        
        if (index >= Data.Length)
        {
            return SampleSegment.Zero;
        }
        
        return Data[index];
    }
    
    public static AudioEvent CreateFromAudioFile(AudioFile file, int startTime)
    {
        var e = new AudioEvent();
        e.EndTime = startTime + file.Length;
        e.StartTime = startTime;
        
        e.Data = file.GetSamples();
        
        return e;
    }

    public int MaxEnd()
    {
        return (int)(EndTime);
    }
}

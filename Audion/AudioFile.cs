namespace Ion.Audion;

public class AudioFile
{
    public string FilePath { get; }
    public byte[] Data { get; private set; }
    
    public string Name => System.IO.Path.GetFileName(FilePath);
    public string Extension => System.IO.Path.GetExtension(FilePath);
    public int Length { get; private set; } // In samples
    public long Size => new System.IO.FileInfo(FilePath).Length;
    
    public AudioFile(string filePath)
    {
        this.FilePath = filePath;
        
        Load();
    }

    private void Load()
    {
        var r = AudioExtensions.ReadAudioData(FilePath);
        Data = r;
        Length = r.Length / 8;
    }

    public SampleSegment[] GetSamples()
    {
        // We use 32-bit PCM format, so we can read the data as floats
        var samples = new SampleSegment[Length];
        
        for (int i = 0; i < Length; i++)
        {
            // Read the sample data
            float left = BitConverter.ToSingle(Data, i * 8);
            float right = BitConverter.ToSingle(Data, i * 8 + 4);
            
            // Create a new sample segment
            samples[i] = new SampleSegment(left, right);
        }

        return samples;
    }
}
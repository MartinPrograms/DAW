using StupidSimpleLogger; // Not best suited for daws, but we can use it for now.

namespace Ion.Audion;

/// <summary>
/// Render's a buffer of audio data.
/// It is important to note, NO EFFECTS/PANNING/ETC are applied.
/// It is purely for cross-platform audio playback, all other audio data must be processed before being passed to this class.
/// </summary>
public class Audio : IDisposable
{
    public static Audio Instance { get; private set; }
    
    public int BufferSize { get; }
    public int SampleRate { get; }

    private PortAudioSharpEngine _stream;
    public Action<int> ProcessBuffer; // used by the audio engine to process the buffer
    public unsafe Audio(int bufferSize, int sampleRate)
    {
        Logger.Info("Creating audio instance...");
        
        this.BufferSize = bufferSize;
        this.SampleRate = sampleRate;
        
        Instance = this;

        
        // Generate OpenAL buffers
        _stream = new PortAudioSharpEngine();
        /*_stream.ProcessBuffer = (int frameCount) =>
        {
            if (!_isPlaying)
            {
                return;
            }
            ProcessBuffer?.Invoke(frameCount);
        };*/
        
        _stream.Start();
        
        // Check for OpenAL errors
        
        Logger.Info("Audio instance created.");
    }
    private bool _isPlaying = false;
    public unsafe void Play()
    {
        if (_isPlaying)
        {
            return;
        }
        _isPlaying = true;

    }
    
    public void Stop()
    {
        _isPlaying = false;
    }

    private unsafe void ReleaseUnmanagedResources()
    {
        _stream.Dispose();
    }

    public void Dispose()
    {
        _stream.Stop();
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~Audio()
    {
        ReleaseUnmanagedResources();
    }

    public bool BuffersFull()
    {
        return false;
    }

    public void Close()
    {
        Dispose();
    }

    public void TogglePlayback()
    {
        if (_isPlaying)
        {
            Stop();
        }
        else
        {
            Play();
        }
    }
    
    private AudioEngine _engine;

    public void SetEngine(AudioEngine audioEngine)
    {
        _engine = audioEngine;
    }

    public SampleSegment ProcessSample()
    {
        if (_isPlaying)
            return _engine.ProcessSample();
        return SampleSegment.Zero;
    }
}

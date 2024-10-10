using System.Runtime.InteropServices;
using PortAudioSharp;
using StupidSimpleLogger;

namespace Ion.Audion;

public class PortAudioSharpEngine
{
    private PortAudioSharp.Stream stream;
    private int bufferSize = 64; // Configurable buffer size
    private int sampleRate = 44100; // Configurable sample rate
    private bool isPlaying = false;
    private int writeOffset = 0; // Track the write position in the buffer

    public bool BufferSpaceAvailable { get; private set; } = true;
    private float[] _buffer;
    private int _bufferIndex = 0;
    StreamParameters inParams = new StreamParameters();
    StreamParameters outParams = new StreamParameters();

    public PortAudioSharpEngine()
    {
        

        PortAudioSharp.PortAudio.Initialize();
        var devices = PortAudioSharp.PortAudio.DeviceCount;
        
        Logger.Info($"Found {devices} devices.");
        if (devices == 0)
        {
            Logger.Error("No audio devices found.");
            return;
        }
        
        for (int i = 0; i < devices; i++)
        {
            var deviceInfo = PortAudioSharp.PortAudio.GetDeviceInfo(i);
            Logger.Info($"Device {i}: {deviceInfo.name}");
        }
        
        inParams.device = PortAudioSharp.PortAudio.DefaultInputDevice;
        inParams.channelCount = PortAudioSharp.PortAudio.GetDeviceInfo(inParams.device).maxInputChannels; // error if wrong
        inParams.sampleFormat = SampleFormat.Float32;
        inParams.suggestedLatency = PortAudioSharp.PortAudio.GetDeviceInfo(inParams.device).defaultLowInputLatency;
        
        outParams.device = PortAudioSharp.PortAudio.DefaultOutputDevice;
        outParams.channelCount = 2;
        outParams.sampleFormat = SampleFormat.Float32;
        outParams.suggestedLatency = PortAudioSharp.PortAudio.GetDeviceInfo(outParams.device).defaultLowOutputLatency;
        
        Logger.Info($"Input device: {PortAudioSharp.PortAudio.GetDeviceInfo(inParams.device).name}");
        Logger.Info($"Output device: {PortAudioSharp.PortAudio.GetDeviceInfo(outParams.device).name}");
        
        uint framesPerBuffer = (uint)bufferSize;
        
        _buffer = new float[framesPerBuffer * 2];
        
        stream = new PortAudioSharp.Stream(inParams, outParams, sampleRate, framesPerBuffer, StreamFlags.NoFlag, StreamCallback, IntPtr.Zero); 
    }

    private unsafe StreamCallbackResult StreamCallback(IntPtr input, IntPtr output, uint framecount, ref StreamCallbackTimeInfo timeinfo, StreamCallbackFlags statusflags, IntPtr userdataptr)
    {
        float *inBuffer = (float *)input;
        float *outBuffer = (float *)output;
        ulong outIndex = 0;
        
        for (int i = 0; i < framecount; i++)
        {
            if (_bufferIndex >= _buffer.Length)
            {
                BufferSpaceAvailable = false;
                break;
            }

       
                SampleSegment sample = Audio.Instance.ProcessSample();
                _buffer[_bufferIndex] = sample.L;
                _buffer[_bufferIndex + 1] = sample.R;
            
            
            outBuffer[outIndex++] = _buffer[_bufferIndex];
            outBuffer[outIndex++] = _buffer[_bufferIndex + 1];
        }
        
        // yeah something like that idk
        
        return StreamCallbackResult.Continue;
    }

    public void Start()
    {
        stream.Start();
        isPlaying = true;
    }

    public void Stop()
    {
        stream.Stop();
        isPlaying = false;
    }

    public void Dispose()
    {
        stream.Dispose();
    }
}
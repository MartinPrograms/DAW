using System.Diagnostics;
using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.WAV;
using StupidSimpleLogger;

namespace Ion.Audion;

public static class AudioExtensions
{
    public static byte[] ReadAudioData(string path)
    {
        try
        {
            using (IWaveSource waveSource = CodecFactory.Instance.GetCodec(path))
            {
                var stereoSource = waveSource;
                // Convert to stereo and ensure it's 16-bit PCM format
                if (waveSource.WaveFormat.Channels != 2)
                {
                    Logger.Warning("Audio is not stereo. Converting to stereo.");
                    stereoSource = waveSource.ToStereo();
                }

                var pcm32 = stereoSource.ToSampleSource()
                    .ToWaveSource(32); // Convert to 16-bit PCM

                // Prepare a buffer to read the audio data
                byte[] buffer = new byte[pcm32.WaveFormat.BytesPerSecond];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = pcm32.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }

                    // Return the audio data as a byte array
                    return ms.ToArray();
                }

                // Write it back to a file
            }
        }
        catch (Exception e)
        {
            Logger.Error($"Failed to read audio data: {e.Message}");
            return null;
        }
    }

    public static int GetAudioLength(byte[] data)
    {
        return (int)(data.Length / 8); // in samples
    }

    public static byte[] GetAudioData(this SampleSegment[] samples)
    {
        List<byte> data = new List<byte>();

        foreach (SampleSegment sample in samples)
        {
            data.AddRange(sample.Data());
        }

        return data.ToArray();
    }
    
    public static float[] GetAudioDataFloat(this SampleSegment[] samples)
    {
        List<float> data = new List<float>();

        foreach (SampleSegment sample in samples)
        {
            data.Add(sample.R);
            data.Add(sample.L);
        }

        return data.ToArray();
    }
    
    private static byte[] ConvertToPCM16(byte[] float32Data)
    {
        int numSamples = float32Data.Length / 4;
        byte[] pcm16Data = new byte[numSamples * 2];

        for (int i = 0; i < numSamples; i++)
        {
            float sample = BitConverter.ToSingle(float32Data, i * 4);

            // Normalize the sample to the range [-1, 1]
            sample = Math.Clamp(sample, -1f, 1f);

            // Convert to 16-bit PCM
            short pcmSample = (short)(sample * short.MaxValue);

            // Write the 16-bit sample to the PCM data array
            BitConverter.GetBytes(pcmSample).CopyTo(pcm16Data, i * 2);
        }

        return pcm16Data;
    }

    public static void WriteAudioToWaveFile(byte[] input, string outputPath)
    {
        if (input.Length % 4 != 0)
        {
            throw new ArgumentException("input size is not a multiple of 4, indicating invalid format.");
        }
        
        // Convert the 32-bit float data to 16-bit PCM
        byte[] pcm16Data = ConvertToPCM16(input);
        
        using (WaveWriter writer = new WaveWriter(outputPath, new WaveFormat(44100, 16, 2)))
        {
            writer.Write(pcm16Data, 0, pcm16Data.Length);
        }

        if (OperatingSystem.IsWindows())
        {
            // Open the file in the default application
            Process.Start("explorer.exe", "/select, \"" + Path.GetFullPath(outputPath) + "\"");
        }
    }

    public static float DbToVolume(float db)
    {
        return (float)Math.Pow(10, db / 20);
    }
    
    public static float VolumeToDb(float volume)
    {
        return (float)(20 * Math.Log10(volume));
    }
    
    public static void SetVolumeLinear(ref SampleSegment sample, float volume)
    {
        // Clamp the volume value to the range [0, 1]
        volume = Math.Clamp(volume, 0f, 1f);
        
        // Set the volume of the left and right channels
        sample.R *= volume;
        sample.L *= volume;
    }
    
    public static void SetVolumeDb(ref SampleSegment sample, float db)
    {
        // Convert the decibel value to a linear volume value
        float volume = DbToVolume(db);
        
        // Set the volume of the left and right channels
        SetVolumeLinear(ref sample, volume);
    }

    public static void SetPan(ref SampleSegment sample, float pan)
    {
        // Clamp the pan value to the range [-1, 1]
        pan = Math.Clamp(pan, -1f, 1f);
        
        // We use the following pan formula:
        // L = cos((pan + 1) * pi / 4)
        // R = sin((pan + 1) * pi / 4)
        // This formula ensures that the sum of the squares of L and R is always 1.
        float angle = (pan + 1) * MathF.PI / 4;
        float l = MathF.Cos(angle);
        float r = MathF.Sin(angle);
        
        // Set the volume of the left and right channels
        sample.L *= l;
        sample.R *= r;
        
        // Normalize the sample to prevent clipping
        sample.Normalize();
    }
}
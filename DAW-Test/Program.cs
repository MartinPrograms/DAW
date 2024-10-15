
using Ion.Audion;
using Ion.Drawing.Tooling;
using StupidSimpleLogger;

Logger.Init();

Logger.Info("Starting... (DAW-Test)");
AssetManager.CreateDefaultFolderStructure();

if (Environment.Is64BitProcess)
{
    Logger.Info("64-bit process detected.");
}
else
{
    Logger.Info("32-bit process detected.");
}

var audio = new Audio(44100, 44100); // This is the Audio class, we can play audio with it.

var audioEngine = new AudioEngine(); // This is the AudioEngine class, this handles everything audio related, apart from playback.
audio.SetEngine(audioEngine); // Set the audio engine for the audio class.



audio.Stop();
audio.Close();

Logger.Info("Exiting... (DAW-Test)");
Logger.DumpLogs();

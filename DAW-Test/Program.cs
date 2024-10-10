using DAW_Test.UI;
using Ion.Drawing; 
using Ion.Audion;
using Ion.Drawing.Tooling;
using Ion.Drawing.UI;
using Ion.Drawing.UI.Definitions;
using OpenTK.Audio.OpenAL;
using OpenTK.Mathematics;
using OpenTK.Platform;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StupidSimpleLogger;
using Window = Ion.Drawing.UI.Definitions.Window;

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

var window = new Window<UIElement>(); // This is the Window class, we can draw ui with it. (OpenGL 3.3, supported on basically all devices)
var audio = new Audio(44100, 44100); // This is the Audio class, we can play audio with it.

var audioEngine = new AudioEngine(); // This is the AudioEngine class, this handles everything audio related, apart from playback.
audio.SetEngine(audioEngine); // Set the audio engine for the audio class.

window.CreateWindow("ION", WindowState.Maximized, VSyncMode.On); // Create a window with the title "ION", maximized, and with VSync on.

window.Load += () =>
{
    window.SetBackgroundColor(0.4f, 0.5f, 0.7f);
    var queue = window.GetRenderQueue(); // Get the render queue for UIElements.
    // To draw a ui element, we append it to the render queue, and then call window.Render(queue).
    // It does not draw in immediate mode like ImGui, this saves CPU time.
    // All queue elements have an ID, ZIndex, and a Render method.

    var dropWindow = new Window(new Vector2(-200, -200), new Vector2(200, 200), new Vector4(0.9f, 0.9f, 0.9f, 1), "Drop audio file here");
    dropWindow.EnableFileDrop = true;
    dropWindow.OnFileDrop += (files) =>
    {
        var file = files[0];
        var a = new AudioChannel(Path.GetFileNameWithoutExtension(file)[..5]);
        a.AddEvent(AudioEvent.CreateFromAudioFile(new AudioFile(file), audioEngine.Length())); // means they will play after the others
        
        audioEngine.AddChannel(a);
    };
    
    queue.Append(dropWindow);

    var channelRack = new SampleRack(audioEngine);
    queue.Append(channelRack.GetWindow());

};

window.RegisterHotkey(new[] { Keys.F10 }, () =>
{
    audioEngine.Reset();
        
    var buffer = audioEngine.GetBuffer(44100, 0);
        
    AudioExtensions.WriteAudioToWaveFile(buffer.GetAudioData(), "output.wav");
    Logger.Info("Rendered audio");
    
    
}, "Quick render audio", "Instantly renders the audio to a file.");

window.RegisterHotkey(new[] { Keys.Space }, () =>
{
    audioEngine.TogglePlayback();
    
    Logger.Info("Toggled audio playback");
}, "Toggle audio", "Toggles the audio playback.");

window.RegisterHotkey(new[] { Keys.R }, () =>
{
    audioEngine.Reset();
    audio.Stop();
    Logger.Info("Reset audio");
}, "Reset audio", "Resets the audio playback.");

window.Show(); // Show the window.

audio.Stop();
audio.Close();

Logger.Info("Exiting... (DAW-Test)");
Logger.DumpLogs();

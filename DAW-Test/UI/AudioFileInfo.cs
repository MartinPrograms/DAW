using Ion.Audion;
using Ion.Drawing;
using Ion.Drawing.UI.Definitions;
using Ion.Drawing.UI.Definitions.Renderables;
using OpenTK.Mathematics;

namespace DAW_Test.UI;

public class AudioFileInfo
{
    private Window _window;
    
    public string FilePath;
    public string FileName;
    public string FileExtension;
    public string FileSize;
    public string FileDuration;
    public AudioFile AudioFile { get; private set; }

    public AudioFileInfo(string filePath)
    {
        this.FilePath = filePath;
        this.FileName = System.IO.Path.GetFileName(filePath);
        this.FileExtension = System.IO.Path.GetExtension(filePath);
        this.FileSize = new System.IO.FileInfo(filePath).Length.ToString();
        this.FileDuration = "0"; // TODO: Implement Audio Engine
        
        LoadFile();
        Initialize();
    }

    private void LoadFile()
    {
        AudioFile = new AudioFile(FilePath);
        
        FileDuration = AudioFile.Length.ToString("0.00") + "s";
    }

    private void Initialize()
    {
        _window = new Window(new Vector2(0,0), new Vector2(400, 200), new Vector4(0.9f,0.9f,0.9f,1), FileExtension);
        
        _window.Title = Path.GetFileName(FilePath);
        
        var title = new Text(FileName, new Vector2(10,10));
        var extension = new Text(FileExtension, new Vector2(10,30));
        var size = new Text(FileSize, new Vector2(10,50));
        var duration = new Text(FileDuration, new Vector2(10,70));
        
        
        _window.AddElement(title);
        _window.AddElement(extension);
        _window.AddElement(size);
        _window.AddElement(duration);
    }

    public UIElement GetWindow()
    {
        return _window;
    }
}
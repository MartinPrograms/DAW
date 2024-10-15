using System.ComponentModel;
using Ion.Drawing.Rendering;
using Ion.Drawing.Tooling;
using Ion.Drawing.UI;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Platform;
using OpenTK.Windowing;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using StupidSimpleLogger;
using FileDropEventArgs = OpenTK.Windowing.Common.FileDropEventArgs;

namespace Ion.Drawing;

[Obsolete("Decided to switch to a premade UI library, because this just isn't worth the time.")]
public class Window<T> where T : IRenderable
{
    private GameWindow _window;
    private RenderQueue<T> _renderQueue; // easy way to manage renderables
    public float Scale { get; set; } = 1.5f; // in %, 1.0f is 100%, so 150% would be more zoomed in.
    public Vector3 BackgroundColor { get; set; }
    
    public Action<string[]> FileDropped { get; set; }

    public Action Load;

    public Window()
    {
        Instance = this;
    }
    
    public RenderQueue<T> GetRenderQueue()
    {
        _renderQueue = new RenderQueue<T>(this);
        return _renderQueue;
    }

    public void CreateWindow(string title, WindowState maximized, VSyncMode on)
    {
        var nws = NativeWindowSettings.Default;
        nws.Vsync = on.ToOpenTK();
        nws.APIVersion = new Version(3, 3);
        nws.NumberOfSamples = 64; // lol lets blow up the gpu
        
        var ws = maximized.ToOpenTK();
        
        nws.WindowState = OpenTK.Windowing.Common.WindowState.Minimized;

        var gws = GameWindowSettings.Default;

        _window = new GameWindow(gws, nws);
        _window.Title = title;

        _window.Load += () =>
        {
            ShaderManager.Initialize();
            MeshManager.Initialize();
            TextManager.Initialize();
            TextureManager.Initialize();
            /*float aspect = viewportWidth / viewportHeight;
float top = 1.0f;
float bottom = -1.0f;
float right = top * aspect;  // Adjust right based on aspect ratio
float left = -right;

glm::mat4 ortho = glm::ortho(left, right, bottom, top, near, far);

            
            */
            float aspect = (float)_window.FramebufferSize.X / _window.FramebufferSize.Y;
            Matrix4 projection = Matrix4.CreateOrthographic(this._window.FramebufferSize.X / Scale, this._window.FramebufferSize.Y / Scale, 0.001f, 1000.0f);
            InteractionSystem.Far = 1000.0f;

            ShaderManager.ProjectionMatrix = projection;
            
            Matrix4 view = Matrix4.LookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            ShaderManager.ViewMatrix = view;
            
            Logger.Info("Window loaded");
            
            Load?.Invoke();
            
            _window.WindowState = ws; 
        };
        _window.RenderFrame += WindowOnRenderFrame;    
        _window.UpdateFrame += WindowOnUpdateFrame;
        _window.Resize += WindowOnResize;
        _window.Closing += WindowOnClosing;
        _window.FileDrop += WindowOnFileDrop;
    }

    private void WindowOnFileDrop(FileDropEventArgs obj)
    {
        Logger.Info($"File dropped: {string.Join(", ", obj.FileNames)}");
        FileDropped?.Invoke(obj.FileNames);

        // To find the ui element that was dropped on, we need to get all elements currently being rendered, find which one is under the mouse, and find the one with the highest layer.
        // Then we can call the OnFileDrop method of that element.

        var element = InteractionSystem.MouseOver3D();
        if (element != null)
            if (element.EnableFileDrop)
                element?.OnFileDrop?.Invoke(obj.FileNames);

    }

    private void WindowOnClosing(CancelEventArgs obj)
    {
        Logger.Info("Closing window");
        this._renderQueue.Dispose();
        
    }

    private void WindowOnResize(ResizeEventArgs obj)
    {
        Logger.Info($"Resized to {obj.Width}x{obj.Height}");
        
        float aspect = (float)_window.FramebufferSize.X / _window.FramebufferSize.Y;
        Matrix4 projection = Matrix4.CreateOrthographic(this._window.FramebufferSize.X / Scale, this._window.FramebufferSize.Y / Scale, 0.001f, 1000.0f);
            
        ShaderManager.ProjectionMatrix = projection;
InteractionSystem.Far = 1000.0f;
    }

    private void WindowOnUpdateFrame(FrameEventArgs obj)
    {
        InteractionSystem.Resolution.X = _window.FramebufferSize.X;
        InteractionSystem.Resolution.Y = _window.FramebufferSize.Y;

        InteractionSystem.Mouse = _window.MouseState;
        
        this._window.Title = $"FPS: {1 / obj.Time:0}";
        
        InteractionSystem.Update();
        Time.Update((float)obj.Time);
        
        foreach (var hotkey in _hotkeys)
        {
            if (hotkey.IsPressed(_window.KeyboardState))
            {
                hotkey.Action();
            }
        }
     
        WindowManager.Update();
    }

    private int lastCount = 0;
    private void WindowOnRenderFrame(FrameEventArgs obj)
    {
        GL.Viewport(0, 0, _window.FramebufferSize.X, _window.FramebufferSize.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.ClearColor(BackgroundColor.X, BackgroundColor.Y, BackgroundColor.Z, 1.0f);
        GL.Enable(EnableCap.Blend); // For transparency
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.DepthTest);
        
        _renderQueue.Render();
        
        GL.Flush();
        _window.SwapBuffers();
        
        if (Logger.Logs.Count != lastCount)
        {
            lastCount = Logger.Logs.Count;
            Console.WriteLine(Logger.Logs[^1].Format());
        }
    }

    public void SetBackgroundColor(float r, float g, float b)
    {
        BackgroundColor = new Vector3(r, g, b);
    }
    
    public void SetBackgroundColor(Vector3 color)
    {
        BackgroundColor = color;
    }

    public void Show()
    {
        _window.Run();
    }

    public void RenderQueue()
    {
        _renderQueue.Render();
    }

    private List<Hotkey> _hotkeys = new List<Hotkey>();
    public void RegisterHotkey(Keys[] f1, Action action, string name = "Hotkey", string description = "No description")
    {
        _hotkeys.Add(new Hotkey(name, description, f1, action));
    }

    public static Window<T> Instance { get; set; }

    public T []GetQueueItems()
    {
        return _renderQueue.GetItems().ToArray();
    }
}
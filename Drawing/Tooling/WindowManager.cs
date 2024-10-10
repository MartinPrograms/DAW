using Window = Ion.Drawing.UI.Definitions.Window;

namespace Ion.Drawing.Tooling;

public static class WindowManager
{
    public const int LayerOffset = 10;
    
    private static List<Window> _windows = new List<Window>();
    
    public static void RegisterWindow(Window window)
    {
        _windows.Add(window);
        
        window.WantsFocus += (w) =>
        {
            // Move this window to the top
            _windows.Remove(w);
            // Append to the end
            _windows.Add(w);
        };
    }
    
    public static void UnregisterWindow(Window window)
    {
        _windows.Remove(window);
    }
    
    public static void Update()
    {
        for (int i = _windows.Count - 1; i >= 0; i--)
        {
            var window = _windows[i];
            
            window.Layer =( _windows.Count - i) * LayerOffset;
        }
    }
}
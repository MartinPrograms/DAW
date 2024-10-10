namespace Ion.Drawing;

public enum WindowState
{
    Normal,
    Minimized,
    Maximized,
    Fullscreen
}

public enum VSyncMode
{
    Off,
    On
}

public static class WindowStateExtensions
{
    public static OpenTK.Windowing.Common.WindowState ToOpenTK(this WindowState state)
    {
        return state switch
        {
            WindowState.Normal => OpenTK.Windowing.Common.WindowState.Normal,
            WindowState.Minimized => OpenTK.Windowing.Common.WindowState.Minimized,
            WindowState.Maximized => OpenTK.Windowing.Common.WindowState.Maximized,
            WindowState.Fullscreen => OpenTK.Windowing.Common.WindowState.Fullscreen,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
    
    public static OpenTK.Windowing.Common.VSyncMode ToOpenTK(this VSyncMode mode)
    {
        return mode switch
        {
            VSyncMode.Off => OpenTK.Windowing.Common.VSyncMode.Off,
            VSyncMode.On => OpenTK.Windowing.Common.VSyncMode.On,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}
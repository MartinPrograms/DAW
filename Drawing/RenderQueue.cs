using Ion.Drawing.UI;
using StupidSimpleLogger;

namespace Ion.Drawing;

public class RenderQueue<T> where T : IRenderable
{
    private List<T> _renderables = new List<T>();
    
    private Window<T> _window;
    public RenderQueue(Window<T> window)
    {
        this._window = window;
    }

    public void Render()
    {
        foreach (var renderable in _renderables)
        {
            renderable.Render();
        }
    }

    public void Dispose()
    {
        Logger.Info("Disposing render queue");
        Logger.Info($"Disposing {_renderables.Count} renderables");
        
        _renderables.Clear();
    }

    public void Append(T button)
    {
        _renderables.Add(button);
    }

    public List<T> GetItems()
    {
        return _renderables;
    }
}
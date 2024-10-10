global using OpenTK.Mathematics;
using Ion.Drawing.Rendering;
using Ion.Drawing.UI.Definitions;

namespace Ion.Drawing.UI;

public static class UIRenderer
{
    // A class that handles the rendering of UI elements.
    public static void Render(List<IRenderable> renderActions)
    {
        foreach (var action in renderActions) // Actions is a list of GL calls.
        {
            if (action == null) continue;
            action.Render();
        }
    }
}
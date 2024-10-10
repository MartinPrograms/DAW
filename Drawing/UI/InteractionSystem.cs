global using OpenTK.Windowing.GraphicsLibraryFramework;
using Ion.Drawing.Rendering;

namespace Ion.Drawing.UI;

public static class InteractionSystem {
    public static Vector2 Resolution;
    
    public static MouseState Mouse { get; set; }
    public static Vector2 MousePosition;
    public static float AspectRatio => (float)Resolution.X / Resolution.Y;
    public static Vector2 MouseDelta;
    public static float Far { get; set; }

    public static void Update()
    {
        var prevMousePosition = MousePosition;
        
        var mp = Mouse.Position;

        // Convert mouse position to NDC (normalized device coordinates)
        // With the origin at the center of the screen
        float ndcX = (2.0f * mp.X) / Resolution.X - 1.0f;
        float ndcY = 1.0f - (2.0f * mp.Y) / Resolution.Y;
        
        Vector4 mouseNDC = new Vector4(ndcX, ndcY, 0.0f, 1.0f);

        // Get the projection, view, and model matrices from the shader manager
        var projectionMatrix = ShaderManager.ProjectionMatrix;
        var viewMatrix = ShaderManager.ViewMatrix;
        var modelMatrix = Matrix4.CreateTranslation(new Vector3(0,0, 0.0f)) * Matrix4.CreateScale(new Vector3(1,-1, 1.0f));

        // Combine all matrices into a single transformation matrix
        var mvpMatrix = modelMatrix * viewMatrix * projectionMatrix;
        
        // Invert the MVP matrix to get the world position
        var invertedMVP = Matrix4.Invert(mvpMatrix);
        var worldPosition = Vector4.TransformRow(mouseNDC, invertedMVP);

        // Convert to Vector2 since we only care about 2D position (ignore Z)
        MousePosition = new Vector2(worldPosition.X, worldPosition.Y);
        
        MouseDelta = MousePosition - prevMousePosition; // world space delta
        
    }


    public static bool MouseOver(Vector2 position, Vector2 size)
    {
        return MousePosition.X >= position.X && MousePosition.X <= position.X + size.X && MousePosition.Y >= position.Y && MousePosition.Y <= position.Y + size.Y;
    }

    public static bool MouseButtonPressed(MouseButton button1)
    {
        return Mouse.IsButtonPressed(button1);
    }

    public static bool MouseButtonReleased(MouseButton button1)
    {
        return Mouse.IsButtonReleased(button1);
    }

    public static bool MouseButtonDown(MouseButton button1)
    {
        return Mouse.IsButtonDown(button1);
    }
    
    public static object GetTopElement()
    {
        // Get all elements that are currently being rendered
        var elements = Window<UIElement>.Instance.GetQueueItems().ToList();
        if (elements.Count == 0)
        {
            return null;
        }
        // Find the element that is under the mouse
        var elementsUnderMouse = elements.Where(e => MouseOver(e.Position, e.Size)).ToList();
        
        if (elementsUnderMouse.Count == 0)
        {
            return null;
        }
        
        elementsUnderMouse = elementsUnderMouse.OrderBy(e => e.Layer).ToList();

        // We need a recursive function to crawl through the children of the elements
        var find = elementsUnderMouse[0]; // grab the topmost element 
        
        return find;
    }

    public static UIElement[] GetElementsAtMouse()
    {
        var elements = Window<UIElement>.Instance.GetQueueItems().ToList();
        if (elements.Count == 0)
        {
            return new UIElement[0];
        }
        
        var elementsUnderMouse = elements.Where(e => MouseOver(e.Position, e.Size)).ToList();
        
        if (elementsUnderMouse.Count == 0)
        {
            return new UIElement[0];
        }
        
        return elementsUnderMouse.ToArray();
    }
    
    public static object GetFileDropElement<T> () where T : IRenderable
    {
        // Get all elements that are currently being rendered
        var elements = Window<UIElement>.Instance.GetQueueItems().ToList();
        if (elements.Count == 0)
        {
            return null;
        }
        // Find the element that is under the mouse
        var elementsUnderMouse = elements.Where(e => MouseOver(e.Position, e.Size) && e.EnableFileDrop).ToList();
        
        if (elementsUnderMouse.Count == 0)
        {
            return null;
        }
        
        elementsUnderMouse = elementsUnderMouse.OrderBy(e => e.Layer).ToList();

        // We need a recursive function to crawl through the children of the elements
        var find = RecursiveFindElementFileDrop(elementsUnderMouse[0]); // grab the topmost element
        
        return find;
    }
    
    public static UIElement RecursiveFindElementFileDrop(UIElement element)
    {
        // we must also check that the element is visible, and has the EnableFileDrop property set to true
        if (element.Children.Count > 0)
        {
            foreach (var child in element.Children)
            {
                if (MouseOver(child.Position, child.Size) && child.Visible && child.EnableFileDrop)
                {
                    return RecursiveFindElementFileDrop(child);
                }
            }
        }
        
        return element;
    }    
    
    
    public static UIElement? MouseOver3D()
    {
        var elements = Window<UIElement>.Instance.GetQueueItems().ToList();
        if (elements.Count == 0)
        {
            return null;
        }
        
        var elementsUnderMouse = elements.Where(e => MouseOver(e.Position2D, e.Size)).ToList();
        
        if (elementsUnderMouse.Count == 0)
        {
            return null;
        }
        
        elementsUnderMouse = elementsUnderMouse.OrderBy(e => e.Layer).ToList();

        var find = elementsUnderMouse[0]; // already top beacuse of the order by layer
        
        return find;
    }
    
    public static bool MouseOver3D(Vector3 position, Vector2 size)
    {
        var elements = Window<UIElement>.Instance.GetQueueItems().ToList();
        if (elements.Count == 0)
        {
            return false;
        }
        
        var elementsUnderMouse = elements.Where(e => MouseOver(e.Position2D, e.Size)).ToList();
        
        if (elementsUnderMouse.Count == 0)
        {
            return false;
        }
        
        elementsUnderMouse = elementsUnderMouse.OrderBy(e => e.Layer).ToList();

        Logger.Info(elementsUnderMouse.Count.ToString());
        
        if (elementsUnderMouse[0].Position3D == position && elementsUnderMouse[0].Size == size)
        {
            return true;
        }
        
        return false;
    }
}
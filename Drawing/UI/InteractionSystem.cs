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
    
    public static List<UIElement> InteractiveElements = new();
    
    public static void AddInteractiveElement(UIElement element)
    {
        InteractiveElements.Add(element);
    }
    
    public static void RemoveInteractiveElement(UIElement element)
    {
        InteractiveElements.Remove(element);
    }

    public static void Update()
    {
        var prevMousePosition = MousePosition;
        
        var mp = Mouse.Position;

        float ndcX = (2.0f * mp.X) / Resolution.X - 1.0f;
        float ndcY = 1.0f - (2.0f * mp.Y) / Resolution.Y;
        
        Vector4 mouseNDC = new Vector4(ndcX, ndcY, 0.0f, 1.0f);

        var projectionMatrix = ShaderManager.ProjectionMatrix;
        var viewMatrix = ShaderManager.ViewMatrix;
        var modelMatrix = Matrix4.CreateTranslation(new Vector3(0,0, 0.0f)) * Matrix4.CreateScale(new Vector3(1,-1, 1.0f));

        var mvpMatrix = modelMatrix * viewMatrix * projectionMatrix;
        
        var invertedMVP = Matrix4.Invert(mvpMatrix);
        var worldPosition = Vector4.TransformRow(mouseNDC, invertedMVP);

        MousePosition = new Vector2(worldPosition.X, worldPosition.Y);
        
        MouseDelta = MousePosition - prevMousePosition; // world space delta
        
    }

    public static Vector4 ViewToScreen(Vector4 rect)
    {
        var topLeft = new Vector4(rect.X, rect.Y, 0.0f, 1.0f);
        var bottomRight = new Vector4(rect.Z, rect.W, 0.0f, 1.0f);

        var projectionMatrix = ShaderManager.ProjectionMatrix;
        var viewMatrix = ShaderManager.ViewMatrix;
        var modelMatrix = Matrix4.CreateTranslation(new Vector3(0,0, 0.0f)) * Matrix4.CreateScale(new Vector3(1,-1, 1.0f));

        var mvpMatrix = modelMatrix * viewMatrix * projectionMatrix;

        var ndcTopLeft = Vector4.TransformRow(topLeft, mvpMatrix);
        var ndcBottomRight = Vector4.TransformRow(bottomRight, mvpMatrix);

        // Convert NDC to screen space
        var screenTopLeft = new Vector4(
            ((ndcTopLeft.X + 1.0f) / 2.0f) * Resolution.X,
            ((1.0f - ndcTopLeft.Y) / 2.0f) * Resolution.Y,
            0.0f, 1.0f
        );

        var screenBottomRight = new Vector4(
            ((ndcBottomRight.X + 1.0f) / 2.0f) * Resolution.X,
            ((1.0f - ndcBottomRight.Y) / 2.0f) * Resolution.Y,
            0.0f, 1.0f
        );

        return new Vector4(screenTopLeft.X, screenTopLeft.Y, screenBottomRight.X, screenBottomRight.Y);
    }

    public static Vector4 ScreenToView(Vector4 rect)
    {
        float ndcX = (2.0f * rect.X) / Resolution.X - 1.0f;
        float ndcY = 1.0f - (2.0f * rect.Y) / Resolution.Y;
        
        Vector4 mouseNDC = new Vector4(ndcX, ndcY, 0.0f, 1.0f);

        var projectionMatrix = ShaderManager.ProjectionMatrix;
        var viewMatrix = ShaderManager.ViewMatrix;
        var modelMatrix = Matrix4.CreateTranslation(new Vector3(0,0, 0.0f)) * Matrix4.CreateScale(new Vector3(1,-1, 1.0f));

        var mvpMatrix = modelMatrix * viewMatrix * projectionMatrix;
        
        var invertedMVP = Matrix4.Invert(mvpMatrix);
        var worldPosition = Vector4.TransformRow(mouseNDC, invertedMVP);

        var ndcX2 = (2.0f * rect.Z) / Resolution.X - 1.0f;
        var ndcY2 = 1.0f - (2.0f * rect.W) / Resolution.Y;
        
        Vector4 mouseNDC2 = new Vector4(ndcX2, ndcY2, 0.0f, 1.0f);
            
        var worldPosition2 = Vector4.TransformRow(mouseNDC2, invertedMVP);
        
        return new Vector4(worldPosition.X, worldPosition.Y, worldPosition2.X, worldPosition2.Y);
    }


    public static bool MouseOver(Vector2 position, Vector2 size, Vector4 clipRect)
    {
        if (clipRect != Vector4.Zero)
        {
            clipRect = ViewToScreen(clipRect);
            var inRect = MousePosition.X >= clipRect.X && MousePosition.X <= clipRect.Z && MousePosition.Y >= clipRect.Y && MousePosition.Y <= clipRect.W; // because mouse position is already in world space.
            if (!inRect)
            {
                return false;
            }
        }
        
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
    


    public static bool MouseOver3D(Vector2 position, Vector2 size, int layer) // Z component is layer
    {
        var elements = InteractiveElements;
        var hoveredElements = elements.Where(e => MouseOver(e.Position2D, e.Size, e.ClipRect)).ToList();
        
        if (hoveredElements.Count == 0)
        {
            return false;
        }
        
        // Get the top most element
        var topElement = hoveredElements.OrderBy(e => e.Layer).ToList()[0];
        
        return topElement.Layer == layer && topElement.Position2D == position && topElement.Size == size;
    }
    
    public static UIElement? MouseOver3D()
    {
        var elements = InteractiveElements;
        var hoveredElements = elements.Where(e => MouseOver(e.Position2D, e.Size, e.ClipRect)).ToList();
        
        if (hoveredElements.Count == 0)
        {
            return null;
        }
        
        // Get the top most element
        var topElement = hoveredElements.OrderBy(e => e.Layer).ToList()[0];
        
        return topElement;
    }
}
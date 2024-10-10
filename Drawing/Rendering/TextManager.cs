using Ion.Common;
using Ion.Drawing.UI;
using SkiaSharp; // For rendering text.

namespace Ion.Drawing.Rendering;

public static class TextManager
{
    private static Dictionary<string, Font> fonts = new Dictionary<string, Font>();
    
    public static Font LoadFont(string name, int size)
    {
        var f = new Font(name, size);
        fonts.Add(name, f);
        return f;
    }
    
    public static void RenderText(string fontName, string text, Vector2 position, Vector4 color, int layer = 0, float scale = 1.0f, float spacing = 1.0f)
    {
        if (!fonts.ContainsKey(fontName))
        {
            Logger.Error($"Font '{fontName}' not found.");
            return;
        }
        
        fonts[fontName].RenderText(text, position, color, layer, scale, spacing);
    }
    
    public static void UnloadFont(string name)
    {
        fonts.Remove(name);
    }
    
    public static Font GetFont(string name)
    {
        if (!fonts.ContainsKey(name))
        {
            Logger.Error($"Font '{name}' not found.");
            return null;
        }
        return fonts[name];
    }
    
    public static void Initialize()
    {
        LoadFont("default", 16).Load("Assets/Fonts/AfacadFlux-Regular.ttf");
        LoadFont("title", 24).Load("Assets/Fonts/AfacadFlux-Bold.ttf");
        LoadFont("small", 12).Load("Assets/Fonts/AfacadFlux-Regular.ttf");
    }
}

public struct Character
{
    public int TextureID;
    public Vector2 Size;
    public Vector2 Bearing;
    public int Advance;
}

public class Font : IDisposable
{
    public static float RenderScale = 8.0f; // Quality of the text.
    
    public string Name;
    public int Size;
    public Dictionary<char, Character> Characters;
    
    /// <summary>
    /// Size is in pixels, not points. (1 point = 1.333 pixels)
    /// </summary>
    /// <param name="name"></param>
    /// <param name="size"></param>
    public Font(string name, int size)
    {
        this.Name = name;
        this.Size = size;
        this.Characters = new Dictionary<char, Character>();
    }
    
    public void Load(string ttfPath)
    {
        // Load the font.
        if (!File.Exists(ttfPath))
        {
            Logger.Error($"Font '{ttfPath}' not found.");
            return;
        }
        
        
        var typeface = SKTypeface.FromFile(ttfPath);
        
        // Generate the characters.
        var paint = new SKFont()
        {
            Typeface = typeface,
            Size = this.Size * RenderScale,
            Edging = SKFontEdging.SubpixelAntialias, Hinting = SKFontHinting.Full, Subpixel = true, Embolden = false
        };
        
        

        /*
        string text = "A"; // You can change this to measure other characters

        // Get glyph indices for the text
        ushort[] glyphs = paint.Typeface.GetGlyphs(text);

        // Get advance, bounds, and bearing for the first glyph
        if (glyphs.Length > 0)
        {
            SKRect[] bounds;
            var advance = paint.GetGlyphWidths(glyphs, out bounds);
            var origin = paint.GetGlyphPositions(glyphs);

            // Print the results
            Logger.Info($"Advance: {advance[0]}");
            Logger.Info($"Bounds: {bounds[0]}");
            Logger.Info($"Origin: {origin[0]}");
        }
        */

        var metrics = paint.Metrics;
        // Generate the characters, we want to generate all from 32 to 126.
        for (int i = 32; i < 127; i++)
        {
            var c = ((char)i).ToString();
            var glyph = paint.Typeface.GetGlyphs(c);
            
            SKRect[] bounds;
            var advance = paint.GetGlyphWidths(glyph, out bounds);
            var origin = paint.GetGlyphPositions(glyph, new SKPoint(0,0));
            
            var bound = bounds[0];
            bound.Size = new SKSize(bound.Width, bound.Height+4); // Add 1 to the height to prevent clipping.
            bound.Offset(0, -4); // Offset the bounds to prevent clipping.

            var descender =(bound.Top - metrics.Ascent- metrics.Descent) + 2; // 2 (half of the 4) is the offset.
            
            this.Characters.Add((char)i, new Character()
            {
                TextureID = TextureManager.LoadFromFont(paint, c[0].ToString(), bound),
                Size = new Vector2(bound.Width / RenderScale, bound.Height / RenderScale),
                Bearing = new Vector2(origin[0].X / RenderScale, descender / RenderScale),
                Advance = (int)(advance[0] / RenderScale)
            });
        }
        
        // Dispose the paint.
        paint.Dispose();
        Logger.Info($"Font '{this.Name}' loaded.");
    }
    
    public void RenderText(string text, Vector2 position, Vector4 color, int layer = 0, float scale = 1.0f, float spacing = 1.0f)
    {
        // Render the text.
        
        // Set the shader.
        var square = MeshManager.GetSquare();
        square.Bind();
        
        var shader = ShaderManager.GetShader("text");
        shader.Use();
        
        // Set the color.
        shader.SetVector4("color", color);
        
        shader.SetMatrix4("view", ShaderManager.ViewMatrix);
        shader.SetMatrix4("projection", ShaderManager.ProjectionMatrix);
        shader.SetFloat("aspectRatio", InteractionSystem.AspectRatio);
        shader.SetFloat("far", InteractionSystem.Far);
        
        // Set the resolution.
        shader.SetVector2("resolution", InteractionSystem.Resolution);
        
        // Set the layer.
        shader.SetInt("layer", layer);
        
        // Render each character.
        float x = position.X;
        float y = position.Y;
        
        
        foreach (var c in text)
        {
            if (!this.Characters.ContainsKey(c))
            {
                Logger.Error($"Character '{c}' not found in font '{this.Name}'.");
                continue;
            }
            
            var character = this.Characters[c];
            
            // Set the position.
            var pos = new Vector3(x + character.Bearing.X, y + character.Bearing.Y, layer * 0.1f);
            pos.X *= scale;
            pos.Y *= scale;
            
            shader.SetVector3("position", pos);
            shader.SetVector2("size", character.Size);
            
            var model = Matrix4.CreateTranslation(new Vector3(0,0, 0.0f)) * Matrix4.CreateScale(new Vector3(1,1, 1.0f));
            shader.SetMatrix4("model", model);
            
            // Bind the texture.
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2d, character.TextureID);
            shader.SetInt("tex", 0);
            
            square.Render();
            
            // Move the cursor.
            x += character.Advance * scale * spacing;
        }
        
        square.Unbind();
        shader.Stop();
        
        // Unbind the texture.
        GL.BindTexture(TextureTarget.Texture2d, 0);
        
    }
    
    public void Dispose()
    {
        foreach (var c in this.Characters)
        {
            GL.DeleteTexture(c.Value.TextureID);
        }
    }

    public Vector2 MeasureText(string content, float scale)
    {
        float x = 0;
        float y = 0;
        
        float[] yValues = new float[content.Length];

        int i = 0;
        foreach (var c in content)
        {
            if (!this.Characters.ContainsKey(c))
            {
                Logger.Error($"Character '{c}' not found in font '{this.Name}'.");
                continue;
            }
            
            var character = this.Characters[c];
            
            x += character.Advance * scale;
            yValues[i] = character.Size.Y;
            i++;
        }
        
        // average the y values.
        y = yValues.Max();
        y *= scale;
        
        return new Vector2(x, y);
    }
}
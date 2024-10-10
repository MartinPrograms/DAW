using SkiaSharp;
using StbImageSharp;

namespace Ion.Drawing.Rendering;

public static class TextureManager
{
    private static Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
    
    public static Texture LoadTexture(string name, string path)
    {
        var t = new Texture(name, path);
        textures.Add(name, t);
        return t;
    }
    
    public static void UnloadTexture(string name)
    {
        textures.Remove(name);
    }
    
    public static Texture GetTexture(string name)
    {
        if (!textures.ContainsKey(name))
        {
            Logger.Error($"Texture '{name}' not found.");
            return null;
        }
        return textures[name];
    }
    
    public static void Initialize()
    {
        LoadTexture("panel", "Assets/Textures/panel.svg");
        LoadTexture("circle", "Assets/Textures/circle.svg");
        LoadTexture("knob", "Assets/Textures/knob.svg");
    }

    public static int LoadFromFont(SKFont paint, string character, SKRect bounds) // These are NOT added to the dictionary, as these are textures in fonts. They are meant to be used only by the text renderer.
    // The font should also remove the textures when it is disposed.
    {
        if (character == " ") return 0; // No texture for space.
        if (character == "\n") return 0; // No texture for newline.
        if (character == "\t") return 0; // No texture for tab.
        if (character == "\r") return 0; // No texture for carriage return.
        if (character == "\0") return 0; // No texture for null character.
        if (character == "\v") return 0; // No texture for vertical tab.
        if (character == "\f") return 0; // No texture for form feed.
        if (string.IsNullOrWhiteSpace(character)) return 0; // No texture for whitespace.
        
        var skPaint = new SKPaint
        {
            Color = SKColors.White, // 1,1,1,1, colors will be applied in the shader. This saves memory.
            IsAntialias = true,
        };
        
        SKBitmap bmp = new SKBitmap((int)bounds.Width, (int)bounds.Height);
        SKCanvas canvas = new SKCanvas(bmp);
        
        canvas.Clear(SKColors.Transparent);
        

        var x = -bounds.Left;
        var baseY = bounds.Height;
        var y = baseY - bounds.Bottom;        
        canvas.DrawText(character, x, y, paint, skPaint);
        
        canvas.Flush();

        SKImage img = SKImage.FromBitmap(bmp);
        SKData data = img.Encode();
        
        int id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2d, id);
        
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        StbImage.stbi_set_flip_vertically_on_load(1);
        ImageResult result = ImageResult.FromMemory(data.ToArray());
        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, result.Width, result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, result.Data);
        
        //GL.GenerateMipmap(TextureTarget.Texture2d);
        
        GL.BindTexture(TextureTarget.Texture2d, 0);
        
        return id;
    }

    public static void UseTexture(string panel)
    {
        var texture = GetTexture(panel);
        if (texture == null) return;
        texture.Bind();
    }
}
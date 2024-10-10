using SkiaSharp;
using StbImageSharp;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using Svg.Skia;
using SKSvg = Svg.Skia.SKSvg;

namespace Ion.Drawing.Rendering;

public class Texture
{
    public string Name;
    public int ID;
    
    public Texture(string name, string path)
    {
        Name = name;
        ID = LoadTexture(path);
    }
    
    private int LoadTexture(string path)
    {
        SKBitmap bmp = SKBitmap.Decode(path);
                
        if (Path.GetExtension(path) == ".svg")
        {
            var svg = new SKSvg();
            svg.Load(path);
            bmp = new SKBitmap((int)svg.Picture.CullRect.Width, (int)svg.Picture.CullRect.Height);
            using (SKCanvas canvas = new SKCanvas(bmp))
            {
                canvas.Clear(SKColors.Transparent);
                canvas.DrawPicture(svg.Picture);
            }
            
            Logger.Info($"SVG loaded: {path}");
        }
        
        int id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2d, id);
        
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        
        SKImage img = SKImage.FromBitmap(bmp);
        SKData data = img.Encode();
        
        var result = ImageResult.FromMemory(data.ToArray());
        
        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, result.Width, result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, result.Data);
        
        GL.GenerateMipmap(TextureTarget.Texture2d);
        
        GL.BindTexture(TextureTarget.Texture2d, 0);
        
        Logger.Info($"Texture loaded: {path}");
        
        return id;
    }
    
    public void Bind(int slot = 0)
    {
        GL.ActiveTexture(TextureUnit.Texture0 + (uint)slot);
        GL.BindTexture(TextureTarget.Texture2d, ID);
    }
}
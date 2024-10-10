global using StupidSimpleLogger;

namespace Ion.Drawing.Rendering;

public static class ShaderManager
{
    private static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();
    
    public static void LoadShader(string name, string vertexPath, string fragmentPath)
    {
        shaders.Add(name, new Shader(vertexPath, fragmentPath));
    }
    
    public static void UseShader(string name)
    {
        if (!shaders.ContainsKey(name))
        {
            Logger.Error($"Shader '{name}' not found.");
            return;
        }
        
        shaders[name].Use();
    }
    
    public static void UnloadShader(string name)
    {
        shaders[name].Dispose();
        shaders.Remove(name);
    }
    
    public static Shader GetShader(string name)
    {
        if (!shaders.ContainsKey(name))
        {
            Logger.Error($"Shader '{name}' not found.");
            return null;
        }
        return shaders[name];
    }
    
    public static void Initialize()
    {
        LoadShader("default", "Assets/Shaders/default.vert", "Assets/Shaders/default.frag");
        LoadShader("text", "Assets/Shaders/text.vert", "Assets/Shaders/text.frag");
    }

    public static Matrix4 ViewMatrix { get; set; }
    public static Matrix4 ProjectionMatrix { get; set; }
}
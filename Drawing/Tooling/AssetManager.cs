namespace Ion.Drawing.Tooling;

public static class AssetManager
{
    public static void CreateDefaultFolderStructure()
    {
        // Create the following directories if they do not exist.
        Directory.CreateDirectory("Assets");
        Directory.CreateDirectory("Assets/Textures");
        Directory.CreateDirectory("Assets/Audio");
        Directory.CreateDirectory("Assets/Fonts");
        Directory.CreateDirectory("Assets/Shaders");
    }
}
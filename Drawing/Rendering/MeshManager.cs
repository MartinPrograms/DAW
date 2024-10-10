namespace Ion.Drawing.Rendering;

public static class MeshManager
{
    private static Dictionary<string, Mesh> meshes = new Dictionary<string, Mesh>();
    
    public static void LoadMesh(string name, Mesh mesh)
    {
        meshes.Add(name, mesh);
    }
    
    public static void UnloadMesh(string name)
    {
        meshes.Remove(name);
    }
    
    public static void Initialize()
    {
        LoadMesh("square", GetSquare());
    }
    
    public static Mesh GetSquare()
    {
        if (meshes.ContainsKey("square"))
        {
            return meshes["square"];
        }
        
        Mesh mesh = new Mesh();
        mesh.SetVertices(new Vector3[]
        {
            new Vector3(1.0f, 1.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f)
        });
        
        mesh.SetIndices(new uint[]
        {
            0, 1, 3,
            1, 2, 3
        });
        
        mesh.SetUVs(new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 1.0f)
        });
        
        mesh.Allocate();
        
        return mesh;
    }
}
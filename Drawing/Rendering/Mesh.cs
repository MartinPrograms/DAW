global using OpenTK.Graphics.OpenGL;

namespace Ion.Drawing.Rendering;

public class Mesh : IDisposable
{
    private int _vao;
    private int _vbo;
    private int _ebo;
    private int _vertexCount;
    
    private List<Vector3> _vertices;
    private List<uint> _indices;
    private List<Vector2> _uvs; // these are woven together later
    
    public Mesh()
    {
        _vertices = new List<Vector3>();
        _indices = new List<uint>();
        _uvs = new List<Vector2>();
        // Generate the VAO, VBO, and EBO
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();
    }
    
    public void SetVertices(Vector3[] vector3s)
    {
        _vertices = new List<Vector3>(vector3s);
    }

    public void SetIndices(uint[] uints)
    {
        _indices = new List<uint>(uints);
    }

    public void SetUVs(Vector2[] vector2s)
    {
        _uvs = new List<Vector2>(vector2s);
    }

    public void Allocate()
    {
        List<float> interleaved = new List<float>();

        for (int i = 0; i < _vertices.Count; i++)
        {
            interleaved.Add(_vertices[i].X);
            interleaved.Add(_vertices[i].Y);
            interleaved.Add(_vertices[i].Z);
            interleaved.Add(_uvs[i].X);
            interleaved.Add(_uvs[i].Y);
        }

        _vertexCount = _indices.Count;
        
        GL.BindVertexArray(_vao);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * interleaved.Count, interleaved.ToArray(), BufferUsage.StaticDraw);
        
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _indices.Count, _indices.ToArray(), BufferUsage.StaticDraw);
        
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        
        _vertices.Clear();
        _vertices = null;
        
        _indices.Clear();
        _indices = null;
        
        _uvs.Clear();
        _uvs = null;
    }
    
    private bool _bound = false;
    
    public void Render()
    {
        // All this does is call gl.DrawElements, this assumes a shader is already bound
        
        if (!_bound)
        {
            GL.BindVertexArray(_vao);
            _bound = true;
        }
        
        GL.DrawElements(PrimitiveType.Triangles, _vertexCount, DrawElementsType.UnsignedInt, 0);

    }

    public void Bind()
    {
        GL.BindVertexArray(_vao);
        _bound = true;
    }
    
    public void Unbind()
    {
        GL.BindVertexArray(0);
        _bound = false;
    }

    private void ReleaseUnmanagedResources()
    {
        GL.DeleteVertexArray(ref _vao);
        GL.DeleteBuffer(ref _vbo);
        GL.DeleteBuffer(ref _ebo);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~Mesh()
    {
        ReleaseUnmanagedResources();
    }

}
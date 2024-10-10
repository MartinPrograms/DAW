namespace Ion.Drawing.Rendering;

public class Shader : IDisposable
{  
    private string _vertexPath;
    private string _fragmentPath;
    private int _program;
    
    public Shader(string vertexPath, string fragmentPath)
    {
        this._vertexPath = vertexPath;
        this._fragmentPath = fragmentPath;
        
        Compile();
    }

    private void Compile()
    {
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, File.ReadAllText(_vertexPath));
        GL.CompileShader(vertexShader);

        string infoLog = GL.GetShaderInfoLog(vertexShader, 256, out int length);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Logger.Error($"Vertex shader compilation failed: {infoLog}");
        }
        
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, File.ReadAllText(_fragmentPath));
        GL.CompileShader(fragmentShader);
        
        infoLog = GL.GetShaderInfoLog(fragmentShader, 256, out length);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Logger.Error($"Fragment shader compilation failed: {infoLog}");
        }

        _program = GL.CreateProgram();
        GL.AttachShader(_program, vertexShader);
        GL.AttachShader(_program, fragmentShader);
        GL.LinkProgram(_program);
        
        infoLog = GL.GetProgramInfoLog(_program, 256, out length);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Logger.Error($"Shader program linking failed: {infoLog}");
        }
        
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
        
        Logger.Info($"Shader compiled: {_vertexPath} {_fragmentPath}");
    }

    public void Use()
    {
        GL.UseProgram(_program);
    }

    private void ReleaseUnmanagedResources()
    {
        GL.DeleteProgram(_program);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~Shader()
    {
        ReleaseUnmanagedResources();
    }

    public void Stop()
    {
        GL.UseProgram(0);
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        var location = GL.GetUniformLocation(_program, name);
        if (location == -1)
        {
            //Logger.Error($"Uniform '{name}' not found.");
            return;
        }
        
        GL.UniformMatrix4f(location, 1, false, ref matrix);
    }

    public void SetVector4(string name, Vector4 vector4)
    {
        var location = GL.GetUniformLocation(_program, name);
        if (location == -1)
        {
            //Logger.Error($"Uniform '{name}' not found.");
            return;
        }
        
        GL.Uniform4f(location, 1, ref vector4);
    }

    public void SetFloat(string name, float value)
    {
        var location = GL.GetUniformLocation(_program, name);
        if (location == -1)
        {
            //Logger.Error($"Uniform '{name}' not found.");
            return;
        }
        
        GL.Uniform1f(location, value);
    }

    public void SetVector2(string size, Vector2 vector2)
    {
        var location = GL.GetUniformLocation(_program, size);
        if (location == -1)
        {
            //Logger.Error($"Uniform '{size}' not found.");
            return;
        }
        
        GL.Uniform2f(location, 1, ref vector2);
    }

    public void SetInt(string layer, int i)
    {
        var location = GL.GetUniformLocation(_program, layer);
        if (location == -1)
        {
            //Logger.Error($"Uniform '{layer}' not found.");
            return;
        }
        
        GL.Uniform1i(location, i);
    }

    public void SetBool(string flipy, bool flipY)
    {
        var location = GL.GetUniformLocation(_program, flipy);
        if (location == -1)
        {
            //Logger.Error($"Uniform '{flipy}' not found.");
            return;
        }
        
        GL.Uniform1i(location, flipY ? 1 : 0);
    }

    public void SetVector3(string position, Vector3 pos)
    {
        var location = GL.GetUniformLocation(_program, position);
        if (location == -1)
        {
            //Logger.Error($"Uniform '{position}' not found.");
            return;
        }
        
        GL.Uniform3f(location, 1, ref pos);
    }
}
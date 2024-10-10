using Ion.Drawing.Rendering;

namespace Ion.Drawing.UI.Definitions.Renderables;

public class Square : UIElement
{
    private Mesh _mesh;
    private Shader _shader;
    
    public Square(float x, float y, float width, float height, Vector4 color, int layer)
    {
        this.Position = new Vector2(x, y);
        this.Size = new Vector2(width, height);
        this.Color = color;
        this.Layer = layer;
        
        this._mesh = MeshManager.GetSquare();
        this._shader = ShaderManager.GetShader("default");
    }
    
    public Square(Vector2 position, Vector2 size, Vector4 color, int layer)
    {
        this.Position = position;
        this.Size = size;
        this.Color = color;
        this.Layer = layer;
        
        this._mesh = MeshManager.GetSquare();
        this._shader = ShaderManager.GetShader("default");
    }

    public bool FlipY { get; set; } = false;


    public void Render()
    {
        if (!Visible) return;
        this.DoRender();
    }

    public override void DoRender()
    {
        _mesh.Bind();

        _shader.Use();

        _shader.SetMatrix4("model",
            Matrix4.CreateTranslation(new Vector3(0, 0, 0.0f)) * Matrix4.CreateScale(new Vector3(1, 1, 1.0f)));
        _shader.SetMatrix4("view", ShaderManager.ViewMatrix);
        _shader.SetMatrix4("projection", ShaderManager.ProjectionMatrix);
        _shader.SetFloat("aspectRatio", InteractionSystem.AspectRatio);
        _shader.SetVector2("size", Size);
        _shader.SetVector3("position", Position3D);
        _shader.SetVector2("resolution", InteractionSystem.Resolution);
        _shader.SetInt("layer", Layer);
        _shader.SetBool("flipY", FlipY);
        _shader.SetFloat("far", InteractionSystem.Far);
        _shader.SetFloat("rotation", Rotation);

        _shader.SetBool("ignoreAspectRatio", false);

        var tex = TextureManager.GetTexture("panel");

        tex.Bind();
        _shader.SetInt("tex", 0);

        _shader.SetVector4("color", Color);
        _mesh.Render();

        _mesh.Unbind();

        _shader.Stop();
    }
}
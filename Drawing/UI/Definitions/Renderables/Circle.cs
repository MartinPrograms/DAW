using Ion.Drawing.Rendering;

namespace Ion.Drawing.UI.Definitions.Renderables;

public class Circle : UIElement
{
    private Mesh _mesh;
    private Shader _shader;
    private string _texture;
    
    public Circle(float x, float y, float width, float height, float rot, Vector4 color, int layer, string texture = "circle")
    {
        this.Position = new Vector2(x, y);
        this.Size = new Vector2(width, height);
        this.Color = color;
        this.Layer = layer;
        this.Rotation = rot;
        
        this._mesh = MeshManager.GetSquare();
        this._shader = ShaderManager.GetShader("default");
        
        this._texture = texture;
    }
    
    public Circle(Vector2 position, Vector2 size, Vector4 color, int layer, string texture = "circle")
    {
        this.Position = position;
        this.Size = size;
        this.Color = color;
        this.Layer = layer;
        this.Rotation = 0;
        
        this._mesh = MeshManager.GetSquare();
        this._shader = ShaderManager.GetShader("default");
        
        this._texture = texture;
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

        _shader.SetMatrix4("model", Matrix4.Identity * Matrix4.CreateScale(new Vector3(1, 1, 1.0f)));
        _shader.SetMatrix4("view", ShaderManager.ViewMatrix);
        _shader.SetMatrix4("projection", ShaderManager.ProjectionMatrix);
        _shader.SetFloat("aspectRatio", InteractionSystem.AspectRatio);
        _shader.SetFloat("rotation", Rotation);
        _shader.SetVector2("size", Size);

        Vector4 alignmentBox = new Vector4(0, 0, InteractionSystem.Resolution.X, InteractionSystem.Resolution.Y);
        Vector2 padding = Padding;
        
        if (Parent != null)
        {
            alignmentBox = new Vector4(Parent.Position2D.X, Parent.Position2D.Y, Parent.Position2D.X + Parent.Size.X, Parent.Position2D.Y + Parent.Size.Y);
        }
        
        if (ContentAlignmentHorizontal != null)
        {
            switch (ContentAlignmentHorizontal)
            {
                case HorizontalAlignment.Left:
                    Position = new Vector2(alignmentBox.X + padding.X, Position.Y);
                    break;
                case HorizontalAlignment.Center:
                    Position = new Vector2(alignmentBox.X + (alignmentBox.Z - alignmentBox.X) / 2 - Size.X / 2 + padding.X, Position.Y);
                    break;
                case HorizontalAlignment.Right:
                    Position = new Vector2(alignmentBox.Z - Size.X - padding.X, Position.Y);
                    break;
            }
        }
        
        if (ContentAlignmentVertical != null)
        {
            switch (ContentAlignmentVertical)
            {
                case VerticalAlignment.Top:
                    Position = new Vector2(Position.X, alignmentBox.Y + padding.Y);
                    break;
                case VerticalAlignment.Center:
                    Position = new Vector2(Position.X, alignmentBox.Y + (alignmentBox.W - alignmentBox.Y) / 2 - Size.Y / 2 + padding.Y);
                    break;
                case VerticalAlignment.Bottom:
                    Position = new Vector2(Position.X, alignmentBox.W - Size.Y - padding.Y);
                    break;
            }
        }
        
        _shader.SetVector3("position", Position3D);
        _shader.SetVector2("resolution", InteractionSystem.Resolution);
        _shader.SetInt("layer", Layer);
        _shader.SetBool("flipY", FlipY);
        _shader.SetBool("ignoreAspectRatio", true);
        _shader.SetFloat("far", InteractionSystem.Far);

        var tex = TextureManager.GetTexture(_texture);

        tex.Bind();
        _shader.SetInt("tex", 0);

        _shader.SetVector4("color", Color);
        _mesh.Render();

        _mesh.Unbind();

        _shader.Stop();
    }
}
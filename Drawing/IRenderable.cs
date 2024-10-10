namespace Ion.Drawing;

public interface IRenderable
{
    public void Render();
}

public interface IUpdatable
{
    public void Update();
}

public interface IQueueItem
{
    public Guid ID { get; }
    public int Layer { get; set; }
    public bool Visible { get; set; }
}
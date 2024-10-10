namespace Ion.Drawing;

public class Time
{
    public static float DeltaTime { get; private set; }
    public static float TimeScale { get; set; } = 1f;
    public static float TimeSinceStart { get; private set; }
    
    
    public static void Update(float deltaTime)
    {
        DeltaTime = deltaTime * TimeScale;
        TimeSinceStart += DeltaTime;
    }
    
    public static void Reset()
    {
        DeltaTime = 0;
        TimeSinceStart = 0;
    }
}
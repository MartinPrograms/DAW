namespace Ion.Common;

public class MathExtensions
{
    public static int PixelsFromPoints(int points)
    {
        return (int)(points * 1.3333343412075f);
    }
    
    public static int PointsFromPixels(int pixels)
    {
        return (int)(pixels / 1.3333343412075f);
    }
}
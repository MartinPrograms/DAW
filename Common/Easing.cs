using OpenTK.Mathematics;

namespace Ion.Common;

public static class Easing
{
    public static float Linear(float t)
    {
        return t;
    }
    
    public static float InQuad(float t)
    {
        return t * t;
    }
    
    public static float OutQuad(float t)
    {
        return t * (2 - t);
    }
    
    public static float InOutQuad(float t)
    {
        return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
    }
    
    public static float InCubic(float t)
    {
        return t * t * t;
    }
    
    public static float OutCubic(float t)
    {
        return (--t) * t * t + 1;
    }
    
    public static float InOutCubic(float t)
    {
        return t < 0.5f ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
    }
    
    public static float InQuart(float t)
    {
        return t * t * t * t;
    }
    
    public static float OutQuart(float t)
    {
        return 1 - (--t) * t * t * t;
    }
    
    public static float InOutQuart(float t)
    {
        return t < 0.5f ? 8 * t * t * t * t : 1 - 8 * (--t) * t * t * t;
    }
    
    public static float InQuint(float t)
    {
        return t * t * t * t * t;
    }
    
    public static float OutQuint(float t)
    {
        return 1 + (--t) * t * t * t * t;
    }
    
    public static float InOutQuint(float t)
    {
        return t < 0.5f ? 16 * t * t * t * t * t : 1 + 16 * (--t) * t * t * t * t;
    }
    
    public static float InSine(float t)
    {
        return 1 - MathF.Cos(t * MathF.PI / 2);
    }
    
    public static float OutSine(float t)
    {
        return MathF.Sin(t * MathF.PI / 2);
    }
    
    public static float InOutSine(float t)
    {
        return 0.5f * (1 - MathF.Cos(MathF.PI * t));
    }
    
    public static float InExpo(float t)
    {
        return t == 0 ? 0 : MathF.Pow(2, 10 * t - 10);
    }
    
    public static float OutExpo(float t)
    {
        return t == 1 ? 1 : 1 - MathF.Pow(2, -10 * t);
    }
    
    public static float InOutExpo(float t)
    {
        if (t == 0 || t == 1) return t;
        if (t < 0.5f) return 0.5f * MathF.Pow(2, 20 * t - 10);
        return 1 - 0.5f * MathF.Pow(2, -20 * t + 10);
    }
}

public enum EasingType
{
    Linear,
    InQuad,
    OutQuad,
    InOutQuad,
    InCubic,
    OutCubic,
    InOutCubic,
    InQuart,
    OutQuart,
    InOutQuart,
    InQuint,
    OutQuint,
    InOutQuint,
    InSine,
    OutSine,
    InOutSine,
    InExpo,
    OutExpo,
    InOutExpo
}

public static class EasingExtensions
{
    public static float Ease(this EasingType type, float t)
    {
        return type switch
        {
            EasingType.Linear => Easing.Linear(t),
            EasingType.InQuad => Easing.InQuad(t),
            EasingType.OutQuad => Easing.OutQuad(t),
            EasingType.InOutQuad => Easing.InOutQuad(t),
            EasingType.InCubic => Easing.InCubic(t),
            EasingType.OutCubic => Easing.OutCubic(t),
            EasingType.InOutCubic => Easing.InOutCubic(t),
            EasingType.InQuart => Easing.InQuart(t),
            EasingType.OutQuart => Easing.OutQuart(t),
            EasingType.InOutQuart => Easing.InOutQuart(t),
            EasingType.InQuint => Easing.InQuint(t),
            EasingType.OutQuint => Easing.OutQuint(t),
            EasingType.InOutQuint => Easing.InOutQuint(t),
            EasingType.InSine => Easing.InSine(t),
            EasingType.OutSine => Easing.OutSine(t),
            EasingType.InOutSine => Easing.InOutSine(t),
            EasingType.InExpo => Easing.InExpo(t),
            EasingType.OutExpo => Easing.OutExpo(t),
            EasingType.InOutExpo => Easing.InOutExpo(t),
            _ => throw new System.NotImplementedException()
        };
    }
    
    public static float Ease(this EasingType type, float a, float b, float t)
    {
        return a + (b - a) * type.Ease(t);
    }
    
    public static Vector2 Ease(this EasingType type, Vector2 a, Vector2 b, float t)
    {
        return new Vector2(type.Ease(a.X, b.X, t), type.Ease(a.Y, b.Y, t));
    }
}
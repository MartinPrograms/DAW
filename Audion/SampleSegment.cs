namespace Ion.Audion;


public struct SampleSegment
{
    public float R;
    public float L;
    
    public SampleSegment(float r, float l)
    {
        this.R = r;
        this.L = l;
    }

    public byte[] Data()
    {
        // 32-bit PCM format
        byte[] data = new byte[8];

        byte[] r = BitConverter.GetBytes(R);
        byte[] l = BitConverter.GetBytes(L);
        
        Array.Copy(r, 0, data, 0, 4);
        Array.Copy(l, 0, data, 4, 4);
        
        return data;
    }
    
    public static SampleSegment Zero => new SampleSegment(0, 0);
    
    public static SampleSegment operator +(SampleSegment a, SampleSegment b)
    {
        return new SampleSegment(a.R + b.R, a.L + b.L);
    }
    
    public static SampleSegment operator -(SampleSegment a, SampleSegment b)
    {
        return new SampleSegment(a.R - b.R, a.L - b.L);
    }
    
    public static SampleSegment operator *(SampleSegment a, SampleSegment b)
    {
        return new SampleSegment(a.R * b.R, a.L * b.L);
    }
    
    public static SampleSegment operator /(SampleSegment a, SampleSegment b)
    {
        return new SampleSegment(a.R / b.R, a.L / b.L);
    }
    
    public static SampleSegment operator +(SampleSegment a, float b)
    {
        return new SampleSegment(a.R + b, a.L + b);
    }
    
    public static SampleSegment operator -(SampleSegment a, float b)
    {
        return new SampleSegment(a.R - b, a.L - b);
    }
    
    public static SampleSegment operator *(SampleSegment a, float b)
    {
        return new SampleSegment(a.R * b, a.L * b);
    }
    
    public static SampleSegment operator /(SampleSegment a, float b)
    {
        return new SampleSegment(a.R / b, a.L / b);
    }
    
    public static SampleSegment operator +(float a, SampleSegment b)
    {
        return new SampleSegment(a + b.R, a + b.L);
    }
    
    public static SampleSegment operator -(float a, SampleSegment b)
    {
        return new SampleSegment(a - b.R, a - b.L);
    }
    
    public static SampleSegment operator *(float a, SampleSegment b)
    {
        return new SampleSegment(a * b.R, a * b.L);
    }
    
    public static SampleSegment operator /(float a, SampleSegment b)
    {
        return new SampleSegment(a / b.R, a / b.L);
    }
    
    public static SampleSegment operator -(SampleSegment a)
    {
        return new SampleSegment(-a.R, -a.L);
    }
    
    public static bool operator ==(SampleSegment a, SampleSegment b)
    {
        return a.R == b.R && a.L == b.L;
    }
    
    public static bool operator !=(SampleSegment a, SampleSegment b)
    {
        return a.R != b.R || a.L != b.L;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is SampleSegment s)
        {
            return this == s;
        }
        
        return false;
    }

    public void Normalize()
    {
        float max = Math.Max(Math.Abs(R), Math.Abs(L));
        
        if (max > 1)
        {
            R /= max;
            L /= max;
        }
    }
}
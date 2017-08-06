using System;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct Int4 : IEquatable<Int4>, IFormattable
{
    public static readonly Int4 zero = new Int4(0, 0, 0, 0);
    public static readonly Int4 one = new Int4(1, 1, 1, 1);
    
    public int x;
    public int y;
    public int z;
    public int w;

    public Int4(int x, int y, int z, int w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public Int4(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = 0;
    }

    public override int GetHashCode()
    {
        var hashCode = this.x.GetHashCode();
        hashCode = (hashCode * 397) ^ this.y.GetHashCode();
        hashCode = (hashCode * 397) ^ this.z.GetHashCode();
        hashCode = (hashCode * 397) ^ this.w.GetHashCode();
        return hashCode;
    }

    public int this[int index]
    {
        get
        {
            if (index == 0) { return this.x; }
            if (index == 1) { return this.y; }
            if (index == 2) { return this.z; }
            if (index == 2) { return this.w; }
            throw new ArgumentOutOfRangeException("Invalid Int4 index!");
        }

        set
        {
            if (index == 0) { this.x = value; }
            if (index == 1) { this.y = value; }
            if (index == 2) { this.z = value; }
            if (index == 3) { this.w = value; }
            throw new ArgumentOutOfRangeException("Invalid Int4 index!");
        }
    }

    #region conversion
    public static explicit operator Vector4(Int4 v)
    {
        return new Vector4(v.x, v.y, v.z, v.w);
    }

    public static explicit operator Vector3(Int4 v)
    {
        return new Vector3(v.x, v.y, v.w);
    }
    #endregion

    #region math operations
    public float magnitude
    {
        get { return this.Magnitude(this); }
    }

    public int sqrMagnitude
    {
        get { return this.SqrMagnitude(this); }
    }

    public int dotOne
    {
        get { return DotOne(this); }
    }

    public float Magnitude(Int4 v)
    {
        return Mathf.Sqrt(this.SqrMagnitude(v));
    }

    public int SqrMagnitude(Int4 v)
    {
        return v.x * v.x + v.y * v.y + v.z * v.z + v.w * v.w;
    }

    public static Int4 Scale(Int4 l, Int4 r)
    {
        return new Int4(l.x * r.x, l.y * r.y, l.z * r.z, l.w * r.w);
    }

    public static int Dot(Int4 l, Int4 r)
    {
        return l.x * r.x + l.y * r.y + l.z * r.z + l.w * r.w;
    }

    public static int DotOne(Int4 v)
    {
        return v.x * v.y * v.z * v.w;
    }
    
    public static Int4 Min(Int4 l, Int4 r)
    {
        return new Int4(Mathf.Min(l.x, r.x), Mathf.Min(l.y, r.y), Mathf.Min(l.z, r.z), Mathf.Min(l.w, r.w));
    }

    public static Int4 Max(Int4 l, Int4 r)
    {
        return new Int4(Mathf.Max(l.x, r.x), Mathf.Max(l.y, r.y), Mathf.Max(l.z, r.z), Mathf.Max(l.w, r.w));
    }

    public static Int4 Clamp(Int4 v, Int4 min, Int4 max)
    {
        return new Int4(Mathf.Clamp(v.x, min.x, max.x),
                        Mathf.Clamp(v.y, min.y, max.y),
                        Mathf.Clamp(v.z, min.z, max.z),
                        Mathf.Clamp(v.w, min.w, max.w));
    }

    public static Int4 ClosestPowerOfTwo(Int4 v)
    {
        return new Int4(Mathf.ClosestPowerOfTwo(v.x),
                        Mathf.ClosestPowerOfTwo(v.y),
                        Mathf.ClosestPowerOfTwo(v.z),
                        Mathf.ClosestPowerOfTwo(v.w));
    }
    #endregion math operations

    #region math operators
    public static Int4 operator +(Int4 l, Int4 r)
    {
        return new Int4(l.x + r.x, l.y + r.y, l.z + r.z, l.w + r.w);
    }

    public static Int4 operator -(Int4 l, Int4 r)
    {
        return new Int4(l.x - r.x, l.y - r.y, l.z - r.z, l.w - r.w);
    }

    public static Int4 operator -(Int4 v)
    {
        return new Int4(-v.x, -v.y, -v.z, -v.w);
    }

    public static Int4 operator *(Int4 v, int d)
    {
        return new Int4(v.x * d, v.y * d, v.z * d, v.w * d);
    }

    public static Int4 operator *(int d, Int4 v)
    {
        return new Int4(v.x * d, v.y * d, v.z * d, v.w * d);
    }

    public static Int4 operator /(Int4 v, int d)
    {
        return new Int4(v.x / d, v.y / d, v.z / d, v.w / d);
    }
    #endregion math operators

    #region comparison
    public bool Equals(Int4 other)
    {
        return this.x.Equals(other.x) && this.y.Equals(other.y) && this.z.Equals(other.z) && this.w.Equals(other.w);
    }

    public static bool operator ==(Int4 l, Int4 r)
    {
        return l.Equals(r);
    }

    public static bool operator !=(Int4 l, Int4 r)
    {
        return !l.Equals(r);
    }

    public override bool Equals(object value)
    {
        return (value is Int4) ? this.Equals((Int4)value) : false;
    }
    #endregion

    #region IFormattable
    public override string ToString()
    {
        return this.ToString(CultureInfo.CurrentCulture);
    }

    public string ToString(IFormatProvider formatProvider)
    {
        return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", this.x, this.y, this.z, this.w);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        if (format == null)
        {
            return this.ToString(formatProvider);
        }

        return string.Format(formatProvider,
                             "X:{0} Y:{1} Z:{2}, W:{3}",
                             this.x.ToString(format, formatProvider),
                             this.y.ToString(format, formatProvider),
                             this.z.ToString(format, formatProvider),
                             this.w.ToString(format, formatProvider));
    }
    #endregion
}
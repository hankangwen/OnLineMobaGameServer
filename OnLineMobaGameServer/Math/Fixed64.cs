using System;
using System.Globalization;

/// <summary>
/// [帧同步]自定义定点数
/// </summary>
[Serializable]
public struct Fixed64 : IEquatable<Fixed64>, IComparable, IComparable<Fixed64>
{
    public long value;
    /// <summary>
    /// 小数部分的位数
    /// </summary>
    private const int FRACTIONALBITS = 12;
    private const long ONE = 1L << FRACTIONALBITS;
    public static Fixed64 Zero = new Fixed64(0);

    /// <summary>
    /// 私有构造函数，直接对value进行赋值
    /// </summary>
    /// <param name="value"></param>
    private Fixed64(long value)
    {
        this.value = value;
    }
    
    /// <summary>
    /// 公有构造函数，传入具体数字的构造函数
    /// </summary>
    /// <param name="value"></param>
    public Fixed64(int value)
    {
        this.value = value * ONE;
    }

    #region 重载运算符

    public static Fixed64 operator +(Fixed64 a, Fixed64 b)
    {
        return new Fixed64(a.value + b.value);
    }

    public static Fixed64 operator -(Fixed64 a, Fixed64 b)
    {
        return new Fixed64(a.value - b.value);
    }

    public static Fixed64 operator *(Fixed64 a, Fixed64 b)
    {
        return new Fixed64((a.value * b.value) >> FRACTIONALBITS);
    }

    public static Fixed64 operator /(Fixed64 a, Fixed64 b)
    {
        return new Fixed64((a.value << FRACTIONALBITS) / b.value);
    }

    public static bool operator ==(Fixed64 a, Fixed64 b)
    {
        return a.value == b.value;
    }

    public static bool operator !=(Fixed64 a, Fixed64 b)
    {
        return !(a == b);
    }
    
    public static bool operator>(Fixed64 a,Fixed64 b)
    {
        return a.value > b.value;
    }
    public static bool operator <(Fixed64 a, Fixed64 b)
    {
        return a.value < b.value;
    }
    public static bool operator >=(Fixed64 a, Fixed64 b)
    {
        return a.value >= b.value;
    }
    public static bool operator <=(Fixed64 a, Fixed64 b)
    {
        return a.value <= b.value;
    }
    
    #endregion

    #region 重载类型转换
    
    public static explicit operator long(Fixed64 value)
    {
        return value.value >> FRACTIONALBITS;
    }

    public static explicit operator Fixed64(long value)
    {
        return new Fixed64(value);
    }

    public static explicit operator float(Fixed64 value)
    {
        return (float)value.value / ONE;
    }

    public static explicit operator Fixed64(float value)
    {
        return new Fixed64((long)(value * ONE));
    }
    
    #endregion

    public int CompareTo(object obj)
    {
        return value.CompareTo(obj);
    }

    public int CompareTo(Fixed64 other)
    {
        return value.CompareTo(other.value);
    }
    
    public bool Equals(Fixed64 other)
    {
        return value == other.value;
    }

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is Fixed64 && ((Fixed64)obj).value == value;
    }

    public override string ToString()
    {
        return ((float)this).ToString(CultureInfo.InvariantCulture);
    }
}

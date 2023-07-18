//using System;
//using System.Runtime.InteropServices;
//using UnityEngine;
//using UnityEngine.Internal;

//[Serializable]
//[StructLayout(LayoutKind.Sequential, Pack = 4)]
//public struct uVector3 : IEquatable<uVector3>
//{
//    public static readonly uVector3 zero = new uVector3(0);
//    public static readonly uVector3 one = new uVector3(1);

//    public static readonly uVector3 NegativeInfinity = new uVector3(float.NegativeInfinity);
//    public static readonly uVector3 PositiveInfinity = new uVector3(float.PositiveInfinity);

//    public static readonly uVector3 up = new uVector3(0, 1, 0);
//    public static readonly uVector3 down = new uVector3(0, -1, 0);
//    public static readonly uVector3 left = new uVector3(1, 0, 0);
//    public static readonly uVector3 right = new uVector3(-1, 0, 0);
//    public static readonly uVector3 forward = new uVector3(0, 0, 1);
//    public static readonly uVector3 back = new uVector3(0, 0, -1);


//    public float x;
//    public float y;
//    public float z;

//    #region 构造函数
//    public uVector3(float value)
//    {
//        x = y = z = value;
//    }

//    public uVector3(float x, float y)
//    {
//        this.x = x;
//        this.y = y;
//        z = 0f;
//    }

//    public uVector3(float x, float y, float z)
//    {
//        this.x = x;
//        this.y = y;
//        this.z = z;
//    }
//    #endregion

//    public float this[int index]
//    {
//        get
//        {
//            switch (index)
//            {
//                case 0: return x;
//                case 1: return y;
//                case 2: return z;
//                default:
//                    throw new IndexOutOfRangeException("Invalid Vector3 index!");
//            }
//        }
//        set
//        {
//            switch (index)
//            {
//                case 0: x = value; break;
//                case 1: y = value; break;
//                case 2: z = value; break;
//                default:
//                    throw new IndexOutOfRangeException("Invalid Vector3 index!");
//            }
//        }
//    }

//    public bool Equals(uVector3 other)
//    {
//        return false;
//    }

//    #region Override
//    public override bool Equals(object obj)
//    {
//        return base.Equals(obj);
//    }

//    public override int GetHashCode()
//    {
//        return base.GetHashCode();
//    }
//    #endregion

//    #region 归一化
//    public uVector3 normalized
//    {
//        get { return Normalize(this); }
//    }

//    public void Normalize()
//    {
//        var val = Magnitude(this);
//        if (val > 1E-05f)
//        {
//            this /= val;
//        }
//        else
//        {
//            this = zero;
//        }
//    }

//    public static uVector3 Normalize(uVector3 value)
//    {
//        float num = Magnitude(value);
//        if (num > 1E-05f)
//        {
//            return value / num;
//        }
//        return zero;
//    }
//    #endregion

//    #region 取模
//    public float magnitude { get => (float)Math.Sqrt(x * x + y * y + z * z); }

//    public float sqrMagnitude { get => x * x + y * y + z * z; }

//    public static float Magnitude(uVector3 vector)
//    {
//        return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
//    }

//    public static float SqrMagnitude(uVector3 vector)
//    {
//        return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
//    }
//    #endregion

//    #region 二元计算
//    public static uVector3 operator +(uVector3 a, uVector3 b)
//    {
//        return new uVector3(a.x + b.x, a.y + b.y, a.z + b.z);
//    }

//    public static uVector3 operator -(uVector3 a, uVector3 b)
//    {
//        return new uVector3(a.x - b.x, a.y - b.y, a.z - b.z);
//    }

//    public static uVector3 operator -(uVector3 a)
//    {
//        return new uVector3(-a.x, -a.y, -a.z);
//    }

//    public static uVector3 operator *(uVector3 a, float d)
//    {
//        return new uVector3(a.x * d, a.y * d, a.z * d);
//    }

//    public static uVector3 operator *(uVector3 a, int d)
//    {
//        return new uVector3(a.x * d, a.y * d, a.z * d);
//    }

//    public static uVector3 operator *(float d, uVector3 a)
//    {
//        return new uVector3(a.x * d, a.y * d, a.z * d);
//    }

//    public static uVector3 operator /(uVector3 a, float d)
//    {
//        return new uVector3(a.x / d, a.y / d, a.z / d);
//    }

//    public static bool operator ==(uVector3 left, uVector3 right)
//    {
//        return left.Equals(right);
//    }

//    public static bool operator !=(uVector3 left, uVector3 right)
//    {
//        return !left.Equals(right);
//    }
//    #endregion

//    #region V3 方法
//    public void Set(float newX, float newY, float newZ)
//    {
//        x = newX;
//        y = newY;
//        z = newZ;
//    }

//    public static uVector3 Scale(uVector3 a, uVector3 b)
//    {
//        return new uVector3(a.x * b.x, a.y * b.y, a.z * b.z);
//    }

//    public static float SqrDistance(uVector3 value1, uVector3 value2)
//    {
//        float x = value1.x - value2.x;
//        float y = value1.y - value2.y;
//        float z = value1.z - value2.z;

//        return x * x + y * y + z * z;
//    }

//    public static float Distance(uVector3 value1, uVector3 value2)
//    {
//        float x = value1.x - value2.x;
//        float y = value1.y - value2.y;
//        float z = value1.z - value2.z;

//        return (float)Math.Sqrt(x * x + y * y + z * z);
//    }

//    public static uVector3 Lerp(uVector3 start, uVector3 end, float amount)
//    {
//        uVector3 result;
//        result.x = start.x + (end.x - start.x) * amount;
//        result.y = start.y + (end.y - start.y) * amount;
//        result.z = start.z + (end.z - start.z) * amount;
//        return result;
//    }

//    public static uVector3 MoveTowards(uVector3 current, uVector3 target, float delta)
//    {
//        var toward = target - current;
//        var dist = toward.magnitude;
//        if (dist <= delta) return target;
//        return current + delta * toward.normalized;
//    }

//    public static float Dot(uVector3 lhs, uVector3 rhs)
//    {
//        return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
//    }

//    public static uVector3 Cross(uVector3 lhs, uVector3 rhs)
//    {
//        return new uVector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
//    }

//    public static uVector3 SmoothDamp(uVector3 current, uVector3 target, ref uVector3 currentVelocity, float smoothTime, [DefaultValue("Mathf.Infinity")] float maxSpeed, [DefaultValue("Time.deltaTime")] float deltaTime)
//    {
//        float num = 0f;
//        float num2 = 0f;
//        float num3 = 0f;
//        smoothTime = Mathf.Max(0.0001f, smoothTime);
//        float num4 = 2f / smoothTime;
//        float num5 = num4 * deltaTime;
//        float num6 = 1f / (1f + num5 + 0.48f * num5 * num5 + 0.235f * num5 * num5 * num5);
//        float num7 = current.x - target.x;
//        float num8 = current.y - target.y;
//        float num9 = current.z - target.z;
//        uVector3 vector = target;
//        float num10 = maxSpeed * smoothTime;
//        float num11 = num10 * num10;
//        float num12 = num7 * num7 + num8 * num8 + num9 * num9;
//        if (num12 > num11)
//        {
//            float num13 = (float)Math.Sqrt(num12);
//            num7 = num7 / num13 * num10;
//            num8 = num8 / num13 * num10;
//            num9 = num9 / num13 * num10;
//        }

//        target.x = current.x - num7;
//        target.y = current.y - num8;
//        target.z = current.z - num9;
//        float num14 = (currentVelocity.x + num4 * num7) * deltaTime;
//        float num15 = (currentVelocity.y + num4 * num8) * deltaTime;
//        float num16 = (currentVelocity.z + num4 * num9) * deltaTime;
//        currentVelocity.x = (currentVelocity.x - num4 * num14) * num6;
//        currentVelocity.y = (currentVelocity.y - num4 * num15) * num6;
//        currentVelocity.z = (currentVelocity.z - num4 * num16) * num6;
//        num = target.x + (num7 + num14) * num6;
//        num2 = target.y + (num8 + num15) * num6;
//        num3 = target.z + (num9 + num16) * num6;
//        float num17 = vector.x - current.x;
//        float num18 = vector.y - current.y;
//        float num19 = vector.z - current.z;
//        float num20 = num - vector.x;
//        float num21 = num2 - vector.y;
//        float num22 = num3 - vector.z;
//        if (num17 * num20 + num18 * num21 + num19 * num22 > 0f)
//        {
//            num = vector.x;
//            num2 = vector.y;
//            num3 = vector.z;
//            currentVelocity.x = (num - vector.x) / deltaTime;
//            currentVelocity.y = (num2 - vector.y) / deltaTime;
//            currentVelocity.z = (num3 - vector.z) / deltaTime;
//        }

//        return new uVector3(num, num2, num3);
//    }
//    #endregion
//}

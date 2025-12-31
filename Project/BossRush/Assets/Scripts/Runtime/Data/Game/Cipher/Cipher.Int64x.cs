namespace TeamSuneat
{
    /// <summary>
    /// 메모리 위/변조 방지
    /// </summary>
    public class Int64x
    {
        private long m_value;

        private int m_offset;

        public Int64x()
        {
            Value = 0;
        }

        public Int64x(long value)
        {
            Value = value;
        }

        public long Value
        {
            get
            {
                return m_value - m_offset;
            }

            set
            {
                m_offset = RandomEx.Range(-9999999, 9999999);
                m_value = value + m_offset;
            }
        }

        public static explicit operator long(Int64x rhs)
        {
            return rhs.Value;
        }

        public static implicit operator Int64x(long rhs)
        {
            Int64x i32 = new Int64x(rhs);
            return i32;
        }

        public static long operator +(Int64x a, long b)
        {
            return a.Value + b;
        }

        public static long operator +(long a, Int64x b)
        {
            return a + b.Value;
        }

        public static long operator +(Int64x a, Int64x b)
        {
            return a.Value + b.Value;
        }

        public static long operator -(Int64x a, long b)
        {
            return a.Value - b;
        }

        public static long operator -(long a, Int64x b)
        {
            return a - b.Value;
        }

        public static long operator -(Int64x a, Int64x b)
        {
            return a.Value - b.Value;
        }

        public static bool operator ==(Int64x a, Int64x b)
        {
            return a.Equals(b);
        }

        public static bool operator >=(Int64x a, long b)
        {
            return a.Value >= b;
        }

        public static bool operator <=(Int64x a, long b)
        {
            return a.Value <= b;
        }

        public static bool operator >=(Int64x a, int b)
        {
            return a.Value >= b;
        }

        public static bool operator <=(Int64x a, int b)
        {
            return a.Value <= b;
        }

        public static bool operator >(Int64x a, long b)
        {
            return a.Value > b;
        }

        public static bool operator <(Int64x a, long b)
        {
            return a.Value < b;
        }

        public static bool operator >(Int64x a, int b)
        {
            return a.Value > b;
        }

        public static bool operator <(Int64x a, int b)
        {
            return a.Value < b;
        }

        public static bool operator ==(Int64x a, long b)
        {
            return a.Equals(b);
        }

        public static bool operator ==(long a, Int64x b)
        {
            return b.Equals(a);
        }

        public static bool operator !=(Int64x a, Int64x b)
        {
            return !a.Equals(b);
        }

        public static bool operator !=(Int64x a, long b)
        {
            return !a.Equals(b);
        }

        public static bool operator !=(long a, Int64x b)
        {
            return !b.Equals(a);
        }

        public bool Equals(long other)
        {
            return Value == other;
        }

        public bool Equals(Int64x other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (false == (obj is Int64x))
                return false;

            return base.Equals((Int64x)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
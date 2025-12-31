namespace TeamSuneat
{
    /// <summary>
    /// 메모리 위/변조 방지
    /// </summary>
    public class Int32x
    {
        private int m_value;

        private int m_offset;

        public Int32x()
        {
            Value = 0;
        }

        public Int32x(int value)
        {
            Value = value;
        }

        public int Value
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

        public static explicit operator int(Int32x rhs)
        {
            return rhs.Value;
        }

        public static implicit operator Int32x(int rhs)
        {
            Int32x i32 = new Int32x(rhs);
            return i32;
        }

        public static Int32x operator +(Int32x a, int b)
        {
            return new Int32x(a.Value + b);
        }

        public static int operator -(Int32x a, int b)
        {
            return a.Value - b;
        }

        public static int operator -(int a, Int32x b)
        {
            return a - b.Value;
        }

        public static bool operator >=(Int32x a, long b)
        {
            return a.Value >= b;
        }

        public static bool operator <=(Int32x a, long b)
        {
            return a.Value <= b;
        }

        public static bool operator >=(Int32x a, int b)
        {
            return a.Value >= b;
        }

        public static bool operator <=(Int32x a, int b)
        {
            return a.Value <= b;
        }

        public static bool operator >=(int a, Int32x b)
        {
            return a >= b.Value;
        }

        public static bool operator <=(int a, Int32x b)
        {
            return a <= b.Value;
        }

        public static bool operator >(Int32x a, long b)
        {
            return a.Value > b;
        }

        public static bool operator <(Int32x a, long b)
        {
            return a.Value < b;
        }

        public static bool operator >(Int32x a, int b)
        {
            return a.Value > b;
        }

        public static bool operator <(Int32x a, int b)
        {
            return a.Value < b;
        }

        public static bool operator >(int a, Int32x b)
        {
            return a > b.Value;
        }

        public static bool operator <(int a, Int32x b)
        {
            return a < b.Value;
        }

        public static bool operator ==(Int32x a, Int32x b)
        {
            return a.Equals(b);
        }

        public static bool operator ==(Int32x a, int b)
        {
            return a.Equals(b);
        }

        public static bool operator ==(int a, Int32x b)
        {
            return b.Equals(a);
        }

        public static bool operator !=(Int32x a, Int32x b)
        {
            return !a.Equals(b);
        }

        public static bool operator !=(Int32x a, int b)
        {
            return !a.Equals(b);
        }

        public static bool operator !=(int a, Int32x b)
        {
            return !b.Equals(a);
        }

        public bool Equals(int other)
        {
            return Value == other;
        }

        public bool Equals(Int32x other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (false == (obj is Int32x))
                return false;

            return base.Equals((Int32x)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
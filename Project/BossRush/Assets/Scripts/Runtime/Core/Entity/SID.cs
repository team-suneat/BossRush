using System;

namespace TeamSuneat
{
    /// <summary>Security Identifier : 보안 식별자</summary>
    [Serializable]
    public struct SID
    {
        public static readonly SID Null = new SID(0, false);
        private static ulong IssuedValue = 1;

        [UnityEngine.SerializeField]
        private ulong _value;

        private bool _enabled;

        private SID(ulong value, bool enabled)
        {
            _value = value;
            _enabled = enabled;
        }

        public static SID Generate()
        {
            return new SID(++IssuedValue, true);
        }

        public static void Clear()
        {
            IssuedValue = 0;
        }

        public static explicit operator int(SID x)
        {
            return (int)x._value;
        }

        public static explicit operator ulong(SID x)
        {
            return x._value;
        }

        public static implicit operator SID(int x)
        {
            return new SID((ulong)x, true);
        }

        public static bool operator ==(SID a, SID b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(SID a, SID b)
        {
            return a._value != b._value;
        }

        public override bool Equals(object o)
        {
            try
            {
                return this == (SID)o;
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return unchecked((int)_value);
        }

        public int ToInt()
        {
            return Convert.ToInt32(_value);
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public void SetEnabled(bool enabled)
        {
            _enabled = enabled;
        }
    }
}
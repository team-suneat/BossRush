using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string Format(string format, string value, int arguments)
        {
            if (arguments == 1)
            {
                try
                {
                    return string.Format(format, value);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            else
            {
                Debug.LogWarningFormat("스트링 데이터의 인수가 1이 아닙니다. format:[{0}], arguments:[{1}]", format, arguments);
            }

            return format;
        }

        public static string Format(string format, string[] values, int arguments)
        {
            if (values == null)
            {
                Debug.LogWarningFormat("스트링 데이터의 값이 설정되지 않았습니다. format:[{0}]", format);
                return format;
            }

            if (arguments != values.Length)
            {
                Debug.LogWarningFormat("스트링 데이터의 인수의 수({0})보다 값의 수({1})가 같지 않습니다. format: {2}", arguments, values.Length, format);
                return format;
            }

            switch (arguments)
            {
                case 1:
                    {
                        return string.Format(format, values[0]);
                    }
                case 2:
                    {
                        return string.Format(format, values[0], values[1]);
                    }
                case 3:
                    {
                        return string.Format(format, values[0], values[1], values[2]);
                    }
                case 4:
                    {
                        return string.Format(format, values[0], values[1], values[2], values[3]);
                    }
                case 5:
                    {
                        return string.Format(format, values[0], values[1], values[2], values[3], values[4]);
                    }
                case 6:
                    {
                        return string.Format(format, values[0], values[1], values[2], values[3], values[4], values[5]);
                    }
                case 7:
                    {
                        return string.Format(format, values[0], values[1], values[2], values[3], values[4], values[5],
                            values[6]);
                    }
                case 8:
                    {
                        return string.Format(format, values[0], values[1], values[2], values[3], values[4], values[5],
                            values[6], values[7]);
                    }
                case 9:
                    {
                        return string.Format(format, values[0], values[1], values[2], values[3], values[4], values[5],
                            values[6], values[7], values[8]);
                    }
                case 10:
                    {
                        return string.Format(format, values[0], values[1], values[2], values[3], values[4], values[5],
                            values[6], values[7], values[8], values[9]);
                    }
                case 11:
                    {
                        return string.Format(format, values[0], values[1], values[2], values[3], values[4], values[5],
                            values[6], values[7], values[8], values[9], values[10]);
                    }
                case 12:
                    {
                        return string.Format(format, values[0], values[1], values[2], values[3], values[4], values[5],
                            values[6], values[7], values[8], values[9], values[10], values[11]);
                    }
                case 13:
                    {
                        return string.Format(format, values[0], values[1], values[2], values[3], values[4], values[5],
                            values[6], values[7], values[8], values[9], values[10], values[11], values[12]);
                    }
                case 14:
                    {
                        return string.Format(format, values[0], values[1], values[2], values[3], values[4], values[5],
                            values[6], values[7], values[8], values[9], values[10], values[11], values[12], values[13]);
                    }
                default:
                    return format;
            }
        }

        public static string Format(StringData data, string value)
        {
            return Format(data.GetString(), value, data.Arguments);
        }

        private static string Format(StringData data, string[] values)
        {
            return Format(data.GetString(), values, data.Arguments);
        }
    }
}
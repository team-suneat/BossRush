using System.Text;
using UnityEngine;

namespace TeamSuneat
{
    [System.Serializable]
    public class StatModifier
    {
        public float Value;

        public StatModType Type;

        public int Order;

        [System.NonSerialized]
        public Component Source;

        [System.NonSerialized]
        public string SourceName;

        [System.NonSerialized]
        public string SourceType;

        [System.NonSerialized]
        public int SID;

        [System.NonSerialized]
        public int OptionIndex; // 각 아이템의 옵션 인덱스를 명시

        public string GetValueString(bool isRemove = false)
        {
            switch (Type)
            {
                case StatModType.PercentAdd:
                case StatModType.PercentMulti:
                    {
                        if (isRemove)
                        {
                            return ValueStringEx.GetPercentString(-Value, true);
                        }

                        return ValueStringEx.GetPercentString(Value, true);
                    }
            }
            if (isRemove)
            {
                return ValueStringEx.GetValueString(-Value, true);
            }

            return ValueStringEx.GetValueString(Value, true);
        }

        public string GetSourceString()
        {
            StringBuilder stringBuilder = new();

            if (Source != null)
            {
                _ = stringBuilder.Append($"{Source}, ");
            }

            if (!string.IsNullOrEmpty(SourceName))
            {
                _ = stringBuilder.Append($"{SourceName}, ");
            }

            if (!string.IsNullOrEmpty(SourceType))
            {
                _ = stringBuilder.Append($"{SourceType}, ");
            }

            return stringBuilder.ToString();
        }

        public StatModifier()
        {
            // 사용하지 않아도, JSON 파일로 불러오기 위해서 기본 생성자가 선언되어야합니다.
        }
    }
}
using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public sealed class StatRowConverter : IGoogleSheetRowConverter<StatData>
    {
        public bool TryConvert(Dictionary<string, string> row, out StatData model)
        {
            model = null;

            if (!row.TryGetValue("Name", out string nameStr) || !GoogleSheetValueParsers.TryParseEnum(nameStr, out StatNames name))
            {
                Log.Warning($"필수 컬럼 Name 누락 또는 enum 파싱 실패: {nameStr}");
                return false;
            }

            _ = row.TryGetValue("DisplayName", out string displayName);
            _ = row.TryGetValue("DisplayDesc", out string displayDesc);

            if (!row.TryGetValue("DefaultValue", out string defaultValueStr) || !GoogleSheetValueParsers.TryParseFloat(defaultValueStr, out float defaultValue))
            {
                Log.Warning($"Name {name}: DefaultValue 파싱 실패: {defaultValueStr}");
                return false;
            }
            if (!row.TryGetValue("Digit", out string digitStr) || !GoogleSheetValueParsers.TryParseInt(digitStr, out int digit))
            {
                Log.Warning($"Name {name}: Digit 파싱 실패: {digitStr}");
                return false;
            }
            if (!row.TryGetValue("UseRange", out string useRangeStr) || !GoogleSheetValueParsers.TryParseBool(useRangeStr, out bool useRange))
            {
                Log.Warning($"Name {name}: UseRange 파싱 실패: {useRangeStr}");
                return false;
            }
            if (!row.TryGetValue("MinValue", out string minValueStr) || !GoogleSheetValueParsers.TryParseFloat(minValueStr, out float minValue))
            {
                Log.Warning($"Name {name}: MinValue 파싱 실패: {minValueStr}");
                return false;
            }
            if (!row.TryGetValue("MaxValue", out string maxValueStr) || !GoogleSheetValueParsers.TryParseFloat(maxValueStr, out float maxValue))
            {
                Log.Warning($"Name {name}: MaxValue 파싱 실패: {maxValueStr}");
                return false;
            }

            if (!row.TryGetValue("Mod", out string modStr) || !GoogleSheetValueParsers.TryParseEnum(modStr, out StatModType mod))
            {
                Log.Warning($"필수 컬럼 Name 누락 또는 enum 파싱 실패: {modStr}");
                return false;
            }

            // 모델 생성 및 기본 필드 설정
            StatData m = new()
            {
                Name = name,
                DisplayName = displayName,
                DisplayDesc = displayDesc,
                DefaultValue = defaultValue,
                UseRange = useRange,
                MinValue = minValue,
                MaxValue = maxValue,
                Mod = mod,
                Digit = digit,
            };

            model = m;
            return true;
        }
    }
}
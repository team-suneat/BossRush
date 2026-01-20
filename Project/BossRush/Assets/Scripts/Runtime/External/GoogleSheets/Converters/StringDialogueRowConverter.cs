using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public sealed class StringDialogueRowConverter : IGoogleSheetRowConverter<StringDialogueData>
    {
        public bool TryConvert(Dictionary<string, string> row, out StringDialogueData model)
        {
            model = null;

            _ = row.TryGetValue("ID", out string id);
            _ = row.TryGetValue("TimelineName", out string timelineName);

            if (!row.TryGetValue("Index", out string indexStr) || !GoogleSheetValueParsers.TryParseInt(indexStr, out int index))
            {
                Log.Warning($"ID {id}: Index 파싱 실패: {indexStr}");
                return false;
            }

            if (!row.TryGetValue("SpeakerName", out string speakerNameStr) || !GoogleSheetValueParsers.TryParseEnum(speakerNameStr, out CharacterNames speakerName))
            {
                Log.Warning($"ID {id}: SpeakerName 파싱 실패 또는 enum 파싱 실패: {speakerNameStr}");
                return false;
            }

            _ = row.TryGetValue("Korean", out string korean);
            _ = row.TryGetValue("English", out string english);

            if (!row.TryGetValue("Duration", out string durationStr) || !GoogleSheetValueParsers.TryParseFloat(durationStr, out float duration))
            {
                Log.Warning($"ID {id}: Duration 파싱 실패: {durationStr}");
                return false;
            }

            if (!row.TryGetValue("Arguments", out string argumentsStr) || !GoogleSheetValueParsers.TryParseInt(argumentsStr, out int arguments))
            {
                Log.Warning($"ID {id}: Arguments 파싱 실패: {argumentsStr}");
                return false;
            }

            // 모델 생성 및 기본 필드 설정
            StringDialogueData m = new()
            {
                ID = id,
                TimelineName = timelineName,
                Index = index,
                SpeakerName = speakerName,
                Korean = korean,
                English = english,
                Duration = duration,
                Arguments = arguments,
            };

            model = m;
            return true;
        }
    }
}
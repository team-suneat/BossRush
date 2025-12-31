using System.Collections.Generic;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public sealed class StringRowConverter : IGoogleSheetRowConverter<StringData>
    {
        public bool TryConvert(Dictionary<string, string> row, out StringData model)
        {
            model = null;

            _ = row.TryGetValue("ID", out string id);
            _ = row.TryGetValue("Korean", out string korean);
            _ = row.TryGetValue("English", out string english);
            if (!row.TryGetValue("Arguments", out string argumentsString) || !GoogleSheetValueParsers.TryParseInt(argumentsString, out int arguments))
            {
                Log.Warning($"{id}: Arguments 파싱 실패: {argumentsString}");
                return false;
            }

            StringData m = new()
            {
                ID = id,
                Korean = korean,
                English = english,
                Arguments = arguments,
            };

            model = m;
            return true;
        }
    }
}
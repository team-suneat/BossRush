using System.Text;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetLoadingTipString(this int index)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Loading_Tip_");
            stringBuilder.Append(index.ToString());

            string content = JsonDataManager.FindStringClone(stringBuilder.ToString());

            return ReplaceLoadingString(content);
        }

        public static string GetLoadingTitleString(this int index)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Loading_Worldview_Title_");
            stringBuilder.Append(index.ToString());

            string content = JsonDataManager.FindStringClone(stringBuilder.ToString());

            return ReplaceLoadingString(content);
        }

        public static string GetLoadingDescString(this int index)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Loading_Worldview_Desc_");
            stringBuilder.Append(index.ToString());

            string content = JsonDataManager.FindStringClone(stringBuilder.ToString());

            return ReplaceLoadingString(content);
        }

        public static string ReplaceLoadingString(string content)
        {
            content = ReplaceCharacterName(content);
            content = ReplaceAreaName(content);
            content = ReplaceStageName(content);
            
            content = ReplaceStatName(content);
            content = ReplaceStateEffect(content);

            return content;
        }
    }
}
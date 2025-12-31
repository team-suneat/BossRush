using System.Text;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetStringKey(this MapObjectNames key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Interaction_Name_");
            stringBuilder.Append(key.ToString());

            return stringBuilder.ToString();
        }

        public static string GetOperateString(this MapObjectNames key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Interaction_Operate_");
            stringBuilder.Append(key.ToString());

            return JsonDataManager.FindStringClone(stringBuilder.ToString());
        }

        public static string GetSubStringKey(this MapObjectNames key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Interaction_Sub_Name_");
            stringBuilder.Append(key.ToString());
            return stringBuilder.ToString();
        }
    }
}
using System.Text;
using TeamSuneat.Data;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        public static string GetLocalizedString(this UIPopupNames key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Popup_Title_");
            stringBuilder.Append(key.ToString());

            return JsonDataManager.FindStringClone(stringBuilder.ToString());
        }

        public static string GetLocalizedString(this DescriptionNames key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Description_Name_");
            stringBuilder.Append(key.ToString());

            return JsonDataManager.FindStringClone(stringBuilder.ToString());
        }

        public static string GetLocalizedString(this DetailsStringNames key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Details_Name_");
            stringBuilder.Append(key.ToString());

            return JsonDataManager.FindStringClone(stringBuilder.ToString());
        }

        public static string GetDescString(this DetailsStringNames key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Details_Description_");
            stringBuilder.Append(key.ToString());

            return JsonDataManager.FindStringClone(stringBuilder.ToString());
        }

        public static string GetLocalizedString(this SoliloquyTypes key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Soliloquy_");
            stringBuilder.Append(key.ToString());

            return JsonDataManager.FindStringClone(stringBuilder.ToString());
        }

        public static string GetLocalizedString(this EventMessageNames key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("EventMessage_Name_");
            stringBuilder.Append(key.ToString());

            return JsonDataManager.FindStringClone(stringBuilder.ToString());
        }

        public static string GetLocalizedString(this EventMessageNames key, string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("EventMessage_Name_");
            stringBuilder.Append(key.ToString());

            StringData data = JsonDataManager.FindStringData(stringBuilder.ToString());
            string result = Format(data, value);
            return result;
        }

        public static string GetDescString(this EventMessageNames key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("EventMessage_Description_");
            stringBuilder.Append(key.ToString());

            return JsonDataManager.FindStringClone(stringBuilder.ToString());
        }

        public static string GetDescString(this EventMessageNames key, string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("EventMessage_Description_");
            stringBuilder.Append(key.ToString());

            StringData data = JsonDataManager.FindStringData(stringBuilder.ToString());
            string result = Format(data, value);
            return result;
        }

        public static string GetDescString(this EventMessageNames key, string[] values)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("EventMessage_Description_");
            stringBuilder.Append(key.ToString());

            StringData data = JsonDataManager.FindStringData(stringBuilder.ToString());
            string result = Format(data, values);
            return result;
        }

        public static string GetLocalizedString(this ToggleSlotTypes key)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Tab_Name_");
            stringBuilder.Append(key.ToString());

            return JsonDataManager.FindStringClone(stringBuilder.ToString());
        }
    }
}
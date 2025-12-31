namespace TeamSuneat.Data
{
    public static class XScriptableObjectValidator
    {
        public static bool IsValid(this XScriptableObject scriptableObject)
        {
            if (scriptableObject == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(scriptableObject.NameString))
            {
                Log.Warning($"Scriptable Object가 유효하지 않습니다. ({scriptableObject.name})");
                return false;
            }

            return true;
        }
    }
}
namespace TeamSuneat
{
    public class Entity : XBehaviour
    {
        public SID SID;

        public void Generate()
        {
            SID = SID.Generate();

            Log.Info(LogTags.Vital, "{0}, 새로운 SID를 설정합니다. {1}", this.GetHierarchyName(), SID.ToString());
        }
    }
}
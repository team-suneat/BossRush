namespace TeamSuneat.Audio
{
    public class BGMPlayer : XBehaviour
    {
        public SoundNames BGMName;
        public string BGMNameString;

        public override void AutoSetting()
        {
            base.AutoSetting();

            // BGM 이름이 설정되어 있으면 문자열로 변환
            if (BGMName != SoundNames.None)
            {
                BGMNameString = BGMName.ToString();
            }
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref BGMName, BGMNameString);
        }

        protected override void OnStart()
        {
            base.OnStart();

            AudioManager.Instance.TryStartChangeBGM(BGMName);
        }
    }
}
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDStage : XBehaviour
    {
        [SerializeField]
        private UILocalizedText _indexText;

        [SerializeField]
        private UILocalizedText _nameText;

        protected override void OnStart()
        {
            base.OnStart();
            Refresh();
        }

        public void Refresh()
        {
            VProfile profileInfo = GameApp.GetSelectedProfile();
            if (profileInfo == null)
            {
                return;
            }

            int index = profileInfo.Stage.CurrentStage.ToInt();
            StringData data = JsonDataManager.FindStringData("Format_Stage_Index");
            string content = StringGetter.Format(data, index.ToString());
            _indexText.SetText(content);

            string stringKey = profileInfo.Stage.CurrentStage.GetStringKey();
            _nameText.SetStringKey(stringKey);
        }
    }
}
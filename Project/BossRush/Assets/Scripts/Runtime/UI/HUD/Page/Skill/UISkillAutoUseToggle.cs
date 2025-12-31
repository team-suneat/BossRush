using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UISkillAutoUseToggle : UIToggle
    {
        [SerializeField]
        private GameObject ActiveObject;

        protected override void OnToggleValueChange(bool isOn)
        {
            base.OnToggleValueChange(isOn);

            RefreshAutoUse(isOn);

            ActiveObject?.SetActive(isOn);
        }

        private void RefreshAutoUse(bool isOn)
        {
            VProfile profileInfo = GameApp.GetSelectedProfile();
            if (profileInfo != null)
            {
                profileInfo.Skill.IsAutoUse = isOn;
            }
        }
    }
}
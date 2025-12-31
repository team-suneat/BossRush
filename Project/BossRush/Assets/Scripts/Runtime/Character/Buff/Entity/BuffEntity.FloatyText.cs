using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
using Lean.Pool;

namespace TeamSuneat
{
    public partial class BuffEntity : XBehaviour, IPoolable
    {
        private Dictionary<string, UIFloatyText> _floatyTexts = new Dictionary<string, UIFloatyText>();

        public void SpawnSoliloquyForStat(int value)
        {
            // 버프의 스택 관련 텍스트를 생성하지 않습니다.
            if (AssetData.BlockSpawnSoliloquyOnStackChanged) { return; }

            // 능력치 버프가 아니라면 스택 관련 텍스트를 표시하지않습니다.
            if (Type != BuffTypes.Stat) { return; }

            // 지속시간을 가진 버프라면 스택 관련 텍스트를 표시하지않습니다.
            if (Duration > 0) { return; }

            // 플레이어 캐릭터의 버프가 아니라면 스택 관련 텍스트를 표시하지않습니다.
            if (!Owner.IsPlayer) { return; }

            if (value > 0)
            {
                string content = Name.GetLocalizedString().ToValueString();
                UIManager.Instance.SpawnSoliloquyIngame(SoliloquyTypes.StackEffect, content);
            }
            else if (value < 0)
            {
                string content = Name.GetLocalizedString().ToValueString();
                UIManager.Instance.SpawnSoliloquyIngame(SoliloquyTypes.UnstackEffect, content);
            }
        }

        public void SpawnFloatyTextByType()
        {
            switch (Type)
            {
                case BuffTypes.Immune:
                case BuffTypes.StateEffect:
                    {
                        if (!GameSetting.Instance.Play.UseStateEffectText)
                        {
                            return;
                        }

                        string content = null;
                        if (Type == BuffTypes.Immune)
                        {
                            string format = JsonDataManager.FindStringClone(StringDataLabels.FORMAT_IMMUNE);
                            content = string.Format(format, IncompatibleStateEffect.GetLocalizedString());
                        }
                        else if (Type == BuffTypes.StateEffect)
                        {
                            content = StateEffect.GetLocalizedString();
                            content = content.ToColorString(StateEffect.GetColor());
                        }

                        if (!_floatyTexts.ContainsKey(content))
                        {
                            UIFloatyText floatyText = SpawnFloatyText(content, UIFloatyMoveNames.Content);
                            if (floatyText != null)
                            {
                                RegisterFloatyText(content, floatyText);
                                floatyText.RegisterDespawnEvent(UnregisterFloatyText);
                            }
                        }
                        else
                        {
                            LogWarning("버프의 Floaty Text를 생성할 수 없습니다. 이미 같은 내용이 생성되어있습니다.");
                        }
                        break;
                    }
            }
        }

        private UIFloatyText SpawnFloatyText(string content, UIFloatyMoveNames floatyMove)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            if (Owner.MyVital != null)
            {
                return ResourcesManager.SpawnFloatyText(content, floatyMove, Owner.MyVital.transform);
            }

            return null;
        }

        private void RegisterFloatyText(string content, UIFloatyText floatyText)
        {
            _floatyTexts.Add(content, floatyText);
        }

        private void UnregisterFloatyText(UIFloatyText floatyText)
        {
            if (_floatyTexts.ContainsKey(floatyText.Content))
            {
                _floatyTexts.Remove(floatyText.Content);
            }
            else
            {
                LogWarning("버프의 Floaty Text를 등록 해제할 수 없습니다. 같은 내용을 찾을 수 없습니다. 내용: {0}, 등록된 Floaty Text 수: {1}", floatyText.Content, _floatyTexts.Count);
            }
        }
    }
}
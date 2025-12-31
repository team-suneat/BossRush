using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Passive", menuName = "TeamSuneat/Scriptable/Passive")]
    public class PassiveAsset : XScriptableObject
    {
        public PassiveNames Name;
        public int MaxLevel;

        public PassiveTriggerSettings TriggerSettings;
        public PassiveConditionSettings ConditionSettings;
        public PassiveEffectSettings EffectSettings;

        public int TID => BitConvert.Enum32ToInt(Name);

        public override void OnLoadData()
        {
            base.OnLoadData();

            TriggerSettings?.OnLoadData();
            ConditionSettings?.OnLoadData();
            EffectSettings?.OnLoadData();

            LogErrorInvalid();
        }

        private void LogErrorInvalid()
        {
#if UNITY_EDITOR
            if (Name == PassiveNames.None)
            {
                Log.Error("패시브의 이름이 설정되지 않았습니다: {0}", name);
            }
            if (MaxLevel == 0)
            {
                Log.Error("패시브의 최대 레벨이 설정되지 않았습니다: {0}", name);
            }
            if (TriggerSettings == null)
            {
                Log.Error("패시브의 발동(Trigger)이 설정되지 않았습니다. {0}", name);
            }
            else
            {
                if (TriggerSettings.Name != Name)
                {
                    Log.Error("패시브의 이름과 TriggerSettings의 이름이 같지 않습니다. {0}, {1}, {2}", TriggerSettings.name, Name, name);
                }
            }
            if (ConditionSettings != null)
            {
                if (ConditionSettings.Name != Name)
                {
                    Log.Error("패시브의 이름과 ConditionSettings의 이름이 같지 않습니다. {0}, {1}, {2}", ConditionSettings.name, Name, name);
                }
            }
            if (EffectSettings == null)
            {
                Log.Error("패시브의 효과가 설정되지 않았습니다. {0}", name);
            }
            else
            {
                if (EffectSettings.Name != Name)
                {
                    Log.Error("패시브의 이름과 EffectSettings의 이름이 같지 않습니다. {0}, {1}, {2}", EffectSettings.name, Name, name);
                }
            }

#endif
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            _ = EnumEx.ConvertTo(ref Name, NameString);

            TriggerSettings?.Validate();
            ConditionSettings?.Validate();
            EffectSettings?.Validate();
        }

        public override void Refresh()
        {
            NameString = Name.ToString();

            TriggerSettings?.Refresh();
            ConditionSettings?.Refresh();
            EffectSettings?.Refresh();

            base.Refresh();
        }

        public override bool RefreshWithoutSave()
        {
            bool hasChanged = false;

            UpdateIfChanged(ref NameString, Name);

            if (TriggerSettings != null)
            {
                if (TriggerSettings.RefreshWithoutSave())
                {
                    hasChanged = true;
                }
            }
            if (ConditionSettings != null)
            {
                if (ConditionSettings.RefreshWithoutSave())
                {
                    hasChanged = true;
                }
            }
            if (EffectSettings != null)
            {
                if (EffectSettings.RefreshWithoutSave())
                {
                    hasChanged = true;
                }
            }

            base.RefreshWithoutSave();

            return hasChanged;
        }

        public override void Rename()
        {
            Rename("Passive");
        }

        protected override void RefreshAll()
        {
#if UNITY_EDITOR
            if (Selection.objects.Length > 1)
            {
                Debug.LogWarning("여러 개의 스크립터블 오브젝트가 선택되었습니다. 하나만 선택한 상태에서 실행하세요.");
                return;
            }
#endif
            PassiveNames[] passiveNames = EnumEx.GetValues<PassiveNames>();
            int passiveCount = 0;

            Debug.LogFormat("모든 패시브 에셋의 갱신을 시작합니다: {0}", passiveNames.Length);

            base.RefreshAll();

            for (int i = 1; i < passiveNames.Length; i++)
            {
                if (passiveNames[i] != PassiveNames.None)
                {
                    PassiveAsset asset = ScriptableDataManager.Instance.FindPassive(passiveNames[i]);
                    if (asset.IsValid())
                    {
                        if (asset.RefreshWithoutSave())
                        {
                            passiveCount += 1;
                        }
                    }
                }

                float progressRate = (i + 1).SafeDivide(passiveNames.Length);
                EditorUtility.DisplayProgressBar("모든 패시브 에셋의 갱신", passiveNames[i].ToString(), progressRate);
            }

            EditorUtility.ClearProgressBar();
            OnRefreshAll();

            Debug.LogFormat("모든 패시브 에셋의 갱신을 종료합니다: {0}/{1}", passiveCount.ToSelectString(passiveNames.Length), passiveNames.Length);
        }

        [FoldoutGroup("#Button", false, 7)]
        [Button("패시브 설정 에셋 자동 생성", ButtonSizes.Large)]
        private void CreateSettings()
        {
            PassiveTriggerSettings trigger = CreateAssetInSameFolder<PassiveTriggerSettings>("Trigger", NameString.ToString(), true);
            PassiveConditionSettings condition = CreateAssetInSameFolder<PassiveConditionSettings>("Condition", NameString.ToString(), true);
            PassiveEffectSettings effect = CreateAssetInSameFolder<PassiveEffectSettings>("Effect", NameString.ToString(), true);

            if (trigger != null)
            {
                trigger.Name = Name;
                trigger.NameString = Name.ToString();
                trigger.RefreshWithoutSave();
                TriggerSettings = trigger;
            }
            if (condition != null)
            {
                condition.Name = Name;
                condition.NameString = Name.ToString();
                condition.RefreshWithoutSave();
                ConditionSettings = condition;
            }
            if (effect != null)
            {
                effect.Name = Name;
                effect.NameString = Name.ToString();
                effect.RefreshWithoutSave();
                EffectSettings = effect;
            }

            OnRefreshAll();
        }

        protected override void CreateAll()
        {
            base.CreateAll();

            PassiveNames[] passiveNames = EnumEx.GetValues<PassiveNames>();
            for (int i = 1; i < passiveNames.Length; i++)
            {
                if (passiveNames[i] == PassiveNames.None)
                {
                    continue;
                }

                PassiveAsset asset = ScriptableDataManager.Instance.FindPassive(passiveNames[i]);
                if (asset == null)
                {
                    asset = CreateAsset<PassiveAsset>("Passive", passiveNames[i].ToString(), true);
                    if (asset != null)
                    {
                        asset.NameString = passiveNames[i].ToString();
                        asset.Refresh();
                    }
                }
            }

            PathManager.UpdatePathMetaData();
        }

#endif
    }
}
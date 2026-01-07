using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;

namespace TeamSuneat
{
    public partial class AttackAreaEntity : AttackEntity
    {
        #region Editor

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _attackCollider = GetComponent<Collider2D>();
            ChainLightning = GetComponentInChildren<BaseChainLightning>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (_attackCollider != null)
            {
                _attackCollider.isTrigger = true;
            }

            if (StatNameOfSize != StatNames.None)
            {
                StatNameOfSizeString = StatNameOfSize.ToString();
            }

            if (StatNameOfActiveDuration != StatNames.None)
            {
                StatNameOfActiveDurationString = StatNameOfActiveDuration.ToString();
            }
            gameObject.layer = GameLayers.Detectable;
        }

        protected override void Validate()
        {
            base.Validate();

            EnumEx.ConvertTo(ref StatNameOfSize, StatNameOfSizeString);
            EnumEx.ConvertTo(ref StatNameOfActiveDuration, StatNameOfActiveDurationString);
        }


        private void OnDrawGizmos()
        {
            if (_attackCollider != null && _attackCollider.enabled)
            {
                GizmoEx.DrawGizmoCube(position + (Vector3)_attackCollider.offset, _attackCollider.bounds.size, GameColors.Dev);
            }
        }

        #endregion Editor

        #region Log

        protected override void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                if (Owner != null)
                {
                    Log.Progress(LogTags.Attack, StringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        protected override void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                if (Owner != null)
                {
                    Log.Info(LogTags.Attack, StringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        protected override void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                if (Owner != null)
                {
                    Log.Warning(LogTags.Attack, StringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }

            }
        }

        #endregion Log

        #region Button

#if UNITY_EDITOR

        [FoldoutGroup("#Custom Buttons", 1000)]
        [Button("플래그 영역을 포함하는 모든 공격 개체 검사 확인", ButtonSizes.Medium)]
        private void CheckFlagArea()
        {
            List<Character> characters = LoadPrefabsWithComponent<Character>("Assets/Resources/Prefabs/Character", null);
            for (int i1 = 0; i1 < characters.Count; i1++)
            {
                Character character = characters[i1];

                AttackAreaEntity[] entities = character.GetComponentsInChildren<AttackAreaEntity>();
                if (!entities.IsValid()) continue;

                for (int i = 0; i < entities.Length; i++)
                {
                    AttackAreaEntity entity = entities[i];
                    if (!entity.InitialApplyDelay.IsZero())
                    {
                        Log.Warning($"{entity.Name.ToLogString()} 영역 공격 개체에 지연 시간({entity.InitialApplyDelay.ToSelectString()}초)이 설정되어있습니다. 일반 공격 중 발사되면 비활성화 전에 적용되지 않을 가능성이 있습니다.");
                    }
                }
            }
        }

#endif

        #endregion Button
    }
}
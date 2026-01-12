using NodeCanvas.BehaviourTrees;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    // BehaviourTreeOwner를 사용하여 AI 행동을 제어하는 핸들러 클래스
    public class AIBrain : MonoBehaviour
    {
        private Character _ownerCharacter;
        private BehaviourTreeOwner _behaviourTreeOwner;

        public bool IsEnabled => _behaviourTreeOwner != null && enabled;

        private void Awake()
        {
            _ownerCharacter = GetComponent<Character>();
            _behaviourTreeOwner = GetComponent<BehaviourTreeOwner>();

            Deactivate();
        }

        public void Activate()
        {
            if (_behaviourTreeOwner != null)
            {
                _behaviourTreeOwner.enabled = true;

                if (Log.LevelInfo && _ownerCharacter != null)
                {
                    Log.Info(LogTags.Monster, $"{_ownerCharacter.Name.ToLogString()}, AI Brain을 활성화합니다.");
                }
            }
        }

        public void Deactivate()
        {
            if (_behaviourTreeOwner != null)
            {
                _behaviourTreeOwner.enabled = false;

                if (Log.LevelInfo && _ownerCharacter != null)
                {
                    Log.Info(LogTags.Monster, $"{_ownerCharacter.Name.ToLogString()}, AI Brain을 비활성화합니다.");
                }
            }
        }
    }
}
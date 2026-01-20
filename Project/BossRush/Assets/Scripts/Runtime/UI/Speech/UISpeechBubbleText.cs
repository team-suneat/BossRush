using Lean.Pool;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    public class UISpeechBubbleText : XBehaviour, IPoolable
    {
        public UILocalizedText Text;
        public AutoDespawn Despawner;
        public UIFollowObject FollowObject;

        private StringDialogueData _dialogueData;
        private SpeakerDialogueAsset _speakerAsset;
        private Character _speaker;
        private UnityAction<UISpeechBubbleText> _despawnCallback;

        public string Content => Text != null ? Text.Content : string.Empty;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Despawner ??= GetComponent<AutoDespawn>();
            FollowObject ??= GetComponent<UIFollowObject>();
            Text ??= GetComponentInChildren<UILocalizedText>();
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
        }

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
            CallDespawnEvent();
            ResetText();
            _dialogueData = null;
            _speakerAsset = null;
            _speaker = null;
        }

        public void RegisterDespawnEvent(UnityAction<UISpeechBubbleText> onDespawn)
        {
            _despawnCallback = onDespawn;
        }

        private void CallDespawnEvent()
        {
            if (_despawnCallback != null)
            {
                _despawnCallback.Invoke(this);
                _despawnCallback = null;
            }
        }

        public void Despawn()
        {
            Despawner?.Despawn();
        }

        public void Setup(StringDialogueData dialogueData, Character speaker)
        {
            if (dialogueData == null || speaker == null)
            {
                Log.Warning("UISpeechBubbleText Setup 실패: dialogueData 또는 speaker가 null입니다.");
                return;
            }

            _dialogueData = dialogueData;
            _speaker = speaker;

            // 화자 정보 조회
            _speakerAsset = ScriptableDataManager.Instance.FindSpeakerDialogue(dialogueData.SpeakerName);
            if (_speakerAsset == null)
            {
                Log.Warning("SpeakerDialogueAsset을 찾을 수 없습니다: {0}", dialogueData.SpeakerName);
            }

            // FollowObject 설정
            if (FollowObject != null)
            {
                Transform headPoint = speaker.HeadPoint != null ? speaker.HeadPoint : speaker.transform;
                FollowObject.Setup(headPoint);

                if (_speakerAsset != null && !_speakerAsset.SpeechBubbleOffset.IsZero())
                {
                    FollowObject.SetWorldOffset(_speakerAsset.SpeechBubbleOffset);
                }
            }

            // 텍스트 설정
            SetText(dialogueData.GetString());

            // 텍스트 색상 적용
            ApplyTextColor();

            // AutoDespawn Duration 설정
            if (Despawner != null)
            {
                Despawner.Duration = dialogueData.Duration;
            }
        }

        public void Interrupt()
        {
            if (Despawner != null)
            {
                Despawner.ForceDespawn();
            }
            else
            {
                Despawn();
            }
        }

        #region Text

        private void ResetText()
        {
            if (Text != null)
            {
                Text.ResetText();
            }
        }

        private void SetText(string content)
        {
            if (Text != null)
            {
                Text.SetText(content);
            }
        }

        private void ApplyTextColor()
        {
            if (Text != null && _speakerAsset != null)
            {
                Text.SetTextColor(_speakerAsset.TextColor);
            }
        }

        #endregion Text
    }
}

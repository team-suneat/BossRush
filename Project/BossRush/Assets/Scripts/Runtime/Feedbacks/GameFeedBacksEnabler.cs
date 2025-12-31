using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat.Feedbacks
{
    /// <summary>
    /// AutoPlayOnEnable 모드에 있는 경우 GameFeedbacks에 의해 자동으로 추가되는 도우미 클래스
    /// 부모 게임 개체가 비활성화/활성화된 경우 다시 플레이할 수 있습니다.
    /// </summary>
    [AddComponentMenu("")]
    public class GameFeedbacksEnabler : MonoBehaviour
    {
        /// 파일럿에 대한 TS 피드백
        public GameFeedbacks TargetGameFeedbacks { get; set; }

        /// <summary>
        /// 활성화 시 필요한 경우 GameFeedback을 다시 활성화(및 재생)합니다.
        /// </summary>
        protected virtual void OnEnable()
        {
            if ((TargetGameFeedbacks != null) && !TargetGameFeedbacks.enabled && TargetGameFeedbacks.AutoPlayOnEnable)
            {
                TargetGameFeedbacks.enabled = true;
            }
        }
    }
}


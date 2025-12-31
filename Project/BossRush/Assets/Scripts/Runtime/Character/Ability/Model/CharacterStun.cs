using System.Collections;
using TeamSuneat.Feedbacks;
using UnityEngine;

namespace TeamSuneat
{
    public class CharacterStun : CharacterAbility
    {
        public override Types Type => Types.Stun;

        private const string _stunnedAnimationParameterName = "Stunned";
        private int _stunnedAnimationParameter;

        private Coroutine _stunCoroutine;

        private float _duration;
        private float _elapsedTime;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            AbilityStartFeedbacks = this.FindComponent<GameFeedbacks>("Feedbacks/StunFeedbacks");
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
        }

        public void StartStun(float duration)
        {
            if (_stunCoroutine == null)
            {
                LogInfo("캐릭터의 기절을 시작합니다. {0}초", duration);
                _duration = duration;
                _stunCoroutine = StartXCoroutine(ProcessStun());
            }
            else
            {
                ResetElapsedTime();
            }
        }

        public void ResetElapsedTime()
        {
            LogInfo("기절의 지난시간을 초기화합니다. {0}/{1}", _elapsedTime, _duration);
            _elapsedTime = 0f;
        }

        public bool CheckStunning()
        {
            return _stunCoroutine != null;
        }

        protected IEnumerator ProcessStun()
        {
            Stun();

            _elapsedTime = 0f;

            while (_elapsedTime < _duration)
            {
                yield return null;

                _elapsedTime += Time.deltaTime;
            }

            OnStunExit();

            _stunCoroutine = null;
        }

        private void Stun()
        {
            if (Owner != null)
            {
                Owner.Attack?.DeactivateAll();
                Owner.ChangeConditionState(CharacterConditions.Stunned);

                Owner.CharacterAnimator?.StopAttacking();

                AbilityStartFeedbacks?.PlayFeedbacks();
            }
        }

        private void OnStunExit()
        {
            LogInfo("캐릭터의 기절을 종료합니다. {0}/{1}초", _elapsedTime, _duration);

            if (AbilityStartFeedbacks != null)
            {
                AbilityStartFeedbacks.StopFeedbacks();
            }
            if (AbilityStopFeedbacks != null)
            {
                AbilityStopFeedbacks.PlayFeedbacks();
            }

            if (Owner != null)
            {
                if (!Owner.IsAlive)
                {
                    LogProgress("캐릭터의 남은 생명력이 0입니다. 기절 상태에서 사망 상태로 설정합니다: {0}", Owner.ConditionState.CurrentState);
                    Owner.ChangeConditionState(CharacterConditions.Dead);
                }
                else if (Owner.ConditionState.Compare(CharacterConditions.Stunned))
                {
                    LogProgress("캐릭터의 조건 상태를 되돌립니다: {0}", Owner.ConditionState.CurrentState);
                    Owner.ChangeConditionState(CharacterConditions.Normal);
                }
                else
                {
                    LogProgress("캐릭터의 조건 상태를 초기화하지 않습니다: {0}", Owner.ConditionState.CurrentState);
                }

                if (Owner.IsPlayer)
                {
                    Owner.SetupAnimatorLayerWeight();
                }

                if (Owner != null)
                {
                    Owner.ExitCrwodControlToState();
                }
            }
        }

        public void ExitStun()
        {
            OnStunExit();

            StopXCoroutine(ref _stunCoroutine);
        }

        protected override void InitializeAnimatorParameters()
        {
            base.InitializeAnimatorParameters();

            if (Owner == null)
            {
                return;
            }

            if (Owner.CharacterAnimator == null)
            {
                return;
            }

            Owner.CharacterAnimator.AddAnimatorParameterIfExists(_stunnedAnimationParameterName, out _stunnedAnimationParameter, AnimatorControllerParameterType.Bool);
        }

        public override void UpdateAnimator()
        {
            base.UpdateAnimator();
            if (Owner == null)
            {
                return;
            }

            if (Owner.CharacterAnimator == null)
            {
                return;
            }

            bool isStunned = _conditionState.CurrentState == CharacterConditions.Stunned;
            Owner.CharacterAnimator.UpdateAnimatorBool(_stunnedAnimationParameter, isStunned);
        }
    }
}
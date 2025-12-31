using System.Linq;

namespace TeamSuneat
{
    /// <summary> Shield 클래스의 이벤트 관리 기능을 담당합니다. </summary>
    public partial class Shield
    {
        public delegate void OnDamageDelegate(DamageResult result);

        public delegate void OnDestroyDelegate();

        public OnDamageDelegate OnDamageEvent;
        public OnDestroyDelegate OnDestroyEvent;

        /// <summary> 보호막 피격 이벤트를 등록합니다. </summary>
        public void RegisterDamageEvent(OnDamageDelegate action)
        {
            if (OnDamageEvent != null)
            {
                System.Delegate[] delegateArray = OnDamageEvent.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    LogError("중복된 보호막 피격 이벤트를 등록할 수 없습니다. {0}", action.Method);
                    return;
                }
            }

            OnDamageEvent += action;
        }

        /// <summary> 보호막 피격 이벤트를 해제합니다. </summary>
        public void UnregisterDamageEvent(OnDamageDelegate action)
        {
            if (OnDamageEvent != null)
            {
                System.Delegate[] delegateArray = OnDamageEvent.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    OnDamageEvent -= action;
                    return;
                }
            }
        }

        /// <summary> 보호막 파괴 이벤트를 등록합니다. </summary>
        public void RegisterDestroyEvent(OnDestroyDelegate action)
        {
            if (OnDestroyEvent != null)
            {
                System.Delegate[] delegateArray = OnDestroyEvent.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    LogError("중복된 보호막 파괴 이벤트를 등록할 수 없습니다. {0}", action.Method);
                    return;
                }
            }

            OnDestroyEvent += action;
        }

        /// <summary> 보호막 파괴 이벤트를 해제합니다. </summary>
        public void UnregisterDestroyEvent(OnDestroyDelegate action)
        {
            if (OnDestroyEvent != null)
            {
                System.Delegate[] delegateArray = OnDestroyEvent.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    OnDestroyEvent -= action;
                    return;
                }
            }
        }

        /// <summary> 보호막 피격 시 호출됩니다. </summary>
        private void OnDamageShield(DamageResult damageResult)
        {
            DamageFeedbacks?.PlayFeedbacks();
            OnDamageEvent?.Invoke(damageResult);
        }

        /// <summary> 보호막 획득 시 호출됩니다. </summary>
        private void OnGainShield()
        {
            ActivateFeedbacks?.PlayFeedbacks();

            if (Vital.Owner != null && Vital.Owner.IsPlayer)
            {
                GlobalEvent<int, int>.Send(GlobalEventType.PLAYER_CHARACTER_SHIELD_CHARGE, Max, Max);
            }
        }

        /// <summary> 보호막 파괴 시 호출됩니다. </summary>
        private void OnDestroyShield()
        {
            ActivateFeedbacks?.StopFeedbacks();
            DestroyFeedbacks?.PlayFeedbacks();
            OnDestroyEvent?.Invoke();

            if (Vital.Owner != null && Vital.Owner.IsPlayer)
            {
                GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_SHIELD_CHARGE);
            }
        }
    }
}
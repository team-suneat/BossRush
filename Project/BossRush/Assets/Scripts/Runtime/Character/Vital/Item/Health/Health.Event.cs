using System;
using System.Linq;

namespace TeamSuneat
{
    /// <summary>
    /// 캐릭터의 생명력을 관리하는 클래스입니다.
    /// </summary>
    public partial class Health : VitalResource
    {
        #region Delegate

        public delegate void OnDamageDelegate(DamageResult result);

        public delegate void OnDamageZeroDelegate();

        public delegate void OnReviveDelegate();

        public delegate void OnDeathDelegate(DamageResult result);

        public delegate void OnKilledDelegate(Character attacker);

        public OnDamageDelegate OnDamage;
        public OnDamageZeroDelegate OnDamageZero;
        public OnReviveDelegate OnRevive;
        public OnDeathDelegate OnDeath;
        public OnKilledDelegate OnKilled;

        #endregion Delegate

        //─────────────────────────────────────────────────────────────────────────────────────────────────

        public void RegisterOnDamageEvent(OnDamageDelegate action)
        {
            if (OnDamage != null)
            {
                System.Delegate[] delegateArray = OnDamage.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    LogError("중복된 피격 이벤트는 등록할 수 없습니다. {0}", action.Method);
                    return;
                }
            }

            OnDamage += action;
        }

        public void RegisterOnDamageZeroEvent(OnDamageZeroDelegate action)
        {
            if (OnDamageZero != null)
            {
                System.Delegate[] delegateArray = OnDamageZero.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    LogError("중복된 피격 0 이벤트는 등록할 수 없습니다. {0}", action.Method);
                    return;
                }
            }

            OnDamageZero += action;
        }

        public void RegisterOnReviveEvent(OnReviveDelegate action)
        {
            if (OnRevive != null)
            {
                System.Delegate[] delegateArray = OnRevive.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    LogError("중복된 부활 이벤트는 등록할 수 없습니다. {0}", action.Method);
                    return;
                }
            }

            OnRevive += action;
        }

        public void RegisterOnDeathEvent(OnDeathDelegate action)
        {
            if (OnDeath != null)
            {
                System.Delegate[] delegateArray = OnDeath.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    LogError("중복된 사망 이벤트는 등록할 수 없습니다. {0}", action.Method);
                    return;
                }
            }

            OnDeath += action;
        }

        public void RegisterOnKilledEvent(OnKilledDelegate action)
        {
            if (OnKilled != null)
            {
                System.Delegate[] delegateArray = OnKilled.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    LogError("중복된 처치 이벤트는 등록할 수 없습니다. {0}", action.Method);
                    return;
                }
            }

            OnKilled += action;
        }

        //─────────────────────────────────────────────────────────────────────────────────────────────────

        public void UnregisterOnDamageEvent(OnDamageDelegate action)
        {
            if (OnDamage != null)
            {
                System.Delegate[] delegateArray = OnDamage.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    OnDamage -= action;
                    return;
                }
            }

            LogWarning("해제하려는 피격 이벤트가 등록되어 있지 않습니다. {0}", action.Method);
        }

        public void UnregisterOnDamageZeroEvent(OnDamageZeroDelegate action)
        {
            if (OnDamageZero != null)
            {
                System.Delegate[] delegateArray = OnDamageZero.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    OnDamageZero -= action;
                    return;
                }
            }

            LogWarning("해제하려는 피격 0 이벤트가 등록되어 있지 않습니다. {0}", action.Method);
        }

        public void UnregisterOnReviveEvent(OnReviveDelegate action)
        {
            if (OnRevive != null)
            {
                System.Delegate[] delegateArray = OnRevive.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    OnRevive -= action;
                    return;
                }
            }

            LogWarning("해제하려는 부활 이벤트가 등록되어 있지 않습니다. {0}", action.Method);
        }

        public void UnregisterOnDeathEvent(OnDeathDelegate action)
        {
            if (OnDeath != null)
            {
                System.Delegate[] delegateArray = OnDeath.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    OnDeath -= action;
                    return;
                }
            }

            LogWarning("해제하려는 사망 이벤트가 등록되어 있지 않습니다. {0}", action.Method);
        }

        public void UnregisterOnKilledEvent(OnKilledDelegate action)
        {
            if (OnKilled != null)
            {
                System.Delegate[] delegateArray = OnKilled.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    OnKilled -= action;
                    return;
                }
            }

            LogWarning("해제하려는 처치 이벤트가 등록되어 있지 않습니다. {0}", action.Method);
        }
    }
}
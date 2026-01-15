using System;
using System.Linq;

namespace TeamSuneat
{
    /// <summary>
    /// 캐릭터의 생명력을 관리하는 클래스입니다.
    /// </summary>
    public partial class Life : VitalResource
    {
        #region Delegate

        public delegate void OnDamageDelegate(DamageResult result);

        public delegate void OnDamageZeroDelegate();

        public delegate void OnReviveDelegate();

        public delegate void OnDeathDelegate(DamageResult result);

        public delegate void OnKilledDelegate(Character attacker);

        public event OnDamageDelegate OnDamage;
        public event OnDamageZeroDelegate OnDamageZero;
        public event OnReviveDelegate OnRevive;
        public event OnDeathDelegate OnDeath;
        public event OnKilledDelegate OnKilled;

        #endregion Delegate

        //─────────────────────────────────────────────────────────────────────────────────────────────────

        private bool IsEventRegistered<T>(T eventDelegate, T action) where T : Delegate
        {
            if (eventDelegate != null)
            {
                Delegate[] delegateArray = eventDelegate.GetInvocationList();
                return delegateArray.Contains(action);
            }
            return false;
        }

        //─────────────────────────────────────────────────────────────────────────────────────────────────

        public void RegisterOnDamageEvent(OnDamageDelegate action)
        {
            if (IsEventRegistered(OnDamage, action))
            {
                LogError("중복된 피격 이벤트는 등록할 수 없습니다. {0}", action.Method);
                return;
            }

            OnDamage += action;
        }

        public void RegisterOnDamageZeroEvent(OnDamageZeroDelegate action)
        {
            if (IsEventRegistered(OnDamageZero, action))
            {
                LogError("중복된 피격 0 이벤트는 등록할 수 없습니다. {0}", action.Method);
                return;
            }

            OnDamageZero += action;
        }

        public void RegisterOnReviveEvent(OnReviveDelegate action)
        {
            if (IsEventRegistered(OnRevive, action))
            {
                LogError("중복된 부활 이벤트는 등록할 수 없습니다. {0}", action.Method);
                return;
            }

            OnRevive += action;
        }

        public void RegisterOnDeathEvent(OnDeathDelegate action)
        {
            if (IsEventRegistered(OnDeath, action))
            {
                LogError("중복된 사망 이벤트는 등록할 수 없습니다. {0}", action.Method);
                return;
            }

            OnDeath += action;
        }

        public void RegisterOnKilledEvent(OnKilledDelegate action)
        {
            if (IsEventRegistered(OnKilled, action))
            {
                LogError("중복된 처치 이벤트는 등록할 수 없습니다. {0}", action.Method);
                return;
            }

            OnKilled += action;
        }

        //─────────────────────────────────────────────────────────────────────────────────────────────────

        public void UnregisterOnDamageEvent(OnDamageDelegate action)
        {
            if (IsEventRegistered(OnDamage, action))
            {
                OnDamage -= action;
            }
            else
            {
                LogWarning("해제하려는 피격 이벤트가 등록되어 있지 않습니다. {0}", action.Method);
            }
        }

        public void UnregisterOnDamageZeroEvent(OnDamageZeroDelegate action)
        {
            if (IsEventRegistered(OnDamageZero, action))
            {
                OnDamageZero -= action;
            }
            else
            {
                LogWarning("해제하려는 피격 0 이벤트가 등록되어 있지 않습니다. {0}", action.Method);
            }
        }

        public void UnregisterOnReviveEvent(OnReviveDelegate action)
        {
            if (IsEventRegistered(OnRevive, action))
            {
                OnRevive -= action;
            }
            else
            {
                LogWarning("해제하려는 부활 이벤트가 등록되어 있지 않습니다. {0}", action.Method);
            }
        }

        public void UnregisterOnDeathEvent(OnDeathDelegate action)
        {
            if (IsEventRegistered(OnDeath, action))
            {
                OnDeath -= action;
            }
            else
            {
                LogWarning("해제하려는 사망 이벤트가 등록되어 있지 않습니다. {0}", action.Method);
            }
        }

        public void UnregisterOnKilledEvent(OnKilledDelegate action)
        {
            if (IsEventRegistered(OnKilled, action))
            {
                OnKilled -= action;
            }
            else
            {
                LogWarning("해제하려는 처치 이벤트가 등록되어 있지 않습니다. {0}", action.Method);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public partial class BuffSystem : XBehaviour
    {
        public Character Owner;

        /// <summary> 버프의 스택을 관리합니다. </summary>
        private Dictionary<BuffNames, BuffStack> _stacks = new();

        /// <summary> 버프 독립체를 관리합니다. </summary>
        private Dictionary<BuffNames, BuffEntity> _entities = new();

        /// <summary> 상태이상 버프 독립체를 관리합니다. </summary>
        private ListMultiMap<StateEffects, BuffEntity> _stateEntities = new();

        private HashSet<StateEffects> _activeStateEffects = new();

        /// <summary> 유휴중인 버프 이름을 관리합니다. </summary>
        private List<BuffNames> _restingBuffs = new();

        /// <summary>
        /// 호환 불가한 버프 이름을 관리합니다.
        /// Key: 호환 불가 버프 이름 / Value: 호환 불가를 설정한 버프 이름
        /// </summary>
        private ListMultiMap<BuffNames, BuffNames> _incompatibleBuffs = new();

        /// <summary>
        /// 호환 불가한 상태이상을 관리합니다.
        /// Key: 호환 불가 상태이상 / Value: 호환 불가를 설정한 버프 이름
        /// </summary>
        private ListMultiMap<StateEffects, BuffNames> _incompatibleStateEffects = new();

        private readonly UnityEvent<BuffNames, int> _onRefreshEvent = new();

        public BuffEntity[] Entities => _entities.Values.ToArray();

        protected void Awake()
        {
            RegisterBuffStackComponents();
        }

        private void RegisterBuffStackComponents()
        {
            BuffStack[] stacks = GetComponentsInChildren<BuffStack>();
            if (stacks != null)
            {
                for (int i = 0; i < stacks.Length; i++)
                {
                    _stacks.Add(stacks[i].Name, stacks[i]);
                }
            }
        }

        public void OnBattleReady()
        {
        }

        #region 레벨 (Level)

        public void SetLevel(BuffNames buffName, int level)
        {
            if (ContainsKey(buffName))
            {
                _entities[buffName].SetLevel(level);
                _entities[buffName].RefreshStats();
            }
        }

        #endregion 레벨 (Level)

        #region 한 번에 적용 (Apply At Once)

        public void ApplyAtOnce(StateEffects stateEffect)
        {
            List<BuffEntity> stateBuffEntities;
            if (_stateEntities.TryGetValue(stateEffect, out stateBuffEntities))
            {
                for (int i = stateBuffEntities.Count - 1; i >= 0; i--)
                {
                    BuffEntity entity = stateBuffEntities[i];
                    entity.ApplyAtOnce();
                }
            }
        }

        #endregion 한 번에 적용 (Apply At Once)

        #region 글로벌 이벤트 (Global Event)

        /// <summary>
        /// 버프 추가 글로벌 이벤트를 전송합니다.
        /// </summary>
        private IEnumerator SendAddedBuffGlobalEvent(BuffNames buffName, int buffLevel)
        {
            yield return null;

            if (Owner.IsPlayer)
            {
                GlobalEvent<BuffNames, int>.Send(GlobalEventType.PLAYER_CHARACTER_ADD_BUFF, buffName, buffLevel);
            }
            else
            {
                GlobalEvent<BuffNames, int, SID>.Send(GlobalEventType.MONSTER_CHARACTER_ADD_BUFF, buffName, buffLevel, Owner.SID);
            }
        }

        /// <summary>
        /// 버프 삭제 글로벌 이벤트를 전송합니다.
        /// </summary>
        private IEnumerator SendRemovedBuffGlobalEvent(BuffNames buffName)
        {
            yield return null;

            if (Owner.IsPlayer)
            {
                GlobalEvent<BuffNames>.Send(GlobalEventType.PLAYER_CHARACTER_REMOVE_BUFF, buffName);
            }
            else
            {
                GlobalEvent<BuffNames, SID>.Send(GlobalEventType.MONSTER_CHARACTER_REMOVE_BUFF, buffName, Owner.SID);
            }
        }

        /// <summary>
        /// 상태이상 추가 글로벌 이벤트를 전송합니다.
        /// </summary>
        private IEnumerator SendAddedStateEffectGlobalEvent(StateEffects stateEffect)
        {
            yield return null;

            if (Owner.IsPlayer)
            {
                GlobalEvent<StateEffects>.Send(GlobalEventType.PLAYER_CHARACTER_ADD_STATE_EFFECT, stateEffect);
            }
            else
            {
                GlobalEvent<StateEffects>.Send(GlobalEventType.MONSTER_CHARACTER_ADD_STATE_EFFECT, stateEffect);
            }
        }

        /// <summary>
        /// 상태이상 삭제 글로벌 이벤트를 전송합니다.
        /// </summary>
        private IEnumerator SendRemovedStateEffectGlobalEvent(StateEffects stateEffect)
        {
            yield return null;

            if (Owner.IsPlayer)
            {
                GlobalEvent<StateEffects>.Send(GlobalEventType.PLAYER_CHARACTER_REMOVE_STATE_EFFECT, stateEffect);
            }
            else
            {
                GlobalEvent<Character, StateEffects>.Send(GlobalEventType.MONSTER_CHARACTER_REMOVE_STATE_EFFECT, Owner, stateEffect);
            }
        }

        #endregion 글로벌 이벤트 (Global Event)

        #region 유휴 시간 (Resting)

        private bool ContainsRestingBuff(BuffNames buffName)
        {
            if (_restingBuffs.Contains(buffName))
            {
                return true;
            }

            return false;
        }

        public void StartRestTimer(BuffNames buffName, float restTime)
        {
            if (!_restingBuffs.Contains(buffName))
            {
                _ = StartXCoroutine(ProcessResting(buffName, restTime));
            }
        }

        private IEnumerator ProcessResting(BuffNames buffName, float restTime)
        {
            LogInfo("{0}, 버프의 유휴를 시작합니다. 유휴시간동안 추가되는 버프를 무시합니다. 유휴 시간: {1}", buffName.ToLogString(), restTime.ToSelectString());

            _restingBuffs.Add(buffName);

            yield return new WaitForSeconds(restTime);

            _restingBuffs.Remove(buffName);

            LogInfo("{0}, 버프의 유휴를 종료합니다.", buffName.ToLogString());
        }

        #endregion 유휴 시간 (Resting)
    }
}
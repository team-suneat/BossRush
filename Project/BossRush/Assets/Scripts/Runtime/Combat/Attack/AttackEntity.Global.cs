using System.Collections;

using UnityEngine;

namespace TeamSuneat
{
    public partial class AttackEntity : XBehaviour
    {
        private void StartSendExecuteAttackGlobalEvent()
        {
            StartXCoroutine(SendExecuteAttackGlobalEvent());
        }

        private IEnumerator SendExecuteAttackGlobalEvent()
        {
            if (Owner == null) { yield break; }
            if (!Owner.IsPlayer) { yield break; }
            if (_damageCaculator == null)
            {
                Log.Error("데미지 클래스가 설정되지 않았습니다: {0}", Name.ToLogString());
                yield break;
            }

            yield return null;

            if (!_damageCaculator.DamageResults.IsValid())
            {
                GlobalEvent<HitmarkNames, int, Vector3, AttackEntityTypes>.Send(GlobalEventType.PLAYER_CHARACTER_EXECUTE_ATTACK_FAILED,
                    Name, Index, position, EntityType);
            }
            else
            {
                for (int i = 0; i < _damageCaculator.DamageResults.Count; i++)
                {
                    DamageResult item = _damageCaculator.DamageResults[i];
                    GlobalEvent<DamageResult, int>.Send(GlobalEventType.PLAYER_CHARACTER_EXECUTE_ATTACK_SUCCESS, item, Index);
                }
            }
        }
    }
}
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Attack")]
    public class ActionSetAttackOrder : ActionTask<Character>
    {
        [Tooltip("공격 순서 리스트 (예: 0, 1, 2 = Attack1, Attack2, Attack3)")]
        public List<int> AttackOrder = new List<int> { 0 };

        protected override void OnExecute()
        {
            if (agent == null || agent.StateMachine == null)
            {
                EndAction(false);
                return;
            }

            if (agent.StateMachine is MonsterStateMachine monsterStateMachine)
            {
                // 유효성 검사
                if (AttackOrder == null || AttackOrder.Count == 0)
                {
                    Log.Warning(LogTags.CharacterState, "ActionSetAttackOrder: 공격 순서가 비어있습니다.");
                    EndAction(false);
                    return;
                }

                monsterStateMachine.SetAttackOrder(AttackOrder);
                EndAction(true);
            }
            else
            {
                Log.Warning(LogTags.CharacterState, "ActionSetAttackOrder는 MonsterStateMachine에서만 사용할 수 있습니다.");
                EndAction(false);
            }
        }

        protected override string info
        {
            get
            {
                if (AttackOrder == null || AttackOrder.Count == 0)
                {
                    return "공격 순서 설정 (비어있음)";
                }

                string orderString = string.Join(", ", AttackOrder);
                return $"공격 순서 설정: [{orderString}]";
            }
        }
    }
}

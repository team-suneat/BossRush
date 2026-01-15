using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Attack")]
    public class ActionSetRandomAttackOrder : ActionTask<Character>
    {
        [Tooltip("공격 인덱스 최소값")]
        public int MinAttackIndex = 0;

        [Tooltip("공격 인덱스 최대값")]
        public int MaxAttackIndex = 2;

        [Tooltip("설정된 공격 인덱스")]
        public List<int> AttackOrderList;

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
                if (MinAttackIndex > MaxAttackIndex)
                {
                    Log.Warning(LogTags.CharacterState, $"ActionSetRandomAttackOrder: MinAttackIndex({MinAttackIndex})가 MaxAttackIndex({MaxAttackIndex})보다 큽니다.");
                    EndAction(false);
                    return;
                }

                // 무작위 공격 순서 생성
                GenerateRandomAttackOrder();

                monsterStateMachine.SetAttackOrder(AttackOrderList);
                EndAction(true);
            }
            else
            {
                Log.Warning(LogTags.CharacterState, "ActionSetRandomAttackOrder는 MonsterStateMachine에서만 사용할 수 있습니다.");
                EndAction(false);
            }
        }

        private void GenerateRandomAttackOrder()
        {
            AttackOrderList ??= new List<int>();
            AttackOrderList.Clear();
            int randomIndex = RandomEx.Range(MinAttackIndex, MaxAttackIndex + 1);
            AttackOrderList.Add(randomIndex);
        }

        protected override string info
        {
            get
            {
                return $"무작위 공격 순서 설정: [{MinAttackIndex}~{MaxAttackIndex}]";
            }
        }
    }
}
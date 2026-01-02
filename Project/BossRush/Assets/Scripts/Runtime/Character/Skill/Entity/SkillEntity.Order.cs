using System.Collections;
using Lean.Pool;
using Sirenix.OdinInspector;

using UnityEngine;

namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        [FoldoutGroup("#SkillEntity-Order", 3)]
        [SuffixLabel("공격 순서를 초기화하기까지의 지연 시간")]
        public float DropOrderWaitTime;

        [FoldoutGroup("#SkillEntity-Order", 3)]
        [SuffixLabel("착지시 공격 순서 초기화")]
        public bool DropOrderInAirOnGrounded = false;

        [FoldoutGroup("#SkillEntity-Order", 3)]
        [LabelText("공격 순서에 따른 VFX")]
        public OrderSkillVFXData[] OrderSkillVFXs;

        private readonly Order _skillOrderOnGround = new();
        private readonly Order _skillOrderInAir = new();

        private float _elapsedTimeForOrder;

        private void SetOrderRange()
        {
            int groundMaxOrder = GetSkillMaxOrder(true);
            _skillOrderOnGround.SetMin(0);
            _skillOrderOnGround.SetMax(groundMaxOrder);

            int airMaxOrder = GetSkillMaxOrder(false);
            _skillOrderInAir.SetMin(0);
            _skillOrderInAir.SetMax(airMaxOrder);

            LogProgressMaxOrderSet(groundMaxOrder, airMaxOrder);
        }

        private Order GetCurrentOrder()
        {
            if (Owner == null)
            {
                return null;
            }

            return Owner.Controller.State.IsGrounded ? _skillOrderOnGround : _skillOrderInAir;
        }

        //------------------------------------------------------------------------------------------------------------------

        public bool CheckWaitingCombo()
        {
            if (DropOrderWaitTime > 0)
            {
                Order skillOrder = GetCurrentOrder();
                if (skillOrder != null && skillOrder.DropCoroutine != null)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckCastingSkillOrder()
        {
            Order skillOrder = GetCurrentOrder();
            return skillOrder != null && skillOrder.Current > 0 && skillOrder.Current < skillOrder.Max;
        }

        private bool CheckMaxOrderValid(Order order)
        {
            return order != null && order.Max > 0;
        }

        //------------------------------------------------------------------------------------------------------------------

        public void SetSkillOrderToFirst(Order skillOrder)
        {
            skillOrder.First();
            LogProgressSkillOrderReset(skillOrder.Current, skillOrder.Max);
        }

        private void NextSkillOrder(Order skillOrder)
        {
            if (!CheckMaxOrderValid(skillOrder))
            {
                return;
            }

            if (!skillOrder.CheckMax())
            {
                _ = skillOrder.Next();
                LogProgressNextSkillOrder(skillOrder.Current, skillOrder.Max);
            }

            if (skillOrder.CheckMax())
            {
                LogProgressMaxSkillOrderReached(_skillCount, _skillData.MaxCount);
                UseSkillCount();
            }
        }

        private void StartDropOrder(Order skillOrder)
        {
            if (skillOrder.DropCoroutine == null)
            {
                LogProgressStartDropOrder(DropOrderWaitTime);
                skillOrder.DropCoroutine = StartXCoroutine(ProcessDropOrder(skillOrder, DropOrderWaitTime));
            }
            else
            {
                _elapsedTimeForOrder = Mathf.Max(_elapsedTimeForOrder - DropOrderWaitTime, 0);
                LogProgressReduceElapsedTime(DropOrderWaitTime, _elapsedTimeForOrder);
            }
        }

        private void StopDropOrder(Order skillOrder)
        {
            StopXCoroutine(ref skillOrder.DropCoroutine);
            _elapsedTimeForOrder = 0f;
            LogProgressStopDropOrder();
        }

        private IEnumerator ProcessDropOrder(Order skillOrder, float dropOrderTime)
        {
            while (dropOrderTime > _elapsedTimeForOrder)
            {
                _elapsedTimeForOrder += Time.deltaTime;
                yield return null;
            }

            SetSkillOrderToFirst(skillOrder);
            UseSkillCount();
            
            // ReduceCooldownTime(_elapsedTimeForOrder);
            // StartCooldown();

            skillOrder.DropCoroutine = null;
            _elapsedTimeForOrder = 0f;
        }

        //

        public bool CheckMaxOrderOfGround()
        {
            if (_skillOrderOnGround == null)
            {
                return false;
            }

            if (!_skillOrderOnGround.CheckMax())
            {
                return false;
            }

            return true;
        }

        public void StopDropOrderByGround()
        {
            StopDropOrder(_skillOrderOnGround);
        }

        public void SetFirstSkillOrderGround()
        {
            SetSkillOrderToFirst(_skillOrderOnGround);
        }

        #region Log

        public void LogProgressRestOrderGroundLastSkill()
        {
            if (Log.LevelProgress)
            {
                string content = $"이전 기술의 순서가 최대로 설정되어있다면, 기술의 지상 순서({_skillOrderOnGround.Current})를 초기화합니다.";
                Log.Progress(LogTags.Skill_Order, string.Format("[Entity] {0}, {1}", Name.ToLogString(), content));
            }
        }

        private void LogProgressMaxOrderSet(int groundMaxOrder, int airMaxOrder)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format("기술의 최대 순서를 설정합니다. 지상:{0}, 공중:{1}", groundMaxOrder.ToSelectString(0), airMaxOrder.ToSelectString(0));
                Log.Progress(LogTags.Skill_Order, string.Format("[Entity] {0}, {1}", Name.ToLogString(), content));
            }
        }

        private void LogProgressSkillOrderReset(int current, int max)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format("지상 기술의 순서를 처음으로 돌립니다. {0}/{1}", current, max);
                Log.Progress(LogTags.Skill_Order, string.Format("[Entity] {0}, {1}", Name.ToLogString(), content));
            }
        }

        private void LogProgressNextSkillOrder(int current, int max)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format("지상 기술의 순서를 다음 순서로 넘깁니다. {0}/{1}", current, max);
                Log.Progress(LogTags.Skill_Order, string.Format("[Entity] {0}, {1}", Name.ToLogString(), content));
            }
        }

        private void LogProgressMaxSkillOrderReached(int skillCount, int maxCount)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format("지상 기술의 순서가 최대 순서라면 기술 횟수를 사용합니다. {0}/{1}", skillCount, maxCount);
                Log.Progress(LogTags.Skill_Order, string.Format("[Entity] {0}, {1}", Name.ToLogString(), content));
            }
        }

        private void LogProgressStartDropOrder(float waitTime)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format("지상 기술의 콤보 대기를 시작합니다. {0}", waitTime.ToString());
                Log.Progress(LogTags.Skill_Order, string.Format("[Entity] {0}, {1}", Name.ToLogString(), content));
            }
        }

        private void LogProgressReduceElapsedTime(float reduceTime, float elapsedTime)
        {
            if (Log.LevelProgress)
            {
                string content = string.Format("지상 기술의 콤보의 지난 시간을 감소시킵니다. 감소시간: {0}, 지난 시간:{1}", ValueStringEx.GetValueString(reduceTime), elapsedTime);
                Log.Progress(LogTags.Skill_Order, string.Format("[Entity] {0}, {1}", Name.ToLogString(), content));
            }
        }

        private void LogProgressStopDropOrder()
        {
            if (Log.LevelProgress)
            {
                string content = "지상 기술의 콤보 대기를 종료합니다.";
                Log.Progress(LogTags.Skill_Order, string.Format("[Entity] {0}, {1}", Name.ToLogString(), content));
            }
        }

        #endregion Log
    }
}
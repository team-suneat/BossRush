using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public class PathController : XBehaviour
    {
        public Character Owner;
        public Transform Parent;
        public PositionGroup[] Groups;
        public float[] SpeedMultipliers;

        private UnityAction m_onMovedArrivalCallback;
        private float m_defaultSpeedMultiplier = 1f;

#if UNITY_EDITOR

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner ??= this.FindFirstParentComponent<Character>();
            Parent ??= this.FindFirstParentComponent<Transform>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (Groups != null && Groups.Length > 0)
            {
                if (SpeedMultipliers == null || SpeedMultipliers.Length != Groups.Length)
                {
                    SpeedMultipliers = new float[Groups.Length];

                    for (int i = 0; i < SpeedMultipliers.Length; i++)
                    {
                        SpeedMultipliers[i] = 1f;
                    }
                }
            }
        }

#endif

        protected virtual void Awake()
        {
            if (Owner == null)
            {
                Owner = this.FindFirstParentComponent<Character>();
            }
        }

        public void StartMove(int index)
        {
            if (Groups.Length <= index)
            {
                return;
            }

            if (Groups[index] == null)
            {
                return;
            }

            Vector3 targetPosition = Groups[index].GetPosition(position, index);
            DebugEx.DrawLine(transform.position, targetPosition, Color.yellow, 3f);
            DebugEx.DrawCross(targetPosition, 3f);

            if (Owner != null)
            {
                if (Owner.Controller != null)
                {
                    Owner.Controller.UseOnceFollowing = true;
                    m_defaultSpeedMultiplier = Owner.Controller.MoveSpeedMultiplier;
                    Owner.Controller.MoveSpeedMultiplier = SpeedMultipliers[index];
                    Owner.Controller.SetTargetToFollow(targetPosition);
                    Owner.Controller.SetFollowCompletedEvent(CallMovedArrivalEvent);
                }
            }
        }

        public void StopMove()
        {
            CallMovedArrivalEvent();
        }

        public void SetMovedArrivalCallback(UnityAction callback)
        {
            m_onMovedArrivalCallback = callback;
        }

        private void CallMovedArrivalEvent()
        {
            if (Owner != null)
            {
                if (Owner.Controller != null)
                {
                    Owner.Controller.MoveSpeedMultiplier = m_defaultSpeedMultiplier;

                    Owner.Controller.ResetTargetToFollow();
                }
            }

            if (m_onMovedArrivalCallback != null)
            {
                m_onMovedArrivalCallback.Invoke();
            }
        }
    }
}

/*
 * 패턴을 정하면
 * 패턴에 지정된 종료 지점 후보군에서 종료지점을 뽑아서 이동한다.
 */
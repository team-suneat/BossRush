using UnityEngine;

namespace TeamSuneat
{
    public class AnimatorStateMachine : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            IAnimatorStateMachine stateMachine = animator.GetComponent<IAnimatorStateMachine>();

            if (null != stateMachine)
            {
                stateMachine.OnAnimatorStateEnter(animator, stateInfo, layerIndex);
            }
            else
            {
                stateMachine = animator.GetComponentInParent<IAnimatorStateMachine>();

                if (null != stateMachine)
                {
                    stateMachine.OnAnimatorStateEnter(animator, stateInfo, layerIndex);
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            IAnimatorStateMachine stateMachine = animator.GetComponent<IAnimatorStateMachine>();

            if (null != stateMachine)
            {
                stateMachine.OnAnimatorStateExit(animator, stateInfo, layerIndex);
            }
            else
            {
                stateMachine = animator.GetComponentInParent<IAnimatorStateMachine>();

                if (null != stateMachine)
                {
                    stateMachine.OnAnimatorStateExit(animator, stateInfo, layerIndex);
                }
            }
        }
    }
}
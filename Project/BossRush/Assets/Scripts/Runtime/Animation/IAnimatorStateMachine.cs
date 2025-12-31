using UnityEngine;

namespace TeamSuneat
{
    public interface IAnimatorStateMachine
    {
        void OnAnimatorStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);

        void OnAnimatorStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
    }
}
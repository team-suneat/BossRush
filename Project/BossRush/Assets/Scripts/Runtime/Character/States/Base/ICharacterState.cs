using UnityEngine;

namespace TeamSuneat
{
    public interface ICharacterState
    {
        void OnEnter();

        void OnUpdate();

        void OnFixedUpdate();

        void OnExit();

        void OnJumpRequested();

        void OnDashRequested(Vector2 direction);

        bool CanTransitionTo(CharacterState targetState);
    }
}
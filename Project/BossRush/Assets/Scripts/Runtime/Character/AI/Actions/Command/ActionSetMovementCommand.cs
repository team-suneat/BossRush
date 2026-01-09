using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Command")]
    [Description("이동 명령을 설정합니다.")]
    public class ActionSetMovementCommand : ActionTask<Character>
    {
        [BlackboardOnly]
        public BBParameter<Vector2> direction = Vector2.zero;

        [BlackboardOnly]
        public BBParameter<float> horizontalInput = 0f;

        [BlackboardOnly]
        public BBParameter<float> verticalInput = 0f;

        protected override void OnUpdate()
        {
            if (agent == null)
            {
                EndAction(false);
                return;
            }

            if (direction.value != Vector2.zero)
            {
                agent.SetHorizontalInput(direction.value.x);
                agent.SetVerticalInput(direction.value.y);
            }
            else
            {
                if (horizontalInput.value != 0f)
                {
                    agent.SetHorizontalInput(horizontalInput.value);
                }
                if (verticalInput.value != 0f)
                {
                    agent.SetVerticalInput(verticalInput.value);
                }
            }

            EndAction(true);
        }
    }
}

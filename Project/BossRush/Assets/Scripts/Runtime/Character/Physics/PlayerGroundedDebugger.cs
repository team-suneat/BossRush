using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 플레이어의 IsGrounded 상태를 시각적으로 디버깅하기 위한 컴포넌트
    /// 에디터에서만 사용되며, 빌드에는 포함되지 않습니다.
    /// </summary>
    public class PlayerGroundedDebugger : MonoBehaviour
    {
        [SerializeField] private Color _groundedColor = Color.green;
        [SerializeField] private Color _airborneColor = Color.red;
        [SerializeField] private string _modelChildName = "GroundDebug";

        private CharacterPhysics _physics;
        private SpriteRenderer _spriteRenderer;
        private bool _lastGroundedState;

        private void Awake()
        {
            if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
            {
                gameObject.SetActive(false);
                return;
            }

            _physics = GetComponent<CharacterPhysics>();
            if (_physics == null)
            {
                Log.Warning("PlayerGroundedDebugger: CharacterPhysics를 찾을 수 없습니다.");
                gameObject.SetActive(false);
                return;
            }

            SetupSpriteRenderer();
        }

        private void SetupSpriteRenderer()
        {
            Transform modelChild = transform.Find(_modelChildName);
            if (modelChild != null)
            {
                _spriteRenderer = modelChild.GetComponent<SpriteRenderer>();

                if (_spriteRenderer == null)
                {
                    Log.Warning($"PlayerGroundedDebugger: '{_modelChildName}'에 SpriteRenderer가 없습니다.");
                    enabled = false;
                }
            }
            else
            {
                Log.Warning($"PlayerGroundedDebugger: '{_modelChildName}' 자식 오브젝트를 찾을 수 없습니다.");
                enabled = false;
            }
        }

        private void Update()
        {
            if (_spriteRenderer == null || _physics == null)
            {
                return;
            }

            bool currentGrounded = _physics.IsGrounded;

            // 상태가 변경될 때만 색상 업데이트
            if (currentGrounded != _lastGroundedState)
            {
                _spriteRenderer.color = currentGrounded ? _groundedColor : _airborneColor;
                _lastGroundedState = currentGrounded;
            }
        }
    }
}
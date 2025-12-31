using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public class VFXAnimator : XBehaviour, IAnimatorStateMachine
    {
        [SerializeField]
        private VFXObject _owner;

        [SerializeField]
        private Animator[] _animators;

        public bool UseDespawnAnimation;

        [LabelText("무작위 파라메터 사용")]
        public bool UseRandomParameter;

        [LabelText("무작위 최대 파라메터")]
        [EnableIf("UseRandomParameter")]
        public int MaxRandomParameter;

        private const string CYCLE_OFFSET_PARAMETER_NAME = "CycleOffset";

        private const string RANDOM_PARAMTER_NAME = "Random";

        private bool _isDespawning;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _owner = this.FindFirstParentComponent<VFXObject>();

            _animators = GetComponentsInChildren<Animator>();
        }

        protected void Awake()
        {
            if (_owner == null)
            {
                _owner = this.FindFirstParentComponent<VFXObject>();
            }

            if (_animators == null)
            {
                _animators = GetComponentsInChildren<Animator>();
            }
        }

        public void Initialize()
        {
            if (_animators != null)
            {
                for (int i = 0; i < _animators.Length; i++)
                {
                    _animators[i].UpdateAnimatorFloatIfExists(CYCLE_OFFSET_PARAMETER_NAME, RandomEx.GetFloatValue());

                    if (UseRandomParameter)
                    {
                        _animators[i].UpdateAnimatorFloatIfExists(RANDOM_PARAMTER_NAME, RandomEx.Range(0, MaxRandomParameter));
                    }
                }
            }
        }

        public void SetSpeed(float speed)
        {
            if (_animators != null)
            {
                _animators.SetAnimatorSpeed(speed);
            }
        }

        public bool PlayDespawnAnimation()
        {
            if (_animators == null || _animators.Length == 0)
            {
                return false;
            }

            if (!UseDespawnAnimation || _isDespawning)
            {
                return false;
            }

            for (int i = 0; i < _animators.Length; i++)
            {
                if (_animators[i].UpdateAnimatorTriggerIfExists("Despawn"))
                {
                    _isDespawning = true;
                }
            }

            if (_isDespawning)
            {
                return true;
            }

            Log.Error("{0}, Despawn 애니메이션을 사용하지 않는 애니메이터에 UseDespawnAnimation이 설정되어있습니다.", this.GetHierarchyPath());
            return false;
        }

        public virtual void OnAnimatorStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public virtual void OnAnimatorStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName("Play"))
            {
                _owner.InstantDespawn();
            }
            else if (stateInfo.IsName("Despawn"))
            {
                ResetDespawningState();

                _owner.InstantDespawn();
            }
        }

        public void ResetDespawningState()
        {
            _isDespawning = false;
        }

        public void UpdateAnimatorFloatIfExists(string parameterName, float value)
        {
            for (int i = 0; i < _animators.Length; i++)
            {
                _animators[i].UpdateAnimatorFloatIfExists(parameterName, value);
            }
        }
    }
}
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace TeamSuneat
{
    public class MonsterAttackableArea : XBehaviour
    {
        [SerializeField] private DetectAreaTypes _detectAreaType;

        [ShowIf("_detectAreaType", DetectAreaTypes.Box)]
        [SerializeField] private Vector2 _boxSize;

        [ShowIf("_detectAreaType", DetectAreaTypes.Circle)]
        [SerializeField] private float _radius;

        private Character _ownerCharacter;

        private void Awake()
        {
            _ownerCharacter = this.FindFirstParentComponent<Character>();
        }

        public bool CheckTargetInArea()
        {
            if (_ownerCharacter == null)
            {
                return false;
            }

            if (_ownerCharacter.TargetCharacter == null)
            {
                return false;
            }

            if (_detectAreaType == DetectAreaTypes.Circle)
            {
                return _ownerCharacter.MyVital.CheckColliderInCircle(position, _radius);
            }
            else if (_detectAreaType == DetectAreaTypes.Box)
            {
                return _ownerCharacter.MyVital.CheckColliderInBox(position, _boxSize);
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            if (_detectAreaType == DetectAreaTypes.Circle)
            {
                GizmoEx.DrawWireSphere(position, _radius);
            }
            else if (_detectAreaType == DetectAreaTypes.Box)
            {
                GizmoEx.DrawGizmoRectangle(position, _boxSize);
            }
        }
    }
}
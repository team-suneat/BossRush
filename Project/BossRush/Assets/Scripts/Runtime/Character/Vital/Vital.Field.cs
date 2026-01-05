using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        [FoldoutGroup("#Vital")]
        [SuffixLabel("Vital의 충돌체 크기를 캐릭터의 충돌체 크기에 비례합니다")]
        public bool BasedOnOwnerCollider = true;

        [FoldoutGroup("#Vital")]
        public bool UseIndividualCollider;

        [FoldoutGroup("#Vital")]
        public VitalColliderHandler VitalColliderHandler = new();

        //--------------------------------------------------------------------------------------------------------=

        [FoldoutGroup("#Vital-Components")]
        public Character Owner;

        [FoldoutGroup("#Vital-Components")]
        public BoxCollider2D Collider;

        [FoldoutGroup("#Vital-Components")]
        public BoxCollider2D[] Colliders;

        //--------------------------------------------------------------------------------------------------------=

        [FoldoutGroup("#Vital-Battle Resource")]
        public VitalResourceTypes ResourceType;

        [FoldoutGroup("#Vital-Battle Resource")]
        public Life Life;

        [FoldoutGroup("#Vital-Battle Resource")]
        public Barrier Barrier;

        [FoldoutGroup("#Vital-Battle Resource")]
        public Mana Mana;

        //--------------------------------------------------------------------------------------------------------=

        [FoldoutGroup("#Vital-Guard")]
        public List<int> GuardColliderIndexes = new();

        [FoldoutGroup("#Vital-Guard")]
        [SuffixLabel("설정된 인덱스의 충돌체는 보호막만 피해입음")]
        public List<int> OnlyUseBarrierColliderIndexes = new();

        //--------------------------------------------------------------------------------------------------------=

        [FoldoutGroup("#Vital-Event")]
        public UnityEvent DieEvent; // 파괴 가능한 오브젝트에서 사용

        [FoldoutGroup("#Vital-Feedback")]
        public GameFeedbacks GuardFeedbacks;
    }
}
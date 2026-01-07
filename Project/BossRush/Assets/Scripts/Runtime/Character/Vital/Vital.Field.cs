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
        public VitalColliderHandler VitalColliderHandler = new();

        //--------------------------------------------------------------------------------------------------------=

        public Character Owner { get; set; }
        public BoxCollider2D Collider { get; set; }

        //--------------------------------------------------------------------------------------------------------=

        [FoldoutGroup("#Vital-Battle Resource")]
        public VitalResourceTypes ResourceType;

        [FoldoutGroup("#Vital-Battle Resource")]
        public Life Life;

        [FoldoutGroup("#Vital-Battle Resource")]
        public Barrier Barrier;

        [FoldoutGroup("#Vital-Battle Resource")]
        public Mana Mana;

        [FoldoutGroup("#Vital-Battle Resource")]
        public Pulse Pulse;

        //--------------------------------------------------------------------------------------------------------=

        [FoldoutGroup("#Vital-Event")]
        public UnityEvent DieEvent; // 파괴 가능한 오브젝트에서 사용

        [FoldoutGroup("#Vital-Feedback")]
        public GameFeedbacks GuardFeedbacks;
    }
}

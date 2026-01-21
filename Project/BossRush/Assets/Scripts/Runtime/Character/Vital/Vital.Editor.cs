using Sirenix.OdinInspector;
using System.Diagnostics;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Vital : Entity
    {
        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = this.FindFirstParentComponent<Character>();

            Life = GetComponent<Life>();
            Barrier = GetComponent<Barrier>();
            Mana = GetComponent<Mana>();
        }

        public override void AutoNaming()
        {
            if (Owner != null)
            {
                SetGameObjectName($"Vital({Owner.Name})");
            }
        }

        [FoldoutGroup("#Buttons", 999)]
        [Button("Resize Coliider", ButtonSizes.Medium)]
        [Conditional("UNITY_EDITOR")]
        private void ResizeColliderForEditor()
        {
            BoxCollider2D characterCollider = this.FindFirstParentComponent<BoxCollider2D>();
            BoxCollider2D vitalCollider = GetComponent<BoxCollider2D>();
            if (characterCollider != null && vitalCollider != null)
            {
                localPosition = characterCollider.offset;
                vitalCollider.size = characterCollider.size + (Vector2.one * 0.1f);
            }
        }
    }
}
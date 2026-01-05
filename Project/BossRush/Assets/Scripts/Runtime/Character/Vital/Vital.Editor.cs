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

        private Transform GetParentTransform()
        {
            return Owner != null ? Owner.transform : null;
        }

        private Transform GetFeedbackParentTransform(Transform parentTransform)
        {
            Transform feedbackParent = parentTransform.FindTransform("#Feedbacks");
            if (feedbackParent == null)
            {
                feedbackParent = parentTransform.FindTransform("Model/#Feedbacks");
            }

            return feedbackParent;
        }

        public override void AutoAddComponents()
        {
            base.AutoAddComponents();
        }

        public override void AutoNaming()
        {
            if (Owner != null)
            {
                SetGameObjectName($"Vital({Owner.Name})");
            }
        }
    }
}
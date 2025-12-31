using Sirenix.OdinInspector;
using System.Diagnostics;
using UnityEngine;

namespace TeamSuneat
{
    public class XBehaviourGroup : MonoBehaviour
    {
        [FoldoutGroup("#Buttons")]
        [Button("Auto Get Components", ButtonSizes.Medium)]
        [Conditional("UNITY_EDITOR")]
        public virtual void AutoGetComponents()
        {
            XBehaviour[] xBehaviours = GetComponentsInChildren<XBehaviour>();

            for (int i = 0; i < xBehaviours.Length; i++)
            {
                xBehaviours[i].AutoGetComponents();
            }
        }

        [FoldoutGroup("#Buttons")]
        [Button("Auto Add Components", ButtonSizes.Medium)]
        [Conditional("UNITY_EDITOR")]
        public virtual void AutoAddComponents()
        {
            XBehaviour[] xBehaviours = GetComponentsInChildren<XBehaviour>();

            for (int i = 0; i < xBehaviours.Length; i++)
            {
                xBehaviours[i].AutoAddComponents();
            }
        }

        [FoldoutGroup("#Buttons")]
        [Button("Auto Setting", ButtonSizes.Medium)]
        [Conditional("UNITY_EDITOR")]
        public virtual void AutoSetting()
        {
            XBehaviour[] xBehaviours = GetComponentsInChildren<XBehaviour>();

            for (int i = 0; i < xBehaviours.Length; i++)
            {
                xBehaviours[i].AutoSetting();
            }
        }

        [FoldoutGroup("#Buttons")]
        [Button("Auto Naming", ButtonSizes.Medium)]
        [Conditional("UNITY_EDITOR")]
        public virtual void AutoNaming()
        {
            XBehaviour[] xBehaviours = GetComponentsInChildren<XBehaviour>();

            for (int i = 0; i < xBehaviours.Length; i++)
            {
                xBehaviours[i].AutoNaming();
            }
        }
    }
}
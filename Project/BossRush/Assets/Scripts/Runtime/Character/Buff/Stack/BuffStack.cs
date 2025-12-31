using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// 버프 스택에 따라 아이콘 스프라이트 랜더러를 설정합니다.
    /// </summary>
    public class BuffStack : XBehaviour
    {
        public BuffNames Name;
        public string NameString;

        [FoldoutGroup("#Renderer")]
        public SpriteRenderer[] Renderers;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Renderers = this.FindComponentsInChildren<SpriteRenderer>("#Stack");
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (Name != 0)
            {
                NameString = Name.ToString();
            }
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }

        public override void AutoNaming()
        {
            SetGameObjectName("BuffStack(" + Name.ToString() + ")");
        }

        protected void Awake()
        {
            DeactivateStacks();
        }

        public void ActivateStacks(int stackCount)
        {
            for (int i = 0; i < Renderers.Length; i++)
            {
                Renderers[i].gameObject.SetActive(i < stackCount);
            }
        }

        public void DeactivateStacks()
        {
            for (int i = 0; i < Renderers.Length; i++)
            {
                Renderers[i].gameObject.SetActive(false);
            }
        }
    }
}
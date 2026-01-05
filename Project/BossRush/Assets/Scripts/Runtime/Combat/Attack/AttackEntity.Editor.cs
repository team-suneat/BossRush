using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR

using UnityEditor;

#endif

namespace TeamSuneat
{
    public partial class AttackEntity : XBehaviour
    {
        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            AutoGetOwnerComponents();
            AutoGetFeedbackComponents();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (Name != 0)
            {
                NameString = Name.ToString();
            }
        }

        public override void AutoNaming()
        {
            if (Name != HitmarkNames.None)
            {
                SetGameObjectName(Name.ToString());
            }
        }

#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            Validate();
        }

#endif

        protected virtual void Validate()
        {
            if (!EnumEx.ConvertTo(ref Name, NameString))
            {
                Log.Error($"공격 독립체의 히트마크 이름({NameString})을 변환할 수 없습니다. {this.GetHierarchyPath()}");
            }
        }

        public void AutoGetOwnerComponents()
        {
            Owner = this.FindFirstParentComponent<Character>();

            if (Owner != null)
            {
                Vital = Owner.MyVital;
            }
        }

#if UNITY_EDITOR

        protected List<T> LoadPrefabsWithComponent<T>(string path, string findKey) where T : Component
        {
            // 경로에서 모든 프리팹 파일 경로를 가져옴
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { path });
            List<T> components = new List<T>();

            foreach (string guid in guids)
            {
                // 프리팹의 실제 경로를 가져옴
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(findKey))
                {
                    if (!assetPath.Contains(findKey))
                    {
                        continue;
                    }
                }

                // 프리팹을 로드
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (prefab != null)
                {
                    // 프리팹에서 컴포넌트를 가져옴
                    T component = prefab.GetComponent<T>();

                    // 컴포넌트가 존재하면 리스트에 추가
                    if (component != null)
                    {
                        components.Add(component);
                    }
                }
            }

            return components;
        }

#endif
    }
}
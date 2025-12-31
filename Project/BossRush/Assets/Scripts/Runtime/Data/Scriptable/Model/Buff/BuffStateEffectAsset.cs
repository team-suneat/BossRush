using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Buff", menuName = "TeamSuneat/Scriptable/Buff StateEffect")]
    public class BuffStateEffectAsset : XScriptableObject
    {
        public int TID => BitConvert.Enum32ToInt(Data.Name);

        public StateEffects Name => Data.Name;

        public BuffStateEffectAssetData Data;

        [FoldoutGroup("#String")] public string BuffString;
        [FoldoutGroup("#String")] public string MaxStackBuffString;
        [FoldoutGroup("#String")] public string MaxStackHitmarkString;

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!Data.IsChangingAsset)
            {
                if (!EnumEx.ConvertTo(ref Data.Name, NameString))
                {
                    Log.Error("Buff StateEffect Asset 내 Name 변수 변환에 실패했습니다. {0}", name);
                }

                if (!EnumEx.ConvertTo(ref Data.BuffName, BuffString))
                {
                    Log.Error("Buff StateEffect Asset 내 Buff 변수 변환에 실패했습니다. {0}, {1}", name, BuffString);
                }

                if (!EnumEx.ConvertTo(ref Data.BuffOnMaxStack, MaxStackBuffString))
                {
                    Log.Error("Buff StateEffect Asset 내 BuffOnMaxStack 변수 변환에 실패했습니다. {0}, {1}", name, MaxStackBuffString);
                }

                if (!EnumEx.ConvertTo(ref Data.HitmarkOnMaxStack, MaxStackHitmarkString))
                {
                    Log.Error("Buff StateEffect Asset 내 HitmarkOnMaxStack 변수 변환에 실패했습니다. {0}, {1}", name, MaxStackHitmarkString);
                }
            }
        }

        public override void Refresh()
        {
            if (Data.IsChangingAsset)
            {
                if (Data.Name != StateEffects.None)
                {
                    NameString = Data.Name.ToString();
                }

                if (Data.BuffName != BuffNames.None)
                {
                    BuffString = Data.BuffName.ToString();
                }

                if (Data.BuffOnMaxStack != BuffNames.None)
                {
                    MaxStackBuffString = Data.BuffOnMaxStack.ToString();
                }

                if (Data.HitmarkOnMaxStack != HitmarkNames.None)
                {
                    MaxStackHitmarkString = Data.HitmarkOnMaxStack.ToString();
                }

                Data.IsChangingAsset = false;
            }

            base.Refresh();
        }

        public override void Rename()
        {
            Rename("BuffStateEffect");
        }

#endif

        public BuffStateEffectAssetData Clone()
        {
            return Data.Clone();
        }
    }
}
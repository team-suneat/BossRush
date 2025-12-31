using Sirenix.OdinInspector;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class BuffStateEffectAssetData : IData<int>
    {
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        public StateEffects Name;

        [EnableIf("IsChangingAsset")]
        public BuffNames BuffName;

        public int MaxStack;

        [EnableIf("IsChangingAsset")]
        [HideIf("MaxStack", 0)]
        public BuffNames BuffOnMaxStack;

        [EnableIf("IsChangingAsset")]
        [HideIf("MaxStack", 0)]
        public HitmarkNames HitmarkOnMaxStack;

        public int GetKey()
        {
            return BitConvert.Enum32ToInt(Name);
        }

        public void Refresh()
        { }

        public void OnLoadData()
        {
            EnumLog();
        }

        private void EnumLog()
        {
#if UNITY_EDITOR
            string type = "BuffStateEffectAssetData".ToSelectString();
            EnumExplorer.LogBuff(type, Name.ToString(), BuffName);
            EnumExplorer.LogBuff(type, Name.ToString(), BuffOnMaxStack);
#endif
        }

        public BuffStateEffectAssetData Clone()
        {
            BuffStateEffectAssetData assetData = new()
            {
                Name = Name,
                BuffName = BuffName,

                MaxStack = MaxStack,
                BuffOnMaxStack = BuffOnMaxStack,
                HitmarkOnMaxStack = HitmarkOnMaxStack,
            };

            return assetData;
        }
    }
}
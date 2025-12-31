using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "ForceVelocity", menuName = "TeamSuneat/Scriptable/ForceVelocity")]
    public class ForceVelocityAsset : XScriptableObject
    {
        public int TID => BitConvert.Enum32ToInt(Data.Name);

        public FVNames Name => Data.Name;

        public ForceVelocityAssetData Data;

        public override void OnLoadData()
        {
            base.OnLoadData();
            LogError();
            Data.OnLoadData();
        }

        private void LogError()
        {
#if UNITY_EDITOR

            if (Data.IsChangingAsset)
            {
                Log.Error("Asset의 IsChangingAsset 변수가 활성화되어있습니다. {0}", name);
            }

#endif
        }

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!Data.IsChangingAsset)
            {
                EnumEx.ConvertTo(ref Data.Name, NameString);
            }

            Data.Validate();
        }

        public override void Refresh()
        {
            NameString = Data.Name.ToString();
            Data.Refresh();

            base.Refresh();
        }

        public override void Rename()
        {
            Rename("ForceVelocity");
        }

#endif

        public ForceVelocityAssetData Clone()
        {
            return Data.Clone();
        }
    }
}
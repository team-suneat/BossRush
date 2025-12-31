using System.Collections.Generic;
using System.Linq;

namespace TeamSuneat.UserInterface
{
    public interface ICharacterGaugeView
    {
        void Clear();
        void LogicUpdate();
    }

    public class UIGaugeManager : XBehaviour
    {
        public Dictionary<Vital, ICharacterGaugeView> CharacterGauges = new Dictionary<Vital, ICharacterGaugeView>();

        private void Update()
        {
            foreach (var view in CharacterGauges.Values)
            {
                view.LogicUpdate();
            }
        }

        public ICharacterGaugeView FindCharacter(Vital vital)
        {
            if (CharacterGauges.ContainsKey(vital))
            {
                return CharacterGauges[vital];
            }

            return null;
        }

        public bool RegisterCharacter(Vital vital, ICharacterGaugeView view)
        {
            if (CharacterGauges.ContainsKey(vital))
            {
                Log.Warning(LogTags.UI_Gauge, "[Manager] 캐릭터 게이지를 추가할 수 없습니다. 이미 등록된 게이지입니다. Vital: {0}", vital.GetHierarchyName());
                return false;
            }

            CharacterGauges.Add(vital, view);
            return true;
        }

        public bool UnregisterCharacter(Vital vital)
        {
            if (CharacterGauges.ContainsKey(vital))
            {
                CharacterGauges.Remove(vital);
                return true;
            }

            Log.Warning(LogTags.UI_Gauge, "[Manager] 캐릭터 게이지를 삭제할 수 없습니다. 등록된 게이지가 없습니다. Vital: {0}", vital.GetHierarchyName());
            return false;
        }

        public void Clear()
        {
            ICharacterGaugeView[] characterGauges = CharacterGauges.Values.ToArray();
            if (characterGauges != null && characterGauges.Length > 0)
            {
                for (int i = 0; i < characterGauges.Length; i++)
                {
                    if (characterGauges[i] == null) { continue; }

                    characterGauges[i].Clear();
                }
            }

            CharacterGauges.Clear();

            Log.Warning(LogTags.UI_Gauge, "[Manager] 모든 게이지를 삭제합니다. 등록된 게이지를 초기화합니다.");
        }

        internal UIPlayerGauge SpawnPlayerGauge(Character owner)
        {
            return ResourcesManager.SpawnPlayerGauge(owner.MyVital);
        }
        internal UIEnemyGauge SpawnEnemyGauge(Character owner)
        {
            return ResourcesManager.SpawnEnemyGauge(owner.MyVital);
        }
    }
}
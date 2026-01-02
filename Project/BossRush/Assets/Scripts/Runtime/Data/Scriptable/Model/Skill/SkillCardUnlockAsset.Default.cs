using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;

namespace TeamSuneat.Data
{
    public partial class SkillCardUnlockAsset
    {
        [FoldoutGroup("#Button")]
        [Button("�⺻�� ����", ButtonSizes.Medium)]
        public void SetDefaultValues()
        {
            if (UnlockDataList == null)
            {
                UnlockDataList = new List<SkillCardUnlockAssetData>();
            }

            UnlockDataList.Clear();

            // ������ ��õ� �ر� ������ ���� ����
            // ���� 10
            AddUnlockData(SkillNames.FlameSlash, 10);
            AddUnlockData(SkillNames.EarthBlessing, 10);

            // ���� 15
            AddUnlockData(SkillNames.FlowingBlade, 15);
            AddUnlockData(SkillNames.AccelerationSword, 15);

            // ���� 20
            AddUnlockData(SkillNames.FireSword, 20);
            AddUnlockData(SkillNames.StoneStrike, 20);

            // ���� 25
            AddUnlockData(SkillNames.WindingBlade, 25);
            AddUnlockData(SkillNames.LightningSlash, 25);

            // ���� 30
            // Ÿ������ �� (�� �Ӽ�) - �� �Ӽ��� 10�� �ʿ�
            AddUnlockData(SkillNames.BurningSword, 30, requiredCurrencyName: CurrencyNames.AttributeStoneFire, requiredCurrencyCount: 10);
            AddUnlockData(SkillNames.EarthWill, 30);

            // ���� 35
            AddUnlockData(SkillNames.IceStone, 35);
            AddUnlockData(SkillNames.WindSword, 35);

            // ���� 40
            AddUnlockData(SkillNames.HeatWave, 40);
            AddUnlockData(SkillNames.LightningFast, 40);

            // ���� 45
            AddUnlockData(SkillNames.WaveSlash, 45);

            // ���� 50
            AddUnlockData(SkillNames.ThunderStrike, 50);
            // ��ö�� ���� (�� �Ӽ�) - �� �Ӽ��� 20�� �ʿ�
            AddUnlockData(SkillNames.SteelWill, 50, requiredCurrencyName: CurrencyNames.AttributeStoneEarth, requiredCurrencyCount: 20);

            // ���� 60
            // ���̽� ���� (�� �Ӽ�) - �� �Ӽ��� 20�� �ʿ�
            AddUnlockData(SkillNames.IceShower, 60, requiredCurrencyName: CurrencyNames.AttributeStoneWater, requiredCurrencyCount: 20);
            // �Ŀ� ��Ʈ����ũ (�� �Ӽ�) - �� �Ӽ��� 20�� �ʿ�
            AddUnlockData(SkillNames.PowerStrike, 60, requiredCurrencyName: CurrencyNames.AttributeStoneEarth, requiredCurrencyCount: 20);

            // ���� 70
            // ȭ�� ���� (�� �Ӽ�) - �� �Ӽ��� 20�� �ʿ�
            AddUnlockData(SkillNames.FireSlash, 70, requiredCurrencyName: CurrencyNames.AttributeStoneFire, requiredCurrencyCount: 20);
            // õ�� ���� (�ٶ� �Ӽ�) - �ٶ� �Ӽ��� 20�� �ʿ�
            AddUnlockData(SkillNames.ThunderSlash, 70, requiredCurrencyName: CurrencyNames.AttributeStoneWind, requiredCurrencyCount: 20);

            // ���� 80
            AddUnlockData(SkillNames.DancingWave, 80);

            // ���� 90
            AddUnlockData(SkillNames.FireWave, 90);
            AddUnlockData(SkillNames.HighSpeedMovement, 90);

            // ���� 100
            // �޵����̼� (�� �Ӽ�) - �� �Ӽ��� 50�� �ʿ�
            AddUnlockData(SkillNames.Meditation, 100, requiredCurrencyName: CurrencyNames.AttributeStoneWater, requiredCurrencyCount: 50);
            AddUnlockData(SkillNames.PowerImpact, 100);

            // ���� 120
            // ���� ȭ�� ���� (�� �Ӽ�) - �� �Ӽ��� 300�� �ʿ�
            AddUnlockData(SkillNames.HellfireSlash, 120, requiredCurrencyName: CurrencyNames.AttributeStoneFire, requiredCurrencyCount: 300);

            // ���� 140
            // ���� ���� ���� (�ٶ� �Ӽ�) - �ٶ� �Ӽ��� 300�� �ʿ�
            AddUnlockData(SkillNames.AsuraLightningSlash, 140, requiredCurrencyName: CurrencyNames.AttributeStoneWind, requiredCurrencyCount: 300);
            // ������ ���� (�� �Ӽ�) - �� �Ӽ��� 50�� �ʿ�
            AddUnlockData(SkillNames.HealthMana, 140, requiredCurrencyName: CurrencyNames.AttributeStoneEarth, requiredCurrencyCount: 50);

            // ���� 160
            // ���̽� Ÿ�� (�� �Ӽ�) - �� �Ӽ��� 300�� �ʿ�
            AddUnlockData(SkillNames.IceTime, 160, requiredCurrencyName: CurrencyNames.AttributeStoneWater, requiredCurrencyCount: 300);

            // ���� 180
            // �Ⱑ ��Ʈ����ũ (�� �Ӽ�) - �� �Ӽ��� 300�� �ʿ�
            AddUnlockData(SkillNames.GigaStrike, 180, requiredCurrencyName: CurrencyNames.AttributeStoneEarth, requiredCurrencyCount: 300);

            // ���� 200
            // �г� (�� �Ӽ�) - �� �Ӽ��� 100�� �ʿ�
            AddUnlockData(SkillNames.Rage, 200, requiredCurrencyName: CurrencyNames.AttributeStoneFire, requiredCurrencyCount: 100);
            AddUnlockData(SkillNames.RedThunder, 200);

            // ���� 250
            AddUnlockData(SkillNames.ManaBlessing, 250);

            // ���� 300
            AddUnlockData(SkillNames.TrueHeatWave, 300);
            AddUnlockData(SkillNames.ThunderGod, 300);

            // ���� 350
            AddUnlockData(SkillNames.Blizzard, 350);

            // ���� 400
            AddUnlockData(SkillNames.FirePillar, 400);

            // ���� 450
            AddUnlockData(SkillNames.Swiftness, 450);

            // ���� 500
            AddUnlockData(SkillNames.BeastHunt, 500);

            // ��Ÿ ���� (���� 0���� ���� - Ư�� ó�� �ʿ�)
            // �Ⱑ ����Ʈ - ���� 1��� �Ǽ��縮 4�� �ʿ� (���/���� �����̹Ƿ� ItemNames.None���� ����)
            AddUnlockData(SkillNames.GigaImpact, 0);
            // ������� - ��ȭ 1��� �Ǽ��縮 4�� �ʿ� (���/���� �����̹Ƿ� ItemNames.None���� ����)
            AddUnlockData(SkillNames.WarriorBurn, 0);
            // �ݷ� - ��ȭ 4��� �Ǽ��縮 4�� �ʿ� (���/���� �����̹Ƿ� ItemNames.None���� ����)
            AddUnlockData(SkillNames.Torrent, 0);
            // ���³��� - ��ȭ 3��� �Ǽ��縮 4�� �ʿ� (���/���� �����̹Ƿ� ItemNames.None���� ����)
            AddUnlockData(SkillNames.SuperhumanStrength, 0);

            // �Ҹ� ��� (���� 0���� ���� - Ư�� ó�� �ʿ�)
            // ���̺� - �Ҹ� ���� 1�� �Ǵ� ��ȭ 1��� ���� 1�� �ʿ� (���/���� �����̹Ƿ� ItemNames.None���� ����)
            AddUnlockData(SkillNames.Rave, 0);
            // ��Ʈ�� - �Ҹ� �Ǽ��縮 1�� �Ǵ� ��ȭ 1��� �Ǽ��縮 1�� �ʿ� (���/���� �����̹Ƿ� ItemNames.None���� ����)
            AddUnlockData(SkillNames.Mantra, 0);

            EditorUtility.SetDirty(this);
            Log.Info(LogTags.ScriptableData, "[SkillCardUnlock] �⺻���� �����Ǿ����ϴ�. �� {0}���� ��ų: {1}", UnlockDataList.Count, name);
        }

        private void AddUnlockData(SkillNames skillName, int unlockLevel, ItemNames requiredItemName = ItemNames.None, int requiredItemCount = 0, CurrencyNames requiredCurrencyName = CurrencyNames.None, int requiredCurrencyCount = 0)
        {
            UnlockDataList.Add(new SkillCardUnlockAssetData
            {
                SkillName = skillName,
                UnlockLevel = unlockLevel,
                RequiredItemName = requiredItemName,
                RequiredItemCount = requiredItemCount,
                RequiredCurrencyName = requiredCurrencyName,
                RequiredCurrencyCount = requiredCurrencyCount
            });
        }
    }
}
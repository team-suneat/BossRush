using System.Collections.Generic;
using System.IO;
using TeamSuneat;
using TeamSuneat.Data;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TeamSuneat
{
    [CreateAssetMenu(fileName = "Sound", menuName = "TeamSuneat/Scriptable/Sound", order = 1)]
    public class SoundAsset : XScriptableObject
    {
        public enum MinimumProgressType
        {
            None, Rate, Seconds,
        }

        public int TID => Name.ToInt();

        public SoundNames Name;

        [LabelText("반복 재생")]
        public bool IsLoop;

        [LabelText("개별 볼륨 스케일")]
        [Range(0, 1)]
        public float VolumeScale = 1.0f;

        [LabelText("최대 생성 수")]
        public int MaxPlayCount = 1;

        [LabelText("재생까지 최소 시간")]
        public float MinTimeToPlay = 0.02f;

        [LabelText("덮어씌움 위한 오디오 재생 최소 진행률")]
        public MinimumProgressType MinProgressType = MinimumProgressType.Rate;

        [LabelText("최소 진행률 (%)")]
        [EnableIf("MinProgressType", MinimumProgressType.Rate)]
        [Range(0f, 1f)]
        public float MinProgressRateToPlayNext = 0.3f;

        [LabelText("최소 재생 시간 (Sec)")]
        [EnableIf("MinProgressType", MinimumProgressType.Seconds)]
        public float MinProgressSecondsToPlayNext;

        [LabelText("카메라와의 거리 검사")]
        public bool UseCheckDistance;

        public List<AudioClip> SoundClips = new();

        public override void OnLoadData()
        {
            base.OnLoadData();

            if (SoundClips == null || SoundClips.Count == 0)
            {
                Log.Error("사운드 클립이 설정되지 않았습니다. {0}", name.ToColorString(GameColors.CreamIvory));
            }
        }

#if UNITY_EDITOR

        public override void Refresh()
        {
            if (Name != 0)
            {
                NameString = Name.ToString();
            }
            else if (SoundClips.Count > 0)
            {
                Name = ConvertName(EnumEx.GetNames<SoundNames>(), SoundClips[0].name);
            }
            else
            {
                Name = ConvertName(EnumEx.GetNames<SoundNames>(), name);
            }

            base.Refresh();
        }

        public override void Validate()
        {
            base.Validate();

            if (!EnumEx.ConvertTo(ref Name, NameString))
            {
                Log.Error("사운드의 이름을 갱신할 수 없습니다. {0}", name);
            }
        }

        public override void Rename()
        {
            Rename("Sound");
        }

        protected override void CreateAll()
        {
            base.CreateAll();

            string soundFolderPath = "Assets/Sound";
            string[] clipPaths = Directory.GetFiles(soundFolderPath, "*.wav", SearchOption.AllDirectories);
            string[] clipPaths2 = Directory.GetFiles(soundFolderPath, "*.mp3", SearchOption.AllDirectories);
            string[] soundNames = EnumEx.GetNames<SoundNames>();

            CreateSoundAssets(soundNames, clipPaths);
            CreateSoundAssets(soundNames, clipPaths2);

            PathManager.UpdatePathMetaData();
        }

        protected void CreateSoundAssets(string[] soundNames, string[] clipPaths)
        {
            foreach (string clipPath in clipPaths)
            {
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(clipPath);
                if (clip == null)
                {
                    continue;
                }

                SoundNames soundName = ConvertName(soundNames, clip.name);
                if (soundName == SoundNames.None)
                {
                    Log.Warning("사운드 클립을 사운드 이름으로 변환할 수 없습니다. 해당 사운드의 에셋을 생성하지 않습니다. {0}", clip.name);
                    continue;
                }

                SoundAsset asset = ScriptableDataManager.Instance.FindSound(soundName);
                if (asset == null)
                {
                    asset = CreateAsset<SoundAsset>("Sound", soundName.ToString(), true);
                    if (asset != null)
                    {
                        asset.Name = soundName;
                        if (soundName != SoundNames.None)
                        {
                            asset.NameString = soundName.ToString();
                        }
                    }
                }

                if (asset != null)
                {
                    if (!asset.SoundClips.Contains(clip))
                    {
                        asset.SoundClips.Add(clip);
                    }
                }
            }
        }

#endif

        private SoundNames ConvertName(string[] soundNames, string clipName)
        {
            for (int i = 1; i < soundNames.Length; i++)
            {
                if (clipName.Contains(soundNames[i]))
                {
                    return soundNames[i].ToEnum<SoundNames>();
                }
            }

            return SoundNames.None;
        }

#if UNITY_EDITOR

        [FoldoutGroup("#Custom Button", true, 5)]
        [Button("대응하는 오디오 클립 불러오기", ButtonSizes.Large)]
        private void LoadClips()
        {
            SoundClips.RemoveNull();

            if (SoundClips.Count > 0)
            {
                Log.Warning("{0}({1})의 오디오 클립을 불러올 수 없습니다. 이미 설정된 오디오 클립이 있습니다.", Name, name);
            }
            else
            {
                string soundFolderPath = "Assets/Sound";
                List<string> clipPaths = new List<string>();

                string[] sound1 = Directory.GetFiles(soundFolderPath, "*.wav", SearchOption.AllDirectories);
                string[] sound2 = Directory.GetFiles(soundFolderPath, "*.mp3", SearchOption.AllDirectories);

                clipPaths.AddRange(sound1);
                clipPaths.AddRange(sound2);

                foreach (string clipPath in clipPaths)
                {
                    string fileName = Path.GetFileName(clipPath);
                    if (!fileName.Contains(NameString))
                    {
                        continue;
                    }

                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(clipPath);
                    if (clip != null && !SoundClips.Contains(clip))
                    {
                        SoundClips.Add(clip);
                    }
                }
            }
        }

#endif
    }
}
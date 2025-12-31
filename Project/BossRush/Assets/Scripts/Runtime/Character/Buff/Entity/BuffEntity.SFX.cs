using TeamSuneat.Audio;
using Lean.Pool;

namespace TeamSuneat
{
    public partial class BuffEntity : XBehaviour, IPoolable
    {
        private AudioObject _audioObject;

        public void SpawnBuffSFX()
        {
            if (AssetData.SFXName != SoundNames.None)
            {
                LogInfo("버프의 효과음을 생성합니다. 효과음: {0}", AssetData.SFXName);

                if (AssetData.IsLoopSFX)
                {
                    _audioObject = AudioManager.Instance.PlaySFXLoop(AssetData.SFXName, position);
                }
                else
                {
                    AudioManager.Instance.PlaySFXOneShotScaled(AssetData.SFXName, position);
                }
            }
        }

        public void DespawnBuffSFX()
        {
            if (_audioObject != null)
            {
                AudioManager.Instance.StopSFX(_audioObject);
                _audioObject = null;
            }
        }
    }
}
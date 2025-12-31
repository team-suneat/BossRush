using System;
using System.Collections;
using TeamSuneat;
using UnityEngine;

namespace TeamSuneat.Audio
{
    public partial class AudioManager : SingletonMonoBehaviour<AudioManager>
    {
        public static void SpawnDropSFX()//ItemCategories itemCategory)
        {
           //  switch (itemCategory)
           //  {
           //      case ItemCategories.Sword:
           //      case ItemCategories.Knife:
           //      case ItemCategories.Axe:
           //      case ItemCategories.Blunt:
           //          _ = Instance.PlaySFXOneShotUnscaled(SoundNames.UI_Sound_Weapon_Drop);
           //          break;
           // 
           //      case ItemCategories.Helmet:
           //      case ItemCategories.Armor:
           //      case ItemCategories.Belt:
           //      case ItemCategories.Gloves:
           //      case ItemCategories.Boots:
           //          _ = Instance.PlaySFXOneShotUnscaled(SoundNames.UI_Sound_Armor_Drop);
           //          break;
           // 
           //      case ItemCategories.Accessories:
           //          _ = Instance.PlaySFXOneShotUnscaled(SoundNames.UI_Sound_Accessory_Drop);
           //          break;
           // 
           //      default:
           //          _ = Instance.PlaySFXOneShotUnscaled(SoundNames.UI_Sound_Miscellaneous_Drop);
           //          break;
           //  }
        }

        public static IEnumerator CrossFade(AudioObject from, AudioObject to, float duration, Action<AudioObject> onComplete = null)
        {
            if (to == null || duration <= 0f)
            {
                yield break;
            }

            float fromStartVolume = from != null ? from.Volume : 0f;
            float toTargetVolume = to.Volume;

            // 페이드 인 준비
            to.SetVolume(0f);
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);

                if (from != null)
                {
                    from.SetVolume(Mathf.Lerp(fromStartVolume, 0f, t));
                }

                to.SetVolume(Mathf.Lerp(0f, toTargetVolume, t));

                yield return null;
            }

            // 마무리 정리
            if (from != null)
            {
                from.Stop();
                from.Despawn();
            }

            to.SetVolume(toTargetVolume);
            onComplete?.Invoke(to);
        }
    }
}
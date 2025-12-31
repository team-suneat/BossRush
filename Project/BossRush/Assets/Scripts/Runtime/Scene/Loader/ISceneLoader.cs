using System.Collections;

using UnityEngine;

namespace TeamSuneat
{
    public abstract class ISceneLoader : MonoBehaviour
    {
        protected Coroutine LoadCoroutine { get; set; }

        public Coroutine Load(string prevSceneName, string targetSceneName)
        {
            if (gameObject == null || LoadCoroutine != null)
            {
                return default;
            }

            try
            {
                LoadCoroutine = StartCoroutine(OnAsyncLoad(prevSceneName, targetSceneName));
                return LoadCoroutine;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"{targetSceneName} 씬을 불러오는 데 실패했습니다: {e.Message}");
                return default;
            }
        }

        protected abstract IEnumerator OnAsyncLoad(string prevSceneName, string targetSceneName);
    }
}
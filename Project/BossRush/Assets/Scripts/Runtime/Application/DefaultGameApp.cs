using TeamSuneat;
using UnityEngine;

namespace TeamSuneat
{
    public class DefaultGameApp : XGameApp
    {
        public static DefaultGameApp Create()
        {
            GameObject gameApp = new GameObject("@DefaultGameApp");

            return gameApp.AddComponent<DefaultGameApp>();
        }

        protected override void OnApplicationStart()
        {
        }

        protected override void OnApplicationStarted()
        {
        }
    }
}
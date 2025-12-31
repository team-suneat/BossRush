using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    public class UIDetailsManager : XBehaviour
    {
        public List<UIDetails> Details = new List<UIDetails>();


        public UIDetails SpawnDetails(UIDetailsNames detailsName)
        {
            return SpawnDetails(detailsName, null, null);
        }

        public UIDetails SpawnDetails(UIDetailsNames detailsName, Transform parent)
        {
            return SpawnDetails(detailsName, parent, null);
        }

        private UIDetails SpawnDetails(UIDetailsNames detailsName, Transform parent, UnityAction<bool> despawnCallback)
        {
            UIDetails details = ResourcesManager.SpawnDetails(detailsName, parent);
            if (details != null)
            {
                details.RegisterDespawnCallback(despawnCallback);
                return details;
            }

            return null;
        }

        //

        public UIDetails SpawnIngameDetails(UIDetailsNames detailsName, Transform transform)
        {
            return SpawnIngameDetails(detailsName, transform, null);
        }

        private UIDetails SpawnIngameDetails(UIDetailsNames detailsName, Transform toFollow, UnityAction<bool> despawnCallback)
        {
            UIDetails details = ResourcesManager.SpawnIngameDetails(detailsName, toFollow);
            if (details != null)
            {
                details.RegisterDespawnCallback(despawnCallback);

                return details;
            }

            return null;
        }

        public void Register(UIDetails details)
        {
            if (Details != null)
            {
                Details.Add(details);
            }
        }

        public bool Contains(UIDetails details)
        {
            if (Details.Contains(details))
            {
                return true;
            }

            return false;
        }

        public void Unregister(UIDetails details)
        {
            if (Details.Contains(details))
            {
                Details.Remove(details);
            }
        }

        public void Clear()
        {
            if (Details.IsValid())
            {
                Log.Info(LogTags.UI, "열려있는 상세정보를 모두 닫습니다. 상세정보 개수: {0}", Details.Count);

                List<UIDetails> opendDetails = new();

                for (int i = 0; i < Details.Count; i++)
                {
                    opendDetails.Add(Details[i]);
                }

                for (int i = 0; i < opendDetails.Count; i++)
                {
                    opendDetails[i].CloseWithFailure();
                }

                Details.Clear();
            }
        }
    }
}
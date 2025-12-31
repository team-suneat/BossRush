using UnityEngine;

namespace TeamSuneat
{
    public static class GameObjectEx
    {
        public static GameObject CreateGameObject(string name, Transform parent)
        {
            GameObject newGameObject = new(name);
            newGameObject.transform.SetParent(parent);

            return newGameObject;
        }

        public static T CreateGameObject<T>(string name, Transform parent) where T : Component
        {
            GameObject newGameObject = new(name);
            newGameObject.transform.SetParent(parent);

            return newGameObject.AddComponent<T>();
        }
    }
}
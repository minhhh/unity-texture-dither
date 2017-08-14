using UnityEngine;

namespace UBootstrap
{
    static public class GameObjectExtension
    {
        public static T GetOrAddComponent<T> (this GameObject go) where T : Component
        {
            return go.GetComponent<T> () ?? go.AddComponent<T> ();
        }

        public static bool HasComponent<T> (this GameObject go) where T : Component
        {
            return go.GetComponent<T> () != null;
        }

        public static bool HasComponentInChildren <T> (this GameObject go) where T : Component
        {
            return go.GetComponentInChildren <T> () != null;
        }
    }
}

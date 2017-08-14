using UnityEngine;
using System.Linq;

// TODO: remove linq
using System.Collections.Generic;

namespace UBootstrap
{
    public class GameObjectHelper
    {
        public static bool IsPrefab (Object o)
        {
            var go = o as GameObject;
            if (go != null) {
                return IsPrefab (go);
            }
            var comp = o as Component;
            if (comp != null) {
                return IsPrefab (comp);
            }

            return false;
        }

        public static bool IsPrefab (GameObject go)
        {
            return go.transform.hideFlags == HideFlags.HideInHierarchy;
        }

        public static bool IsPrefab (Component comp)
        {
            return comp.gameObject.transform.hideFlags == HideFlags.HideInHierarchy;
        }

        // Slow. Don't use in Update
        public static GameObject Find (string name)
        {
            Object [] objects = Resources.FindObjectsOfTypeAll (typeof(GameObject));
            Object o;
            for (int i = 0; i < objects.Length; i++) {
                o = objects [i];
                if (o != null && o.name.Equals (name) & !IsPrefab (o as GameObject)) {
                    return o as GameObject;
                }
            }
            return null;
        }

        // Slow. Don't use in Update
        public static T FindObjectOfType<T> () where T:Object
        {
            Object [] objects = Resources.FindObjectsOfTypeAll (typeof(T));
            Object o;
            for (int i = 0; i < objects.Length; i++) {
                o = objects [i];
                if (o != null && !IsPrefab (o as Component)) {
                    return o as T;
                }
            }
            return default (T);
        }

        // Slow. Don't use in Update
        public static Object FindObjectOfType (System.Type T)
        {
            Object [] objects = Resources.FindObjectsOfTypeAll (T);
            Object o;
            for (int i = 0; i < objects.Length; i++) {
                o = objects [i];
                if (o != null && !IsPrefab (o as Component)) {
                    return o;
                }
            }
            return null;
        }

        // Slow. Don't use in Update
        public static T[] FindObjectsOfType<T> () where T:Object
        {
            Object [] objects = Resources.FindObjectsOfTypeAll (typeof(T));
            Object o;
            var results = new List<T> ();
            for (int i = 0; i < objects.Length; i++) {
                o = objects [i];
                if (o != null && !IsPrefab (o as Component)) {
                    results.Add (o as T);
                }
            }

            return results.ToArray ();
        }

        // Slow. Don't use in Update
        public static Object[] FindObjectsOfType (System.Type T)
        {
            Object [] objects = Resources.FindObjectsOfTypeAll (T);

            Object o;
            var results = new List<Object> ();
            for (int i = 0; i < objects.Length; i++) {
                o = objects [i];
                if (o != null && !IsPrefab (o)) {
                    results.Add (o);
                }
            }

            return results.ToArray ();
        }

        // Slow. Don't use in Update
        public static GameObject FindGameObjectWithTag (string tag)
        {
            Object [] objects = Resources.FindObjectsOfTypeAll (typeof(GameObject));
            return objects.Select (go => go as GameObject).First (go => go.CompareTag (tag) && !IsPrefab (go));
        }

        // Slow. Don't use in Update
        public static GameObject[] FindGameObjectsWithTag (string tag)
        {
            Object [] objects = Resources.FindObjectsOfTypeAll (typeof(GameObject));
            return objects.Select (go => go as GameObject).Where (go => go.CompareTag (tag) && !IsPrefab (go)).ToArray ();
        }

        public static bool IsNull (object o)
        {
            return o == null || o.Equals (null);
        }

        public static bool IsNotNull (object o)
        {
            return !IsNull (o);
        }
    }
}

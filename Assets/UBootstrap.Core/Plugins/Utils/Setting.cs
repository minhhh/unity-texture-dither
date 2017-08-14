using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UBootstrap
{
    public class Setting<T> : ScriptableObject where T : Setting<T>
    {
        protected static readonly string defaultAssetName = typeof(T).Name;

        #if UNITY_EDITOR
        protected static readonly string defaultAssetFolder = "Resources";
        protected static readonly string assetFile = typeof(T).Name + ".asset";
        #endif

        protected static T instance;

        public static T Instance {
            get {
                #if UNITY_EDITOR
                string fullPath = null;
                if (!EditorApplication.isPlaying) {
                    FileSystemHelper.MakeFolderInAssets (AssetFolder);
                    fullPath = Path.Combine (Path.Combine ("Assets", AssetFolder), assetFile);
                    if (instance == null) {
                        var objs = Resources.FindObjectsOfTypeAll (typeof(T));
                        if (objs.Length > 0) {
                            instance = objs [0] as T;
                        }
                    }
                }

                #endif

                if (instance == null) {
                    instance = Resources.Load (AssetName) as T;

                    if (instance == null) {
                        // If not found, autocreate the asset object.
                        instance = CreateInstance<T> ();
                        instance.OnCreated ();

                        #if UNITY_EDITOR
                        if (!EditorApplication.isPlaying) {
                            AssetDatabase.CreateAsset (instance, fullPath);
                        }
                        #endif
                    }
                }
                #if UNITY_EDITOR
                if (!EditorApplication.isPlaying && AssetDatabase.GetAssetPath (instance) != fullPath) {
                    AssetDatabase.MoveAsset (AssetDatabase.GetAssetPath (instance), fullPath);
                    AssetDatabase.Refresh ();
                }
                #endif

                return instance;
            }
        }

        protected static string AssetName {
            get {
                var pathSetting = PathSetting.Instance;
                if (pathSetting == null) {
                    return defaultAssetName;
                }
                return Path.Combine (pathSetting.SettingFolderPostPart, defaultAssetName);
            }
        }

        #if UNITY_EDITOR
        protected static string AssetFolder {
            get {
                var pathSetting = PathSetting.Instance;
                if (pathSetting == null) {
                    return defaultAssetFolder;
                }
                return Path.Combine (Path.Combine (pathSetting.SettingFolderPrePart, defaultAssetFolder), pathSetting.SettingFolderPostPart);
            }
        }
        #endif

        protected virtual void OnCreated ()
        {
            // override in subclass
        }
    }
}
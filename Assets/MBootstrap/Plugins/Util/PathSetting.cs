using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MBootStrap
{
    public class PathSetting : Setting<PathSetting>
    {
        [MenuItem ("Settings/PathSetting")]
        public static void Edit ()
        {
            Selection.activeObject = Instance;
        }

        new public static PathSetting Instance {
            get {
                if (instance == null) {
                    instance = Resources.Load (defaultAssetName) as PathSetting;

                    #if UNITY_EDITOR
                    FileSystemHelper.MakeFolderInAssets (defaultAssetFolder);
                    string fullPath = Path.Combine (Path.Combine ("Assets", AssetFolder), assetFile);
                    #endif

                    if (instance == null) {
                        // If not found, autocreate the asset object.
                        instance = CreateInstance<PathSetting> ();

                        #if UNITY_EDITOR
                        AssetDatabase.CreateAsset (instance, fullPath);
                        #endif
                    }

                }
                return instance;
            }
        }

        // The part in Assets folder which will contains Resources folder
        public string SettingFolderPrePart = "Settings";

        // The part inside Resources folder
        public string SettingFolderPostPart = "Settings";
    }
}
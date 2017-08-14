using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace UBootstrap.Editor
{
    public class MissingScriptFinder : EditorWindow
    {
        private Vector2 scrollPos;
        Dictionary<ReferencerAssetParam, List<UnityEngine.Object>> referencesByTypes = new Dictionary<ReferencerAssetParam, List<UnityEngine.Object>> ();
        bool OnlyActiveScene = true;

        [MenuItem ("UBootstrap/Missing Script Finder")]
        static void Init ()
        {
            MissingScriptFinder window = EditorWindow.GetWindow <MissingScriptFinder> ("Missing Script Finder");
            window.Show ();
        }

        MissingScriptFinder ()
        {
        }

        void OnGUI ()
        {
            GUI.color = Color.white;

            OnlyActiveScene = GUILayout.Toggle (OnlyActiveScene, "Only Active Scene", GUILayout.Width (300));

            if (GUILayout.Button ("Find")) {
                List<UnityEngine.Object> referencers;
                if (OnlyActiveScene) {
                    referencers = FindMissingScriptObjectsInActiveScene ();
                } else {
                    List<string> objectsPath = ListUpAllObjectPath ();
                    referencers = FindAllMissingScriptObjects (objectsPath);
                }
                referencesByTypes = AggregateReferencerByAssetType (referencers);
            }

            scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
            EditorGUILayout.BeginVertical ();

            GUI.color = Color.red;
            EditorGUILayout.LabelField (string.Format ("Total Missing Script Objects: {0}", referencesByTypes.Sum ((x) => x.Value.Count)));

            var keys = referencesByTypes.Keys.ToList ();
            for (int i = 0; i < keys.Count; i++) {
                var e = referencesByTypes [keys [i]];
                var count = e.Count;
                GUI.color = count > 0 ? Color.green : Color.gray;
                EditorGUILayout.LabelField (string.Format ("{0}:{1}", keys [i].assetType, count));
                for (int j = 0; j < e.Count; j++) {
                    EditorGUILayout.ObjectField (e [j], typeof(Object), true);
                }
            }

            EditorGUILayout.EndVertical ();
            EditorGUILayout.EndScrollView ();
        }

        class ReferencerAssetParam
        {
            public string assetDirectory { get; private set; }

            public string assetType { get; private set; }

            public string assetExtension { get; private set; }

            public ReferencerAssetParam (string assetDirectory, string assetType, string assetExtension)
            {
                this.assetDirectory = assetDirectory;
                this.assetType = assetType;
                this.assetExtension = assetExtension;
            }
        }

        static readonly ReferencerAssetParam[] referencerAssetParams = new ReferencerAssetParam[] {
            new ReferencerAssetParam ("Assets", "Scene", ".unity"),
            new ReferencerAssetParam ("Assets", "Prefab", ".prefab"),
            //            new ReferencerAssetParam ("Assets", "ScriptableObject", ".asset") // does not work with scriptable object yet
            //            new ReferencerAssetParam ("Assets", "AnimatorController", ".controller"),
            //            new ReferencerAssetParam ("Assets", "AnimatorOverrideController", ".overrideController")
        };

        static readonly ReferencerAssetParam referencerAssetParamGO = new ReferencerAssetParam ("Assets", "GameObject in ActiveScene", "");

        private static List<string> ListUpAllObjectPath ()
        {
            List<string> path = new List<string> ();

            foreach (var param in referencerAssetParams) {
                path.AddRange (ListUpAllObjectPathByType (param.assetDirectory, param.assetType, param.assetExtension));
            }

            return path;
        }


        private static List<UnityEngine.Object> GetReferencerByAssetType (string assetType, string assetExtension, List<UnityEngine.Object> referencerObjects)
        {
            var result = new List <UnityEngine.Object> ();

            var assets = referencerObjects.FindAll ((x) => {
                return AssetDatabase.GetAssetPath (x).EndsWith (assetExtension);
            });

            foreach (var o in assets) {
                result.Add (o);
            }


            return result;
        }

        private static Dictionary<ReferencerAssetParam, List<UnityEngine.Object>> AggregateReferencerByAssetType (List<UnityEngine.Object> referencerObjects)
        {
            var result = new Dictionary<ReferencerAssetParam, List<UnityEngine.Object>> ();
            foreach (ReferencerAssetParam param in referencerAssetParams) {
                result [param] = GetReferencerByAssetType (param.assetType, param.assetExtension, referencerObjects);
                referencerObjects = referencerObjects.Except (result [param]).ToList ();
            }

            result [referencerAssetParamGO] = referencerObjects.Where (x => x != null).ToList ();

            return result;
        }

        private static List<UnityEngine.Object> FindMissingScriptObjectsInActiveScene ()
        {
            var activeScene = SceneManager.GetActiveScene ();
            return FindMissingScriptObjectInScene (activeScene);
        }

        private static List<UnityEngine.Object> FindAllMissingScriptObjects (List<string> objectPaths)
        {
            List<UnityEngine.Object> results = new List<UnityEngine.Object> ();

            // Store the active scene
            var activeScene = SceneManager.GetActiveScene ();
            string activeScenePath = string.Empty;
            if (activeScene != null) {
                activeScenePath = activeScene.path;
            }

            int index = 0;
            foreach (string path in objectPaths) {
                if (index % 10 == 0) {
                    string progressBarSubTile = string.Format ("[{0}/{1}] : {2}", index + 1, objectPaths.Count, path);
                    if (UnityEditor.EditorUtility.DisplayCancelableProgressBar (
                            "find referencer object", progressBarSubTile, (float)(index + 1) / objectPaths.Count)) {
                        break;
                    }
                }

                UnityEngine.Object referencer = AssetDatabase.LoadAssetAtPath (path, typeof(UnityEngine.Object));
                results.AddRange (FindMissingScriptObjectInObject (referencer));

                index++;
            }

            if (activeScenePath != string.Empty) {
                EditorSceneManager.OpenScene (activeScenePath);
            }

            Resources.UnloadUnusedAssets ();

            EditorUtility.ClearProgressBar ();

            return results;
        }

        private static List<UnityEngine.Object> FindMissingScriptObjectInObject (Object o)
        {
            if (o is GameObject) {
                return FindMissingScriptObjectInGO (o as GameObject);
            } else if (o is SceneAsset) {
                if (EditorApplication.SaveCurrentSceneIfUserWantsTo ()) {
                    EditorSceneManager.OpenScene (AssetDatabase.GetAssetPath (o));
                    var result = FindMissingScriptObjectInScene (SceneManager.GetActiveScene ());
                    if (result.Count > 0) {
                        result.Add (o);
                    }
                    return result;
                }
            }

            return new List<UnityEngine.Object> ();
        }

        private static List<UnityEngine.Object> FindMissingScriptObjectInGO (GameObject g)
        {
            var result = new List<UnityEngine.Object> ();
            Component[] components = g.GetComponents<Component> ();
            for (int i = 0; i < components.Length; i++) {
                if (components [i] == null) {
                    result.Add (g);
                    break;
                }
            }

            foreach (Transform child in g.transform) {
                result.AddRange (FindMissingScriptObjectInGO (child.gameObject));
            }

            return result;
        }

        private static List<UnityEngine.Object> FindMissingScriptObjectInScene (Scene s)
        {
            var result = new List<UnityEngine.Object> ();

            foreach (GameObject go in s.GetRootGameObjects()) {
                result.AddRange (FindMissingScriptObjectInGO (go));
            }

            return result;
        }

        private static List<string> ListUpAllObjectPathByType (string directory, string assetType, string assetFileExtension)
        {
            string assetSearchCond = "t:" + assetType + " ";
            string[] assetPaths = new string[1] {
                directory,
            };

            string[] guids = AssetDatabase.FindAssets (assetSearchCond, assetPaths);

            List<string> scenePath = new List<string> ();

            foreach (string guid in guids) {

                string assetPath = AssetDatabase.GUIDToAssetPath (guid);

                if (!assetPath.EndsWith (assetFileExtension)) {
                    continue;
                }
                scenePath.Add (assetPath);
            }

            return scenePath;
        }

    }
}

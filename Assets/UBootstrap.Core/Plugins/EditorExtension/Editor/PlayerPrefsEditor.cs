using UnityEngine;
using UnityEditor;


namespace UBootstrap.Editor
{
    public class PlayerPrefsEditor
    {
        [MenuItem ("UBootstrap/PlayerPrefs/DeleteAll")]
        static void DeleteAll ()
        {
            PlayerPrefs.DeleteAll ();
        }
    }
}

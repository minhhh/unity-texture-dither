/*
The MIT License (MIT)

Copyright (c) 2015 John

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

https://github.com/pharan/Unity-MeshSaver
*/
using UnityEditor;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UBootstrap.Editor
{
    public static class MeshSaverEditor
    {
        [MenuItem ("CONTEXT/MeshFilter/Save Mesh...")]
        public static void SaveMeshInPlace (MenuCommand menuCommand)
        {
            MeshFilter mf = menuCommand.context as MeshFilter;
            Mesh m = mf.sharedMesh;
            SaveMesh (m, m.name, false, true);
        }

        [MenuItem ("CONTEXT/MeshFilter/Save Mesh As New Instance...")]
        public static void SaveMeshNewInstanceItem (MenuCommand menuCommand)
        {
            MeshFilter mf = menuCommand.context as MeshFilter;
            Mesh m = mf.sharedMesh;
            SaveMesh (m, m.name, true, true);
        }

        public static void SaveMesh (Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
        {
            string path = EditorUtility.SaveFilePanel ("Save Separate Mesh Asset", "Assets/", name, "asset");
            if (string.IsNullOrEmpty (path))
                return;

            path = FileUtil.GetProjectRelativePath (path);

            Mesh meshToSave = (makeNewInstance) ? Object.Instantiate (mesh) as Mesh : mesh;

            if (optimizeMesh)
                ;

            AssetDatabase.CreateAsset (meshToSave, path);
            AssetDatabase.SaveAssets ();
        }
    }
}
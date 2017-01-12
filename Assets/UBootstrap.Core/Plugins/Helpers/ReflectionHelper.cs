using System;
using System.Reflection;
using UnityEngine;

namespace UBootstrap
{
    public static class ReflectionHelper
    {
        // from http://answers.unity3d.com/questions/206665/typegettypestring-does-not-work-in-unity.html
        public static Type GetType (string typeName)
        {
            var type = Type.GetType (typeName);

            if (type != null)
                return type;

            var currentAssembly = Assembly.GetExecutingAssembly ();
            if (currentAssembly == null)
                return null;

            // If we still haven't found the proper type, we can enumerate all of the 
            // loaded assemblies and see if any of them define the type

            var referencedAssemblies = currentAssembly.GetReferencedAssemblies ();
            foreach (var assemblyName in referencedAssemblies) {
                var assembly = Assembly.Load (assemblyName);
                if (assembly != null) {
                    type = assembly.GetType (typeName);
                    if (type != null) {
                        return type;
                    }
                        
                }
            }

            // The type just couldn't be found...
            return null;
        }

    }
}


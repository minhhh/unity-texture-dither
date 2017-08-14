using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace UnityGameBase.Core.Editor
{
    [InitializeOnLoad ()]
    class GameScriptExecutionOrder
    {
        static GameScriptExecutionOrder ()
        {
            foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts()) {
                if (monoScript.GetClass () != null) {
                    foreach (var attribute in Attribute.GetCustomAttributes(monoScript.GetClass(), typeof(ScriptExecutionOrderAttribute))) {
                        var currentScriptOrder = MonoImporter.GetExecutionOrder (monoScript);
                        var definedScriptOrder = ((ScriptExecutionOrderAttribute)attribute).value;
                        if (currentScriptOrder != definedScriptOrder)
                            MonoImporter.SetExecutionOrder (monoScript, definedScriptOrder);
                    }
                }
            }
        }
    }
}
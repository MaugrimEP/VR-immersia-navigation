using UnityEditor;
using UnityEngine;
using System.Collections;

/// <summary>
/// Add define symbol to Unity if the dependency are found for conditionnal code.
/// Dependency presence is based on folder name.
/// </summary>
[InitializeOnLoad]
class DependencyChecker
{
    const string ASSETS = "Assets/";
    const string MIDDLEVR = "MiddleVR";

    static DependencyChecker()
    {
        UpdateSymbols();
    }

    static void UpdateSymbols()
    {
        if (AssetDatabase.IsValidFolder(ASSETS + MIDDLEVR))
            addDefine(MIDDLEVR.ToUpper());
    }

    static void addDefine(string define)
    {
        string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        if (!defineSymbols.Contains(define))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defineSymbols + ";" + define);
            Debug.Log("[VRTools] Add define " + define + " symbol to group : " + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone));
        }
    }
}
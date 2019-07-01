using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CommandLineDefineSymbols
{

    private static string BATCH_MODE_PARAM = "-batchmode";
    private static string DEFINE_PARAM = "-define";

    private static List<string> args;

    static CommandLineDefineSymbols()
    {
        if (System.Environment.GetCommandLineArgs().Any(arg => arg.ToLower().Equals(BATCH_MODE_PARAM)))
        {
            Debug.LogFormat("CommandLineDefineSymbols will try to parse the command line to set define symbols (which can remove previously set ones).\n" +
                "\t Use {0} \"TARGET:SYMBOL1;-SYMBOL2;...SYMBOLx\"\n" +
                "\t Possible values for TARGET : {1}\n" +
                "\t -SYMBOL will remove the SYMBOL define from the list if present\n" +
                "\t If no symbol is specified, any symbol defined in the project will be unset for the specified target\n"
                , DEFINE_PARAM, string.Join(",", Enum.GetNames(typeof(BuildTargetGroup))));
        }
        args = System.Environment.GetCommandLineArgs().ToList();
        for (int i = 0; i < args.Count; i++)
        {
            if (args[i].ToLower().Equals(DEFINE_PARAM) && i + 1 < args.Count)
            {
                string[] parameters = args[i + 1].Split(':');
                if (parameters.Length >= 1 && parameters.Length < 3)
                {
                    BuildTargetGroup targetGroup = (BuildTargetGroup)Enum.Parse(typeof(BuildTargetGroup), parameters[0], true);
                    HandleSymbols(targetGroup, parameters.Length > 1 ? parameters[1] : string.Empty);
                }
                else
                    Debug.LogErrorFormat("Incorrect symbole define format : {0}", args[i + 1]);
            }

        }
    }

    private static void HandleSymbols(BuildTargetGroup targetGroup, string symbols)
    {
        string originalSymbolString = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        Debug.LogFormat("Original symbols {0} for target group {1}", originalSymbolString, targetGroup);
        List<string> newSymbols = originalSymbolString.Split(';').ToList();
        if (string.IsNullOrEmpty(symbols))
            newSymbols = new List<string>();
        foreach (string symbol in symbols.Split(';'))
        {
            if (symbol.StartsWith("-"))
            {
                string symbolToRemove = symbol.Substring(1);
                newSymbols.Remove(symbolToRemove);
            }
            else
                newSymbols.Add(symbol);

        }
        string newSymbolsString = String.Join(";", newSymbols.ToArray());
        if (originalSymbolString != newSymbolsString)
        {
            Debug.LogFormat("Setting symbols {0} to target group {1}", newSymbolsString, targetGroup);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newSymbolsString);
            // Ensure that the symbols stay between sessions
            AssetDatabase.SaveAssets();
        }
        else
        {
            Debug.LogFormat("Not changing symbols for target {0}", targetGroup);
        }

    }
}


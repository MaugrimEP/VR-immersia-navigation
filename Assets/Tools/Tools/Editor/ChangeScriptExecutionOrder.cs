using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Tools
{
    [InitializeOnLoad]
    public static class ChangeScriptExecutionOrder
    {
        private static string BATCH_MODE_PARAM = "-batchmode";
        private static string SET_EXECUTION_ORDER_VALUE_COMMAND = "-scriptorder";

        static ChangeScriptExecutionOrder()
        {

            List<String> args = Environment.GetCommandLineArgs().ToList();

            if (args.Any(arg => arg.ToLower().Equals(BATCH_MODE_PARAM)))
            {
                Debug.LogFormat("ChangeScriptExecutionOrder will try to parse the command line to change the script execution order\n" +
                    "\t Use {0} \"componentname\" \"value\" for every script or component for which you wish to change the execution order"
                    , SET_EXECUTION_ORDER_VALUE_COMMAND);
            }

            if (args.Any(arg => arg.ToLower().Equals(SET_EXECUTION_ORDER_VALUE_COMMAND))) // is an execution order change requested ?
            {
                int lastIndex = 0;
                while (lastIndex != -1)
                {
                    lastIndex = args.FindIndex(lastIndex, arg => arg.ToLower().Equals(SET_EXECUTION_ORDER_VALUE_COMMAND));
                    if (lastIndex >= 0 && lastIndex + 2 < args.Count)
                    {
                        string scriptToOrder = args[lastIndex + 1];
                        int order = int.Parse(args[lastIndex + 2]);
                        lastIndex++;
                        bool found = false;
                        foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
                        {
                            if (monoScript.name == scriptToOrder)
                            {
                                found = true;
                                // Setting the execution order my trigger a recompilation which will relaunch this script so check if the value is already correct before doing anything
                                if (MonoImporter.GetExecutionOrder(monoScript) != order)
                                {
                                    Debug.LogFormat("Setting script {0} order to {1}", scriptToOrder, order);
                                    MonoImporter.SetExecutionOrder(monoScript, order);
                                }
                                break;
                            }
                        }
                        if (!found)
                            Debug.LogWarningFormat("Could not find script named {0}", scriptToOrder);
                    }

                }
            }
        }
    }
}

using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using System.Collections.Generic;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

[InitializeOnLoad]
public static class CommandLineBuilder
{
    private static string BATCH_MODE_PARAM = "-batchmode";
    private static string SET_RUMTIME_COMMAND = "-executeMethod CommandLineBuilder.SetRuntime";
    private static string BUILD_COMMAND = "-executeMethod CommandLineBuilder.Build";
    private static string BUILD_PARAM = "-build";
    private static string SCENE_PARAM = "-scenes";
    private static string TARGET_PARAM = "-target";
    private static string RUNTIME_PARAM = "-runtime";
    private static string API_LEVEL_PARAM = "-api_level";
    private static string BACKEND_PARAM = "-backend";

    private static List<string> args;


    static CommandLineBuilder()
    {
        if (System.Environment.GetCommandLineArgs().Any(arg => arg.ToLower().Equals(BATCH_MODE_PARAM)))
        {
            Debug.LogFormat("CommandLineBuilder will try to parse the command line to automate the build. If you need to change the runtime, you need to do it before on a separate run of unity editor\n" +
                "\t To set the runtime, use {0}\n" +
                "\t\t Use the {5} parameter to specify the runtime (default: .NET 4.x)\n" +
                "\t Use {1} {2} \"executable pathname\"\n" +
                "\t\t Use the {3} \"scenefile1\";\"scenefile2\";... parameter to specify the scenes to include in the build\n" +
                "\t\t Use the {4} or -buildTarget [win32|win64] parameter to specify the targeted platform (default: win64)\n" +
                "\t\t Use the {6} parameter to specify the API level (default: .NET .2.0 for 3.5 runtime and .NET 4.x for 4.x runtime)\n" +
                "\t\t Use the {7} parameter to specify the scripting scripting backend (default: mono)\n"
                , SET_RUMTIME_COMMAND, BUILD_COMMAND, BUILD_PARAM, SCENE_PARAM, TARGET_PARAM, RUNTIME_PARAM, API_LEVEL_PARAM, BACKEND_PARAM);
        }
    }

    private static void SetRuntime()
    {
        args = System.Environment.GetCommandLineArgs().ToList();

        ScriptingRuntimeVersion runtimeVersion = GetRuntimeVersion();

        SetRuntime(runtimeVersion);
    }

    private static void Build()
    {

        args = System.Environment.GetCommandLineArgs().ToList();

        string buildPath = String.Empty;

        int build_index = args.IndexOf(BUILD_PARAM);
        if (build_index >= 0 && build_index < args.Count - 1)
        {
            buildPath = args[build_index + 1].Replace("\"", "");
        }
        else
            Debug.LogErrorFormat("CommandLineBuilder could not parse the {0} parameter in the command line", BUILD_PARAM);

        ScriptingRuntimeVersion runtimeVersion = PlayerSettings.scriptingRuntimeVersion;

        ApiCompatibilityLevel apiLevel = GetApiLevel(runtimeVersion);

        ScriptingImplementation scriptingBackend = GetScriptingBackend();

        if (!BuildWithOptions(buildPath, GetScenes(), GetTarget(), apiLevel, scriptingBackend))
            EditorApplication.Exit(1);

    }

    public static void SetRuntime(ScriptingRuntimeVersion runtimeVersion)
    {
        PlayerSettings.scriptingRuntimeVersion = runtimeVersion;


        string preBuildMessage = String.Format("******************************\n" +
               "Setting runtime version to : {0}\n" +
               "******************************", PlayerSettings.scriptingRuntimeVersion);

        Debug.Log(preBuildMessage);
    }

    public static bool BuildWithOptions(string buildPath, string[] scenes, BuildTarget buildTarget, ApiCompatibilityLevel apiLevel, ScriptingImplementation scriptingBackend)
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.options = BuildOptions.None;

        buildPlayerOptions.locationPathName = buildPath;
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.target = buildTarget;
        buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;

        PlayerSettings.SetApiCompatibilityLevel(buildPlayerOptions.targetGroup, apiLevel);
        PlayerSettings.SetScriptingBackend(buildPlayerOptions.targetGroup, scriptingBackend);


        string preBuildMessage = String.Format("******************************\n" +
                "Starting build with the following parameters : \n" +
                "executable : {0} - scene(s) : {1} - target : {2}\n" +
                "API Compatibility Level : {3} - Scripting Backend : {4}\n" +
                "******************************", buildPlayerOptions.locationPathName, String.Join(" ", buildPlayerOptions.scenes), buildPlayerOptions.target,
                PlayerSettings.GetApiCompatibilityLevel(buildPlayerOptions.targetGroup), PlayerSettings.GetScriptingBackend(buildPlayerOptions.targetGroup));

        Debug.Log(preBuildMessage);

#if UNITY_2018_1_OR_NEWER
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (report.summary.result != BuildResult.Succeeded)
        {
            String res = "";
            foreach (BuildStep step in report.steps)
                foreach (BuildStepMessage message in step.messages)
                    //if (message.type == LogType.Error || message.type == LogType.Exception)
                    res += message.content + "\n";

            Debug.LogError("Build report : \n" + res);
#else
        string res = BuildPipeline.BuildPlayer(buildPlayerOptions);
        if (!String.IsNullOrEmpty(res))
        {
            //writer.WriteLine(res);
#endif

            Debug.LogErrorFormat("Buidling failed : {0}", report.summary.result);
            return false;
        }
        return true;
    }

    private static string[] GetScenes()
    {
        string[] scenes = { };
        int scene_index = args.IndexOf(SCENE_PARAM);
        if (scene_index >= 0 && scene_index < args.Count - 1)
        {
            scenes = args[scene_index + 1].Split(';').Select(entry => entry.Replace("\"", "")).ToArray();
        }
        else
            Debug.LogErrorFormat("CommandLineBuilder could not parse the {0} parameter in the command line", SCENE_PARAM);
        return scenes;
    }

    private static BuildTarget GetTarget()
    {
        BuildTarget target = BuildTarget.StandaloneWindows64;
        int target_index = args.IndexOf(TARGET_PARAM);
        if (target_index >= 0 && target_index < args.Count - 1)
        {
            switch (args[target_index + 1])
            {
                case "win32":
                    target = BuildTarget.StandaloneWindows;
                    break;
                case "win64":
                    target = BuildTarget.StandaloneWindows64;
                    break;
                default:
                    target = BuildTarget.StandaloneWindows64;
                    break;
            }
        }
        else
            Debug.LogWarningFormat("CommandLineBuilder could not parse the {0} parameter in the command line. Default is win64", TARGET_PARAM);
        return target;
    }

    private static ApiCompatibilityLevel GetApiLevel(ScriptingRuntimeVersion runtimeVersion)
    {
        ApiCompatibilityLevel apiLevel = runtimeVersion == ScriptingRuntimeVersion.Legacy ? ApiCompatibilityLevel.NET_2_0 : ApiCompatibilityLevel.NET_4_6;
        int api_level_index = args.IndexOf(API_LEVEL_PARAM);
        if (api_level_index >= 0 && api_level_index < args.Count - 1)
        {
            switch (args[api_level_index + 1])
            {
                case "2.0subset":
                    apiLevel = ApiCompatibilityLevel.NET_2_0_Subset;
                    break;
                case "2.0":
                    apiLevel = ApiCompatibilityLevel.NET_2_0;
                    break;
                case "4.x":
                    apiLevel = ApiCompatibilityLevel.NET_4_6;
                    break;
#if UNITY_2018_1_OR_NEWER
                case "standard2.0":
                    apiLevel = ApiCompatibilityLevel.NET_Standard_2_0;
                    break;
#endif
                default:
                    apiLevel = runtimeVersion == ScriptingRuntimeVersion.Legacy ? ApiCompatibilityLevel.NET_2_0 : ApiCompatibilityLevel.NET_4_6;
                    break;
            }
        }
        else
            Debug.LogWarningFormat("CommandLineBuilder could not parse the {0} parameter in the command line. " +
                "Valid value are : \n" +
                "For .NET 3.5 (Default is 2.0)\n" +
                "\t\"2.0subset\" for .NET 2.0 subset\n" +
                "\t\"2.0\" for .NET 2.0\n" +
                "For .NET 4.x (Default is 4.x)\n" +
                "\t\"4.x\" for .NET 4.x\n"
#if UNITY_2018_1_OR_NEWER
                + "\t\"standard2.0\" for .NET standard 2.0"
#endif
                , API_LEVEL_PARAM);
        return apiLevel;
    }

    private static ScriptingImplementation GetScriptingBackend()
    {
        ScriptingImplementation scriptingBackend = ScriptingImplementation.Mono2x;
        int backend_index = args.IndexOf(BACKEND_PARAM);
        if (backend_index >= 0 && backend_index < args.Count - 1)
        {
            switch (args[backend_index + 1])
            {
                case "il2cpp":
                    scriptingBackend = ScriptingImplementation.IL2CPP;
                    break;
                case "winrt":
                    scriptingBackend = ScriptingImplementation.WinRTDotNET;
                    break;
                case "mono":
                    scriptingBackend = ScriptingImplementation.Mono2x;
                    break;
                default:
                    scriptingBackend = ScriptingImplementation.Mono2x;
                    break;
            }
        }
        else
            Debug.LogWarningFormat("CommandLineBuilder could not parse the {0} parameter in the command line. " +
                "Valid value are : \n" +
                "\"mono\" for Mono backend\n" +
                "\"il2cpp\" for IL2CPP backend\n" +
                "\"winrt\" for WinRTDotNET backend\n" +
                "Default is : mono", BACKEND_PARAM);
        return scriptingBackend;
    }

    private static ScriptingRuntimeVersion GetRuntimeVersion()
    {
        ScriptingRuntimeVersion runtimeVersion = ScriptingRuntimeVersion.Latest;
        int runtime_index = args.IndexOf(RUNTIME_PARAM);
        if (runtime_index >= 0 && runtime_index < args.Count - 1)
        {
            switch (args[runtime_index + 1])
            {
                case "4.x":
                    runtimeVersion = ScriptingRuntimeVersion.Latest;
                    break;
                case "3.5":
                    runtimeVersion = ScriptingRuntimeVersion.Legacy;
                    break;
                default:
                    runtimeVersion = ScriptingRuntimeVersion.Latest;
                    break;
            }
        }
        else
            Debug.LogWarningFormat("CommandLineBuilder could not parse the {0} parameter in the command line. " +
                "Valid value are : \n" +
                "\"3.5\" for legacy .NET 3.5\n" +
                "\"4.x\" for latest .NET 4.x\n" +
                "Default is : 4.x", RUNTIME_PARAM);
        return runtimeVersion;
    }

}

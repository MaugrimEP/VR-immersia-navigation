using UnityEditor;

public static class GenerateSolutionFile
{
    public static void GenerateFiles()
    {
        EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
    }
}
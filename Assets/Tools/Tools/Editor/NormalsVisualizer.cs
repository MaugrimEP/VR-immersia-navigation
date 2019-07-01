using UnityEditor;
using UnityEngine;

// found at : https://gist.github.com/mandarinx/ed733369fbb2eea6c7fa9e3da65a0e17
// displays the normals of the selected object

[CustomEditor(typeof(MeshFilter))]
public class NormalsVisualizer : Editor
{
    private static bool enabled = false;

    private static MeshFilter[] selectedMeshFilters;

    private static Mesh currentMesh;

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {

        SceneView.RepaintAll();
        enabled = EditorPrefs.GetBool("ShowNormalsVisualizer");
        if (Menu.GetChecked("Tools/Normals Visualizer"))
            Menu.SetChecked("Tools/Normals Visualizer", enabled);
    }

    // Get selected objects and draw their normals
    void OnSceneGUI()
    {
        // not using deep for now, so only one mesh at a time
        selectedMeshFilters = Selection.GetFiltered<MeshFilter>(/*SelectionMode.Deep |*/ SelectionMode.Editable);
        DrawNormals();
    }

    static void DrawNormals()
    {
        if (!enabled || selectedMeshFilters.Length == 0)
        {
            return;
        }
        foreach (MeshFilter meshFilter in selectedMeshFilters)
        {
            currentMesh = meshFilter.sharedMesh;
            for (int i = 0; i < currentMesh.vertexCount; i++)
            {
                Handles.matrix = meshFilter.transform.localToWorldMatrix;
                Handles.color = Color.yellow;
                Handles.DrawLine(
                    currentMesh.vertices[i],
                    currentMesh.vertices[i] + currentMesh.normals[i]);
            }
        }
    }

    // This allows to update the menu item before showing it
    [MenuItem("Tools/Normals Visualizer", true)]
    private static bool PerformActionValidation()
    {
        enabled = EditorPrefs.GetBool("ShowNormalsVisualizer");
        Menu.SetChecked("Tools/Normals Visualizer", enabled);
        SceneView.RepaintAll();
        return true;
    }

    // the menu item
    [MenuItem("Tools/Normals Visualizer")]
    public static void Toggle()
    {
        enabled = !enabled;
        EditorPrefs.SetBool("ShowNormalsVisualizer", enabled);
        Menu.SetChecked("Tools/Normals Visualizer", enabled);
        SceneView.RepaintAll();
    }


}

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CubeController))]
public class CubeControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var cubeController = (CubeController)target;

        if (GUILayout.Button("Перезаписать силу на инспектор-значение"))
        {
            cubeController.ApplyInspectorForce();
        }

        if (GUILayout.Button("Перезаписать силу на ремоут-значение"))
        {
            cubeController.ApplyRemoteForce();
        }
    }
}

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineGenerator))]
public class SplineGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SplineGenerator generator = (SplineGenerator)target;

        GUILayout.Space(10);

        if (GUILayout.Button("✨ Generate Spline From Children"))
        {
            generator.GenerateSpline();
        }
    }
}
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PassTextures))]
[CanEditMultipleObjects]
public class PassTextureEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate"))
        {
            foreach (PassTextures pass in targets)
            {
                pass.GenerateTexture();
            }
        }

        if (GUILayout.Button("Save"))
        {
            foreach (PassTextures pass in targets)
            {
                pass.BakeOutput();
            }
        }

        if (GUILayout.Button("Save to New"))
        {
            foreach (PassTextures pass in targets)
            {
                pass.BakeAsNew();
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
}

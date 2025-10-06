using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(InputManager))]
public class InputManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        InputManager inputManager = (InputManager)target;
        
        if (inputManager == null || InputManager.Instance == null)
            return;
            
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current Input Mappings", EditorStyles.boldLabel);
        
        if (Application.isPlaying)
        {
            var mappings = InputManager.Instance.GetAllMappings();
            
            EditorGUILayout.BeginVertical("box");
            foreach (var kvp in mappings)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"ID {kvp.Key}:", GUILayout.Width(50));
                EditorGUILayout.LabelField(kvp.Value.ToString(), GUILayout.Width(100));
                
                // Botão para testar o input
                if (GUILayout.Button("Test", GUILayout.Width(50)))
                {
                    InputManager.Instance.TriggerInput(kvp.Key);
                }
                
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Global Reset Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical("box");
            bool globalResetEnabled = InputManager.Instance.IsGlobalResetEnabled();
            EditorGUILayout.LabelField($"Global Reset Enabled: {(globalResetEnabled ? "YES" : "NO")}");
            
            int[] resetIds = InputManager.Instance.GetResetInputIds();
            string resetIdsText = string.Join(", ", resetIds);
            EditorGUILayout.LabelField($"Reset Input IDs: {resetIdsText}");
            
            EditorGUILayout.HelpBox("Durante o gameplay, pressionar os inputs de reset (IDs: " + resetIdsText + 
                                   ") voltará automaticamente para a tela inicial.", MessageType.Info);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Runtime Input Remapping", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("Durante o runtime, você pode chamar:\n" +
                                   "InputManager.Instance.RemapInput(inputId, newKeyCode)\n" +
                                   "para alterar o mapeamento de qualquer input.", MessageType.Info);
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.HelpBox("Execute o jogo para ver os mapeamentos atuais e testar inputs.", MessageType.Info);
        }
        
        if (Application.isPlaying)
        {
            Repaint();
        }
    }
}
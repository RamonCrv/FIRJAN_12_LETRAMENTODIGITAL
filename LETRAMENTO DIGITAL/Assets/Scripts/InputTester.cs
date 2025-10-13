using UnityEngine;

/// <summary>
/// Script helper para testar se os inputs estão funcionando corretamente.
/// Adicione este script a qualquer GameObject na cena para ver logs dos inputs detectados.
/// </summary>
public class InputTester : MonoBehaviour
{
    private void Start()
    {
        // Subscribe to input events
        if (InputManager.Instance != null)
        {
            InputManager.OnInputTriggered += OnInputReceived;
            Debug.Log("InputTester: Subscribed to input events");
            
            // Log current mappings
            var mappings = InputManager.Instance.GetAllMappings();
            Debug.Log($"InputTester: Found {mappings.Count} input mappings:");
            foreach (var mapping in mappings)
            {
                Debug.Log($"  ID {mapping.Key}: {mapping.Value}");
            }
        }
        else
        {
            Debug.LogWarning("InputTester: InputManager.Instance is null!");
        }
    }
    
    private void OnInputReceived(int inputId)
    {
        var keyCode = InputManager.Instance.GetKeyForInputId(inputId);
        Debug.Log($"InputTester: Input received - ID: {inputId}, Key: {keyCode}");
    }
    
    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.OnInputTriggered -= OnInputReceived;
        }
    }
}
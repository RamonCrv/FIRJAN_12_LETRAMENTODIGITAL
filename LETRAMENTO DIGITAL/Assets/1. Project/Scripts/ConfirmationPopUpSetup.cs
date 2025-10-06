using UnityEngine;

/// <summary>
/// Script para ajudar na configuração automática do ConfirmationPopUp
/// Adicione este script ao GameObject do popup para configuração automática
/// </summary>
public class ConfirmationPopUpSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetupOnStart = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupPopup();
        }
    }
    
    [ContextMenu("Setup Popup Components")]
    public void SetupPopup()
    {
        ConfirmationPopUp popup = GetComponent<ConfirmationPopUp>();
        if (popup == null)
        {
            popup = gameObject.AddComponent<ConfirmationPopUp>();
        }
        
        // Try to get CanvasGroup
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            Debug.Log("Added CanvasGroup to ConfirmationPopUp");
        }
        
        // Try to find TextMeshProUGUI in children
        TMPro.TextMeshProUGUI textComponent = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (textComponent != null)
        {
            Debug.Log($"Found TextMeshProUGUI component: {textComponent.name}");
        }
        else
        {
            Debug.LogWarning("No TextMeshProUGUI component found in children. Make sure to assign it manually.");
        }
        
        Debug.Log("ConfirmationPopUp setup completed!");
    }
}
using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedTextComponent : MonoBehaviour
{
    [Header("Localization")]
    [SerializeField] private string textId;
    [SerializeField] private bool updateOnStart = true;
    
    private TextMeshProUGUI textComponent;
    
    public string TextId 
    { 
        get => textId; 
        set 
        { 
            textId = value; 
            UpdateText(); 
        } 
    }

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (updateOnStart)
        {
            // Wait a frame to ensure LocalizationManager is fully initialized
            StartCoroutine(UpdateTextDelayed());
        }
    }

    private System.Collections.IEnumerator UpdateTextDelayed()
    {
        // Wait until LocalizationManager is ready
        while (LocalizationManager.Instance == null || !LocalizationManager.Instance.IsReady)
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        UpdateText();
    }

    /// <summary>
    /// Updates the text component with localized text
    /// </summary>
    public void UpdateText()
    {
        if (textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();

        if (string.IsNullOrEmpty(textId))
        {
            Debug.LogWarning($"Text ID is empty for GameObject: {gameObject.name}");
            return;
        }

        Debug.Log($"UpdateText called for GameObject: {gameObject.name} with ID: '{textId}'");

        if (LocalizationManager.Instance != null)
        {
            string localizedText = LocalizationManager.Instance.GetText(textId);
            textComponent.text = localizedText;
            Debug.Log($"Text updated for {gameObject.name}: '{localizedText}'");
        }
        else
        {
            Debug.LogWarning($"LocalizationManager not found for GameObject: {gameObject.name}");
        }
    }

    /// <summary>
    /// Sets text ID and updates the text immediately
    /// </summary>
    public void SetTextId(string newTextId)
    {
        textId = newTextId;
        UpdateText();
    }

    /// <summary>
    /// Validates text ID in editor
    /// </summary>
    private void OnValidate()
    {
        if (Application.isPlaying && !string.IsNullOrEmpty(textId))
        {
            UpdateText();
        }
    }
}
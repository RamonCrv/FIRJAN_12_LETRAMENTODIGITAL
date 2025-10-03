using UnityEngine;
using TMPro;

public class LocalizationExample : MonoBehaviour
{
    [Header("Example: Using Localization System")]
    [SerializeField] private LocalizedTextComponent[] localizedTexts;
    [SerializeField] private TextMeshProUGUI dynamicScoreText;

    private void Start()
    {
        // Example of updating a dynamic text with localization
        UpdateScoreDisplay(1250);
    }

    /// <summary>
    /// Example of updating dynamic text with localized prefix
    /// </summary>
    public void UpdateScoreDisplay(int score)
    {
        if (dynamicScoreText != null && LocalizationManager.Instance != null)
        {
            string scoreLabel = LocalizationManager.Instance.GetText("score_label");
            dynamicScoreText.text = $"{scoreLabel} {score}";
        }
    }

    /// <summary>
    /// Example of programmatically changing text IDs
    /// </summary>
    public void ShowGameOverTexts()
    {
        if (localizedTexts.Length > 0)
        {
            localizedTexts[0].SetTextId("game_over");
        }
    }

    /// <summary>
    /// Example of getting localized text for use in code
    /// </summary>
    public void ShowConfirmationDialog()
    {
        if (LocalizationManager.Instance != null)
        {
            string confirmText = LocalizationManager.Instance.GetText("confirm_button");
            string cancelText = LocalizationManager.Instance.GetText("cancel_button");
            
            Debug.Log($"Dialog buttons: {confirmText} / {cancelText}");
        }
    }
}
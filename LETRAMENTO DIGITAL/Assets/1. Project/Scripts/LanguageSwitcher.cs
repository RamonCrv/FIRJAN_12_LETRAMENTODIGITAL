using UnityEngine;
using UnityEngine.UI;

public class LanguageSwitcher : MonoBehaviour
{
    [Header("Language Switch")]
    [SerializeField] private Button portugueseButton;
    [SerializeField] private Button englishButton;
    [SerializeField] private GameObject currentLanguageIndicator;

    private void Start()
    {
        SetupButtons();
        UpdateLanguageIndicator();
    }

    /// <summary>
    /// Sets up button click events
    /// </summary>
    private void SetupButtons()
    {
        if (portugueseButton != null)
        {
            portugueseButton.onClick.AddListener(() => SwitchLanguage(SystemLanguage.Portuguese));
        }

        if (englishButton != null)
        {
            englishButton.onClick.AddListener(() => SwitchLanguage(SystemLanguage.English));
        }
    }

    /// <summary>
    /// Switches to the specified language
    /// </summary>
    public void SwitchLanguage(SystemLanguage language)
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.ChangeLanguage(language);
            UpdateLanguageIndicator();
        }
    }

    /// <summary>
    /// Updates visual indicator for current language
    /// </summary>
    private void UpdateLanguageIndicator()
    {
        if (LocalizationManager.Instance == null || currentLanguageIndicator == null)
            return;

        SystemLanguage currentLang = LocalizationManager.Instance.GetCurrentLanguage();
        
        // Move indicator to current language button
        if (currentLang == SystemLanguage.Portuguese && portugueseButton != null)
        {
            currentLanguageIndicator.transform.SetParent(portugueseButton.transform);
        }
        else if (currentLang == SystemLanguage.English && englishButton != null)
        {
            currentLanguageIndicator.transform.SetParent(englishButton.transform);
        }

        currentLanguageIndicator.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Switches to Portuguese language
    /// </summary>
    public void SwitchToPortuguese()
    {
        SwitchLanguage(SystemLanguage.Portuguese);
    }

    /// <summary>
    /// Switches to English language
    /// </summary>
    public void SwitchToEnglish()
    {
        SwitchLanguage(SystemLanguage.English);
    }
}
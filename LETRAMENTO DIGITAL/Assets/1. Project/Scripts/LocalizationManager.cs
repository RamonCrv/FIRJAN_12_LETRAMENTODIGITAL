using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class LocalizationManager : MonoBehaviour
{
    private static LocalizationManager instance;
    
    public static LocalizationManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<LocalizationManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("LocalizationManager");
                    instance = go.AddComponent<LocalizationManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    [Header("Localization Settings")]
    [SerializeField] private SystemLanguage currentLanguage = SystemLanguage.Portuguese;
    [SerializeField] private string localizationFileName = "localization_texts.json";
    
    private LocalizationData localizationData;
    private const string LANGUAGE_PREF_KEY = "selected_language";
    private bool isReady = false;
    private GameLanguage lastGameLanguage = GameLanguage.Portuguese;

    public bool IsReady => isReady;

    private void Awake()
    {
        Debug.Log("LocalizationManager Awake called");
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("LocalizationManager instance set and starting initialization");
        }
        else if (instance != this)
        {
            Debug.Log("Destroying duplicate LocalizationManager");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (instance == this)
        {
            Debug.Log("LocalizationManager Start - Initializing localization");
            InitializeLocalization();
            
            // Sync with DigitalLiteracyGameController if present
            SyncWithGameController();
        }
    }

    private void Update()
    {
        // Monitor GameController language changes
        if (isReady)
        {
            CheckForLanguageChange();
        }
    }

    /// <summary>
    /// Syncs localization with DigitalLiteracyGameController language settings
    /// </summary>
    private void SyncWithGameController()
    {
        DigitalLiteracyGameController gameController = GetComponent<DigitalLiteracyGameController>();
        if (gameController != null)
        {
            // Convert GameLanguage to SystemLanguage
            GameLanguage currentGameLanguage = gameController.GetCurrentLanguage();
            lastGameLanguage = currentGameLanguage;
            
            SystemLanguage gameLanguage = currentGameLanguage == GameLanguage.English 
                ? SystemLanguage.English 
                : SystemLanguage.Portuguese;
                
            if (gameLanguage != currentLanguage)
            {
                Debug.Log($"Syncing language with GameController: {gameLanguage}");
                ChangeLanguage(gameLanguage, false); // Don't save preference during auto-sync
            }
        }
    }

    /// <summary>
    /// Checks if GameController language has changed and syncs accordingly
    /// </summary>
    private void CheckForLanguageChange()
    {
        DigitalLiteracyGameController gameController = GetComponent<DigitalLiteracyGameController>();
        if (gameController != null)
        {
            GameLanguage currentGameLanguage = gameController.GetCurrentLanguage();
            
            if (currentGameLanguage != lastGameLanguage)
            {
                Debug.Log($"GameController language changed from {lastGameLanguage} to {currentGameLanguage}");
                lastGameLanguage = currentGameLanguage;
                
                SystemLanguage newSystemLanguage = currentGameLanguage == GameLanguage.English 
                    ? SystemLanguage.English 
                    : SystemLanguage.Portuguese;
                    
                if (newSystemLanguage != currentLanguage)
                {
                    Debug.Log($"Updating LocalizationManager language to: {newSystemLanguage}");
                    ChangeLanguage(newSystemLanguage, false); // Don't save preference during auto-sync
                }
            }
        }
    }

    /// <summary>
    /// Initializes the localization system and loads text data
    /// </summary>
    private void InitializeLocalization()
    {
        Debug.Log("InitializeLocalization started");
        LoadLanguagePreference();
        LoadLocalizationData();
        isReady = true;
        Debug.Log($"LocalizationManager initialization completed. Ready: {isReady}");
        
        // Update all existing LocalizedTextComponents
        StartCoroutine(UpdateAllLocalizedTexts());
    }

    /// <summary>
    /// Updates all LocalizedTextComponent instances after initialization
    /// </summary>
    private System.Collections.IEnumerator UpdateAllLocalizedTexts()
    {
        yield return null; // Wait one frame
        
        LocalizedTextComponent[] localizedTexts = FindObjectsByType<LocalizedTextComponent>(FindObjectsSortMode.None);
        Debug.Log($"Found {localizedTexts.Length} LocalizedTextComponent instances to update");
        
        foreach (var localizedText in localizedTexts)
        {
            localizedText.UpdateText();
        }
    }

    /// <summary>
    /// Loads language preference from PlayerPrefs
    /// </summary>
    private void LoadLanguagePreference()
    {
        if (PlayerPrefs.HasKey(LANGUAGE_PREF_KEY))
        {
            int languageIndex = PlayerPrefs.GetInt(LANGUAGE_PREF_KEY);
            currentLanguage = (SystemLanguage)languageIndex;
        }
        else
        {
            // Auto-detect system language
            currentLanguage = Application.systemLanguage == SystemLanguage.Portuguese 
                ? SystemLanguage.Portuguese 
                : SystemLanguage.English;
        }
    }

    /// <summary>
    /// Loads localization data from JSON file
    /// </summary>
    private void LoadLocalizationData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, localizationFileName);
        Debug.Log($"Attempting to load localization from: {filePath}");
        
        if (File.Exists(filePath))
        {
            try
            {
                string jsonData = File.ReadAllText(filePath);
                Debug.Log($"JSON data loaded: {jsonData}");
                
                localizationData = JsonConvert.DeserializeObject<LocalizationData>(jsonData);
                
                if (localizationData == null)
                {
                    Debug.LogError("Failed to deserialize localization data - result is null");
                    localizationData = new LocalizationData();
                }
                else
                {
                    Debug.Log($"Localization data loaded successfully. Found {localizationData.texts.Count} texts.");
                    
                    // Debug: Print all loaded text IDs
                    foreach (var text in localizationData.texts)
                    {
                        Debug.Log($"Loaded text ID: '{text.id}' - PT: '{text.portuguese}' - EN: '{text.english}'");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading localization data: {e.Message}");
                Debug.LogError($"Exception details: {e.StackTrace}");
                localizationData = new LocalizationData();
            }
        }
        else
        {
            Debug.LogWarning($"Localization file not found at: {filePath}");
            localizationData = new LocalizationData();
            CreateExampleLocalizationFile();
        }
    }

    /// <summary>
    /// Creates an example localization file with sample data
    /// </summary>
    private void CreateExampleLocalizationFile()
    {
        localizationData.AddText("welcome_message", "Bem-vindo ao jogo!", "Welcome to the game!");
        localizationData.AddText("start_button", "Iniciar", "Start");
        localizationData.AddText("settings_button", "Configurações", "Settings");
        localizationData.AddText("exit_button", "Sair", "Exit");
        localizationData.AddText("score_label", "Pontuação:", "Score:");
        
        SaveLocalizationData();
    }

    /// <summary>
    /// Saves localization data to JSON file
    /// </summary>
    private void SaveLocalizationData()
    {
        try
        {
            string jsonData = JsonConvert.SerializeObject(localizationData, Formatting.Indented);
            string filePath = Path.Combine(Application.streamingAssetsPath, localizationFileName);
            
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, jsonData);
            
            Debug.Log($"Localization data saved to: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving localization data: {e.Message}");
        }
    }

    /// <summary>
    /// Gets localized text by ID
    /// </summary>
    public string GetText(string id)
    {
        if (!isReady)
        {
            Debug.LogWarning($"LocalizationManager not ready yet for text ID: {id}");
            return $"[LOADING: {id}]";
        }
        
        if (localizationData == null)
        {
            Debug.LogError("Localization data not loaded");
            return $"[ERROR: {id}]";
        }
        
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("Text ID is null or empty");
            return "[ERROR: NULL_ID]";
        }
        
        Debug.Log($"Looking for text ID: '{id}' in {localizationData.texts.Count} texts");
        
        string result = localizationData.GetText(id, currentLanguage);
        Debug.Log($"Result for '{id}': '{result}'");
        
        return result;
    }

    /// <summary>
    /// Changes the current language and optionally saves preference
    /// </summary>
    public void ChangeLanguage(SystemLanguage newLanguage, bool savePreference = true)
    {
        currentLanguage = newLanguage;
        
        if (savePreference)
        {
            PlayerPrefs.SetInt(LANGUAGE_PREF_KEY, (int)currentLanguage);
            PlayerPrefs.Save();
        }
        
        // Notify all localized text components to update
        LocalizedTextComponent[] localizedTexts = FindObjectsByType<LocalizedTextComponent>(FindObjectsSortMode.None);
        foreach (var localizedText in localizedTexts)
        {
            localizedText.UpdateText();
        }
        
        Debug.Log($"Language changed to: {currentLanguage}");
    }

    /// <summary>
    /// Changes the current language and saves preference
    /// </summary>
    public void ChangeLanguage(SystemLanguage newLanguage)
    {
        ChangeLanguage(newLanguage, true);
    }

    /// <summary>
    /// Gets the current language
    /// </summary>
    public SystemLanguage GetCurrentLanguage()
    {
        return currentLanguage;
    }

    /// <summary>
    /// Checks if text ID exists
    /// </summary>
    public bool HasText(string id)
    {
        return localizationData?.HasText(id) ?? false;
    }

    /// <summary>
    /// Forces update of all LocalizedTextComponent instances
    /// </summary>
    public void ForceUpdateAllTexts()
    {
        LocalizedTextComponent[] localizedTexts = FindObjectsByType<LocalizedTextComponent>(FindObjectsSortMode.None);
        Debug.Log($"Force updating {localizedTexts.Length} LocalizedTextComponent instances");
        
        foreach (var localizedText in localizedTexts)
        {
            localizedText.UpdateText();
        }
    }

    /// <summary>
    /// Debug method to log current language and available texts
    /// </summary>
    public void DebugLogStatus()
    {
        Debug.Log($"=== LocalizationManager Debug ===");
        Debug.Log($"Is Ready: {isReady}");
        Debug.Log($"Current Language: {currentLanguage}");
        Debug.Log($"Game Controller Language: {lastGameLanguage}");
        Debug.Log($"Available texts: {localizationData?.texts?.Count ?? 0}");
        
        if (localizationData?.texts != null)
        {
            foreach (var text in localizationData.texts)
            {
                Debug.Log($"  - ID: '{text.id}' | PT: '{text.portuguese}' | EN: '{text.english}'");
            }
        }
        Debug.Log($"================================");
    }
}
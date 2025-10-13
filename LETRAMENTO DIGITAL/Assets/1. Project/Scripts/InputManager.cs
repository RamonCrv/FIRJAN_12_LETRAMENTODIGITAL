using UnityEngine;
using System;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    [Header("Input Configuration")]
    [SerializeField] private InputConfiguration inputConfig;
    
    [Header("Global Reset Settings")]
    [SerializeField] private bool enableGlobalReset = true;
    [SerializeField] private int[] resetInputIds = { 0, -1 };
    
    private Dictionary<int, KeyCode> inputMappings = new Dictionary<int, KeyCode>();
    
    public static event Action<int> OnInputTriggered;
    public static event Action OnGlobalReset;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDefaultMappings();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Subscribe to our own input events for global reset handling
        OnInputTriggered += HandleGlobalResetInputs;
    }
    
    private void OnDestroy()
    {
        OnInputTriggered -= HandleGlobalResetInputs;
    }
    
    private void InitializeDefaultMappings()
    {
        // Primeiro carregar mapeamentos do InputConfiguration se disponível
        if (inputConfig != null)
        {
            var configMappings = inputConfig.GetMappingsDictionary();
            foreach (var mapping in configMappings)
            {
                inputMappings[mapping.Key] = mapping.Value;
            }
            Debug.Log($"Loaded {configMappings.Count} input mappings from InputConfiguration");
            
            // Adicionar backspace para ID -1 se não estiver configurado
            if (!inputMappings.ContainsKey(-1))
            {
                inputMappings[-1] = KeyCode.Backspace;
            }
        }
        else
        {
            // Fallback para mapeamentos padrão (números) se não houver InputConfiguration
            inputMappings[0] = KeyCode.Alpha0;
            inputMappings[1] = KeyCode.Alpha1;
            inputMappings[2] = KeyCode.Alpha2;
            inputMappings[3] = KeyCode.Alpha3;
            inputMappings[4] = KeyCode.Alpha4;
            inputMappings[5] = KeyCode.Alpha5;
            inputMappings[6] = KeyCode.Alpha6;
            inputMappings[7] = KeyCode.Alpha7;
            inputMappings[8] = KeyCode.Alpha8;
            inputMappings[9] = KeyCode.Alpha9;
            
            // Backspace como ID -1
            inputMappings[-1] = KeyCode.Backspace;
            
            Debug.LogWarning("InputConfiguration not assigned, using default number mappings");
        }
    }
    
    private void Update()
    {
        CheckForInputs();
    }
    
    private void CheckForInputs()
    {
        foreach (var kvp in inputMappings)
        {
            if (Input.GetKeyDown(kvp.Value))
            {
                TriggerInput(kvp.Key);
            }
        }
    }
    
    private void HandleGlobalResetInputs(int inputId)
    {
        if (!enableGlobalReset) return;
        
        Debug.Log($"InputManager: Checking input {inputId} for global reset");
        
        // Check if this input should trigger a global reset
        bool shouldReset = false;
        foreach (int resetId in resetInputIds)
        {
            if (inputId == resetId)
            {
                shouldReset = true;
                break;
            }
        }
        
        if (shouldReset)
        {
            Debug.Log($"InputManager: Reset input {inputId} triggered!");
            
            // Only reset if we're not in the idle screen and popup is not active
            if (IsInGameplay() && !IsPopupActive())
            {
                var currentState = DigitalLiteracyGameController.Instance.currentState;
                
                // Different behavior based on game state
                if (currentState == DigitalLiteracyGameController.GameState.Final)
                {
                    // Direct reset for final screen
                    Debug.Log($"Direct reset from Final screen triggered by input ID: {inputId}");
                    TriggerGlobalReset();
                }
                else if (currentState == DigitalLiteracyGameController.GameState.Question || 
                         currentState == DigitalLiteracyGameController.GameState.Feedback)
                {
                    // Show confirmation popup for question and feedback screens
                    Debug.Log($"Showing confirmation popup triggered by input ID: {inputId}");
                    ShowExitConfirmationPopup();
                }
            }
            // If popup is active, let it handle the reset input
            else if (IsPopupActive())
            {
                Debug.Log($"Popup is active, reset input {inputId} will close popup and reset");
                // Properly close popup and reset its state
                if (ConfirmationPopUp.Instance != null)
                {
                    ConfirmationPopUp.Instance.ForceHide();
                }
                TriggerGlobalReset();
            }
        }
    }
    
    private bool IsPopupActive()
    {
        return ConfirmationPopUp.Instance != null && ConfirmationPopUp.Instance.IsActive();
    }
    
    private void ShowExitConfirmationPopup()
    {
        if (ConfirmationPopUp.Instance != null)
        {
            ConfirmationPopUp.Instance.ShowExitConfirmation(
                onConfirmCallback: TriggerGlobalReset,
                onCancelCallback: () => Debug.Log("Exit cancelled, continuing game")
            );
        }
        else
        {
            Debug.LogWarning("ConfirmationPopUp instance not found, falling back to direct reset");
            TriggerGlobalReset();
        }
    }
    
    private bool IsInGameplay()
    {
        // Check if we're currently in gameplay (not idle screen)
        if (DigitalLiteracyGameController.Instance != null)
        {
            var currentState = DigitalLiteracyGameController.Instance.currentState;
            return currentState != DigitalLiteracyGameController.GameState.Idle;
        }
        return false;
    }
    
    private void TriggerGlobalReset()
    {
        OnGlobalReset?.Invoke();
        
        // Also trigger the reset in the game controller
        if (DigitalLiteracyGameController.Instance != null)
        {
            DigitalLiteracyGameController.Instance.ReturnToIdle();
        }
    }
    
    public void TriggerInput(int inputId)
    {
        OnInputTriggered?.Invoke(inputId);
    }
    
    public bool IsInputPressed(int inputId)
    {
        if (inputMappings.ContainsKey(inputId))
        {
            return Input.GetKey(inputMappings[inputId]);
        }
        return false;
    }
    
    public bool IsInputDown(int inputId)
    {
        if (inputMappings.ContainsKey(inputId))
        {
            return Input.GetKeyDown(inputMappings[inputId]);
        }
        return false;
    }
    
    public bool IsInputUp(int inputId)
    {
        if (inputMappings.ContainsKey(inputId))
        {
            return Input.GetKeyUp(inputMappings[inputId]);
        }
        return false;
    }
    
    public void RemapInput(int inputId, KeyCode newKey)
    {
        if (inputMappings.ContainsKey(inputId))
        {
            inputMappings[inputId] = newKey;
            Debug.Log($"Input ID {inputId} remapped to {newKey}");
        }
        else
        {
            inputMappings.Add(inputId, newKey);
            Debug.Log($"New input ID {inputId} added with key {newKey}");
        }
    }
    
    public KeyCode GetKeyForInputId(int inputId)
    {
        return inputMappings.ContainsKey(inputId) ? inputMappings[inputId] : KeyCode.None;
    }
    
    public Dictionary<int, KeyCode> GetAllMappings()
    {
        return new Dictionary<int, KeyCode>(inputMappings);
    }
    
    public void SetGlobalResetEnabled(bool enabled)
    {
        enableGlobalReset = enabled;
    }
    
    public bool IsGlobalResetEnabled()
    {
        return enableGlobalReset;
    }
    
    public void SetResetInputIds(int[] newResetIds)
    {
        resetInputIds = newResetIds;
    }
    
    public int[] GetResetInputIds()
    {
        return resetInputIds;
    }
    
    /// <summary>
    /// Recarrega os mapeamentos de input do InputConfiguration asset
    /// </summary>
    public void ReloadInputConfiguration()
    {
        inputMappings.Clear();
        InitializeDefaultMappings();
    }
    
    /// <summary>
    /// Define uma nova configuração de input e recarrega os mapeamentos
    /// </summary>
    public void SetInputConfiguration(InputConfiguration newInputConfig)
    {
        inputConfig = newInputConfig;
        ReloadInputConfiguration();
    }
}
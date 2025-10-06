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
        // Mapear números 0-9 para os IDs correspondentes
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
            // Only reset if we're not in the idle screen
            if (IsInGameplay())
            {
                Debug.Log($"Global reset triggered by input ID: {inputId}");
                TriggerGlobalReset();
            }
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
}
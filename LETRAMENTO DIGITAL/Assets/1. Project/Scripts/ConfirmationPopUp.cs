using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ConfirmationPopUp : MonoBehaviour
{
    [Header("Popup Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI confirmationText;
    
    [Header("Input IDs")]
    [SerializeField] private int confirmInputId = 0;
    [SerializeField] private int alternateConfirmInputId = -1; // Changed from 1 to -1 so only A and Z confirm
    
    public static ConfirmationPopUp Instance { get; private set; }
    
    private bool isActive = false;
    private Action onConfirm;
    private Action onCancel;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            // Get components if not assigned
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
                
            if (confirmationText == null)
                confirmationText = GetComponentInChildren<TextMeshProUGUI>();
            
            // Start hidden
            HidePopup();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        SubscribeToInputs();
    }
    
    void OnDestroy()
    {
        UnsubscribeFromInputs();
    }
    
    void SubscribeToInputs()
    {
        InputManager.OnInputTriggered += HandleInputTriggered;
    }
    
    void UnsubscribeFromInputs()
    {
        InputManager.OnInputTriggered -= HandleInputTriggered;
    }
    
    void HandleInputTriggered(int inputId)
    {
        if (!isActive) return;
        
        // Check if this is a global reset input (0 or -1) - let InputManager handle it
        if (IsGlobalResetInput(inputId))
        {
            // Don't handle reset inputs here - let InputManager process them
            return;
        }
        
        if (inputId == confirmInputId || inputId == alternateConfirmInputId)
        {
            Debug.Log($"ConfirmationPopUp: Input {inputId} is confirming popup");
            ConfirmAction();
        }
        else
        {
            Debug.Log($"ConfirmationPopUp: Input {inputId} is cancelling popup (just closing)");
            // Any other input just cancels the popup (closes it without reset)
            CancelAction();
        }
    }
    
    private bool IsGlobalResetInput(int inputId)
    {
        if (InputManager.Instance != null && InputManager.Instance.IsGlobalResetEnabled())
        {
            int[] resetIds = InputManager.Instance.GetResetInputIds();
            foreach (int resetId in resetIds)
            {
                if (inputId == resetId)
                {
                    Debug.Log($"ConfirmationPopUp: Input {inputId} is a global reset input, letting InputManager handle it");
                    return true;
                }
            }
        }
        Debug.Log($"ConfirmationPopUp: Input {inputId} is NOT a global reset input, handling locally");
        return false;
    }
    
    public void ShowConfirmationPopup(Action onConfirmCallback, Action onCancelCallback = null)
    {
        
        onConfirm = onConfirmCallback;
        onCancel = onCancelCallback;
        
        ShowPopup();
    }
    
    public void ShowExitConfirmation(Action onConfirmCallback, Action onCancelCallback = null)
    {
        
        ShowConfirmationPopup(onConfirmCallback, onCancelCallback);
    }
    
    private void ShowPopup()
    {
        isActive = true;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            gameObject.SetActive(true);
        }
        
        Debug.Log("Confirmation popup shown");
    }
    
    private void HidePopup()
    {
        Debug.Log("ConfirmationPopUp: Hiding popup and setting isActive = false");
        isActive = false;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            gameObject.SetActive(false);
        }
        
        Debug.Log($"ConfirmationPopUp: Popup hidden, isActive = {isActive}");
    }
    
    private void ConfirmAction()
    {
        Debug.Log("Popup confirmed");
        HidePopup();
        onConfirm?.Invoke();
        ClearCallbacks();
    }
    
    private void CancelAction()
    {
        Debug.Log("Popup cancelled - just closing popup, no reset");
        HidePopup();
        onCancel?.Invoke();
        ClearCallbacks();
    }
    
    private void ClearCallbacks()
    {
        onConfirm = null;
        onCancel = null;
    }
    
    public bool IsActive()
    {
        return isActive;
    }
    
    /// <summary>
    /// Force hide the popup and reset its state (used for external reset)
    /// </summary>
    public void ForceHide()
    {
        Debug.Log("ConfirmationPopUp: Force hiding popup");
        HidePopup();
        ClearCallbacks();
    }
}
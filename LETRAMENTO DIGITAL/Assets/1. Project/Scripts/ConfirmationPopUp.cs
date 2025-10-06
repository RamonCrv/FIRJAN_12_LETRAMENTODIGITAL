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
    [SerializeField] private int alternateConfirmInputId = 1;
    
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
        
        if (inputId == confirmInputId || inputId == alternateConfirmInputId)
        {
            ConfirmAction();
        }
        else
        {
            CancelAction();
        }
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
        
        Debug.Log("Confirmation popup hidden");
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
        Debug.Log("Popup cancelled");
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
}
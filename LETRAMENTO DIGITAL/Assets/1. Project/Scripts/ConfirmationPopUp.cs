using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class ConfirmationPopUp : MonoBehaviour
{
    [Header("Popup Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI confirmationText;
    [SerializeField] private TextMeshProUGUI timerText;
    
    [Header("Input IDs")]
    [SerializeField] private int confirmInputId = 0;
    [SerializeField] private int alternateConfirmInputId = -1;
    
    [Header("Auto Close Settings")]
    [SerializeField] private float autoCloseTime = 5f;
    
    public static ConfirmationPopUp Instance { get; private set; }
    
    private bool isActive = false;
    private Action onConfirm;
    private Action onCancel;
    private Coroutine autoCloseCoroutine;
    private float remainingTime;
    
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
        
        StopAutoClose();
        
        if (IsResetInput(inputId))
        {
            ConfirmAction();
        }
        else if (inputId == confirmInputId || inputId == alternateConfirmInputId)
        {
            ConfirmAction();
        }
        else
        {
            CancelAction();
        }
    }
    
    private bool IsResetInput(int inputId)
    {
        if (InputManager.Instance != null && InputManager.Instance.IsGlobalResetEnabled())
        {
            int[] resetIds = InputManager.Instance.GetResetInputIds();
            foreach (int resetId in resetIds)
            {
                if (inputId == resetId)
                {
                    return true;
                }
            }
        }
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
        
        StartAutoClose();
    }
    
    private void HidePopup()
    {
        isActive = false;
        StopAutoClose();
        
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
    }
    
    private void StartAutoClose()
    {
        StopAutoClose();
        autoCloseCoroutine = StartCoroutine(AutoCloseCountdown());
    }
    
    private void StopAutoClose()
    {
        if (autoCloseCoroutine != null)
        {
            StopCoroutine(autoCloseCoroutine);
            autoCloseCoroutine = null;
        }
        
        if (timerText != null)
        {
            timerText.text = string.Empty;
        }
    }
    
    private IEnumerator AutoCloseCountdown()
    {
        remainingTime = autoCloseTime;
        
        while (remainingTime > 0f)
        {
            if (timerText != null)
            {
                timerText.text = $"{Mathf.CeilToInt(remainingTime)}s";
            }
            
            remainingTime -= Time.deltaTime;
            yield return null;
        }
        
        CancelAction();
    }
    
    private void ConfirmAction()
    {
        HidePopup();
        onConfirm?.Invoke();
        ClearCallbacks();
    }
    
    private void CancelAction()
    {
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
        HidePopup();
        ClearCallbacks();
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RealGames;
using System.Collections;

public class FeedbackScreen : CanvasScreen
{
    [Header("Feedback Screen Components")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Image resultIcon;
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timerFill;
    
    [Header("Result Icons")]
    [SerializeField] private Sprite correctIcon;
    [SerializeField] private Sprite incorrectIcon;
    
    [Header("Result Colors")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;
    
    public static FeedbackScreen Instance;
    private Coroutine timerCoroutine;
    
    void Awake()
    {
        Instance = this;
        screenData.screenName = "FeedbackScreen";
    }
    
    void Start()
    {
        SetupButton();
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
        if (!IsOn()) return;
        
        // Check if this input should be handled globally (reset functionality)
        if (ShouldInputBeHandledGlobally(inputId)) return;
        
        // If popup is active, don't process other inputs here (popup will handle them)
        if (ConfirmationPopUp.Instance != null && ConfirmationPopUp.Instance.IsActive()) return;
        
        // Only allow Letter A (inputId 0) and Letter Z (inputId 25) to continue
        if (inputId == 0 || inputId == 25)
        {
            ContinueToNext();
        }
    }
    
    private bool ShouldInputBeHandledGlobally(int inputId)
    {
        // Let global reset inputs be handled by InputManager instead of local screen
        if (InputManager.Instance != null && InputManager.Instance.IsGlobalResetEnabled())
        {
            int[] resetIds = InputManager.Instance.GetResetInputIds();
            foreach (int resetId in resetIds)
            {
                if (inputId == resetId)
                {
                    // Always let InputManager handle reset inputs (it will show popup if needed)
                    return true;
                }
            }
        }
        return false;
    }
    
    void SetupButton()
    {
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueToNext);
        }
    }
    
    public void SetFeedback(bool isCorrect, Question question)
    {
        DisplayFeedback(isCorrect, question);
        StartTimer();
    }
    
    void DisplayFeedback(bool isCorrect, Question question)
    {
        GameLanguage currentLang = DigitalLiteracyGameController.Instance.GetCurrentLanguage();
        
        if (resultText != null)
        {
            if (currentLang == GameLanguage.English)
            {
                resultText.text = isCorrect ? "CORRECT!" : "INCORRECT!";
            }
            else
            {
                resultText.text = isCorrect ? "CORRETO!" : "INCORRETO!";
            }
            resultText.color = isCorrect ? correctColor : incorrectColor;
        }
        
        if (feedbackText != null)
        {
            if (currentLang == GameLanguage.English)
            {
                string feedbackToShow = isCorrect 
                    ? (!string.IsNullOrEmpty(question.feedback.correctEn) ? question.feedback.correctEn : question.feedback.correct)
                    : (!string.IsNullOrEmpty(question.feedback.incorrectEn) ? question.feedback.incorrectEn : question.feedback.incorrect);
                feedbackText.text = feedbackToShow;
            }
            else
            {
                feedbackText.text = isCorrect ? question.feedback.correct : question.feedback.incorrect;
            }
        }
        
        if (resultIcon != null)
        {
            resultIcon.sprite = isCorrect ? correctIcon : incorrectIcon;
            resultIcon.color = isCorrect ? correctColor : incorrectColor;
        }
    }
    
    void StartTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        
        timerCoroutine = StartCoroutine(TimerCountdown());
    }
    
    IEnumerator TimerCountdown()
    {
        float timeRemaining = DigitalLiteracyGameController.Instance.gameConfig.timeoutSettings.feedbackTimeoutSeconds;
        float totalTime = timeRemaining;
        
        while (timeRemaining > 0)
        {
            if (timerText != null)
            {
                GameLanguage currentLang = DigitalLiteracyGameController.Instance.GetCurrentLanguage();
                if (currentLang == GameLanguage.English)
                {
                    timerText.text = $"Next question in: {Mathf.Ceil(timeRemaining)}s (Press A or Z to continue)";
                }
                else
                {
                    timerText.text = $"Próxima pergunta em: {Mathf.Ceil(timeRemaining)}s (Pressione A ou Z para continuar)";
                }
            }
            
            if (timerFill != null)
            {
                timerFill.fillAmount = timeRemaining / totalTime;
            }
            
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        
        // Auto continue
        ContinueToNext();
    }
    
    public void ContinueToNext()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        
        DigitalLiteracyGameController.Instance?.NextQuestion();
    }
    
    public override void TurnOn()
    {
        base.TurnOn();
        // Screen is now active
    }
    
    public override void TurnOff()
    {
        base.TurnOff();
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }
}
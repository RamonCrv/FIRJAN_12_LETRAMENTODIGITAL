using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RealGames;
using System.Collections;
using System.Collections.Generic;

public class QuestionScreen : CanvasScreen
{
    [Header("Question Screen Components")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI[] alternativeTexts;
    [SerializeField] private Button[] alternativeButtons;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timerFill;
    [SerializeField] private TextMeshProUGUI questionCounterText;
    
    public static QuestionScreen Instance;
    private Question currentQuestion;
    private Coroutine timerCoroutine;
    
    void Awake()
    {
        Instance = this;
        screenData.screenName = "QuestionScreen";
    }
    
    void Start()
    {
        SetupButtons();
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
        
        // InputId corresponde diretamente ao índice da resposta para números 0-9
        if (inputId >= 0 && inputId <= 9)
        {
            SubmitAnswer(inputId);
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
    
    void SetupButtons()
    {
        for (int i = 0; i < alternativeButtons.Length; i++)
        {
            int index = i;
            alternativeButtons[i].onClick.AddListener(() => SubmitAnswer(index));
        }
    }
    
    public void SetQuestion(Question question)
    {
        currentQuestion = question;
        DisplayQuestion();
        StartTimer();
    }
    
    void DisplayQuestion()
    {
        if (questionText != null && currentQuestion != null)
        {
            // Use localized question text
            GameLanguage currentLang = DigitalLiteracyGameController.Instance.GetCurrentLanguage();
            if (currentLang == GameLanguage.English && !string.IsNullOrEmpty(currentQuestion.questionTextEn))
            {
                questionText.text = currentQuestion.questionTextEn;
            }
            else
            {
                questionText.text = currentQuestion.questionText;
            }
        }

        // Update question counter
        if (questionCounterText != null && DigitalLiteracyGameController.Instance != null)
        {
            int currentIndex = DigitalLiteracyGameController.Instance.GetCurrentQuestionIndex() + 1;
            int totalQuestions = DigitalLiteracyGameController.Instance.GetTotalQuestions();

           // questionCounterText.text = $"{currentIndex}/{totalQuestions}";
            questionCounterText.text = $"{currentIndex}";

        }

        // Use localized alternatives
        GameLanguage lang = DigitalLiteracyGameController.Instance.GetCurrentLanguage();
        List<string> alternativesToUse = (lang == GameLanguage.English && currentQuestion.alternativesEn != null && currentQuestion.alternativesEn.Count > 0) 
            ? currentQuestion.alternativesEn 
            : currentQuestion.alternatives;

        for (int i = 0; i < alternativeTexts.Length && i < alternativesToUse.Count; i++)
        {
            if (alternativeTexts[i] != null)
            {
                alternativeTexts[i].text = $"{i + 1}. {alternativesToUse[i]}";
                alternativeTexts[i].gameObject.SetActive(true);
            }
            
            if (alternativeButtons.Length > i && alternativeButtons[i] != null)
            {
                alternativeButtons[i].gameObject.SetActive(true);
            }
        }
        
        // Hide unused alternatives
        for (int i = alternativesToUse.Count; i < alternativeTexts.Length; i++)
        {
            if (alternativeTexts[i] != null)
            {
                alternativeTexts[i].gameObject.SetActive(false);
            }
            
            if (alternativeButtons[i] != null)
            {
                alternativeButtons[i].gameObject.SetActive(false);
            }
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
        float timeRemaining = DigitalLiteracyGameController.Instance.gameConfig.timeoutSettings.questionTimeoutSeconds;
        float totalTime = timeRemaining;
        
        while (timeRemaining > 0)
        {
            if (timerText != null)
            {
                timerText.text = $"{Mathf.Ceil(timeRemaining)}";
            }
            
            if (timerFill != null)
            {
                timerFill.fillAmount = timeRemaining / totalTime;
            }
            
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        
        // Time's up
        if (timerText != null)
        {
            GameLanguage currentLang = DigitalLiteracyGameController.Instance.GetCurrentLanguage();
            if (currentLang == GameLanguage.English)
            {
                timerText.text = "Time's up!";
            }
            else
            {
                timerText.text = "Tempo esgotado!";
            }
        }
        
        if (timerFill != null)
        {
            timerFill.fillAmount = 0;
        }
    }
    
    public void SubmitAnswer(int answerIndex)
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        
        DigitalLiteracyGameController.Instance?.SubmitAnswer(answerIndex);
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

    private void Update()
    {
        // Input handling agora é feito via InputManager e eventos
    }
}
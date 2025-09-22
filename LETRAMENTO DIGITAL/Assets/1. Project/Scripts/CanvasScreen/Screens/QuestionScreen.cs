using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RealGames;
using System.Collections;

public class QuestionScreen : CanvasScreen
{
    [Header("Question Screen Components")]
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI[] alternativeTexts;
    [SerializeField] private Button[] alternativeButtons;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timerFill;
    
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
            questionText.text = currentQuestion.questionText;

        }

        for (int i = 0; i < alternativeTexts.Length && i < currentQuestion.alternatives.Count; i++)
        {
            if (alternativeTexts[i] != null)
            {
                alternativeTexts[i].text = $"{i + 1}. {currentQuestion.alternatives[i]}";
                alternativeTexts[i].gameObject.SetActive(true);
            }
            
            if (alternativeButtons.Length > i  && alternativeButtons[i] != null )
            {
                alternativeButtons[i].gameObject.SetActive(true);
            }
        }
        
        // Hide unused alternatives
        for (int i = currentQuestion.alternatives.Count; i < alternativeTexts.Length; i++)
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
                timerText.text = $"Tempo: {Mathf.Ceil(timeRemaining)}s";
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
            timerText.text = "Tempo esgotado!";
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
        int answerIndex = -1;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            answerIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            answerIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            answerIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            answerIndex = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            answerIndex = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            answerIndex = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            answerIndex = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            answerIndex = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            answerIndex = 9;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            answerIndex = 0;
        }

        if (answerIndex > -1)
        {
            SubmitAnswer(answerIndex);
        }
    }
}
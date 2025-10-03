using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FinalScreen : CanvasScreen
{
    [Header("Final Screen Components")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Image scoreIcon;
    
    [Header("Auto Return Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image timerFill;
    [SerializeField] private Button manualResetButton;
    
    [Header("Score Icons")]
    [SerializeField] private Sprite excellentIcon;
    [SerializeField] private Sprite goodIcon;
    [SerializeField] private Sprite averageIcon;
    [SerializeField] private Sprite poorIcon;
    
    [Header("Score Colors")]
    [SerializeField] private Color excellentColor = Color.green;
    [SerializeField] private Color goodColor = Color.blue;
    [SerializeField] private Color averageColor = Color.yellow;
    [SerializeField] private Color poorColor = Color.red;
    
    public static FinalScreen Instance;
    private Coroutine autoReturnCoroutine;
    
    void Awake()
    {
        Instance = this;
        screenData.screenName = "FinalScreen";
    }
    
    void Start()
    {
        SetupButtons();
    }
    
    void SetupButtons()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (manualResetButton != null)
        {
            manualResetButton.onClick.AddListener(ManualResetGame);
        }
    }
    
    public void SetResults(int correctAnswers, int totalQuestions)
    {
        DisplayResults(correctAnswers, totalQuestions);
        StartAutoReturnTimer();
    }
    
    void DisplayResults(int correctAnswers, int totalQuestions)
    {
        if (titleText != null)
        {
            titleText.text = "RESULTADO FINAL";
        }
        
        if (scoreText != null)
        {
            scoreText.text = $"{correctAnswers}/{totalQuestions} Corretas";
        }
        
        float percentage = (float)correctAnswers / totalQuestions * 100f;
        string message;
        Sprite iconToUse;
        Color colorToUse;
        
        if (percentage >= 90f)
        {
            message = "Excelente! Você demonstra grande conhecimento em letramento digital!";
            iconToUse = excellentIcon;
            colorToUse = excellentColor;
        }
        else if (percentage >= 70f)
        {
            message = "Bom trabalho! Você tem um bom conhecimento sobre ferramentas digitais.";
            iconToUse = goodIcon;
            colorToUse = goodColor;
        }
        else if (percentage >= 50f)
        {
            message = "Você tem conhecimento básico. Continue aprendendo sobre letramento digital!";
            iconToUse = averageIcon;
            colorToUse = averageColor;
        }
        else
        {
            message = "É importante aprender mais sobre letramento digital. Continue praticando!";
            iconToUse = poorIcon;
            colorToUse = poorColor;
        }
        
        if (messageText != null)
        {
            messageText.text = message;
        }
        
        if (scoreIcon != null && iconToUse != null)
        {
            scoreIcon.sprite = iconToUse;
            scoreIcon.color = colorToUse;
        }
        
        if (scoreText != null)
        {
            scoreText.color = colorToUse;
        }
    }
    
    void StartAutoReturnTimer()
    {
        if (autoReturnCoroutine != null)
        {
            StopCoroutine(autoReturnCoroutine);
        }
        
        autoReturnCoroutine = StartCoroutine(AutoReturnTimerCountdown());
    }
    
    IEnumerator AutoReturnTimerCountdown()
    {
        float timeRemaining = DigitalLiteracyGameController.Instance.gameConfig.timeoutSettings.finalScreenTimeoutSeconds;
        float totalTime = timeRemaining;
        
        while (timeRemaining > 0)
        {
            if (timerText != null)
            {
                timerText.text = $"Voltando ao início em: {Mathf.Ceil(timeRemaining)}s";
            }
            
            if (timerFill != null)
            {
                timerFill.fillAmount = timeRemaining / totalTime;
            }
            
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        
        // Auto return to idle
        RestartGame();
    }
    
    public void RestartGame()
    {
        if (autoReturnCoroutine != null)
        {
            StopCoroutine(autoReturnCoroutine);
        }
        
        DigitalLiteracyGameController.Instance?.ReturnToIdle();
    }
    
    public void ManualResetGame()
    {
        if (autoReturnCoroutine != null)
        {
            StopCoroutine(autoReturnCoroutine);
        }
        
        DigitalLiteracyGameController.Instance?.ReturnToIdle();
    }
    
    public override void TurnOn()
    {
        base.TurnOn();
        // Screen is now active
    }
    
    public override void TurnOff()
    {
        base.TurnOff();
        if (autoReturnCoroutine != null)
        {
            StopCoroutine(autoReturnCoroutine);
        }
    }
    
    void Update()
    {
        // Check if player presses "0" to return to idle screen
        if (Input.GetKeyDown(KeyCode.Alpha0) && IsOn())
        {
            RestartGame();
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalScreen : CanvasScreen
{
    [Header("Final Screen Components")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Image scoreIcon;
    
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
    
    void Awake()
    {
        Instance = this;
        screenData.screenName = "FinalScreen";
    }
    
    void Start()
    {
        SetupButton();
    }
    
    void SetupButton()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }
    
    public void SetResults(int correctAnswers, int totalQuestions)
    {
        DisplayResults(correctAnswers, totalQuestions);
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
    
    public void RestartGame()
    {
        DigitalLiteracyGameController.Instance?.ReturnToIdle();
    }
    
    public override void TurnOn()
    {
        base.TurnOn();
        // Screen is now active
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IdleScreen : CanvasScreen
{
    [Header("Idle Screen Components")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Button startButton;

    
    public static IdleScreen Instance;
    private bool canProcessInput = false;
    
    void Awake()
    {
        Instance = this;
        screenData.screenName = "IdleScreen";
    }
    
    void Start()
    {
        SetupIdleScreen();
    }
    
    void SetupIdleScreen()
    {
        if (titleText != null)
        {
            titleText.text = "LETRAMENTO DIGITAL";
        }
        
        if (instructionText != null)
        {
            instructionText.text = "Pressione 0 para PORTUGUÊS ou BACKSPACE para ENGLISH";
        }
        
        if (startButton != null)
        {
            startButton.onClick.AddListener(() => StartGame(GameLanguage.Portuguese));
        }
    }
    
    public void StartGame(GameLanguage language)
    {
        DigitalLiteracyGameController.Instance?.StartQuestions(language);
    }


    private void Update()
    {
        if (!IsOn() || !canProcessInput) return;
        
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("Starting game in Portuguese");
            StartGame(GameLanguage.Portuguese);
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Debug.Log("Starting game in English");
            StartGame(GameLanguage.English);
        }
    }
    
    public override void TurnOn()
    {
        base.TurnOn();
        // Add delay to prevent immediate input processing when returning from other screens
        Invoke(nameof(EnableInputProcessing), 0.2f);
    }
    
    public override void TurnOff()
    {
        base.TurnOff();
        canProcessInput = false;
        CancelInvoke(nameof(EnableInputProcessing));
    }
    
    private void EnableInputProcessing()
    {
        canProcessInput = true;
    }
}
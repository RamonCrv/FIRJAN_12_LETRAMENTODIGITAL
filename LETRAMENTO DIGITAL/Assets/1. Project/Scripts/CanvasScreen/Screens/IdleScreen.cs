using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IdleScreen : CanvasScreen
{
    [Header("Idle Screen Components")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Button startButton;
    
    [Header("Input IDs")]
    [SerializeField] private int portugueseInputId = 0;
    [SerializeField] private int englishInputId = -1;

    
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
        if (!IsOn() || !canProcessInput) return;
        
        if (inputId == portugueseInputId)
        {
            Debug.Log("Starting game in Portuguese");
            StartGame(GameLanguage.Portuguese);
        }
        else if (inputId == englishInputId)
        {
            Debug.Log("Starting game in English");
            StartGame(GameLanguage.English);
        }
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
        // Input handling agora é feito via InputManager e eventos
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
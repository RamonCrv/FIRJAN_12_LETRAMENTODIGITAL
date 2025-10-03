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
            instructionText.text = "Pressione 0 no teclado para começar";
        }
        
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
    }
    
    public void StartGame()
    {
        DigitalLiteracyGameController.Instance?.StartQuestions();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0) && IsOn() && )
        {
            Debug.Log("Teste");
            StartGame();
        }
    }
    public override void TurnOn()
    {
        base.TurnOn();
        // Reset any animations or effects when screen becomes active
    }
}
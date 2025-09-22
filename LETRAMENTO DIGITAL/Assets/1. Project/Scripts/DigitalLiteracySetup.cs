using UnityEngine;

[System.Serializable]
public class ScreenReferences
{
    [Header("Screen Prefabs or GameObjects")]
    public GameObject idleScreen;
    public GameObject questionScreen;
    public GameObject feedbackScreen;
    public GameObject finalScreen;
}

public class DigitalLiteracySetup : MonoBehaviour
{
    [Header("Screen Setup")]
    public ScreenReferences screens;
    
    [Header("Auto Setup")]
    public bool autoSetupOnStart = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupGame();
        }
    }
    
    public void SetupGame()
    {
        // Ensure all screens are properly set up
        SetupScreen(screens.idleScreen, "IdleScreen");
        SetupScreen(screens.questionScreen, "QuestionScreen");
        SetupScreen(screens.feedbackScreen, "FeedbackScreen");
        SetupScreen(screens.finalScreen, "FinalScreen");
        
        Debug.Log("Digital Literacy Game setup completed!");
    }
    
    void SetupScreen(GameObject screenObject, string screenName)
    {
        if (screenObject != null)
        {
            CanvasScreen canvasScreen = screenObject.GetComponent<CanvasScreen>();
            if (canvasScreen != null)
            {
                canvasScreen.screenData.screenName = screenName;
                Debug.Log($"Setup screen: {screenName}");
            }
            else
            {
                Debug.LogWarning($"Screen {screenName} does not have CanvasScreen component!");
            }
        }
        else
        {
            Debug.LogWarning($"Screen object for {screenName} is not assigned!");
        }
    }
    
    [ContextMenu("Force Setup")]
    public void ForceSetup()
    {
        SetupGame();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RealGames;
using System.IO;
using System.Linq;

public class DigitalLiteracyGameController : MonoBehaviour
{
    [Header("Game Settings")]
    public GameConfig gameConfig;
    private List<Question> selectedQuestions;
    private int currentQuestionIndex = 0;
    private float inactiveTimer = 0f;
    private bool isWaitingForInput = false;
    
    [Header("Game State")]
    public GameState currentState = GameState.Idle;
    public int correctAnswers = 0;
    
    public enum GameState
    {
        Idle,
        Question,
        Feedback,
        Final
    }
    
    public static DigitalLiteracyGameController Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameConfig();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        StartGame();
    }
    
    void Update()
    {
        HandleInput();
        UpdateInactiveTimer();
    }
    
    void LoadGameConfig()
    {
        string configPath = Path.Combine(Application.streamingAssetsPath, "config.json");
        
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            gameConfig = JsonUtility.FromJson<GameConfig>(json);
            Debug.Log("Game config loaded successfully");
        }
        else
        {
            Debug.LogError("Config file not found at: " + configPath);
            CreateDefaultConfig();
        }
    }
    
    void CreateDefaultConfig()
    {
        gameConfig = new GameConfig();
        gameConfig.timeoutSettings = new TimeoutSettings();
        gameConfig.questions = new List<Question>();
        Debug.Log("Using default game config");
    }
    
    void HandleInput()
    {
        if (!isWaitingForInput) return;
        
        // Check for any input to reset timer
        if (Input.inputString.Length > 0 || Input.anyKeyDown)
        {
            ResetInactiveTimer();
        }
        
        // Handle idle screen input
        //if (currentState == GameState.Idle && Input.inputString == "0")
        //{
        //    Debug.Log("Entrou aqui talvez");
        //    StartQuestions();
        //}
        
        // Handle question screen input
        if (currentState == GameState.Question)
        {
            for (int i = 1; i <= 9; i++)
            {
                if (Input.inputString == i.ToString())
                {
                    SubmitAnswer(i - 1);
                    break;
                }
            }
        }
    }
    
    void UpdateInactiveTimer()
    {
        inactiveTimer += Time.deltaTime;
        
        if (inactiveTimer >= gameConfig.timeoutSettings.generalTimeoutSeconds)
        {
            ReturnToIdle();
        }
    }
    
    void ResetInactiveTimer()
    {
        inactiveTimer = 0f;
    }
    
    public void StartGame()
    {
        currentState = GameState.Idle;
        isWaitingForInput = true;
        ResetInactiveTimer();
        ScreenManager.SetCallScreen("IdleScreen");
    }
    
    public void StartQuestions()
    {
        SelectRandomQuestions();
        currentQuestionIndex = 0;
        correctAnswers = 0;
        ShowNextQuestion();
    }
    
    void SelectRandomQuestions()
    {
        selectedQuestions = new List<Question>();
        List<Question> availableQuestions = new List<Question>(gameConfig.questions);
        
        for (int i = 0; i < 5 && availableQuestions.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableQuestions.Count);
            selectedQuestions.Add(availableQuestions[randomIndex]);
            availableQuestions.RemoveAt(randomIndex);
        }
    }
    
    public void ShowNextQuestion()
    {
        if (currentQuestionIndex < selectedQuestions.Count)
        {
            currentState = GameState.Question;
            QuestionScreen.Instance?.SetQuestion(selectedQuestions[currentQuestionIndex]);
            ScreenManager.SetCallScreen("QuestionScreen");
            StartCoroutine(QuestionTimeout());
            ResetInactiveTimer();
        }
        else
        {
            ShowFinalScreen();
        }
    }
    
    IEnumerator QuestionTimeout()
    {
        yield return new WaitForSeconds(gameConfig.timeoutSettings.questionTimeoutSeconds);
        
        if (currentState == GameState.Question)
        {
            SubmitAnswer(-1); // Wrong answer for timeout
        }
    }
    
    public void SubmitAnswer(int answerIndex)
    {
        if (currentState != GameState.Question) return;
        
        StopAllCoroutines();
        
        Question currentQuestion = selectedQuestions[currentQuestionIndex];
        bool isCorrect = answerIndex == currentQuestion.correctAnswer;
        
        if (isCorrect)
        {
            correctAnswers++;
        }
        
        ShowFeedback(isCorrect, currentQuestion);
    }
    
    void ShowFeedback(bool isCorrect, Question question)
    {
        currentState = GameState.Feedback;
        FeedbackScreen.Instance?.SetFeedback(isCorrect, question);
        ScreenManager.SetCallScreen("FeedbackScreen");
        StartCoroutine(FeedbackTimeout());
        ResetInactiveTimer();
    }
    
    IEnumerator FeedbackTimeout()
    {
        yield return new WaitForSeconds(gameConfig.timeoutSettings.feedbackTimeoutSeconds);
        
        if (currentState == GameState.Feedback)
        {
            NextQuestion();
        }
    }
    
    public void NextQuestion()
    {
        currentQuestionIndex++;
        ShowNextQuestion();
    }
    
    void ShowFinalScreen()
    {
        currentState = GameState.Final;
        FinalScreen.Instance?.SetResults(correctAnswers, selectedQuestions.Count);
        ScreenManager.SetCallScreen("FinalScreen");
        ResetInactiveTimer();
    }
    
    public void ReturnToIdle()
    {
        StopAllCoroutines();
        StartGame();
    }
    
    public Question GetCurrentQuestion()
    {
        if (currentQuestionIndex < selectedQuestions?.Count)
        {
            return selectedQuestions[currentQuestionIndex];
        }
        return null;
    }
    
    public int GetCurrentQuestionIndex()
    {
        return currentQuestionIndex;
    }
    
    public int GetTotalQuestions()
    {
        return selectedQuestions?.Count ?? 0;
    }
}
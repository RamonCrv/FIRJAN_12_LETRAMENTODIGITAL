using System.Collections;
using System.Net;
using System.Net.Http;
using TMPro;
using UnityEngine;
using RealGames;
using _4._NFC_Firjan.Scripts.NFC;
using _4._NFC_Firjan.Scripts.Server;

public class NFCGameManager : MonoBehaviour
{
    public static NFCGameManager Instance { get; private set; }
    
    [Header("Game Configuration")]
    public int gameId = 12; // ID único para o jogo de Letramento Digital
    
    [Header("Score Mapping - Alto Desempenho")]
    public int highPerformanceDigitalLiteracy = 8;
    public int highPerformanceAnalyticalThinking = 7;
    public int highPerformanceCuriosity = 6;
    
    [Header("Score Mapping - Médio Desempenho")]
    public int mediumPerformanceDigitalLiteracy = 6;
    public int mediumPerformanceAnalyticalThinking = 5;
    public int mediumPerformanceCuriosity = 4;
    
    [Header("Score Mapping - Baixo Desempenho")]
    public int lowPerformanceDigitalLiteracy = 4;
    public int lowPerformanceAnalyticalThinking = 3;
    public int lowPerformanceCuriosity = 2;
    
    [Header("UI References")]
    public GameObject nfcPanel;
    public TMP_Text nfcStatusText;
    public TMP_Text nfcInstructionText;
    
    [Header("Server Configuration")]
    public string serverIP = "192.168.0.185";
    public int serverPort = 8080;
    
    [Header("NFC Configuration")]
    public float nfcTimeoutSeconds = 30f;
    
    [Header("Component References")]
    public NFCReceiver nfcReceiver;
    public ServerComunication serverCommunication;
    
    // Private variables
    private bool isWaitingForNFC = false;
    private bool gameResultsSent = false;
    private string currentNFCId = "";
    private Coroutine nfcTimeoutCoroutine;
    
    // Properties
    public bool IsWaitingForNFC => isWaitingForNFC;
    public bool GameResultsSent => gameResultsSent;
    public string SystemStatus => GetSystemStatus();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeNFCSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Validar sistema após inicialização completa
        StartCoroutine(ValidateSystemAfterDelay());
    }
    
    IEnumerator ValidateSystemAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        
        if (!ValidateNFCSystem())
        {
            Debug.LogWarning("[NFCGameManager] Sistema NFC não está configurado corretamente. Tentando reinicializar...");
            ForceReinitializeNFC();
        }
    }
    
    void InitializeNFCSystem()
    {
        // Carregar configuração do servidor
        LoadServerConfiguration();
        
        // Configurar componentes NFC
        SetupNFCComponents();
        
        Debug.Log($"[NFCGameManager] Sistema inicializado - Game ID: {gameId}");
        Debug.Log($"[NFCGameManager] Servidor: http://{serverIP}:{serverPort}");
    }
    
    void LoadServerConfiguration()
    {
        var config = ServerConfig.LoadFromJSON();
        if (config?.server != null)
        {
            serverIP = config.server.ip;
            serverPort = config.server.port;
            Debug.Log($"[NFCGameManager] Configuração carregada do config.json");
        }
        else
        {
            Debug.LogWarning($"[NFCGameManager] Usando configuração padrão: {serverIP}:{serverPort}");
        }
        
        // Carregar timeout se disponível
        if (config?.timeoutSettings != null)
        {
            nfcTimeoutSeconds = config.timeoutSettings.generalTimeoutSeconds;
            Debug.Log($"[NFCGameManager] Timeout NFC: {nfcTimeoutSeconds}s");
        }
    }
    
    void SetupNFCComponents()
    {
        try
        {
            // Verificar se NFCReceiver está configurado (seguindo padrão Dilema do Bonde)
            if (nfcReceiver == null)
            {
                nfcReceiver = GetComponent<NFCReceiver>();
                if (nfcReceiver == null)
                {
                    nfcReceiver = gameObject.AddComponent<NFCReceiver>();
                    Debug.Log("[NFCGameManager] NFCReceiver adicionado automaticamente");
                }
            }
            
            // Verificar se ServerCommunication está configurado
            if (serverCommunication == null)
            {
                serverCommunication = GetComponent<ServerComunication>();
                if (serverCommunication == null)
                {
                    serverCommunication = gameObject.AddComponent<ServerComunication>();
                    Debug.Log("[NFCGameManager] ServerComunication adicionado automaticamente");
                }
            }
            
            // Configurar servidor (padrão Dilema do Bonde)
            serverCommunication.Ip = serverIP;
            serverCommunication.Port = serverPort;
            
            Debug.Log($"[NFCGameManager] Componentes NFC configurados");
            Debug.Log($"[NFCGameManager] NFCReceiver: {(nfcReceiver != null ? "OK" : "ERRO")}");
            Debug.Log($"[NFCGameManager] ServerComunication: {(serverCommunication != null ? "OK" : "ERRO")}");
            
            // Aguardar um frame para garantir inicialização completa
            StartCoroutine(DelayedNFCSetup());
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[NFCGameManager] Erro ao configurar componentes NFC: {ex.Message}");
        }
    }
    
    IEnumerator DelayedNFCSetup()
    {
        yield return new WaitForEndOfFrame();
        
        // Inscrever nos eventos NFC após a inicialização
        if (nfcReceiver != null)
        {
            try
            {
                nfcReceiver.OnNFCConnected.AddListener(OnNFCCardDetected);
                nfcReceiver.OnNFCDisconnected.AddListener(OnNFCCardRemoved);
                nfcReceiver.OnNFCReaderConnected.AddListener(OnNFCReaderConnected);
                nfcReceiver.OnNFCReaderDisconected.AddListener(OnNFCReaderDisconnected);
                
                Debug.Log("[NFCGameManager] Listeners NFC configurados com sucesso");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[NFCGameManager] Erro ao configurar listeners NFC: {ex.Message}");
            }
        }
    }
    
    public void StartNFCSession()
    {
        if (isWaitingForNFC)
        {
            Debug.LogWarning("[NFCGameManager] Sessão NFC já está ativa");
            return;
        }
        
        isWaitingForNFC = true;
        gameResultsSent = false;
        currentNFCId = "";
        
        Debug.Log("[NFCGameManager] Iniciando sessão NFC");
        
        // Mostrar UI do NFC
        ShowNFCUI();
        
        // Iniciar timeout
        StartNFCTimeout();
    }
    
    public void StopWaitingForNFC()
    {
        isWaitingForNFC = false;
        
        if (nfcTimeoutCoroutine != null)
        {
            StopCoroutine(nfcTimeoutCoroutine);
            nfcTimeoutCoroutine = null;
        }
        
        HideNFCUI();
        Debug.Log("[NFCGameManager] Parando sessão NFC");
    }
    
    void ShowNFCUI()
    {
        if (nfcPanel != null)
        {
            nfcPanel.SetActive(true);
        }
        
        UpdateNFCStatusText("Aproxime seu cartão NFC para salvar seus resultados");
        UpdateNFCInstructionText("Aguardando cartão...");
    }
    
    void HideNFCUI()
    {
        if (nfcPanel != null)
        {
            nfcPanel.SetActive(false);
        }
    }
    
    void UpdateNFCStatusText(string message)
    {
        if (nfcStatusText != null)
        {
            nfcStatusText.text = message;
        }
    }
    
    void UpdateNFCInstructionText(string message)
    {
        if (nfcInstructionText != null)
        {
            nfcInstructionText.text = message;
        }
    }
    
    void StartNFCTimeout()
    {
        if (nfcTimeoutCoroutine != null)
        {
            StopCoroutine(nfcTimeoutCoroutine);
        }
        
        nfcTimeoutCoroutine = StartCoroutine(NFCTimeoutCoroutine());
    }
    
    IEnumerator NFCTimeoutCoroutine()
    {
        yield return new WaitForSeconds(nfcTimeoutSeconds);
        
        if (isWaitingForNFC)
        {
            Debug.Log("[NFCGameManager] Timeout do NFC atingido");
            UpdateNFCStatusText("Timeout atingido. Retornando ao menu...");
            yield return new WaitForSeconds(2f);
            StopWaitingForNFC();
            ReturnToMenu();
        }
    }
    
    void OnNFCCardDetected(string nfcId, string readerName)
    {
        if (!isWaitingForNFC) return;
        
        currentNFCId = nfcId;
        Debug.Log($"[NFCGameManager] Cartão NFC detectado: {nfcId} no leitor: {readerName}");
        
        UpdateNFCStatusText("Cartão detectado! Enviando dados...");
        UpdateNFCInstructionText("Processando...");
        
        // Enviar dados do jogo
        StartCoroutine(SendGameDataCoroutine());
    }
    
    void OnNFCCardRemoved()
    {
        Debug.Log("[NFCGameManager] Cartão NFC removido");
        
        if (gameResultsSent)
        {
            UpdateNFCStatusText("Dados enviados com sucesso!");
            StartCoroutine(DelayedReturnToMenu(3f));
        }
    }
    
    void OnNFCReaderConnected(string readerName)
    {
        Debug.Log($"[NFCGameManager] Leitor NFC conectado: {readerName}");
    }
    
    void OnNFCReaderDisconnected()
    {
        Debug.Log("[NFCGameManager] Leitor NFC desconectado");
    }
    
    IEnumerator SendGameDataCoroutine()
    {
        PlayerScore currentScore = GetCurrentPlayerScore();
        GameModel gameModel = CreateGameModelFromScore(currentScore);
        
        Debug.Log($"[NFCGameManager] Enviando dados: {gameModel}");
        
        var sendTask = serverCommunication.UpdateNfcInfoFromGame(gameModel);
        
        // Usar o handler de tarefas assíncronas
        yield return StartCoroutine(AsyncTaskHandler.HandleTask(
            sendTask,
            onSuccess: (statusCode) => {
                Debug.Log($"[NFCGameManager] Resposta do servidor: {statusCode}");
                
                if (statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Created)
                {
                    gameResultsSent = true;
                    UpdateNFCStatusText("Dados enviados com sucesso!");
                    UpdateNFCInstructionText("Você pode remover o cartão");
                }
                else
                {
                    UpdateNFCStatusText("Erro ao enviar dados. Tente novamente.");
                    UpdateNFCInstructionText("Código de erro: " + statusCode);
                    StartCoroutine(DelayedAction(3f, () => {
                        StopWaitingForNFC();
                        ReturnToMenu();
                    }));
                }
            },
            onError: (exception) => {
                Debug.LogError($"[NFCGameManager] Erro ao enviar dados: {exception.Message}");
                UpdateNFCStatusText("Erro de conexão. Verifique a rede.");
                UpdateNFCInstructionText("Tentando novamente...");
                StartCoroutine(DelayedAction(3f, () => {
                    StopWaitingForNFC();
                    ReturnToMenu();
                }));
            }
        ));
    }
    
    public virtual PlayerScore GetCurrentPlayerScore()
    {
        // Obter pontuação do DigitalLiteracyGameController
        var gameController = DigitalLiteracyGameController.Instance;
        if (gameController != null)
        {
            return CalculateScoreFromGame(gameController.correctAnswers, gameController.GetTotalQuestions());
        }
        
        return CreateDefaultScore();
    }
    
    PlayerScore CalculateScoreFromGame(int correctAnswers, int totalQuestions)
    {
        if (totalQuestions == 0) return CreateDefaultScore();
        
        float percentage = (float)correctAnswers / totalQuestions;
        
        if (percentage >= 0.8f) // 80% ou mais - Alto desempenho
        {
            return new PlayerScore(
                highPerformanceDigitalLiteracy,
                highPerformanceAnalyticalThinking,
                highPerformanceCuriosity
            );
        }
        else if (percentage >= 0.5f) // 50% a 79% - Médio desempenho
        {
            return new PlayerScore(
                mediumPerformanceDigitalLiteracy,
                mediumPerformanceAnalyticalThinking,
                mediumPerformanceCuriosity
            );
        }
        else // Menos de 50% - Baixo desempenho
        {
            return new PlayerScore(
                lowPerformanceDigitalLiteracy,
                lowPerformanceAnalyticalThinking,
                lowPerformanceCuriosity
            );
        }
    }
    
    PlayerScore CreateDefaultScore()
    {
        return new PlayerScore(
            mediumPerformanceDigitalLiteracy,
            mediumPerformanceAnalyticalThinking,
            mediumPerformanceCuriosity
        );
    }
    
    protected virtual GameModel CreateGameModelFromScore(PlayerScore score)
    {
        return new GameModel
        {
            gameId = gameId,
            skill1 = score.letramentoDigital,
            skill2 = score.pensamentoAnalitico,
            skill3 = score.curiosidade,
            nfcId = currentNFCId
        };
    }
    
    IEnumerator DelayedReturnToMenu(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopWaitingForNFC();
        ReturnToMenu();
    }
    
    IEnumerator DelayedAction(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
    
    void ReturnToMenu()
    {
        DigitalLiteracyGameController.Instance?.ReturnToIdle();
    }
    
    public void ResetForNewGame()
    {
        StopWaitingForNFC();
        gameResultsSent = false;
        currentNFCId = "";
    }
    
    public bool ValidateNFCSystem()
    {
        bool isValid = true;
        
        if (nfcReceiver == null)
        {
            Debug.LogError("[NFCGameManager] NFCReceiver não encontrado");
            isValid = false;
        }
        
        if (serverCommunication == null)
        {
            Debug.LogError("[NFCGameManager] ServerCommunication não encontrado");
            isValid = false;
        }
        
        if (FindObjectOfType<_4._NFC_Firjan.Scripts.UnityMainThreadDispatcher>() == null)
        {
            Debug.LogError("[NFCGameManager] UnityMainThreadDispatcher não encontrado na cena");
            isValid = false;
        }
        
        if (isValid)
        {
            Debug.Log("[NFCGameManager] Sistema NFC validado com sucesso");
        }
        
        return isValid;
    }
    
    public void ForceReinitializeNFC()
    {
        Debug.Log("[NFCGameManager] Reinicializando sistema NFC...");
        
        // Limpar listeners primeiro
        CleanupNFCListeners();
        
        // Reinicializar componentes
        SetupNFCComponents();
    }
    
    string GetSystemStatus()
    {
        if (gameResultsSent) return "Dados enviados";
        if (isWaitingForNFC) return "Aguardando NFC";
        return "Pronto";
    }
    
    void OnDestroy()
    {
        // Parar corrotina de timeout primeiro
        if (nfcTimeoutCoroutine != null)
        {
            StopCoroutine(nfcTimeoutCoroutine);
            nfcTimeoutCoroutine = null;
        }
        
        // Limpar listeners NFC com verificações de segurança
        CleanupNFCListeners();
        
        Debug.Log("[NFCGameManager] Sistema NFC destruído");
    }
    
    void CleanupNFCListeners()
    {
        if (nfcReceiver != null)
        {
            try
            {
                // Verificar se os UnityEvents não são nulos antes de remover listeners
                if (nfcReceiver.OnNFCConnected != null)
                    nfcReceiver.OnNFCConnected.RemoveListener(OnNFCCardDetected);
                
                if (nfcReceiver.OnNFCDisconnected != null)
                    nfcReceiver.OnNFCDisconnected.RemoveListener(OnNFCCardRemoved);
                
                if (nfcReceiver.OnNFCReaderConnected != null)
                    nfcReceiver.OnNFCReaderConnected.RemoveListener(OnNFCReaderConnected);
                
                if (nfcReceiver.OnNFCReaderDisconected != null)
                    nfcReceiver.OnNFCReaderDisconected.RemoveListener(OnNFCReaderDisconnected);
                
                Debug.Log("[NFCGameManager] Listeners NFC removidos com segurança");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[NFCGameManager] Erro ao limpar listeners NFC: {ex.Message}");
            }
        }
    }
}
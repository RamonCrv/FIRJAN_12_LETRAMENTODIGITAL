using System.Collections;
using UnityEngine;
using RealGames;
using _4._NFC_Firjan.Scripts.Server;

/// <summary>
/// Script de validação para testar todas as funcionalidades do sistema NFC
/// Execute este script para validar a integração completa
/// </summary>
public class NFCValidationTest : MonoBehaviour
{
    [Header("Test Configuration")]
    public bool runTestsOnStart = false;
    public string testCardId = "TEST_CARD_001";
    
    [Header("Test Results")]
    public bool allTestsPassed = false;
    public string lastTestResult = "";
    
    void Start()
    {
        if (runTestsOnStart)
        {
            StartCoroutine(RunAllTests());
        }
    }
    
    [ContextMenu("Run All Tests")]
    public void RunAllTestsManual()
    {
        StartCoroutine(RunAllTests());
    }
    
    IEnumerator RunAllTests()
    {
        Debug.Log("=== INICIANDO TESTES DE VALIDAÇÃO NFC ===");
        
        bool configTest = TestConfiguration();
        yield return new WaitForSeconds(0.5f);
        
        bool nfcManagerTest = TestNFCManager();
        yield return new WaitForSeconds(0.5f);
        
        bool gameControllerTest = TestGameController();
        yield return new WaitForSeconds(0.5f);
        
        bool serverConfigTest = TestServerConfiguration();
        yield return new WaitForSeconds(0.5f);
        
        bool scoreCalculationTest = TestScoreCalculation();
        yield return new WaitForSeconds(0.5f);
        
        bool modelTest = TestDataModels();
        yield return new WaitForSeconds(0.5f);
        
        // Resultado final
        allTestsPassed = configTest && nfcManagerTest && gameControllerTest && 
                        serverConfigTest && scoreCalculationTest && modelTest;
        
        lastTestResult = allTestsPassed ? "TODOS OS TESTES PASSARAM!" : "ALGUNS TESTES FALHARAM!";
        
        Debug.Log($"=== RESULTADO FINAL: {lastTestResult} ===");
        Debug.Log($"Configuração: {(configTest ? "✓" : "✗")}");
        Debug.Log($"NFC Manager: {(nfcManagerTest ? "✓" : "✗")}");
        Debug.Log($"Game Controller: {(gameControllerTest ? "✓" : "✗")}");
        Debug.Log($"Server Config: {(serverConfigTest ? "✓" : "✗")}");
        Debug.Log($"Score Calculation: {(scoreCalculationTest ? "✓" : "✗")}");
        Debug.Log($"Data Models: {(modelTest ? "✓" : "✗")}");
    }
    
    bool TestConfiguration()
    {
        Debug.Log("[TEST] Testando configuração...");
        
        try
        {
            var config = ServerConfig.LoadFromJSON();
            
            if (config == null)
            {
                Debug.LogError("[TEST] Falha: Configuração não pôde ser carregada");
                return false;
            }
            
            if (config.server == null)
            {
                Debug.LogError("[TEST] Falha: Configuração do servidor está nula");
                return false;
            }
            
            if (string.IsNullOrEmpty(config.server.ip))
            {
                Debug.LogError("[TEST] Falha: IP do servidor não configurado");
                return false;
            }
            
            if (config.server.port <= 0)
            {
                Debug.LogError("[TEST] Falha: Porta do servidor inválida");
                return false;
            }
            
            Debug.Log($"[TEST] ✓ Configuração válida: {config.server.ip}:{config.server.port}");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[TEST] Falha na configuração: {ex.Message}");
            return false;
        }
    }
    
    bool TestNFCManager()
    {
        Debug.Log("[TEST] Testando NFCGameManager...");
        
        if (NFCGameManager.Instance == null)
        {
            Debug.LogError("[TEST] Falha: NFCGameManager não encontrado na cena");
            return false;
        }
        
        var nfcManager = NFCGameManager.Instance;
        
        if (nfcManager.gameId <= 0)
        {
            Debug.LogError("[TEST] Falha: Game ID não configurado");
            return false;
        }
        
        if (nfcManager.nfcTimeoutSeconds <= 0)
        {
            Debug.LogError("[TEST] Falha: Timeout do NFC não configurado");
            return false;
        }
        
        Debug.Log($"[TEST] ✓ NFCGameManager válido: Game ID {nfcManager.gameId}");
        return true;
    }
    
    bool TestGameController()
    {
        Debug.Log("[TEST] Testando DigitalLiteracyGameController...");
        
        if (DigitalLiteracyGameController.Instance == null)
        {
            Debug.LogError("[TEST] Falha: DigitalLiteracyGameController não encontrado");
            return false;
        }
        
        var gameController = DigitalLiteracyGameController.Instance;
        
        if (gameController.gameConfig == null)
        {
            Debug.LogError("[TEST] Falha: GameConfig não configurado");
            return false;
        }
        
        Debug.Log("[TEST] ✓ DigitalLiteracyGameController válido");
        return true;
    }
    
    bool TestServerConfiguration()
    {
        Debug.Log("[TEST] Testando configuração do servidor...");
        
        if (NFCGameManager.Instance == null) return false;
        
        var nfcManager = NFCGameManager.Instance;
        
        if (string.IsNullOrEmpty(nfcManager.serverIP))
        {
            Debug.LogError("[TEST] Falha: IP do servidor não configurado no NFCManager");
            return false;
        }
        
        if (nfcManager.serverPort <= 0)
        {
            Debug.LogError("[TEST] Falha: Porta do servidor não configurada no NFCManager");
            return false;
        }
        
        Debug.Log($"[TEST] ✓ Servidor configurado: {nfcManager.serverIP}:{nfcManager.serverPort}");
        return true;
    }
    
    bool TestScoreCalculation()
    {
        Debug.Log("[TEST] Testando cálculo de pontuação...");
        
        if (NFCGameManager.Instance == null) return false;
        
        var nfcManager = NFCGameManager.Instance;
        
        // Testar diferentes cenários de pontuação
        var highScore = nfcManager.GetCurrentPlayerScore();
        
        if (highScore == null)
        {
            Debug.LogError("[TEST] Falha: Não foi possível calcular pontuação");
            return false;
        }
        
        Debug.Log($"[TEST] ✓ Cálculo de pontuação funcional: {highScore}");
        return true;
    }
    
    bool TestDataModels()
    {
        Debug.Log("[TEST] Testando modelos de dados...");
        
        try
        {
            // Testar PlayerScore
            var playerScore = new PlayerScore(8, 7, 6);
            if (playerScore.letramentoDigital != 8) return false;
            
            // Testar GameModel
            var gameModel = new GameModel
            {
                nfcId = testCardId,
                gameId = 12,
                skill1 = 8,
                skill2 = 7,
                skill3 = 6
            };
            
            string jsonString = gameModel.ToString();
            if (string.IsNullOrEmpty(jsonString))
            {
                Debug.LogError("[TEST] Falha: Serialização do GameModel");
                return false;
            }
            
            Debug.Log($"[TEST] ✓ Modelos de dados válidos");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[TEST] Falha nos modelos de dados: {ex.Message}");
            return false;
        }
    }
    
    [ContextMenu("Test Score Mapping")]
    public void TestScoreMapping()
    {
        Debug.Log("=== TESTE DE MAPEAMENTO DE PONTUAÇÕES ===");
        
        // Simular diferentes percentuais de acerto
        int[] correctAnswers = { 5, 4, 3, 2, 1, 0 };
        int totalQuestions = 5;
        
        foreach (int correct in correctAnswers)
        {
            float percentage = (float)correct / totalQuestions * 100f;
            
            PlayerScore score;
            if (percentage >= 80f)
            {
                score = new PlayerScore(8, 7, 6);
                Debug.Log($"Alto desempenho ({percentage:F0}%): {score}");
            }
            else if (percentage >= 50f)
            {
                score = new PlayerScore(6, 5, 4);
                Debug.Log($"Médio desempenho ({percentage:F0}%): {score}");
            }
            else
            {
                score = new PlayerScore(4, 3, 2);
                Debug.Log($"Baixo desempenho ({percentage:F0}%): {score}");
            }
        }
    }
}
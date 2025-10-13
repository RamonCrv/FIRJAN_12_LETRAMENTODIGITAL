using System.Collections;
using System.Net;
using System.Net.Http;
using UnityEngine;
using RealGames;
using _4._NFC_Firjan.Scripts.Server;

public class NFCDebugTester : MonoBehaviour
{
    [Header("Debug Configuration")]
    public string debugCardId = "DEBUG_CARD_001";
    public bool enableInBuild = false;
    
    [Header("Debug Display")]
    public string currentStatus = "Pronto";
    
    void Update()
    {
        if (!ShouldEnableDebug()) return;
        
        if (Input.GetKeyDown(KeyCode.F1)) SimulateNFCConnection();
        if (Input.GetKeyDown(KeyCode.F3)) StartCoroutine(TestServerConnection());
        if (Input.GetKeyDown(KeyCode.F4)) ActivateNFCSession();
        if (Input.GetKeyDown(KeyCode.F5)) ResetNFCSystem();
        if (Input.GetKeyDown(KeyCode.F6)) ShowSystemInfo();
    }
    
    bool ShouldEnableDebug()
    {
        return Application.isEditor || enableInBuild;
    }
    
    void SimulateNFCConnection()
    {
        Debug.Log("[NFCDebugTester] Simulando conexão NFC com ID: " + debugCardId);
        currentStatus = "Simulando NFC";
        
        if (NFCGameManager.Instance != null && NFCGameManager.Instance.IsWaitingForNFC)
        {
            // Simular detecção de cartão
            var nfcReceiver = NFCGameManager.Instance.GetComponent<_4._NFC_Firjan.Scripts.NFC.NFCReceiver>();
            if (nfcReceiver != null)
            {
                nfcReceiver.OnNFCConnected.Invoke(debugCardId, "DEBUG_READER");
                
                // Simular remoção após 2 segundos
                StartCoroutine(SimulateNFCRemoval());
            }
        }
        else
        {
            Debug.LogWarning("[NFCDebugTester] Sistema NFC não está aguardando cartões");
        }
    }
    
    IEnumerator SimulateNFCRemoval()
    {
        yield return new WaitForSeconds(2f);
        
        var nfcReceiver = NFCGameManager.Instance?.GetComponent<_4._NFC_Firjan.Scripts.NFC.NFCReceiver>();
        if (nfcReceiver != null)
        {
            nfcReceiver.OnNFCDisconnected.Invoke();
            Debug.Log("[NFCDebugTester] Simulando remoção do cartão NFC");
        }
    }
    
    IEnumerator TestServerConnection()
    {
        Debug.Log("[NFCDebugTester] Testando conexão com servidor...");
        currentStatus = "Testando servidor";
        
        var config = ServerConfig.LoadFromJSON();
        string testUrl = $"http://{config.server.ip}:{config.server.port}/users";
        
        using (var client = new HttpClient())
        {
            var task = client.GetAsync(testUrl);
            
            yield return StartCoroutine(AsyncTaskHandler.HandleTask(
                task,
                onSuccess: (response) => {
                    Debug.Log($"[NFCDebugTester] Resposta do servidor: {response.StatusCode}");
                    currentStatus = $"Servidor: {response.StatusCode}";
                },
                onError: (exception) => {
                    Debug.LogError($"[NFCDebugTester] Erro de conexão: {exception.Message}");
                    currentStatus = "Erro de conexão";
                }
            ));
        }
    }
    
    void ActivateNFCSession()
    {
        Debug.Log("[NFCDebugTester] Ativando sessão NFC manualmente");
        currentStatus = "Ativando NFC";
        
        if (NFCGameManager.Instance != null)
        {
            NFCGameManager.Instance.StartNFCSession();
        }
        else
        {
            Debug.LogError("[NFCDebugTester] NFCGameManager não encontrado");
        }
    }
    
    void ResetNFCSystem()
    {
        Debug.Log("[NFCDebugTester] Resetando sistema NFC");
        currentStatus = "Resetando";
        
        if (NFCGameManager.Instance != null)
        {
            NFCGameManager.Instance.ResetForNewGame();
        }
    }
    
    void ShowSystemInfo()
    {
        Debug.Log("=== INFORMAÇÕES DO SISTEMA NFC ===");
        
        if (NFCGameManager.Instance != null)
        {
            var nfc = NFCGameManager.Instance;
            Debug.Log($"Status: {nfc.SystemStatus}");
            Debug.Log($"Aguardando NFC: {nfc.IsWaitingForNFC}");
            Debug.Log($"Dados enviados: {nfc.GameResultsSent}");
            Debug.Log($"Game ID: {nfc.gameId}");
            Debug.Log($"Servidor: {nfc.serverIP}:{nfc.serverPort}");
        }
        else
        {
            Debug.LogError("NFCGameManager não encontrado");
        }
        
        // Informações do jogo
        if (DigitalLiteracyGameController.Instance != null)
        {
            var game = DigitalLiteracyGameController.Instance;
            Debug.Log($"Estado do jogo: {game.currentState}");
            Debug.Log($"Respostas corretas: {game.correctAnswers}");
            Debug.Log($"Total de perguntas: {game.GetTotalQuestions()}");
        }
    }
    
    void OnGUI()
    {
        if (!Application.isEditor) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 350, 300));
        GUILayout.Label("=== NFC DEBUG TESTER ===");
        
        // Status atual
        GUILayout.Label($"Status: {currentStatus}");
        
        // Informações do sistema
        if (NFCGameManager.Instance != null)
        {
            var nfc = NFCGameManager.Instance;
            GUILayout.Label($"Sistema: {nfc.SystemStatus}");
            GUILayout.Label($"Aguardando: {nfc.IsWaitingForNFC}");
            GUILayout.Label($"Enviado: {nfc.GameResultsSent}");
            GUILayout.Label($"Servidor: {nfc.serverIP}:{nfc.serverPort}");
        }
        
        // Informações do jogo
        if (DigitalLiteracyGameController.Instance != null)
        {
            var game = DigitalLiteracyGameController.Instance;
            GUILayout.Label($"Jogo: {game.currentState}");
            GUILayout.Label($"Pontuação: {game.correctAnswers}/{game.GetTotalQuestions()}");
        }
        
        // Controles
        GUILayout.Space(10);
        GUILayout.Label("--- CONTROLES ---");
        if (GUILayout.Button("F1 - Simular NFC")) SimulateNFCConnection();
        if (GUILayout.Button("F3 - Testar Servidor")) StartCoroutine(TestServerConnection());
        if (GUILayout.Button("F4 - Ativar NFC")) ActivateNFCSession();
        if (GUILayout.Button("F5 - Reset Sistema")) ResetNFCSystem();
        if (GUILayout.Button("F6 - Info Sistema")) ShowSystemInfo();
        
        // Input de debug
        GUILayout.Space(10);
        GUILayout.Label("Debug Card ID:");
        debugCardId = GUILayout.TextField(debugCardId);
        
        GUILayout.EndArea();
    }
}
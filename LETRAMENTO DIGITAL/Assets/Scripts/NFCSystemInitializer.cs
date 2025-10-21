using UnityEngine;

/// <summary>
/// Script de inicialização automática do sistema NFC
/// Adicione este script a um GameObject na cena para configuração automática
/// </summary>
public class NFCSystemInitializer : MonoBehaviour
{
    [Header("Auto Setup")]
    [Tooltip("Se verdadeiro, criará automaticamente o NFCGameManager se não existir")]
    public bool autoCreateNFCManager = true;
    
    [Tooltip("Se verdadeiro, criará automaticamente o Debug Tester")]
    public bool includeDebugTester = true;
    
    [Header("NFC Manager Configuration")]
    [Tooltip("Game ID para este jogo específico (12 = Letramento Digital)")]
    public int gameId = 12;
    
    [Tooltip("Timeout em segundos para aguardar cartão NFC")]
    public float nfcTimeoutSeconds = 30f;
    
    [Header("Score Configuration")]
    [Tooltip("Pontuações para alto desempenho (≥80% acertos)")]
    public NFCScoreConfig highPerformanceScores = new NFCScoreConfig(8, 7, 6);
    
    [Tooltip("Pontuações para médio desempenho (50-79% acertos)")]
    public NFCScoreConfig mediumPerformanceScores = new NFCScoreConfig(6, 5, 4);
    
    [Tooltip("Pontuações para baixo desempenho (<50% acertos)")]
    public NFCScoreConfig lowPerformanceScores = new NFCScoreConfig(4, 3, 2);
    
    void Awake()
    {
        InitializeNFCSystem();
    }
    
    void InitializeNFCSystem()
    {
        if (autoCreateNFCManager)
        {
            CreateNFCManagerIfNeeded();
        }
        
        if (includeDebugTester && Application.isEditor)
        {
            CreateDebugTesterIfNeeded();
        }
    }
    
    void CreateNFCManagerIfNeeded()
    {
        if (NFCGameManager.Instance == null)
        {
            GameObject nfcManagerObj = new GameObject("NFCGameManager");
            var nfcManager = nfcManagerObj.AddComponent<NFCGameManager>();
            
            // Configurar parâmetros
            nfcManager.gameId = gameId;
            nfcManager.nfcTimeoutSeconds = nfcTimeoutSeconds;
            
            // Configurar pontuações
            nfcManager.highPerformanceLetramentoTecnologico = highPerformanceScores.digitalLiteracy;
            nfcManager.highPerformanceIAEBigData = highPerformanceScores.analyticalThinking;
            nfcManager.highPerformancePensamentoCriativo = highPerformanceScores.curiosity;
            
            nfcManager.mediumPerformanceLetramentoTecnologico = mediumPerformanceScores.digitalLiteracy;
            nfcManager.mediumPerformanceIAEBigData = mediumPerformanceScores.analyticalThinking;
            nfcManager.mediumPerformancePensamentoCriativo = mediumPerformanceScores.curiosity;
            
            nfcManager.lowPerformanceLetramentoTecnologico = lowPerformanceScores.digitalLiteracy;
            nfcManager.lowPerformanceIAEBigData = lowPerformanceScores.analyticalThinking;
            nfcManager.lowPerformancePensamentoCriativo = lowPerformanceScores.curiosity;
            
            Debug.Log($"[NFCSystemInitializer] NFCGameManager criado automaticamente com Game ID: {gameId}");
        }
        else
        {
            Debug.Log("[NFCSystemInitializer] NFCGameManager já existe na cena");
        }
    }
    
    void CreateDebugTesterIfNeeded()
    {
        if (FindObjectOfType<NFCDebugTester>() == null)
        {
            GameObject debugTesterObj = new GameObject("NFCDebugTester");
            debugTesterObj.AddComponent<NFCDebugTester>();
            
            Debug.Log("[NFCSystemInitializer] NFCDebugTester criado automaticamente");
        }
    }
}

[System.Serializable]
public class NFCScoreConfig
{
    public int digitalLiteracy;
    public int analyticalThinking;
    public int curiosity;
    
    public NFCScoreConfig(int digital, int analytical, int curiosityVal)
    {
        digitalLiteracy = digital;
        analyticalThinking = analytical;
        curiosity = curiosityVal;
    }
}
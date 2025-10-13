using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Exemplo de como usar o sistema NFC no seu jogo
/// Este script demonstra como integrar manualmente o NFCGameManager
/// </summary>
public class NFCExampleUsage : MonoBehaviour
{
    [Header("UI References")]
    public Button activateNFCButton;
    public Button deactivateNFCButton;
    public TMP_Text statusText;
    
    [Header("NFC Configuration")]
    public GameObject nfcPanel;
    public TMP_Text nfcStatusText;
    public TMP_Text nfcInstructionText;
    
    void Start()
    {
        SetupButtons();
        UpdateStatusDisplay();
    }
    
    void SetupButtons()
    {
        if (activateNFCButton != null)
        {
            activateNFCButton.onClick.AddListener(ActivateNFC);
        }
        
        if (deactivateNFCButton != null)
        {
            deactivateNFCButton.onClick.AddListener(DeactivateNFC);
        }
    }
    
    public void ActivateNFC()
    {
        if (NFCGameManager.Instance == null)
        {
            Debug.LogError("[NFCExampleUsage] NFCGameManager não encontrado na cena!");
            return;
        }
        
        // Configurar referências UI
        ConfigureNFCUI();
        
        // Ativar sessão NFC
        NFCGameManager.Instance.StartNFCSession();
        
        UpdateStatusDisplay();
    }
    
    public void DeactivateNFC()
    {
        if (NFCGameManager.Instance == null) return;
        
        NFCGameManager.Instance.StopWaitingForNFC();
        UpdateStatusDisplay();
    }
    
    void ConfigureNFCUI()
    {
        var nfcManager = NFCGameManager.Instance;
        
        if (nfcPanel != null)
            nfcManager.nfcPanel = nfcPanel;
            
        if (nfcStatusText != null)
            nfcManager.nfcStatusText = nfcStatusText;
            
        if (nfcInstructionText != null)
            nfcManager.nfcInstructionText = nfcInstructionText;
    }
    
    void UpdateStatusDisplay()
    {
        if (statusText == null) return;
        
        if (NFCGameManager.Instance != null)
        {
            statusText.text = $"Sistema NFC: {NFCGameManager.Instance.SystemStatus}";
        }
        else
        {
            statusText.text = "Sistema NFC: Não Disponível";
        }
    }
    
    void Update()
    {
        // Atualizar display de status constantemente
        UpdateStatusDisplay();
        
        // Atualizar estado dos botões
        if (activateNFCButton != null)
        {
            activateNFCButton.interactable = NFCGameManager.Instance != null && !NFCGameManager.Instance.IsWaitingForNFC;
        }
        
        if (deactivateNFCButton != null)
        {
            deactivateNFCButton.interactable = NFCGameManager.Instance != null && NFCGameManager.Instance.IsWaitingForNFC;
        }
    }
}
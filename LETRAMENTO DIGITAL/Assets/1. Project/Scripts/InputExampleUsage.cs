using UnityEngine;

/// <summary>
/// Exemplo de como usar o InputManager e reconfigurar inputs durante o runtime
/// </summary>
public class InputExampleUsage : MonoBehaviour
{
    void Start()
    {
        // Exemplo de como ouvir inputs específicos
        InputManager.OnInputTriggered += HandleInput;
        
        // Exemplo de remapeamento durante o runtime
        ExampleRemapping();
    }
    
    void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.OnInputTriggered -= HandleInput;
        }
    }
    
    void HandleInput(int inputId)
    {
        Debug.Log($"Input ID {inputId} foi pressionado!");
        
        switch (inputId)
        {
            case 0:
                Debug.Log("Ação para ID 0 executada");
                break;
            case 1:
                Debug.Log("Ação para ID 1 executada");
                break;
            case -1:
                Debug.Log("Ação para backspace executada");
                break;
        }
    }
    
    void ExampleRemapping()
    {
        // Exemplo: trocar a tecla que executa a ação do ID 0
        // Antes: tecla "0" executava ID 0
        // Depois: tecla "Enter" executará ID 0
        
        if (InputManager.Instance != null)
        {
            // InputManager.Instance.RemapInput(0, KeyCode.Return);
            // Debug.Log("ID 0 foi remapeado para a tecla Enter");
        }
    }
    
    void Update()
    {
        // Exemplo de verificação direta de input
        if (InputManager.Instance != null)
        {
            // Verificar se o input ID 0 está sendo pressionado
            if (InputManager.Instance.IsInputDown(0))
            {
                Debug.Log("Input ID 0 detectado diretamente");
            }
        }
    }
}
# Sistema de Gerenciamento de Inputs

## Visão Geral

Este sistema centraliza o gerenciamento de inputs do projeto, permitindo que você facilmente remapeie teclas e gerencie inputs através de IDs em vez de KeyCodes diretos. Agora inclui funcionalidade de **Reset Global** que permite voltar à tela inicial a qualquer momento durante o jogo.

## Componentes Principais

### 1. InputManager.cs
- **Singleton** que gerencia todos os inputs do jogo
- Mapeia **Input IDs** para **KeyCodes**
- Dispara eventos quando inputs são detectados
- Permite remapeamento dinâmico durante runtime
- **NOVO:** Sistema de Reset Global integrado

### 2. InputConfiguration.cs (Opcional)
- **ScriptableObject** para configurar inputs via Inspector
- Permite definir mapeamentos padrão
- Documentação de cada input

### 3. Classes Adaptadas
- `IdleScreen.cs` - Usa IDs 0 (português) e -1 (inglês)
- `QuestionScreen.cs` - Usa IDs 0-9 para respostas + popup de confirmação para reset
- `FinalScreen.cs` - Usa ID 0 para reiniciar + reset direto
- `FeedbackScreen.cs` - Suporte a popup de confirmação para reset
- `ConfirmationPopUp.cs` - **NOVO:** Gerencia popup de confirmação de saída

## 🔄 Sistema de Reset Global com Confirmação

### Funcionalidade por Tela

#### **Tela Inicial (IdleScreen)**
- Inputs **`0`** e **`Backspace`** funcionam normalmente para seleção de idioma
- Sem popup de confirmação

#### **Telas de Pergunta e Feedback**
- Pressionar **`0`** ou **`Backspace`** exibe popup de confirmação
- **Popup mostra**: "Deseja voltar ao início?"
- **Para confirmar**: Pressione `0` ou `1`
- **Para cancelar**: Pressione qualquer outra tecla
- **Resultado da confirmação**: Volta para tela inicial
- **Resultado do cancelamento**: Continua o jogo normalmente

#### **Tela Final**
- Pressionar **`0`** ou **`Backspace`** reseta **imediatamente**
- **Sem popup de confirmação** (reset direto)

### Configuração do Popup
```csharp
// O popup deve estar na hierarquia: /Canvas/ConfirmationPopUp
// Componentes necessários:
// - CanvasGroup (para controle de visibilidade)
// - TextMeshProUGUI (para o texto de confirmação)
// - LocalizedTextComponent (opcional, para localização)
```

### Configuração Automática
```csharp
// Adicione o ConfirmationPopUpSetup ao GameObject do popup
// Ele configurará automaticamente os componentes necessários
```

## Mapeamento Atual

| Input ID | Tecla Padrão | Uso |
|----------|--------------|-----|
| 0 | Alpha0 | Português / Opção 0 / Reiniciar |
| 1 | Alpha1 | Opção 1 |
| 2 | Alpha2 | Opção 2 |
| 3 | Alpha3 | Opção 3 |
| 4 | Alpha4 | Opção 4 |
| 5 | Alpha5 | Opção 5 |
| 6 | Alpha6 | Opção 6 |
| 7 | Alpha7 | Opção 7 |
| 8 | Alpha8 | Opção 8 |
| 9 | Alpha9 | Opção 9 |
| -1 | Backspace | Inglês / Voltar |

## Como Usar

### 1. Setup Inicial
```csharp
// O InputManager deve estar em uma cena como singleton
// Adicione o componente InputManager em um GameObject na primeira cena
```

### 2. Ouvir Inputs por Eventos
```csharp
public class MinhaClasse : MonoBehaviour
{
    void Start()
    {
        InputManager.OnInputTriggered += HandleInput;
    }
    
    void OnDestroy()
    {
        InputManager.OnInputTriggered -= HandleInput;
    }
    
    void HandleInput(int inputId)
    {
        switch (inputId)
        {
            case 0:
                Debug.Log("Input ID 0 pressionado!");
                break;
            case 1:
                Debug.Log("Input ID 1 pressionado!");
                break;
            case -1:
                Debug.Log("Backspace pressionado!");
                break;
        }
    }
}
```

### 3. Verificação Direta de Input
```csharp
void Update()
{
    // Verificar se está pressionado agora
    if (InputManager.Instance.IsInputDown(0))
    {
        Debug.Log("ID 0 foi pressionado neste frame");
    }
    
    // Verificar se está sendo mantido pressionado
    if (InputManager.Instance.IsInputPressed(1))
    {
        Debug.Log("ID 1 está sendo mantido pressionado");
    }
    
    // Verificar se foi solto
    if (InputManager.Instance.IsInputUp(2))
    {
        Debug.Log("ID 2 foi solto neste frame");
    }
}
```

### 4. Remapeamento Durante Runtime
```csharp
// Trocar a tecla do ID 0 de Alpha0 para Return
InputManager.Instance.RemapInput(0, KeyCode.Return);

// Adicionar um novo input
InputManager.Instance.RemapInput(10, KeyCode.Space);
```

### 5. Obter Informações dos Mapeamentos
```csharp
// Obter a tecla atual de um ID
KeyCode teclaAtual = InputManager.Instance.GetKeyForInputId(0);

// Obter todos os mapeamentos
var todosMapeamentos = InputManager.Instance.GetAllMappings();
```

## Vantagens do Sistema

1. **Centralização**: Todos os inputs em um lugar
2. **Flexibilidade**: Fácil remapeamento de teclas
3. **Manutenibilidade**: Mudanças não requerem editar múltiplos scripts
4. **Reutilização**: IDs podem ser reutilizados em diferentes contextos
5. **Debug**: Editor personalizado para visualizar mapeamentos

## Exemplo de Remapeamento

Se quiser que a tecla "Enter" faça a mesma coisa que o "0":

```csharp
void Start()
{
    // Trocar 0 para Enter
    InputManager.Instance.RemapInput(0, KeyCode.Return);
    
    // Ou adicionar Enter como um novo ID que faz a mesma coisa
    InputManager.Instance.RemapInput(11, KeyCode.Return);
    // Depois escutar pelo ID 11 também
}
```

## Troubleshooting

### InputManager não encontrado
- Certifique-se de que há um GameObject com o componente InputManager na cena
- O InputManager é um singleton e persiste entre cenas

### Inputs não funcionam
- Verifique se está inscrito nos eventos: `InputManager.OnInputTriggered += HandleInput`
- Certifique-se de cancelar a inscrição no OnDestroy

### Editor não aparece
- O script InputManagerEditor.cs deve estar na pasta Editor
- Selecione o GameObject com InputManager para ver o editor customizado
# Configuração do Popup de Confirmação

## Configuração Automática (Recomendada)

### 1. Adicionar Componente
1. Selecione o GameObject `/Canvas/ConfirmationPopUp`
2. Adicione o componente `ConfirmationPopUpSetup`
3. Certifique-se que `Auto Setup On Start` está marcado
4. Execute o jogo - a configuração será feita automaticamente

### 2. Verificação Manual
Após a configuração automática, verifique se o GameObject possui:
- ✅ `ConfirmationPopUp` (script principal)
- ✅ `CanvasGroup` (para controle de visibilidade)
- ✅ `ConfirmationPopUpSetup` (para configuração automática)

## Configuração Manual

### 1. Componentes Necessários

#### No GameObject Principal (`/Canvas/ConfirmationPopUp`)
```
ConfirmationPopUp
├── CanvasGroup
└── ConfirmationPopUpSetup (opcional)
```

#### No GameObject do Texto (`/Canvas/ConfirmationPopUp/.../Text (TMP)`)
```
Text (TMP)
├── TextMeshProUGUI
└── LocalizedTextComponent (opcional)
```

### 2. Configuração do CanvasGroup
- **Alpha**: 0 (para começar invisível)
- **Interactable**: false
- **Blocks Raycasts**: false

### 3. Configuração do ConfirmationPopUp
- **Canvas Group**: Arraste o CanvasGroup do mesmo GameObject
- **Confirmation Text**: Arraste o componente TextMeshProUGUI do texto
- **Confirm Input Id**: 0
- **Alternate Confirm Input Id**: 1

## Hierarquia Esperada

```
Canvas
└── ConfirmationPopUp                 [ConfirmationPopUp, CanvasGroup]
    └── BackgroundBlack               [Image - fundo escuro]
        └── BackgroundWhite           [Image - fundo do popup]
            └── Text (TMP)            [TextMeshProUGUI, LocalizedTextComponent]
```

## Como Funciona

### Comportamento por Tela

#### **Pergunta/Feedback Screens**
1. Jogador pressiona `0` ou `Backspace`
2. Popup aparece com mensagem de confirmação
3. Jogador pressiona:
   - `0` ou `1` → **CONFIRMA** → Volta para tela inicial
   - Qualquer outra tecla → **CANCELA** → Continua o jogo

#### **Tela Final**
1. Jogador pressiona `0` ou `Backspace`
2. **Reset direto** (sem popup)
3. Volta imediatamente para tela inicial

### Mensagens do Popup

#### Português
```
Deseja voltar ao início?

Pressione 0 ou 1 para CONFIRMAR
Pressione qualquer outra tecla para CONTINUAR
```

#### English
```
Do you want to return to the start?

Press 0 or 1 to CONFIRM
Press any other key to CONTINUE
```

## Troubleshooting

### Popup não aparece
1. ✅ Verifique se `ConfirmationPopUp.Instance` não é null
2. ✅ Certifique-se que há um `CanvasGroup` no GameObject
3. ✅ Verifique se o GameObject está ativo na hierarquia
4. ✅ Execute `ConfirmationPopUpSetup.SetupPopup()` manualmente

### Texto não aparece
1. ✅ Verifique se `TextMeshProUGUI` está assinalado no script
2. ✅ Certifique-se que o componente de texto está ativo
3. ✅ Verifique se o `LocalizedTextComponent` não está interferindo

### Inputs não funcionam
1. ✅ Verifique se `InputManager` está ativo na cena
2. ✅ Certifique-se que os IDs de confirmação estão corretos (0 e 1)
3. ✅ Verifique se não há outros scripts interceptando os inputs

## API para Uso Personalizado

```csharp
// Mostrar popup com mensagem personalizada
ConfirmationPopUp.Instance.ShowConfirmationPopup(
    message: "Sua mensagem aqui",
    onConfirmCallback: () => Debug.Log("Confirmado!"),
    onCancelCallback: () => Debug.Log("Cancelado!")
);

// Mostrar popup de saída padrão
ConfirmationPopUp.Instance.ShowExitConfirmation(
    onConfirmCallback: () => ReturnToStart(),
    onCancelCallback: () => ContinueGame()
);

// Verificar se popup está ativo
bool isActive = ConfirmationPopUp.Instance.IsActive();
```
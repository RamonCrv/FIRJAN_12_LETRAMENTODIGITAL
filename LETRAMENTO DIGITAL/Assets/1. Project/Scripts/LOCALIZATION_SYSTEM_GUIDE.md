# Sistema de Localização - Guia de Uso

## Visão Geral
Este sistema permite gerenciar textos multilíngues (Português/Inglês) de forma centralizada usando JSON e componentes reutilizáveis.

## Estrutura do Sistema

### 1. LocalizationData.cs
- Define a estrutura dos dados de localização
- Contém métodos para buscar e gerenciar textos

### 2. LocalizationManager.cs
- Singleton que gerencia todo o sistema
- Carrega dados do JSON automaticamente
- Salva preferência de idioma do usuário
- Detecta idioma do sistema automaticamente

### 3. LocalizedTextComponent.cs
- Componente que você adiciona aos TextMeshPro
- Requer apenas o ID do texto
- Atualiza automaticamente quando o idioma muda

### 4. LanguageSwitcher.cs
- Componente para criar botões de troca de idioma
- Pode ser usado em menus de configuração

## Como Usar

### Passo 1: Configurar Textos
1. Edite o arquivo `/Assets/StreamingAssets/localization_texts.json`
2. Adicione novos textos com esta estrutura:
```json
{
  "id": "meu_texto_id",
  "portuguese": "Texto em português",
  "english": "Text in English"
}
```

### Passo 2: Usar em TextMeshPro
1. Adicione o componente `LocalizedTextComponent` ao seu TextMeshPro
2. Digite o ID do texto no campo "Text Id"
3. O texto será atualizado automaticamente

### Passo 3: Usar em Código
```csharp
// Obter texto localizado
string texto = LocalizationManager.Instance.GetText("meu_texto_id");

// Trocar idioma
LocalizationManager.Instance.ChangeLanguage(SystemLanguage.English);
```

### Passo 4: Botões de Idioma
1. Adicione o componente `LanguageSwitcher` a um GameObject
2. Conecte os botões nos campos correspondentes
3. Os botões trocarão o idioma automaticamente

## Vantagens do Sistema

✅ **Sem variáveis por texto**: Use apenas IDs  
✅ **Atualização automática**: Troca idioma em tempo real  
✅ **Persistência**: Lembra da escolha do usuário  
✅ **Detecção automática**: Usa idioma do sistema  
✅ **Fácil manutenção**: Todos os textos em um JSON  
✅ **Debugging**: Mostra IDs faltantes claramente  

## Exemplo de Uso Prático

```csharp
public class MenuManager : MonoBehaviour
{
    [SerializeField] private LocalizedTextComponent titleText;
    [SerializeField] private LocalizedTextComponent[] buttonTexts;
    
    private void Start()
    {
        // Os textos são configurados automaticamente
        // baseado nos IDs definidos no Inspector
    }
    
    public void UpdateScore(int score)
    {
        string scoreLabel = LocalizationManager.Instance.GetText("score_label");
        scoreText.text = $"{scoreLabel} {score}";
    }
}
```

## Estrutura de Pastas
```
Assets/
├── StreamingAssets/
│   └── localization_texts.json     # Arquivo de textos
└── 1. Project/Scripts/
    ├── LocalizationData.cs         # Estrutura de dados
    ├── LocalizationManager.cs      # Gerenciador principal
    ├── LocalizedTextComponent.cs   # Componente para UI
    └── LanguageSwitcher.cs         # Componente para trocar idioma
```

## Dicas de Uso

1. **IDs descritivos**: Use IDs claros como "start_button", "score_label"
2. **Organização**: Agrupe IDs por contexto (menu_, game_, ui_)
3. **Backup**: Mantenha backup do JSON antes de grandes mudanças
4. **Teste**: Sempre teste ambos os idiomas
5. **Performance**: O sistema carrega tudo na inicialização para melhor performance
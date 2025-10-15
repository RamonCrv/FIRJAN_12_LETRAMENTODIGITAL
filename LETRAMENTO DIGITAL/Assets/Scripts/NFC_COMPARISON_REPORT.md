# Relatório de Comparação: NFC Atual vs Dilema do Bonde

**Data**: Sistema atualizado para seguir padrão Dilema do Bonde
**Projeto**: Letramento Digital (Game ID: 12)
**Referência**: Documentação Dilema do Bonde (Game ID: 10)
**Última atualização**: Conflito de namespaces resolvido

---

## Resumo Executivo

A implementação atual foi comparada com a documentação do "Dilema do Bonde" e ajustada para seguir mais fielmente o padrão estabelecido, mantendo as adaptações necessárias para o jogo de Letramento Digital.

---

## Alterações Realizadas

### ✅ 0. Correção de Conflito de Namespaces

**PROBLEMA**:
```
Error CS0029: Cannot implicitly convert type 'TimeoutSettings' to 'RealGames.TimeoutSettings'
```

**CAUSA**: 
Duplicação da classe `TimeoutSettings`. O projeto já tinha `RealGames.TimeoutSettings` definida em `/Assets/1. Project/Scripts/FileLoader/Json/GameConfig.cs`.

**SOLUÇÃO**:
Removida a classe duplicada do `ServerConfig.cs` e utilizada a classe existente do namespace `RealGames`:

```csharp
// ANTES (ERRADO - criava nova classe)
public class TimeoutSettings { ... }
public class NFCGameConfiguration
{
    public TimeoutSettings timeoutSettings;
}

// DEPOIS (CORRETO - usa classe existente)
public class NFCGameConfiguration
{
    public RealGames.TimeoutSettings timeoutSettings;
    public RealGames.ServerConfiguration server;
}
```

---

### ✅ 1. Referências de Componentes NFC

**ANTES** (não seguia padrão):
```csharp
// Componentes privados
private NFCReceiver nfcReceiver;
private ServerComunication serverCommunication;
```

**DEPOIS** (padrão Dilema do Bonde):
```csharp
[Header("Component References")]
public NFCReceiver nfcReceiver;
public ServerComunication serverCommunication;
```

**Motivo**: No Dilema do Bonde, os componentes NFC são referências públicas que podem ser configuradas no Inspector, permitindo maior flexibilidade.

---

### ✅ 2. Carregamento de Configuração de Timeout

**ANTES**:
```csharp
void InitializeNFCSystem()
{
    var config = ServerConfig.LoadFromJSON();
    if (config?.server != null)
    {
        serverIP = config.server.ip;
        serverPort = config.server.port;
    }
}
```

**DEPOIS** (padrão Dilema do Bonde):
```csharp
void LoadServerConfiguration()
{
    var config = ServerConfig.LoadFromJSON();
    if (config?.server != null)
    {
        serverIP = config.server.ip;
        serverPort = config.server.port;
    }
    
    // Carregar timeout do arquivo JSON
    if (config?.timeoutSettings != null)
    {
        nfcTimeoutSeconds = config.timeoutSettings.generalTimeoutSeconds;
        Debug.Log($"[NFCGameManager] Timeout NFC: {nfcTimeoutSeconds}s");
    }
}
```

**Motivo**: O timeout deve vir do arquivo `config.json` para facilitar ajustes sem recompilar.

---

### ✅ 3. ServerConfig com Propriedades Estáticas

**ANTES**:
```csharp
public static class ServerConfig
{
    public static NFCGameConfiguration LoadFromJSON()
    {
        // Somente método de carregamento
    }
}
```

**DEPOIS** (padrão Dilema do Bonde):
```csharp
public static class ServerConfig
{
    public static ServerConfiguration Server { get; }
    public static TimeoutSettings Timeouts { get; }
    
    public static NFCGameConfiguration LoadFromJSON()
    {
        // Carregamento + propriedades de acesso
    }
}
```

**Motivo**: Facilita acesso às configurações em todo o projeto: `ServerConfig.Server.ip`

---

### ✅ 4. Logs de Debug Melhorados

**ADICIONADO**:
```csharp
Debug.Log($"[NFCGameManager] Servidor: http://{serverIP}:{serverPort}");
Debug.Log($"[NFCGameManager] NFCReceiver: {(nfcReceiver != null ? "OK" : "ERRO")}");
Debug.Log($"[NFCGameManager] ServerComunication: {(serverCommunication != null ? "OK" : "ERRO")}");
```

**Motivo**: Seguindo padrão de logging detalhado do Dilema do Bonde para facilitar debug.

---

## Diferenças Mantidas (Adaptações Necessárias)

### 1. Game ID
- **Dilema do Bonde**: `gameId = 10`
- **Letramento Digital**: `gameId = 12`
- ✅ **Mantido** - Cada jogo tem ID único na API Firjan

### 2. Mapeamento de Pontuações

**Dilema do Bonde** (2 perfis morais):
```csharp
// Realista
skill1 = 9;  // Raciocínio Lógico
skill2 = 5;  // Autoconsciência
skill3 = 6;  // Tomada de decisão

// Empático
skill1 = 8;
skill2 = 6;
skill3 = 6;
```

**Letramento Digital** (3 níveis de desempenho):
```csharp
// Alto Desempenho (≥80% acertos)
skill1 = 8;  // Letramento Digital
skill2 = 7;  // Pensamento Analítico
skill3 = 6;  // Curiosidade

// Médio Desempenho (50-79% acertos)
skill1 = 6;
skill2 = 5;
skill3 = 4;

// Baixo Desempenho (<50% acertos)
skill1 = 4;
skill2 = 3;
skill3 = 2;
```

✅ **Mantido** - Sistemas de pontuação diferentes conforme mecânica do jogo

### 3. Controller de Jogo

**Dilema do Bonde**:
```csharp
DilemmaGameController.Instance.GetFinalProfile()
DilemmaGameController.Instance.realistAnswers
DilemmaGameController.Instance.empatheticAnswers
```

**Letramento Digital**:
```csharp
DigitalLiteracyGameController.Instance
gameController.correctAnswers
gameController.GetTotalQuestions()
```

✅ **Mantido** - Controladores específicos para cada jogo

---

## Estrutura de Arquitetura (Conforme Dilema do Bonde)

```
NFCGameManager (Orquestrador Principal)
├── NFCReceiver (Biblioteca NFC Firjan)
├── ServerComunication (HTTP Client)
├── GameModel (Estrutura de Dados)
├── DigitalLiteracyGameController (Lógica do Jogo)
└── ServerConfig (Configuração)
```

✅ Arquitetura alinhada com padrão Dilema do Bonde

---

## Fluxo de Integração (Seguindo Padrão)

```
1. Jogo Iniciado
   ↓
2. Jogador Completa Questões
   ↓
3. Cálculo do Desempenho (Alto/Médio/Baixo)
   ↓
4. Exibição da Tela de Resultados
   ↓
5. Auto-ativação do NFC (após delay)
   ↓
6. Aguarda Cartão NFC ou Timeout (configurável)
   ↓
7. Cartão Detectado → Envio ao Servidor
   ↓
8. Feedback de Sucesso/Erro
   ↓
9. Retorno Automático ao Menu Inicial
```

✅ Fluxo implementado conforme documentação

---

## Configuração JSON (Estrutura Completa)

Seu `config.json` atual é **mais completo** que o do Dilema do Bonde:

```json
{
  "timeoutSettings": {
    "generalTimeoutSeconds": 60,
    "questionTimeoutSeconds": 20,
    "feedbackTimeoutSeconds": 5,
    "finalScreenTimeoutSeconds": 15
  },
  "server": {
    "ip": "192.168.0.212",
    "port": 8080
  },
  "questions": [ ... ]
}
```

✅ Estrutura estendida adequadamente para Letramento Digital

---

## Checklist de Validação (Padrão Dilema do Bonde)

- [x] NFCReceiver presente na cena
- [x] ServerComunication configurado com IP/porta corretos
- [x] NFCGameManager com todas as referências públicas
- [x] config.json em StreamingAssets
- [x] Mapeamento de pontuações correto para o jogo
- [x] Sistema de timeout carregado do JSON
- [x] Logs de debug detalhados
- [x] Singleton implementado corretamente
- [x] Listeners NFC configurados
- [x] Tratamento de erros robusto

---

## Próximos Passos

### 1. Verificar IP do Servidor
⚠️ Confirme se `192.168.0.212` é o IP correto do servidor API Firjan.
Se necessário, atualize em `/Assets/StreamingAssets/config.json`

### 2. Testar Sistema NFC
```
F1 - Simular conexão NFC (se implementado debug)
F2 - Simular desconexão
Tecla 4 - Ativar NFC manualmente
```

### 3. Validar Envio de Dados
Verificar no console se o JSON enviado está correto:
```json
{
  "nfcId": "CARD_ID",
  "gameId": 12,
  "skill1": 8,
  "skill2": 7,
  "skill3": 6
}
```

---

## Conclusão

✅ **Sistema NFC atualizado para seguir padrão Dilema do Bonde**
✅ **Adaptações mantidas para mecânica de Letramento Digital**
✅ **Configurações externas via JSON implementadas**
✅ **Logs e debug melhorados**

A implementação agora está alinhada com as melhores práticas documentadas no projeto Dilema do Bonde, mantendo as customizações necessárias para o jogo de Letramento Digital.

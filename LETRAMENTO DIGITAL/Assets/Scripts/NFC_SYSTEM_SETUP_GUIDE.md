# Sistema NFC - Guia de Configuração e Uso

## Visão Geral
Este sistema permite que o jogo de Letramento Digital salve automaticamente os resultados dos jogadores em cartões NFC, enviando os dados para a API Firjan.

## Configuração Inicial

### 1. Configuração do Servidor
Edite o arquivo `/Assets/StreamingAssets/config.json` e adicione a configuração do servidor:

```json
{
  "timeoutSettings": { ... },
  "server": {
    "ip": "192.168.0.185",
    "port": 8080
  },
  "questions": [ ... ]
}
```

### 2. Configuração da Cena
1. Crie um GameObject vazio na cena principal
2. Nomeie como "NFCGameManager"
3. Adicione o script `NFCGameManager`
4. Configure os parâmetros no Inspector

### 3. Configuração da UI
Na tela final do jogo (`FinalScreen`), configure:
- `nfcPanel`: GameObject que contém a UI do NFC
- `nfcStatusText`: Texto que mostra o status atual
- `nfcInstructionText`: Texto com instruções para o usuário

## Como Funciona

### Fluxo Automático
1. Quando o jogador termina o jogo, a `FinalScreen` é exibida
2. Após 3 segundos, o sistema NFC é automaticamente ativado
3. O jogador aproxima o cartão NFC
4. Os dados são enviados para a API Firjan
5. O sistema retorna ao menu principal

### Mapeamento de Pontuações
O sistema mapeia o desempenho do jogador para habilidades específicas:

**Alto Desempenho (≥80% de acertos):**
- Letramento Digital: 8 pontos
- Pensamento Analítico: 7 pontos
- Curiosidade: 6 pontos

**Médio Desempenho (50-79% de acertos):**
- Letramento Digital: 6 pontos
- Pensamento Analítico: 5 pontos
- Curiosidade: 4 pontos

**Baixo Desempenho (<50% de acertos):**
- Letramento Digital: 4 pontos
- Pensamento Analítico: 3 pontos
- Curiosidade: 2 pontos

## API de Uso

### NFCGameManager
Principal classe do sistema:

```csharp
// Iniciar sessão NFC
NFCGameManager.Instance.StartNFCSession();

// Parar sessão NFC
NFCGameManager.Instance.StopWaitingForNFC();

// Verificar status
bool isWaiting = NFCGameManager.Instance.IsWaitingForNFC;
bool dataSent = NFCGameManager.Instance.GameResultsSent;
string status = NFCGameManager.Instance.SystemStatus;
```

### Configuração Manual
Para uso manual fora da `FinalScreen`:

```csharp
// Configurar UI
var nfcManager = NFCGameManager.Instance;
nfcManager.nfcPanel = seuPainelNFC;
nfcManager.nfcStatusText = seuTextoDeStatus;
nfcManager.nfcInstructionText = seuTextoDeInstrucoes;

// Ativar NFC
nfcManager.StartNFCSession();
```

## Ferramentas de Debug

### NFCDebugTester
Script para testes durante desenvolvimento:

**Teclas de Debug (apenas no Editor):**
- `F1`: Simular conexão NFC
- `F3`: Testar conexão com servidor
- `F4`: Ativar sessão NFC manualmente
- `F5`: Resetar sistema NFC
- `F6`: Mostrar informações do sistema

### Interface GUI (Editor)
O `NFCDebugTester` também fornece uma interface gráfica no editor para facilitar os testes.

## Configuração do Servidor

### Endpoints Utilizados
- `POST /users/{nfcId}`: Enviar dados do jogo
- `GET /users/{nfcId}`: Obter dados do usuário

### Formato dos Dados Enviados
```json
{
  "nfcId": "ID_DO_CARTAO",
  "gameId": 12,
  "skill1": 8,  // Letramento Digital
  "skill2": 7,  // Pensamento Analítico  
  "skill3": 6   // Curiosidade
}
```

## Resolução de Problemas

### Erro de Conexão
- Verificar se o IP e porta estão corretos no `config.json`
- Verificar se o servidor está rodando
- Usar F3 no debug para testar conectividade

### NFC Não Detectado
- Verificar se o leitor NFC está conectado
- Usar F1 no debug para simular cartão
- Verificar logs do Unity Console

### Dados Não Enviados
- Verificar se o `gameId` está correto
- Verificar resposta do servidor nos logs
- Verificar se o jogo terminou corretamente

## Personalização

### Alterar Mapeamento de Pontuações
Edite as variáveis no `NFCGameManager`:
- `highPerformance*`: Pontuações para alto desempenho
- `mediumPerformance*`: Pontuações para médio desempenho  
- `lowPerformance*`: Pontuações para baixo desempenho

### Alterar Game ID
Altere a variável `gameId` no `NFCGameManager` para o ID específico do seu jogo.

### Timeout do NFC
Ajuste `nfcTimeoutSeconds` no `NFCGameManager` para alterar o tempo limite de espera do cartão NFC.

## Integração com Outros Jogos

Para usar este sistema em outros jogos:

1. Copie os scripts necessários
2. Implemente `GetCurrentPlayerScore()` no `NFCGameManager`
3. Configure o `gameId` específico
4. Ajuste o mapeamento de pontuações conforme necessário
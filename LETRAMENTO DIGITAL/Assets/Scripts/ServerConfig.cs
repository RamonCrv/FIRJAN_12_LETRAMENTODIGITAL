using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using RealGames;

[System.Serializable]
public class NFCGameConfiguration
{
    public RealGames.TimeoutSettings timeoutSettings;
    public RealGames.ServerConfiguration server;
}

public static class ServerConfig
{
    private static NFCGameConfiguration _config;
    
    public static RealGames.ServerConfiguration Server
    {
        get
        {
            if (_config == null) LoadFromJSON();
            return _config?.server;
        }
    }
    
    public static RealGames.TimeoutSettings Timeouts
    {
        get
        {
            if (_config == null) LoadFromJSON();
            return _config?.timeoutSettings;
        }
    }
    
    public static NFCGameConfiguration LoadFromJSON()
    {
        if (_config != null) return _config;
        
        string configPath = Path.Combine(Application.streamingAssetsPath, "config.json");
        
        if (File.Exists(configPath))
        {
            try
            {
                string jsonContent = File.ReadAllText(configPath);
                _config = JsonConvert.DeserializeObject<NFCGameConfiguration>(jsonContent);
                
                if (_config?.server != null)
                {
                    Debug.Log($"[ServerConfig] Configuração carregada: {_config.server.ip}:{_config.server.port}");
                }
                else
                {
                    Debug.LogWarning("[ServerConfig] Servidor não configurado no config.json");
                    _config = CreateDefaultConfig();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ServerConfig] Erro ao carregar config.json: {ex.Message}");
                _config = CreateDefaultConfig();
            }
        }
        else
        {
            Debug.LogWarning("[ServerConfig] config.json não encontrado em StreamingAssets, usando configuração padrão");
            _config = CreateDefaultConfig();
        }
        
        return _config;
    }
    
    private static NFCGameConfiguration CreateDefaultConfig()
    {
        return new NFCGameConfiguration
        {
            server = new RealGames.ServerConfiguration
            {
                ip = "192.168.0.185",
                port = 8080
            },
            timeoutSettings = new RealGames.TimeoutSettings
            {
                generalTimeoutSeconds = 60,
                questionTimeoutSeconds = 20,
                feedbackTimeoutSeconds = 5,
                finalScreenTimeoutSeconds = 15
            }
        };
    }
}
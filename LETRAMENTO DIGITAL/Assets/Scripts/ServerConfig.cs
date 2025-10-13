using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using RealGames;

[System.Serializable]
public class NFCGameConfiguration
{
    public TimeoutSettings timeoutSettings;
    public ServerConfiguration server;
}

public static class ServerConfig
{
    private static NFCGameConfiguration _config;
    
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
                Debug.Log($"[ServerConfig] Configuração carregada: {_config.server.ip}:{_config.server.port}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ServerConfig] Erro ao carregar config.json: {ex.Message}");
                _config = CreateDefaultConfig();
            }
        }
        else
        {
            Debug.LogWarning("[ServerConfig] config.json não encontrado, usando configuração padrão");
            _config = CreateDefaultConfig();
        }
        
        return _config;
    }
    
    private static NFCGameConfiguration CreateDefaultConfig()
    {
        return new NFCGameConfiguration
        {
            server = new ServerConfiguration
            {
                ip = "192.168.0.185",
                port = 8080
            }
        };
    }
}
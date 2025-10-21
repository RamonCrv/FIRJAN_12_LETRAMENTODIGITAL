using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

[CreateAssetMenu(fileName = "InputConfiguration", menuName = "Game/Input Configuration")]
public class InputConfiguration : ScriptableObject
{
    [System.Serializable]
    public class InputMapping
    {
        [Header("Input Definition")]
        public int inputId;
        public string inputName;
        public KeyCode defaultKey;
        
        [Header("Localized Titles")]
        public string titleEnglish;
        public string titlePortuguese;
        
        [Header("Visual")]
        public Sprite inputIcon;
        public string iconPath;
        
        [Header("Description")]
        [TextArea(2, 4)]
        public string description;
    }
    
    [System.Serializable]
    public class InputConfigurationData
    {
        [System.Serializable]
        public class InputData
        {
            public int inputId;
            public string inputName;
            public string keyCode;
            public string titleEnglish;
            public string titlePortuguese;
            public string iconPath;
            public string description;
        }
        
        public List<InputData> inputs = new List<InputData>();
    }
    
    [Header("Input Mappings")]
    public List<InputMapping> inputMappings = new List<InputMapping>
    {
        new InputMapping { inputId = 0, inputName = "Letter A", defaultKey = KeyCode.A, titleEnglish = "Letter A", titlePortuguese = "Letra A", description = "Letter A input" },
        new InputMapping { inputId = 1, inputName = "Letter B", defaultKey = KeyCode.B, titleEnglish = "Letter B", titlePortuguese = "Letra B", description = "Letter B input" },
        new InputMapping { inputId = 2, inputName = "Letter C", defaultKey = KeyCode.C, titleEnglish = "Letter C", titlePortuguese = "Letra C", description = "Letter C input" },
        new InputMapping { inputId = 3, inputName = "Letter D", defaultKey = KeyCode.D, titleEnglish = "Letter D", titlePortuguese = "Letra D", description = "Letter D input" },
        new InputMapping { inputId = 4, inputName = "Letter E", defaultKey = KeyCode.E, titleEnglish = "Letter E", titlePortuguese = "Letra E", description = "Letter E input" },
        new InputMapping { inputId = 5, inputName = "Letter F", defaultKey = KeyCode.F, titleEnglish = "Letter F", titlePortuguese = "Letra F", description = "Letter F input" },
        new InputMapping { inputId = 6, inputName = "Letter G", defaultKey = KeyCode.G, titleEnglish = "Letter G", titlePortuguese = "Letra G", description = "Letter G input" },
        new InputMapping { inputId = 7, inputName = "Letter H", defaultKey = KeyCode.H, titleEnglish = "Letter H", titlePortuguese = "Letra H", description = "Letter H input" },
        new InputMapping { inputId = 8, inputName = "Letter I", defaultKey = KeyCode.I, titleEnglish = "Letter I", titlePortuguese = "Letra I", description = "Letter I input" },
        new InputMapping { inputId = 9, inputName = "Letter J", defaultKey = KeyCode.J, titleEnglish = "Letter J", titlePortuguese = "Letra J", description = "Letter J input" },
        new InputMapping { inputId = 10, inputName = "Letter K", defaultKey = KeyCode.K, titleEnglish = "Letter K", titlePortuguese = "Letra K", description = "Letter K input" },
        new InputMapping { inputId = 11, inputName = "Letter L", defaultKey = KeyCode.L, titleEnglish = "Letter L", titlePortuguese = "Letra L", description = "Letter L input" },
        new InputMapping { inputId = 12, inputName = "Letter M", defaultKey = KeyCode.M, titleEnglish = "Letter M", titlePortuguese = "Letra M", description = "Letter M input" },
        new InputMapping { inputId = 13, inputName = "Letter N", defaultKey = KeyCode.N, titleEnglish = "Letter N", titlePortuguese = "Letra N", description = "Letter N input" },
        new InputMapping { inputId = 14, inputName = "Letter O", defaultKey = KeyCode.O, titleEnglish = "Letter O", titlePortuguese = "Letra O", description = "Letter O input" },
        new InputMapping { inputId = 15, inputName = "Letter P", defaultKey = KeyCode.P, titleEnglish = "Letter P", titlePortuguese = "Letra P", description = "Letter P input" },
        new InputMapping { inputId = 16, inputName = "Letter Q", defaultKey = KeyCode.Q, titleEnglish = "Letter Q", titlePortuguese = "Letra Q", description = "Letter Q input" },
        new InputMapping { inputId = 17, inputName = "Letter R", defaultKey = KeyCode.R, titleEnglish = "Letter R", titlePortuguese = "Letra R", description = "Letter R input" },
        new InputMapping { inputId = 18, inputName = "Letter S", defaultKey = KeyCode.S, titleEnglish = "Letter S", titlePortuguese = "Letra S", description = "Letter S input" },
        new InputMapping { inputId = 19, inputName = "Letter T", defaultKey = KeyCode.T, titleEnglish = "Letter T", titlePortuguese = "Letra T", description = "Letter T input" },
        new InputMapping { inputId = 20, inputName = "Letter U", defaultKey = KeyCode.U, titleEnglish = "Letter U", titlePortuguese = "Letra U", description = "Letter U input" },
        new InputMapping { inputId = 21, inputName = "Letter V", defaultKey = KeyCode.V, titleEnglish = "Letter V", titlePortuguese = "Letra V", description = "Letter V input" },
        new InputMapping { inputId = 22, inputName = "Letter W", defaultKey = KeyCode.W, titleEnglish = "Letter W", titlePortuguese = "Letra W", description = "Letter W input" },
        new InputMapping { inputId = 23, inputName = "Letter X", defaultKey = KeyCode.X, titleEnglish = "Letter X", titlePortuguese = "Letra X", description = "Letter X input" },
        new InputMapping { inputId = 24, inputName = "Letter Y", defaultKey = KeyCode.Y, titleEnglish = "Letter Y", titlePortuguese = "Letra Y", description = "Letter Y input" },
        new InputMapping { inputId = 25, inputName = "Letter Z", defaultKey = KeyCode.Z, titleEnglish = "Letter Z", titlePortuguese = "Letra Z", description = "Letter Z input" }
    };
    
    [Header("JSON Configuration")]
    [Tooltip("Nome do arquivo JSON em StreamingAssets (ex: inputConfig.json)")]
    public string jsonFileName = "inputConfig.json";
    
    public Dictionary<int, KeyCode> GetMappingsDictionary()
    {
        Dictionary<int, KeyCode> mappings = new Dictionary<int, KeyCode>();
        foreach (var mapping in inputMappings)
        {
            mappings[mapping.inputId] = mapping.defaultKey;
        }
        return mappings;
    }
    
    public InputMapping GetInputMapping(int inputId)
    {
        return inputMappings.Find(m => m.inputId == inputId);
    }
    
    public string GetLocalizedTitle(int inputId, bool useEnglish = false)
    {
        var mapping = GetInputMapping(inputId);
        if (mapping != null)
        {
            return useEnglish ? mapping.titleEnglish : mapping.titlePortuguese;
        }
        return $"Input {inputId}";
    }
    
    public Sprite GetInputIcon(int inputId)
    {
        var mapping = GetInputMapping(inputId);
        return mapping?.inputIcon;
    }
    
    public void LoadFromJSON()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"[InputConfiguration] JSON file not found: {filePath}");
            return;
        }
        
        try
        {
            string jsonContent = File.ReadAllText(filePath);
            InputConfigurationData data = JsonUtility.FromJson<InputConfigurationData>(jsonContent);
            
            if (data != null && data.inputs != null)
            {
                foreach (var inputData in data.inputs)
                {
                    var mapping = GetInputMapping(inputData.inputId);
                    if (mapping != null)
                    {
                        mapping.inputName = inputData.inputName;
                        mapping.titleEnglish = inputData.titleEnglish;
                        mapping.titlePortuguese = inputData.titlePortuguese;
                        mapping.iconPath = inputData.iconPath;
                        mapping.description = inputData.description;
                        
                        if (Enum.TryParse<KeyCode>(inputData.keyCode, out KeyCode parsedKey))
                        {
                            mapping.defaultKey = parsedKey;
                        }
                        
                        if (!string.IsNullOrEmpty(inputData.iconPath))
                        {
                            LoadIconFromPath(mapping, inputData.iconPath);
                        }
                    }
                }
                
                Debug.Log($"[InputConfiguration] Successfully loaded {data.inputs.Count} inputs from JSON");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[InputConfiguration] Error loading JSON: {ex.Message}");
        }
    }
    
    public void SaveToJSON()
    {
        InputConfigurationData data = new InputConfigurationData();
        
        foreach (var mapping in inputMappings)
        {
            data.inputs.Add(new InputConfigurationData.InputData
            {
                inputId = mapping.inputId,
                inputName = mapping.inputName,
                keyCode = mapping.defaultKey.ToString(),
                titleEnglish = mapping.titleEnglish,
                titlePortuguese = mapping.titlePortuguese,
                iconPath = mapping.iconPath,
                description = mapping.description
            });
        }
        
        string jsonContent = JsonUtility.ToJson(data, true);
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);
        
        try
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
            File.WriteAllText(filePath, jsonContent);
            Debug.Log($"[InputConfiguration] Successfully saved to {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[InputConfiguration] Error saving JSON: {ex.Message}");
        }
    }
    
    private void LoadIconFromPath(InputMapping mapping, string iconPath)
    {
        string fullPath = Path.Combine(Application.streamingAssetsPath, iconPath);
        
        if (File.Exists(fullPath))
        {
            try
            {
                byte[] fileData = File.ReadAllBytes(fullPath);
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(fileData))
                {
                    mapping.inputIcon = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[InputConfiguration] Error loading icon from {fullPath}: {ex.Message}");
            }
        }
    }
}
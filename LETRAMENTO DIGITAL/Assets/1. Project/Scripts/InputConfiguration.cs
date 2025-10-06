using UnityEngine;
using System;
using System.Collections.Generic;

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
        
        [Header("Description")]
        [TextArea(2, 4)]
        public string description;
    }
    
    [Header("Input Mappings")]
    public List<InputMapping> inputMappings = new List<InputMapping>
    {
        new InputMapping { inputId = 0, inputName = "Number 0", defaultKey = KeyCode.Alpha0, description = "Used for Portuguese language selection and option 0" },
        new InputMapping { inputId = 1, inputName = "Number 1", defaultKey = KeyCode.Alpha1, description = "Used for option 1 selection" },
        new InputMapping { inputId = 2, inputName = "Number 2", defaultKey = KeyCode.Alpha2, description = "Used for option 2 selection" },
        new InputMapping { inputId = 3, inputName = "Number 3", defaultKey = KeyCode.Alpha3, description = "Used for option 3 selection" },
        new InputMapping { inputId = 4, inputName = "Number 4", defaultKey = KeyCode.Alpha4, description = "Used for option 4 selection" },
        new InputMapping { inputId = 5, inputName = "Number 5", defaultKey = KeyCode.Alpha5, description = "Used for option 5 selection" },
        new InputMapping { inputId = 6, inputName = "Number 6", defaultKey = KeyCode.Alpha6, description = "Used for option 6 selection" },
        new InputMapping { inputId = 7, inputName = "Number 7", defaultKey = KeyCode.Alpha7, description = "Used for option 7 selection" },
        new InputMapping { inputId = 8, inputName = "Number 8", defaultKey = KeyCode.Alpha8, description = "Used for option 8 selection" },
        new InputMapping { inputId = 9, inputName = "Number 9", defaultKey = KeyCode.Alpha9, description = "Used for option 9 selection" },
        new InputMapping { inputId = -1, inputName = "Backspace", defaultKey = KeyCode.Backspace, description = "Used for English language selection and back actions" }
    };
    
    public Dictionary<int, KeyCode> GetMappingsDictionary()
    {
        Dictionary<int, KeyCode> mappings = new Dictionary<int, KeyCode>();
        foreach (var mapping in inputMappings)
        {
            mappings[mapping.inputId] = mapping.defaultKey;
        }
        return mappings;
    }
}
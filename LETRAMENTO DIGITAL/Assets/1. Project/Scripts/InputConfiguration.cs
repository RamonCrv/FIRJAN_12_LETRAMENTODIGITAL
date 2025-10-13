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
        new InputMapping { inputId = 0, inputName = "Letter A", defaultKey = KeyCode.A, description = "Letter A input" },
        new InputMapping { inputId = 1, inputName = "Letter B", defaultKey = KeyCode.B, description = "Letter B input" },
        new InputMapping { inputId = 2, inputName = "Letter C", defaultKey = KeyCode.C, description = "Letter C input" },
        new InputMapping { inputId = 3, inputName = "Letter D", defaultKey = KeyCode.D, description = "Letter D input" },
        new InputMapping { inputId = 4, inputName = "Letter E", defaultKey = KeyCode.E, description = "Letter E input" },
        new InputMapping { inputId = 5, inputName = "Letter F", defaultKey = KeyCode.F, description = "Letter F input" },
        new InputMapping { inputId = 6, inputName = "Letter G", defaultKey = KeyCode.G, description = "Letter G input" },
        new InputMapping { inputId = 7, inputName = "Letter H", defaultKey = KeyCode.H, description = "Letter H input" },
        new InputMapping { inputId = 8, inputName = "Letter I", defaultKey = KeyCode.I, description = "Letter I input" },
        new InputMapping { inputId = 9, inputName = "Letter J", defaultKey = KeyCode.J, description = "Letter J input" },
        new InputMapping { inputId = 10, inputName = "Letter K", defaultKey = KeyCode.K, description = "Letter K input" },
        new InputMapping { inputId = 11, inputName = "Letter L", defaultKey = KeyCode.L, description = "Letter L input" },
        new InputMapping { inputId = 12, inputName = "Letter M", defaultKey = KeyCode.M, description = "Letter M input" },
        new InputMapping { inputId = 13, inputName = "Letter N", defaultKey = KeyCode.N, description = "Letter N input" },
        new InputMapping { inputId = 14, inputName = "Letter O", defaultKey = KeyCode.O, description = "Letter O input" },
        new InputMapping { inputId = 15, inputName = "Letter P", defaultKey = KeyCode.P, description = "Letter P input" },
        new InputMapping { inputId = 16, inputName = "Letter Q", defaultKey = KeyCode.Q, description = "Letter Q input" },
        new InputMapping { inputId = 17, inputName = "Letter R", defaultKey = KeyCode.R, description = "Letter R input" },
        new InputMapping { inputId = 18, inputName = "Letter S", defaultKey = KeyCode.S, description = "Letter S input" },
        new InputMapping { inputId = 19, inputName = "Letter T", defaultKey = KeyCode.T, description = "Letter T input" },
        new InputMapping { inputId = 20, inputName = "Letter U", defaultKey = KeyCode.U, description = "Letter U input" },
        new InputMapping { inputId = 21, inputName = "Letter V", defaultKey = KeyCode.V, description = "Letter V input" },
        new InputMapping { inputId = 22, inputName = "Letter W", defaultKey = KeyCode.W, description = "Letter W input" },
        new InputMapping { inputId = 23, inputName = "Letter X", defaultKey = KeyCode.X, description = "Letter X input" },
        new InputMapping { inputId = 24, inputName = "Letter Y", defaultKey = KeyCode.Y, description = "Letter Y input" },
        new InputMapping { inputId = 25, inputName = "Letter Z", defaultKey = KeyCode.Z, description = "Letter Z input" }
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
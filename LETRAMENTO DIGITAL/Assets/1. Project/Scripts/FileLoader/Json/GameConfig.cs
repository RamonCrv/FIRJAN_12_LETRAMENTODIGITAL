using System.Collections.Generic;
using UnityEngine;

namespace RealGames
{
    [System.Serializable]
    public class TimeoutSettings
    {
        public int generalTimeoutSeconds = 60;
        public int questionTimeoutSeconds = 20;
        public int feedbackTimeoutSeconds = 10;
        public int finalScreenTimeoutSeconds = 15;
    }

    [System.Serializable]
    public class QuestionFeedback
    {
        public string correct;
        public string incorrect;
        public string correctEn;
        public string incorrectEn;
    }

    [System.Serializable]
    public class Question
    {
        public int id;
        public string questionText;
        public string questionTextEn;
        public List<string> alternatives;
        public List<string> alternativesEn;
        public int correctAnswer;
        public QuestionFeedback feedback;
    }

    [System.Serializable]
    public class GameConfig
    {
        public TimeoutSettings timeoutSettings;
        public List<Question> questions;
    }
}
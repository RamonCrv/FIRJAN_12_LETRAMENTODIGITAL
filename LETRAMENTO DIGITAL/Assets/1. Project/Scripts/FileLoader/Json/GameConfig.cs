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
    }

    [System.Serializable]
    public class QuestionFeedback
    {
        public string correct;
        public string incorrect;
    }

    [System.Serializable]
    public class Question
    {
        public int id;
        public string questionText;
        public List<string> alternatives;
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
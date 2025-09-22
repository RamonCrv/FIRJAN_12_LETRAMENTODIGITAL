# Digital Literacy Game - Setup Guide

## Overview

This system creates a complete Digital Literacy educational game with the following features:

- **Idle Screen**: Call-to-action screen (Press 0 to start)
- **Question System**: 5 random questions from a pool of 10
- **Input Handling**: Keyboard numbers 1-9 for answers
- **Timeout System**: Auto-advance after specified times
- **Feedback System**: Shows correct/incorrect with explanations
- **Final Results**: Score display with restart option
- **Auto-timeout**: Returns to idle after 60 seconds of inactivity

## Files Created

### Core System
1. `DigitalLiteracyGameController.cs` - Main game controller
2. `GameConfig.cs` - Data structures for JSON configuration
3. `config.json` - Game configuration with all 10 questions

### Screen Scripts
4. `IdleScreen.cs` - Idle/Start screen
5. `QuestionScreen.cs` - Question display screen
6. `FeedbackScreen.cs` - Answer feedback screen
7. `FinalScreen.cs` - Results screen
8. `DigitalLiteracySetup.cs` - Helper setup script

## Setup Instructions

### 1. Scene Setup
Create a new scene or modify existing with:

```
Scene Hierarchy:
├── Canvas (Screen Space - Overlay)
│   ├── IdleScreen (with CanvasGroup)
│   │   ├── Title (TextMeshPro)
│   │   ├── Instructions (TextMeshPro)
│   │   └── StartButton (Button - Optional)
│   ├── QuestionScreen (with CanvasGroup)
│   │   ├── QuestionText (TextMeshPro)
│   │   ├── Alternative1 (TextMeshPro)
│   │   ├── Alternative2 (TextMeshPro)
│   │   ├── Alternative3 (TextMeshPro)
│   │   ├── AlternativeButtons (3 Buttons)
│   │   ├── TimerText (TextMeshPro)
│   │   └── TimerFill (Image with Fill)
│   ├── FeedbackScreen (with CanvasGroup)
│   │   ├── ResultText (TextMeshPro)
│   │   ├── FeedbackText (TextMeshPro)
│   │   ├── ResultIcon (Image)
│   │   ├── ContinueButton (Button)
│   │   ├── TimerText (TextMeshPro)
│   │   └── TimerFill (Image with Fill)
│   └── FinalScreen (with CanvasGroup)
│       ├── TitleText (TextMeshPro)
│       ├── ScoreText (TextMeshPro)
│       ├── MessageText (TextMeshPro)
│       ├── ScoreIcon (Image)
│       ├── RestartButton (Button)
│       ├── ManualResetButton (Button)
│       ├── TimerText (TextMeshPro)
│       └── TimerFill (Image with Fill)
├── GameController (Empty GameObject)
│   ├── DigitalLiteracyGameController script
│   └── DigitalLiteracySetup script (optional)
└── ScreenCanvasController (from existing prefab)
```

### 2. Component Assignment

#### IdleScreen GameObject:
- Add `CanvasGroup` component
- Add `IdleScreen` script
- Assign UI components in inspector

#### QuestionScreen GameObject:
- Add `CanvasGroup` component
- Add `QuestionScreen` script
- Assign UI components (texts, buttons, timer elements)

#### FeedbackScreen GameObject:
- Add `CanvasGroup` component
- Add `FeedbackScreen` script
- Assign UI components and result icons

#### FinalScreen GameObject:
- Add `CanvasGroup` component
- Add `FinalScreen` script
- Assign UI components, score icons, timer elements, and both restart buttons

### 3. Configuration

The `config.json` file in StreamingAssets contains:
- **timeoutSettings**: General (60s), Question (20s), Feedback (10s), Final Screen (15s) timeouts
- **questions**: Array of 10 questions with alternatives and feedback

### 4. Input System

The game uses Unity's legacy Input system:
- **Number 0**: Start game from idle screen
- **Numbers 1-9**: Answer questions (1-3 for alternatives)

### 5. Screen Flow

```
Idle Screen (Press 0) 
    ↓
Question Screen (20s timeout or answer)
    ↓
Feedback Screen (10s timeout or continue)
    ↓
Next Question (5 total) OR Final Screen
    ↓
Final Screen (Restart button)
    ↓
Back to Idle Screen
```

### 6. Timeout Behavior

- **General Timeout (60s)**: Returns to idle from any screen
- **Question Timeout (20s)**: Auto-submits wrong answer
- **Feedback Timeout (10s)**: Auto-continues to next question
- **Final Screen Timeout (15s)**: Auto-returns to idle screen

## Customization

### Adding New Questions
Edit `StreamingAssets/config.json`:

```json
{
  "id": 11,
  "questionText": "Your question here?",
  "alternatives": [
    "Option A",
    "Option B", 
    "Option C"
  ],
  "correctAnswer": 0,
  "feedback": {
    "correct": "Correct explanation",
    "incorrect": "Incorrect explanation"
  }
}
```

### Changing Timeouts
Modify `timeoutSettings` in `config.json`:

```json
"timeoutSettings": {
  "generalTimeoutSeconds": 60,
  "questionTimeoutSeconds": 20,
  "feedbackTimeoutSeconds": 10,
  "finalScreenTimeoutSeconds": 15
}
```

## Important Notes

1. **StreamingAssets**: The `config.json` must be in StreamingAssets folder
2. **Screen Names**: Each screen must have correct screenName in ScreenData
3. **CanvasGroup**: All screens require CanvasGroup component
4. **Singleton Pattern**: GameController uses singleton pattern
5. **Input Handling**: Uses legacy Input.inputString for compatibility

## Testing

1. Start the game - should show Idle Screen
2. Press 0 - should start questions
3. Press 1-9 - should answer questions
4. Wait for timeouts - should auto-advance
5. Complete 5 questions - should show final screen
6. Wait 60 seconds - should return to idle

## Troubleshooting

- **Config not loading**: Check config.json path and format
- **Screens not switching**: Verify ScreenManager setup and screen names
- **Input not working**: Ensure game has focus and Input system is active
- **Timeouts not working**: Check coroutines aren't being stopped early
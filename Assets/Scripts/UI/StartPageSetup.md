# StartPage Setup Guide

## Overview
The StartPage allows players to choose between:
1. **Play vs Bot** - Player vs AI with selectable difficulty (Easy, Medium, Hard)
2. **Play vs Friend** - Two players on the same device

## Scene Setup (Main.unity)

### 1. StartPage Panel Structure
Create the following hierarchy:
```
Canvas
├── StartPage Panel (GameObject)
│   ├── Title (TextMeshPro)
│   ├── Play vs Bot Button
│   │   └── Text (TMP)
│   ├── Play vs Friend Button
│   │   └── Text (TMP)
│   └── Difficulty Panel
│       ├── Difficulty Label (TMP)
│       ├── Easy Button
│       │   └── Text (TMP)
│       ├── Medium Button
│       │   └── Text (TMP)
│       └── Hard Button
│           └── Text (TMP)
└── Game Panel (GameObject)
    └── [Your existing game UI]
```

### 2. Add StartPageController Component
1. Add `StartPageController` script to Canvas or a dedicated GameObject
2. Assign references in Inspector:
   - **Start Page Panel**: The StartPage Panel GameObject
   - **Game Panel**: The Game Panel GameObject (contains grid, score, etc.)
   - **Play Vs Bot Button**: The button for bot mode
   - **Play Vs Friend Button**: The button for PvP mode
   - **Difficulty Panel**: Container for difficulty buttons
   - **Easy/Medium/Hard Buttons**: The three difficulty buttons
   - **Difficulty Text**: Text showing current selection
   - **Game Presenter**: Reference to GamePresenter component

### 3. Configure GamePresenter
The GamePresenter should:
- Have **DO NOT** auto-initialize on Awake (we call InitializeWithSettings from StartPageController)
- Be attached to the Game Panel or Canvas
- Have all its required fields assigned (GameView, sprites, etc.)

## How It Works

### Flow
1. Game starts → StartPage is visible, Game Panel is hidden
2. Player clicks "Play vs Bot" → Game initializes with PlayerVsBot mode and selected difficulty
3. Player clicks "Play vs Friend" → Game initializes with PlayerVsPlayer mode
4. StartPage hides, Game Panel shows
5. Game plays normally

### Difficulty Selection
- Default: **Easy (0)**
- Easy = 60% random moves
- Medium = 30% random moves  
- Hard = 10% random moves

### Code Integration
```csharp
// StartPageController handles:
- Button clicks for mode selection
- Difficulty selection (Easy/Medium/Hard)
- Calling gamePresenter.InitializeWithSettings(mode, difficulty)
- Showing/hiding panels

// GamePresenter handles:
- InitializeWithSettings(GameMode, AIDifficulty) - NEW METHOD
- All game logic and coordination
- AI move execution
```

## Alternative: Simple 2-Button Setup

If you don't need difficulty selection UI on the start page:

1. Just use 2 buttons: "Play vs Bot" and "Play vs Friend"
2. Set default difficulty in code (Easy)
3. Allow difficulty change in settings menu (optional)

```csharp
// In StartPageController:
private void OnPlayVsBotClicked()
{
    // Always use Easy difficulty
    StartGame(Core.GameMode.PlayerVsBot, AI.AIDifficulty.Easy);
}
```

## Inspector Setup Checklist

- [ ] StartPageController component added
- [ ] All button references assigned
- [ ] All panel references assigned
- [ ] GamePresenter reference assigned
- [ ] StartPage Panel is active by default
- [ ] Game Panel is inactive by default
- [ ] Buttons have onClick listeners (auto-added by script)
- [ ] TextMeshPro components assigned for difficulty text

## Testing
1. Play Scene
2. StartPage should be visible
3. Click "Play vs Bot" → Game starts with AI
4. Click Reset → Can optionally return to StartPage (implement ReturnToStartPage() if needed)
5. Click "Play vs Friend" → Game starts in 2-player mode

## Difficulty Values
- Easy = `AIDifficulty.Easy` = 0
- Medium = `AIDifficulty.Medium` = 1
- Hard = `AIDifficulty.Hard` = 2

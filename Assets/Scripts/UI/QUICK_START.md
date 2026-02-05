# Quick Start Guide - StartPage Setup

## Goal
Set up a start menu with 2 buttons:
1. **Play vs Bot** (with difficulty selection: Easy/Medium/Hard)
2. **Play vs Friend** (2-player mode)

Default difficulty: Easy (0)

## Step-by-Step Setup in Unity

### 1. Open Main.unity Scene
Your scene should have:
- Canvas (with game UI)
- Existing game grid and components

### 2. Create StartPage Panel
Right-click Canvas → Create Empty GameObject → Name it "StartPage"

Add to StartPage:
```
StartPage (GameObject)
├── Background (Image - optional)
├── Title (TextMeshPro - "TIC TAC TOE")
├── PlayVsBotButton (Button - "Play vs Bot")
│   └── Text (TextMeshPro)
├── PlayVsFriendButton (Button - "Play vs Friend")
│   └── Text (TextMeshPro)
└── DifficultyPanel (GameObject)
    ├── DifficultyLabel (TextMeshPro - "Select Difficulty:")
    ├── DifficultySlider (Slider)
    │   ├── Background
    │   ├── Fill Area
    │   └── Handle Slide Area
    └── DifficultyText (TextMeshPro - "Difficulty: Easy")
```

### 3. Create Game Panel
Organize your existing game UI:
- Select all game UI elements (Grid, Score, Status, Reset button)
- Right-click → Create Empty Parent → Name it "GamePanel"
- This panel should contain: Grid, StatusText, ScorePanel, ResetButton

### 4. Add StartPageController
- Select Canvas (or create new GameObject "StartPageManager")
- Add Component → StartPageController script
- Assign references in Inspector:
  - **Start Page Panel**: Drag the StartPage GameObject
  - **Game Panel**: Drag the GamePanel GameObject
  - **Play Vs Bot Button**: The "Play vs Bot" button
  - **Play Vs Friend Button**: The "Play vs Friend" button
  - **Difficulty Panel**: The DifficultyPanel GameObject
  - **Difficulty Slider**: The Slider component (set Min=0, Max=2, Whole Numbers=true)
  - **Difficulty Text**: The TextMeshPro showing "Difficulty: Easy"
  - **Game Presenter**: Drag the GamePresenter component (should be on Canvas or GamePanel)

### 5. Configure GamePresenter
- Find GamePresenter component in your scene
- Make sure these are assigned:
  - Game View
  - X Sprite
  - O Sprite
  - Reset Delay: 2
  - AI Move Delay: 0.5

### 6. Set Initial Panel States
In Inspector:
- **StartPage**: Check the checkbox (Active)
- **GamePanel**: Uncheck the checkbox (Inactive)

### 7. Test
Play the scene:
1. StartPage should be visible
2. Move difficulty slider → Text updates (Easy/Medium/Hard)
3. Click "Play vs Bot" → Game starts with AI at selected difficulty
4. Play a few moves → AI responds
5. Stop and play again
6. Click "Play vs Friend" → 2-player mode works

## Simplified Setup (No Difficulty Selection UI)

If you just want 2 buttons with fixed Easy difficulty:

1. Create only 2 buttons: "Play vs Bot" and "Play vs Friend"
2. Skip the DifficultyPanel entirely
3. In StartPageController Inspector, leave difficulty fields empty (will default to Easy)

The code will still work - it uses Easy as default.

## Code Reference

### Difficulty Values
```csharp
Easy   = 0 (60% random moves)
Medium = 1 (30% random moves)
Hard   = 2 (10% random moves)
```

### Game Modes
```csharp
PlayerVsPlayer = 0 (2-player)
PlayerVsBot    = 1 (vs AI)
```

## Troubleshooting

### "GamePresenter reference is missing"
- Make sure GamePresenter component exists in scene
- Drag it to the StartPageController's "Game Presenter" field

### "Game doesn't start when clicking buttons"
- Check StartPageController's Inspector - all fields assigned?
- Make sure Difficulty Slider has Min=0, Max=2, Whole Numbers=true
- Check Console for errors
- Make sure buttons have Button component

### "AI doesn't move"
- Make sure you clicked "Play vs Bot" not "Play vs Friend"
- Check AI Move Delay is not too high (0.5s is good)
- Check Console for errors

### "StartPage doesn't hide"
- Make sure StartPage Panel and Game Panel are correctly assigned
- Check that GamePresenter.InitializeWithSettings() is being called (add Debug.Log)

## Optional: Back to Menu Button

To add a button that returns to start page:

1. Add button to GamePanel: "Back to Menu"
2. Add onClick listener → StartPageController.ReturnToStartPage

This will hide game and show start page again.

## File Cleanup

You can now delete (optional):
- `GameModeSelector.cs` - Replaced by StartPageController
- `GameModeSelector.cs.meta`

Keep everything else!

## Summary

**What you created**:
- StartPage with 2 mode buttons
- Difficulty selection (Easy/Medium/Hard)
- Clean separation between menu and game

**What happens**:
- Game doesn't auto-start anymore
- Player chooses mode from menu
- Game initializes with chosen settings
- Everything else works the same

**Difficulty settings** (as requested):
- Default: Easy (0)
- Range: 0-2 (Easy, Medium, Hard)
- Focused on these 2 modes only

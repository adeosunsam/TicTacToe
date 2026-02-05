# StartPage Integration Summary

## What Was Created

### 1. StartPageController.cs
**Purpose**: Manages the start menu where players select game mode and difficulty.

**Features**:
- 2 main buttons: "Play vs Bot" and "Play vs Friend"
- Difficulty selection panel (Easy/Medium/Hard) for bot mode
- Panel transitions (StartPage ↔ Game)
- Calls `GamePresenter.InitializeWithSettings(mode, difficulty)`

**Key Methods**:
- `OnPlayVsBotClicked()` - Starts game with AI opponent
- `OnPlayVsFriendClicked()` - Starts 2-player game
- `SetDifficulty(AIDifficulty)` - Changes difficulty (0=Easy, 1=Medium, 2=Hard)
- `ReturnToStartPage()` - Optional method to go back to menu

### 2. GamePresenter Updates
**Changes**:
- Added `InitializeWithSettings(GameMode, AIDifficulty)` - Main entry point from StartPage
- Removed auto-initialization from `Awake()` - Now waits for StartPage to trigger
- Added `isInitialized` flag to prevent operations before initialization

**Purpose**: Clean separation between menu and game. StartPage controls when game starts.

### 3. Existing Files (No Changes Needed)
- **GameView.cs** - Already implements `UpdateScoreText(int, int)` correctly
- **GameController.cs** - Already supports both game modes
- **AIGameController.cs** - Already handles AI logic
- **MinimaxAI.cs** - Already has 3 difficulty levels

## What to Clean Up (Optional)

### GameModeSelector.cs
**Status**: No longer needed if you use StartPageController

**Action**: 
- Can be deleted or kept as alternative implementation
- StartPageController replaces this functionality with simpler approach
- If you want in-game mode switching, keep GameModeSelector
- If you only need menu-based selection, use StartPageController and delete GameModeSelector

**Recommendation**: Delete `GameModeSelector.cs` and `GameModeSelector.cs.meta` if you're using StartPageController.

## Difficulty Settings

### Current AI Behavior
- **Easy (0)**: 60% random moves, 40% optimal
- **Medium (1)**: 30% random moves, 70% optimal  
- **Hard (2)**: 10% random moves, 90% optimal

All difficulties use:
- Opening move optimization (prefer center → corners → edges)
- Strategic tie-breaking with position weights
- Alpha-beta pruning for performance

## Unity Scene Setup

### Required Hierarchy
```
Canvas
├── StartPageController (component on Canvas or separate GameObject)
│
├── StartPage Panel (active by default)
│   ├── Title
│   ├── PlayVsBotButton
│   ├── PlayVsFriendButton
│   └── DifficultyPanel
│       ├── DifficultyText
│       ├── EasyButton
│       ├── MediumButton
│       └── HardButton
│
└── Game Panel (inactive by default)
    ├── Grid (3x3 cells)
    ├── StatusText
    ├── ScorePanel
    ├── ResetButton
    └── GamePresenter (component)
        └── GameView (component)
```

### Inspector Assignments
**StartPageController**:
- Start Page Panel → StartPage Panel GameObject
- Game Panel → Game Panel GameObject  
- Play Vs Bot Button → The "Play vs Bot" Button
- Play Vs Friend Button → The "Play vs Friend" Button
- Difficulty Panel → DifficultyPanel GameObject
- Easy/Medium/Hard Buttons → Respective difficulty buttons
- Difficulty Text → TextMeshPro showing current selection
- Game Presenter → GamePresenter component reference

**GamePresenter**:
- Game View → GameView component
- X Sprite → X symbol image
- O Sprite → O symbol image
- Reset Delay → 2s (default)
- AI Move Delay → 0.5s (default)
- ~~Game Mode → (ignored, set by StartPage)~~
- ~~AI Difficulty → (ignored, set by StartPage)~~

## How It Works

### Flow
1. **Scene Loads**
   - StartPage Panel: Active
   - Game Panel: Inactive
   - GamePresenter: NOT initialized (waits for StartPage)

2. **Player Selects Difficulty** (optional)
   - Clicks Easy/Medium/Hard button
   - UI updates to show selection
   - Default is Easy

3. **Player Clicks Mode**
   - "Play vs Bot" → Calls `StartGame(PlayerVsBot, selectedDifficulty)`
   - "Play vs Friend" → Calls `StartGame(PlayerVsPlayer, Easy)` (difficulty doesn't matter)

4. **Game Initializes**
   - `GamePresenter.InitializeWithSettings(mode, difficulty)` is called
   - Creates AIGameController with correct settings
   - Sets up board, win checker, score manager
   - Subscribes to events

5. **Game Starts**
   - StartPage Panel: Inactive
   - Game Panel: Active
   - Game board is ready
   - If AI's turn (in Bot mode), AI makes first move

6. **Game Ends**
   - Win/Draw detected
   - Board resets after delay
   - Game continues in same mode
   - (Optional) Add "Back to Menu" button that calls `StartPageController.ReturnToStartPage()`

## Code Example: Adding Back Button

If you want a "Back to Menu" button during gameplay:

```csharp
// In StartPageController.cs (already exists):
public void ReturnToStartPage()
{
    ShowStartPage();
    // Optional: Reset the game state
    if (_gamePresenter != null)
    {
        _gamePresenter.OnResetButtonClicked();
    }
}

// In your Game Panel:
// Add a Button with onClick → StartPageController.ReturnToStartPage
```

## Migration from Old Setup

If you had GameModeSelector:

**Before**:
```csharp
// GameModeSelector component in scene
// Dropdown for mode selection
// Dropdown for difficulty selection
// Calls GamePresenter.SetGameMode() and SetAIDifficulty()
```

**After**:
```csharp
// StartPageController component in scene
// Buttons for mode selection
// Buttons for difficulty selection  
// Calls GamePresenter.InitializeWithSettings(mode, difficulty)
```

**Benefits**:
- Simpler UI (buttons instead of dropdowns)
- Cleaner code (one initialization call instead of two)
- Better UX (dedicated start page instead of in-game selector)
- Mode is locked once game starts (no mid-game switching)

## Files Summary

### New Files
- `Assets/Scripts/UI/StartPageController.cs` - Start menu controller
- `Assets/Scripts/UI/StartPageSetup.md` - Setup guide

### Modified Files
- `Assets/Scripts/UI/GamePresenter.cs` - Added InitializeWithSettings, removed auto-init

### Unchanged Files (Ready to Use)
- `Assets/Scripts/Core/` - All core logic files
- `Assets/Scripts/Game/` - GameController, AIGameController
- `Assets/Scripts/AI/` - MinimaxAI
- `Assets/Scripts/UI/GameView.cs` - View implementation
- `Assets/Scripts/UI/TicTacToeCell.cs` - Cell component
- `Assets/Scripts/UI/StrikeLineController.cs` - Win line visuals

### Files to Consider Removing
- `Assets/Scripts/UI/GameModeSelector.cs` - Replaced by StartPageController (optional delete)

## Next Steps

1. **In Unity Editor**:
   - Create StartPage Panel hierarchy
   - Add StartPageController component
   - Assign all references in Inspector
   - Set StartPage Panel active, Game Panel inactive

2. **Test**:
   - Play scene → StartPage visible
   - Click "Play vs Bot" → Game starts
   - Play game → Works normally
   - Click "Play vs Friend" → 2-player mode works

3. **Polish** (optional):
   - Add button hover effects
   - Add transitions between panels
   - Add "Back to Menu" button in game
   - Add sound effects

4. **Clean Up** (optional):
   - Delete GameModeSelector.cs if not needed
   - Remove unused serialized fields in GamePresenter (gameMode, aiDifficulty - they're set programmatically now)

## Difficulty as Int

The user requested difficulty as int (0-2):
- Easy = 0 = `AIDifficulty.Easy`
- Medium = 1 = `AIDifficulty.Medium`
- Hard = 2 = `AIDifficulty.Hard`

The enum can be cast:
```csharp
int difficultyInt = 1; // Medium
AIDifficulty difficulty = (AIDifficulty)difficultyInt;
gamePresenter.InitializeWithSettings(GameMode.PlayerVsBot, difficulty);
```

Current implementation uses the enum directly, which is cleaner and type-safe.

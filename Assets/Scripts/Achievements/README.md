# Achievement System Implementation Guide

## Overview
This achievement system tracks 5 key gameplay milestones with visual notifications and a showcase screen.

## Achievements

1. **First Victory** - Win your first game
2. **AI Conqueror** - Defeat the Hard AI
3. **Hat Trick** - Win 3 games in a row
4. **Friendly Rivalry** - Play 10 games against a friend
5. **Perfect Victory** - Win without opponent getting 2 in a row

## Files Created

### Core System
- `Assets/Scripts/Achievements/AchievementType.cs` - Enum defining achievement types
- `Assets/Scripts/Achievements/Achievement.cs` - Achievement data class
- `Assets/Scripts/Achievements/AchievementManager.cs` - Core achievement tracking logic
- `Assets/Scripts/Achievements/AchievementNotification.cs` - Popup notification UI controller
- `Assets/Scripts/Achievements/AchievementShowcase.cs` - Achievement list screen controller

### Modified Files
- `Assets/Scripts/UI/GamePresenter.cs` - Integrated achievement tracking
- `Assets/Scripts/UI/StartPageController.cs` - Added achievements button
- `Assets/Scripts/Game/AIGameController.cs` - Added board access method

## Unity Setup Instructions

### 1. Achievement Notification Popup

Create a UI panel for the achievement notification:

1. **Create Notification Panel:**
   - Right-click in Hierarchy → UI → Panel
   - Name it "AchievementNotificationPanel"
   - Anchor to top center of screen
   - Add `CanvasGroup` component for fading effects

2. **Add Text Elements:**
   - Add `TextMeshProUGUI` child named "Title"
   - Add `TextMeshProUGUI` child named "Description"
   - Optional: Add `Image` child named "Icon" for achievement icon

3. **Add Component:**
   - Add `AchievementNotification` script to the panel
   - Assign references in Inspector:
     - Notification Panel: The panel itself
     - Title Text: The Title TextMeshPro
     - Description Text: The Description TextMeshPro
     - Icon Image: (Optional) The icon image
   - Adjust timing:
     - Display Duration: 3 seconds (default)
     - Slide In Duration: 0.5 seconds
     - Slide Out Duration: 0.3 seconds

4. **Optional Audio:**
   - Add `AudioSource` component
   - Assign achievement unlock sound clip
   - Reference it in the script

### 2. Achievement Showcase Screen

Create a full-screen panel to display all achievements:

1. **Create Showcase Panel:**
   - Right-click in Hierarchy → UI → Panel
   - Name it "AchievementShowcasePanel"
   - Make it full screen
   - Set sorting order higher than other UI

2. **Add Header:**
   - Add `TextMeshProUGUI` for title: "ACHIEVEMENTS"
   - Add `TextMeshProUGUI` for progress: "Achievements: 0/5"
   - Add `Button` for close button (top-right X)

3. **Create Scroll View:**
   - Add UI → Scroll View
   - Configure Content with Vertical Layout Group
   - Add Content Size Fitter (Vertical Fit: Preferred Size)

4. **Create Achievement Item Prefab:**
   - Create UI → Panel as prefab
   - Add layout:
     - Background `Image`
     - "Icon" `Image` (left side)
     - "Title" `TextMeshProUGUI`
     - "Description" `TextMeshProUGUI`
   - Save as prefab: `AchievementItemPrefab`

5. **Add Component:**
   - Add `AchievementShowcase` script to showcase panel
   - Assign references:
     - Showcase Panel: The panel itself
     - Achievement List Container: The Content object of Scroll View
     - Achievement Item Prefab: Your achievement item prefab
     - Progress Text: The progress text
     - Close Button: The close button
   - Set colors:
     - Unlocked Color: Gold (#FFD700)
     - Locked Color: Gray (#808080)

### 3. Integrate with GamePresenter

1. **Select GamePresenter GameObject:**
   - Find it in your scene hierarchy
   - In Inspector, find the `GamePresenter` script

2. **Assign Achievement Notification:**
   - Drag the `AchievementNotificationPanel` to the "Achievement Notification" field

### 4. Integrate with StartPageController

1. **Create Achievements Button:**
   - Add a button to your start page
   - Label it "Achievements" or use a trophy icon
   - Position it somewhere accessible (e.g., bottom-left)

2. **Update StartPageController:**
   - Select your StartPageController GameObject
   - In Inspector, assign:
     - Achievement Showcase: The showcase panel
     - Achievements Button: The new button you created

3. **Initialize Achievement Manager:**
   - The `GamePresenter` will automatically create and manage the `AchievementManager`
   - The showcase needs to be initialized - add this in StartPageController.Awake():

```csharp
private void Awake()
{
    SetupButtons();
    SetupSlider();
    ShowStartPage();
    SetDifficulty(AI.AIDifficulty.Easy);
    
    // Initialize achievement showcase when game presenter initializes
    if (_achievementShowcase != null && _gamePresenter != null)
    {
        // You'll need to expose the achievement manager from GamePresenter
        // Or initialize it directly in AchievementShowcase
    }
}
```

## Visual Layout Reference

### Achievement Notification Popup
```
┌────────────────────────────────────────┐
│  🏆  ACHIEVEMENT UNLOCKED!             │
│                                        │
│  First Victory                         │
│  You won your first game!              │
└────────────────────────────────────────┘
```

### Achievement Showcase Screen
```
╔══════════════════════════════════════════════════════════╗
║                                                      [X] ║
║                  🏆 ACHIEVEMENTS 🏆                       ║
║                  Unlocked: 2/5                           ║
║                                                          ║
║  ┌────────────────────────────────────────────────────┐ ║
║  │ ╔══════════════════════════════════════════════╗   │ ║
║  │ ║  ✅  First Victory              [UNLOCKED]  ║   │ ║
║  │ ║      Win your first game                    ║   │ ║
║  │ ╚══════════════════════════════════════════════╝   │ ║
║  │                                                    │ ║
║  │ ╔══════════════════════════════════════════════╗   │ ║
║  │ ║  🔒  AI Conqueror                 [LOCKED]  ║   │ ║
║  │ ║      Defeat the Hard AI                     ║   │ ║
║  │ ╚══════════════════════════════════════════════╝   │ ║
║  │                                                    │ ║
║  │ ╔══════════════════════════════════════════════╗   │ ║
║  │ ║  ✅  Hat Trick                   [UNLOCKED]  ║   │ ║
║  │ ║      Win 3 games in a row                   ║   │ ║
║  │ ╚══════════════════════════════════════════════╝   │ ║
║  │                                                    │ ║
║  │ ╔══════════════════════════════════════════════╗   │ ║
║  │ ║  🔒  Friendly Rivalry             [LOCKED]  ║   │ ║
║  │ ║      Play 10 games against a friend         ║   │ ║
║  │ ╚══════════════════════════════════════════════╝   │ ║
║  │                                                    │ ║
║  │ ╔══════════════════════════════════════════════╗   │ ║
║  │ ║  🔒  Perfect Victory              [LOCKED]  ║   │ ║
║  │ ║      Win without opponent getting 2 in row  ║   │ ║
║  │ ╚══════════════════════════════════════════════╝   │ ║
║  └────────────────────────────────────────────────────┘ ║
╚══════════════════════════════════════════════════════════╝
   (Scroll View - Can scroll if more achievements)
```

### Color Scheme Examples
- **Unlocked:** Gold (#FFD700) background with white text
- **Locked:** Gray (#808080) background with darker text
- **Icons:** ✅ checkmark or 🏆 trophy for unlocked, 🔒 lock for locked

### Reference Games with Similar Achievement UIs
- **Steam Achievements** - Classic grid/list layout
- **Xbox Live Achievements** - Vertical list with progress
- **PlayStation Trophies** - Icon-focused cards
- **Mobile Games** (Clash Royale, etc.) - Card-based unlocks

## Design Recommendations

### Notification Popup Style:
- **Background:** Semi-transparent dark panel with border
- **Position:** Top-center, slides down from off-screen
- **Animation:** Smooth ease-out-back for bounce effect
- **Duration:** 3 seconds visible
- **Colors:** Gold/yellow for unlocked achievements

### Showcase Screen Style:
- **Background:** Dark semi-transparent overlay
- **Layout:** Vertical list with padding
- **Unlocked Items:** Full color, gold accents
- **Locked Items:** Grayed out, 50% opacity
- **Icons:** Trophy, star, or checkmark symbols

### Example Layout Specs:

**Notification Panel:**
```
Width: 400px
Height: 120px
Padding: 20px
Title Size: 24pt, Bold
Description Size: 16pt, Regular
```

**Showcase Panel:**
```
Full screen with 50px margin
Header: 48pt Bold
Achievement Items: 80px height
Item Spacing: 10px
```

## Testing

1. **Test First Victory:**
   - Start a new game
   - Win against bot or friend
   - Notification should appear

2. **Test AI Conqueror:**
   - Set difficulty to Hard
   - Beat the Hard AI
   - Achievement unlocks

3. **Test Win Streak:**
   - Win 3 games in a row
   - Achievement unlocks on 3rd win

4. **Test Perfect Victory:**
   - Win without letting opponent get 2 in a row
   - Requires careful play

5. **Test Friendly Rivalry:**
   - Play 10 games in Player vs Player mode
   - Achievement unlocks after 10th game

## Persistence

Achievements are saved using `PlayerPrefs` and persist between sessions:
- Each achievement unlock state is saved
- Win streak counter is saved
- Friend games played count is saved

To reset all achievements (for testing):
```csharp
// Add a debug button that calls:
achievementManager.ResetAllAchievements();
```

## Troubleshooting

**Notification not showing:**
- Check if AchievementNotification reference is assigned in GamePresenter
- Verify notification panel is active in hierarchy
- Check console for errors

**Achievements not unlocking:**
- Verify achievement conditions are being met
- Check PlayerPrefs for saved data
- Use Debug.Log in AchievementManager to trace events

**Showcase not populating:**
- Ensure AchievementItemPrefab is assigned
- Check that Container has proper layout component
- Verify AchievementManager is initialized

## Future Enhancements

Consider adding:
- Achievement icons/sprites
- Sound effects for unlocks
- Particle effects on notification
- More achievements (win streaks of 5, 10, etc.)
- Stat tracking (total wins, games played, etc.)
- Social sharing of achievements

# TicTacToe - Achievement Edition

Experience the classic game of TicTacToe elevated with modern features! Challenge yourself against intelligent AI opponents across three difficulty levels, or compete with a friend in Player vs Player mode on the same device.

## 🎮 Game Features

### 🏆 Achievement System
Unlock 5 unique achievements as you play:
- **First Victory** - Win your first game
- **Hat Trick** - Achieve a 3-game win streak
- **AI Conqueror** - Defeat the Hard AI difficulty
- **Perfect Victory** - Win without letting your opponent get 2-in-a-row
- **Friendly Rivalry** - Complete 10 games in PvP mode

### 🤖 Smart AI Opponents
Three difficulty levels provide the perfect challenge:
- **Easy** - Great for beginners
- **Medium** - Balanced gameplay
- **Hard** - True strategic challenge using advanced algorithms

### ✨ Polished Experience
- Beautiful achievement notifications with smooth animations
- Achievement showcase panel to track your progress
- Clean, intuitive interface
- Audio feedback for unlocked achievements

## 🕹️ How to Play

### Game Modes
1. **Player vs Bot** - Challenge the AI at your chosen difficulty level
2. **Player vs Player** - Face off against a friend on the same device

### Rules
- Players take turns placing X or O on a 3x3 grid
- First player to get 3 in a row (horizontal, vertical, or diagonal) wins
- If all 9 squares are filled with no winner, the game is a draw

### Controls
- Click/Tap on any empty cell to place your mark
- Use the menu to switch between game modes
- Access the achievement showcase from the main menu

## 🚀 Getting Started

### Requirements
- Unity 6 (6000.3.6f1) or later
- Windows/Mac/Linux

### Installation
1. Clone this repository
2. Open the project in Unity 6
3. Open the main scene from `Assets/Scenes/`
4. Press Play to start the game

### Building the Game
1. Go to `File > Build Settings`
2. Select your target platform
3. Click `Build` and choose your output directory
4. Run the executable to play

## 📁 Project Structure

```
Assets/
├── Scenes/                 # Game scenes
├── Scripts/               
│   ├── Achievements/       # Achievement system
│   ├── AI/                 # AI logic
│   ├── Core/               # Game logic & board management
│   ├── Game/               # Game controllers
│   └── UI/                 # UI components
├── Prefabs/                # Reusable game objects
├── ScriptableObject/       # Achievement data assets
├── Sprites/                # Game graphics
└── Fonts/                  # Text fonts
```

## 🏗️ Architecture

The game uses clean architecture patterns:
- **Singleton Pattern** - GameManager for global state
- **ScriptableObjects** - Data-driven achievement system
- **MVP Pattern** - Separation of game logic and UI
- **Observer Pattern** - Event-driven achievement notifications

## 🎯 Achievement Tips

- **First Victory**: Just win any game to unlock!
- **Hat Trick**: Focus on consistency - win 3 games in a row
- **AI Conqueror**: Master the strategies needed to beat Hard AI
- **Perfect Victory**: Play defensively and block your opponent early
- **Friendly Rivalry**: Great for unlocking while practicing with a friend

## 📄 License

See [LICENSE](LICENSE) file for details.

---

*Can you conquer all achievements and master the timeless strategy game?*

# Tic Tac Toe - Production-Ready Architecture

## Overview
This is a production-ready Tic Tac Toe implementation following SOLID principles and clean architecture patterns.

## Architecture

### Layer Structure
```
Core/               # Business logic (framework-agnostic)
├── Interfaces/     # Contracts
├── Models/         # Data models
└── Services/       # Business services

Game/               # Game coordination layer
└── GameController  # Orchestrates game flow

UI/                 # Presentation layer (Unity-specific)
├── GamePresenter   # MVP Presenter
├── GameView        # View implementation
└── Components/     # UI components
```

## SOLID Principles Applied

### Single Responsibility Principle (SRP)
Each class has one reason to change:
- `GameBoard`: Manages board state only
- `WinChecker`: Validates win conditions only
- `ScoreManager`: Tracks scores only
- `GameState`: Manages game state only
- `GameController`: Coordinates game flow only
- `GamePresenter`: Bridges UI and business logic only
- `GameView`: Manages UI rendering only
- `StrikeLineController`: Manages strike line visuals only

### Open/Closed Principle (OCP)
- Interfaces allow extending without modifying:
  - `IGameBoard` can be implemented with different board sizes
  - `IWinChecker` can support different win rules
  - `IScoreManager` can track different scoring systems
  - `IGameView` can have different UI implementations

### Liskov Substitution Principle (LSP)
- Any `IGameBoard` implementation can replace `GameBoard`
- Any `IWinChecker` implementation can replace `WinChecker`
- Interfaces define behavioral contracts

### Interface Segregation Principle (ISP)
- Small, focused interfaces:
  - `IGameBoard`: Only board operations
  - `IWinChecker`: Only validation logic
  - `IScoreManager`: Only score operations
  - `IGameView`: Only UI update operations

### Dependency Inversion Principle (DIP)
- High-level modules depend on abstractions:
  - `GameController` depends on `IGameBoard`, `IWinChecker`, `IScoreManager`
  - `GamePresenter` depends on `GameController` and `IGameView`
  - No direct dependencies on concrete implementations

## Design Patterns

### MVP (Model-View-Presenter)
- **Model**: Core layer (GameBoard, GameState, ScoreManager)
- **View**: GameView (implements IGameView)
- **Presenter**: GamePresenter (coordinates between View and Controller)

### Observer Pattern
- `GameController` uses C# events for loose coupling:
  - `OnCellPlayed`: Notifies when a cell is played
  - `OnWin`: Notifies when a player wins
  - `OnDraw`: Notifies when game ends in draw
  - `OnPlayerChanged`: Notifies when turn changes
  - `OnBoardReset`: Notifies when board resets

### Dependency Injection
- Constructor injection for all dependencies
- Enables testability and flexibility

## Components

### Core Layer (Business Logic)

#### IGameBoard
```csharp
public interface IGameBoard
{
    int GetCell(int index);
    void SetCell(int index, int value);
    void Clear();
    int[] GetBoardState();
    bool IsCellEmpty(int index);
}
```

#### IWinChecker
```csharp
public interface IWinChecker
{
    bool CheckWin(IGameBoard board, int player, out int winLine);
    bool CheckDraw(IGameBoard board);
}
```

#### IScoreManager
```csharp
public interface IScoreManager
{
    int GetScore(int player);
    void AddScore(int player);
    void ResetScores();
}
```

### Game Layer

#### GameController
Orchestrates game flow using injected dependencies. Emits events for UI updates.

### UI Layer

#### GamePresenter (MonoBehaviour)
- Initializes dependencies (Composition Root)
- Subscribes to GameController events
- Translates business events to UI updates
- Handles user input from View

#### GameView
- Pure view implementation
- Updates UI elements
- Delegates user actions to Presenter

#### StrikeLineController
- Manages strike line visuals
- Handles positioning logic
- Separated from game logic

## Testing Strategy

### Unit Tests (Core Layer)
All core classes are testable without Unity:
```csharp
[Test]
public void GameBoard_SetCell_UpdatesCell()
{
    var board = new GameBoard();
    board.SetCell(0, 1);
    Assert.AreEqual(1, board.GetCell(0));
}

[Test]
public void WinChecker_CheckWin_DetectsRow()
{
    var board = new GameBoard();
    board.SetCell(0, 1);
    board.SetCell(1, 1);
    board.SetCell(2, 1);
    
    var checker = new WinChecker();
    bool hasWin = checker.CheckWin(board, 1, out int winLine);
    
    Assert.IsTrue(hasWin);
    Assert.AreEqual(0, winLine); // First row
}
```

### Integration Tests
Test GameController with real implementations:
```csharp
[Test]
public void GameController_PlayCell_SwitchesPlayer()
{
    var controller = CreateGameController();
    int initialPlayer = controller.GetCurrentPlayer();
    
    controller.TryPlayCell(0);
    
    Assert.AreNotEqual(initialPlayer, controller.GetCurrentPlayer());
}
```

## Setup Instructions

### GamePresenter Configuration
Assign in Inspector:
- **Game View**: The GameView component
- **X Sprite**: Sprite for X symbol
- **O Sprite**: Sprite for O symbol
- **Reset Delay**: Time before auto-reset (default: 2s)

### GameView Configuration
Assign in Inspector:
- **Cells**: Array of 9 TicTacToeCell components
- **Status Text**: TextMeshProUGUI for turn/result
- **Score Text**: TextMeshProUGUI for scores
- **Strike Line Controller**: StrikeLineController component

### StrikeLineController Configuration
Assign in Inspector:
- **Horizontal Line**: GameObject for horizontal strikes
- **Vertical Line**: GameObject for vertical strikes
- **Diagonal Line 1**: GameObject for \ diagonal
- **Diagonal Line 2**: GameObject for / diagonal
- **Layout Settings**: Cell size, board size, line thickness

## Benefits

### Maintainability
- Clear separation of concerns
- Easy to locate and fix bugs
- Changes isolated to specific layers

### Testability
- Core logic testable without Unity
- Mock dependencies easily
- High code coverage possible

### Extensibility
- Add new features without changing existing code
- Support different board sizes
- Add AI players
- Implement different rule sets

### Scalability
- Add multiplayer support
- Implement undo/redo
- Add replay system
- Support different game modes

## Migration from Old Code

The old `TicTacToeGame.cs` has been removed and replaced with the new architecture:

**Old (Removed):**
- `TicTacToeGame.cs` - Monolithic script with all logic

**New Architecture:**
- **Core Layer**: GameBoard, WinChecker, ScoreManager, GameState
- **Game Layer**: GameController
- **UI Layer**: GamePresenter, GameView, StrikeLineController

Simply attach `GamePresenter` to your existing UI GameObject and configure the references in the Inspector.

## Future Enhancements

### Easy to Add
- **AI Player**: Implement `IPlayer` interface
- **Network Multiplayer**: Replace local GameController with networked version
- **Different Board Sizes**: Create `GameBoard(int size)` with NxN support
- **Custom Win Rules**: Implement new `IWinChecker` (e.g., 4-in-a-row)
- **Replay System**: Add `GameHistory` with Command pattern
- **Undo/Redo**: Implement Memento pattern in GameState

### Minimal Changes Required
All extensions can be added without modifying existing code (OCP principle).

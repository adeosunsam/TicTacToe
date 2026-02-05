# Unity Scripting Standards

This document outlines the coding standards followed in this project, aligned with Unity best practices and C# conventions.

## Table of Contents
1. [Naming Conventions](#naming-conventions)
2. [Code Organization](#code-organization)
3. [Serialization & Inspector](#serialization--inspector)
4. [XML Documentation](#xml-documentation)
5. [Unity Lifecycle](#unity-lifecycle)
6. [Performance Best Practices](#performance-best-practices)
7. [Error Handling](#error-handling)

---

## Naming Conventions

### Fields
```csharp
// Private fields: _camelCase with underscore prefix
private GameController _gameController;
private Coroutine _resetCoroutine;
private int _cellIndex;

// Serialized fields: _camelCase with underscore prefix
[SerializeField] private GameView _gameView;
[SerializeField] private Sprite _xSprite;

// Constants: UPPER_CASE with underscores
private const int PLAYER_X = 1;
private const string SYMBOL_CHILD_NAME = "Symbol";
private static readonly Color TRANSPARENT_COLOR = new Color(1f, 1f, 1f, 0f);
```

### Methods & Properties
```csharp
// PascalCase for public methods
public void Initialize(int index, Action<int> clickCallback)

// PascalCase for private methods
private void UpdateStatusForCurrentPlayer()
private bool IsValidCellIndex(int index)

// Event handlers: Handle + EventName
private void HandleWin(int player, int winLine)
private void HandleCellPlayed(int cellIndex, int player)
```

### Classes & Interfaces
```csharp
// PascalCase for classes
public class GamePresenter : MonoBehaviour

// PascalCase with 'I' prefix for interfaces
public interface IGameView
public interface IGameBoard
```

---

## Code Organization

### Use Regions for Clarity
```csharp
public class GamePresenter : MonoBehaviour
{
    #region Serialized Fields
    // All [SerializeField] variables
    #endregion

    #region Constants
    // All const and static readonly
    #endregion

    #region Private Fields
    // All private instance variables
    #endregion

    #region Unity Lifecycle
    // Awake, Start, Update, OnDestroy, etc.
    #endregion

    #region Initialization
    // Setup and configuration methods
    #endregion

    #region Event Handlers
    // Event callback methods
    #endregion

    #region Public Methods
    // Public API methods
    #endregion

    #region Private Methods
    // Helper and utility methods
    #endregion

    #region Coroutines
    // All IEnumerator methods
    #endregion
}
```

### Logical Grouping
- Group related functionality together
- Order methods by visibility: public → protected → private
- Place Unity lifecycle methods early in the class

---

## Serialization & Inspector

### SerializeField Best Practices
```csharp
[Header("View")]
[SerializeField]
[Tooltip("Reference to the GameView component that handles UI rendering")]
private GameView _gameView;

[Header("Player Symbols")]
[SerializeField]
[Tooltip("Sprite used for Player X")]
private Sprite _xSprite;

[Header("Settings")]
[SerializeField]
[Tooltip("Delay in seconds before automatically resetting the board")]
[Range(0.5f, 10f)]
private float _resetDelay = 2f;
```

### Key Attributes
- **[Header("Name")]**: Group related fields in Inspector
- **[Tooltip("Description")]**: Provide context for designers
- **[Range(min, max)]**: Constrain numeric values
- **[SerializeField]**: Expose private fields to Inspector
- **[DisallowMultipleComponent]**: Prevent duplicate components
- **[RequireComponent(typeof(T))]**: Ensure dependencies

### Default Values
```csharp
// Provide sensible defaults
private TicTacToeCell[] _cells = new TicTacToeCell[9];
private float _resetDelay = 2f;
```

---

## XML Documentation

### Document Public APIs
```csharp
/// <summary>
/// Initializes the cell with its index and click callback.
/// </summary>
/// <param name="index">The index of this cell in the game board (0-8)</param>
/// <param name="clickCallback">Callback to invoke when cell is clicked</param>
public void Initialize(int index, Action<int> clickCallback)
```

### Class-Level Documentation
```csharp
/// <summary>
/// Presenter component that bridges the UI layer with game logic.
/// Follows MVP pattern and manages game flow, event handling, and UI updates.
/// </summary>
[DisallowMultipleComponent]
public class GamePresenter : MonoBehaviour
```

### When to Document
- ✅ All public classes, methods, and properties
- ✅ Complex private methods
- ✅ Important constants
- ❌ Self-explanatory getters/setters
- ❌ Unity lifecycle methods (unless special behavior)

---

## Unity Lifecycle

### Proper Method Order
```csharp
#region Unity Lifecycle

private void Awake()
{
    // Initialize dependencies
    // Get/cache components
    // Subscribe to events
}

private void Start()
{
    // Initial state setup
    // Start coroutines
}

private void OnEnable()
{
    // Subscribe to events
}

private void OnDisable()
{
    // Unsubscribe from events
}

private void OnDestroy()
{
    // Clean up resources
    // Unsubscribe from events
    // Stop coroutines
}

#if UNITY_EDITOR
private void OnValidate()
{
    // Validate serialized fields
    // Enforce constraints
}
#endif

#endregion
```

### Awake vs Start
- **Awake**: Get components, initialize references, subscribe to events
- **Start**: Configure initial state, start coroutines, call other objects

### OnValidate Usage
```csharp
#if UNITY_EDITOR
private void OnValidate()
{
    if (_resetDelay < 0.5f)
    {
        _resetDelay = 0.5f;
    }

    if (_cells != null && _cells.Length != EXPECTED_CELL_COUNT)
    {
        Debug.LogWarning($"Cells array should contain {EXPECTED_CELL_COUNT} elements", this);
    }
}
#endif
```

---

## Performance Best Practices

### Cache Component References
```csharp
// ❌ BAD: GetComponent every time
private void Update()
{
    GetComponent<Button>().onClick.AddListener(OnClick);
}

// ✅ GOOD: Cache in Awake/Initialize
private Button _button;

private void Awake()
{
    _button = GetComponent<Button>();
}

public void Initialize(...)
{
    if (_button == null)
    {
        _button = GetComponent<Button>();
    }
}
```

### Use Constants
```csharp
// ✅ GOOD: Static allocation
private static readonly Color TRANSPARENT_COLOR = new Color(1f, 1f, 1f, 0f);
private static readonly Color OPAQUE_COLOR = new Color(1f, 1f, 1f, 1f);
private const string SYMBOL_CHILD_NAME = "Symbol";

// Usage
_symbolImage.color = TRANSPARENT_COLOR;
```

### Avoid Allocations in Hot Paths
```csharp
// ❌ BAD: New string allocation every frame
void Update()
{
    statusText.text = "Player " + currentPlayer + "'s Turn";
}

// ✅ GOOD: Update only when needed
private void HandlePlayerChanged(int currentPlayer)
{
    _gameView.UpdateStatusText($"Player {GetPlayerName(currentPlayer)}'s Turn");
}
```

### Proper Coroutine Management
```csharp
private Coroutine _resetCoroutine;

private void HandleWin(int player, int winLine)
{
    // Store coroutine reference
    _resetCoroutine = StartCoroutine(ResetAfterDelay());
}

public void OnResetButtonClicked()
{
    // Stop if running
    if (_resetCoroutine != null)
    {
        StopCoroutine(_resetCoroutine);
        _resetCoroutine = null;
    }
}

private void OnDestroy()
{
    // Clean up on destroy
    if (_resetCoroutine != null)
    {
        StopCoroutine(_resetCoroutine);
    }
}
```

---

## Error Handling

### Null Checks with Context
```csharp
// ✅ Provide helpful error messages with context
private void ValidateReferences()
{
    if (_gameView == null)
    {
        Debug.LogError("[GamePresenter] GameView reference is missing!", this);
    }

    if (_xSprite == null)
    {
        Debug.LogWarning("[GamePresenter] X Sprite is not assigned!", this);
    }
}
```

### Defensive Programming
```csharp
public void UpdateCell(int cellIndex, Sprite sprite)
{
    // Validate inputs
    if (!IsValidCellIndex(cellIndex))
    {
        Debug.LogWarning($"[GameView] Invalid cell index: {cellIndex}", this);
        return;
    }

    // Safe to proceed
    _cells[cellIndex].SetSymbol(sprite);
}
```

### Null-Conditional Operators
```csharp
// Use ?. for safe null checks
_strikeLineController?.HideAll();
_gameController?.ResetBoard();
_onCellClicked?.Invoke(_cellIndex);

// Equivalent to:
if (_strikeLineController != null)
{
    _strikeLineController.HideAll();
}
```

### Debug Categories
```csharp
// Use class name prefix for easy filtering
Debug.Log("[GamePresenter] Initializing game...");
Debug.LogWarning("[GameView] Cell array size mismatch!");
Debug.LogError("[TicTacToeCell] Button component not found!", this);
```

---

## Summary Checklist

When writing Unity scripts, ensure:

- ✅ Private fields use `_camelCase` naming
- ✅ Methods use `PascalCase` naming
- ✅ Constants use `UPPER_CASE` naming
- ✅ Code organized with regions
- ✅ SerializeField attributes have [Header] and [Tooltip]
- ✅ All public APIs have XML documentation
- ✅ Components cached in Awake/Initialize
- ✅ Events unsubscribed in OnDestroy
- ✅ OnValidate used for Editor validation
- ✅ Null checks with helpful error messages
- ✅ DisallowMultipleComponent on MonoBehaviours
- ✅ RequireComponent for dependencies
- ✅ No allocations in Update/frequent calls
- ✅ Coroutines properly managed and cleaned up

---

## Examples in This Project

### Excellent Patterns
- [GamePresenter.cs](UI/GamePresenter.cs) - MVP presenter with full lifecycle management
- [GameView.cs](UI/GameView.cs) - Clean interface implementation with validation
- [TicTacToeCell.cs](TicTacToeCell.cs) - Component caching and proper cleanup
- [StrikeLineController.cs](UI/StrikeLineController.cs) - Well-structured UI controller

### Key Techniques Demonstrated
1. **Dependency Injection** - GamePresenter initializes dependencies
2. **Event Management** - Subscribe/Unsubscribe pattern
3. **Validation** - OnValidate and runtime checks
4. **Performance** - Component caching, constant allocation
5. **Documentation** - XML comments throughout
6. **Organization** - Regions for clarity
7. **Error Handling** - Descriptive messages with context

---

*Last Updated: February 5, 2026*

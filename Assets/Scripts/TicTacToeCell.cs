using UnityEngine;
using UnityEngine.UI;
using System;

namespace TicTacToe.UI
{
    /// <summary>
    /// Represents a single cell in the Tic Tac Toe game board.
    /// Handles cell interaction and visual state management.
    /// </summary>
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public class TicTacToeCell : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        [Tooltip("Image component that displays the player symbol (X or O)")]
        private Image symbolImage;

        #endregion

        #region Private Fields

        private Button _button;
        private int _cellIndex;
        private Action<int> _onCellClicked;
        private bool _isInitialized;

        #endregion

        #region Constants

        private const string SYMBOL_CHILD_NAME = "Symbol";
        private static readonly Color TRANSPARENT_COLOR = new Color(1f, 1f, 1f, 0f);
        private static readonly Color OPAQUE_COLOR = new Color(1f, 1f, 1f, 255f);

        #endregion

        #region Unity Lifecycle

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnClick);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Validates references in the Unity Editor.
        /// </summary>
        private void OnValidate()
        {
            if (symbolImage == null)
            {
                symbolImage = transform.Find(SYMBOL_CHILD_NAME)?.GetComponent<Image>();
            }
        }
#endif

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the cell with its index and click callback.
        /// </summary>
        /// <param name="index">The index of this cell in the game board (0-8)</param>
        /// <param name="clickCallback">Callback to invoke when cell is clicked</param>
        public void Initialize(int index, Action<int> clickCallback)
        {
            _cellIndex = index;
            _onCellClicked = clickCallback;

            if (_button == null)
            {
                _button = GetComponent<Button>();
            }

            if (_button != null)
            {
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(OnClick);
            }
            else
            {
                Debug.LogError("[TicTacToeCell] Button component not found!", this);
            }

            if (symbolImage == null)
            {
                symbolImage = transform.Find(SYMBOL_CHILD_NAME)?.GetComponent<Image>();
                
                if (symbolImage == null)
                {
                    Debug.LogWarning($"[TicTacToeCell] Symbol Image not found. Looking for child named '{SYMBOL_CHILD_NAME}'", this);
                }
            }

            Clear();
            _isInitialized = true;
        }

        /// <summary>
        /// Sets the symbol sprite for this cell.
        /// </summary>
        /// <param name="sprite">The sprite to display (X or O symbol)</param>
        public void SetSymbol(Sprite sprite)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("[TicTacToeCell] Attempting to set symbol on uninitialized cell!", this);
                return;
            }

            if (symbolImage != null)
            {
                symbolImage.sprite = sprite;
                symbolImage.color = OPAQUE_COLOR;
            }
        }

        /// <summary>
        /// Clears the cell, removing any symbol and making it transparent.
        /// </summary>
        public void Clear()
        {
            if (symbolImage != null)
            {
                symbolImage.sprite = null;
                symbolImage.color = TRANSPARENT_COLOR;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles button click events.
        /// </summary>
        private void OnClick()
        {
            _onCellClicked?.Invoke(_cellIndex);
        }

        #endregion
    }
}

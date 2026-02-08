using UnityEngine;
using UnityEngine.UI;
using System;

namespace TicTacToe.UI
{
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public class TicTacToeCell : MonoBehaviour
    {
        [SerializeField]
        private Image symbolImage;

        private Button _button;
        private int _cellIndex;
        private Action<int> _onCellClicked;
        private bool _isInitialized;

        private static readonly Color TRANSPARENT_COLOR = new Color(1f, 1f, 1f, 0f);
        private static readonly Color OPAQUE_COLOR = new Color(1f, 1f, 1f, 255f);

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnClick);
            }
        }

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

            Clear();
            _isInitialized = true;
        }

        public void SetSymbol(Sprite sprite)
        {
            if (!_isInitialized)
            {
                return;
            }

            if (symbolImage != null)
            {
                symbolImage.sprite = sprite;
                symbolImage.color = OPAQUE_COLOR;
            }
        }

        public void Clear()
        {
            if (symbolImage != null)
            {
                symbolImage.sprite = null;
                symbolImage.color = TRANSPARENT_COLOR;
            }
        }

        private void OnClick()
        {
            _onCellClicked?.Invoke(_cellIndex);
        }
    }
}

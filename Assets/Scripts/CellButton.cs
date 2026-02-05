using UnityEngine;
using UnityEngine.UI;

public class CellButton : MonoBehaviour
{
    [SerializeField] private Image symbolImage;
    [SerializeField] private Button button;

    private int row;
    private int col;
    private bool isOccupied = false;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
        
        if (symbolImage == null)
            symbolImage = GetComponentInChildren<Image>();
    }

    public void Initialize(int rowIndex, int colIndex)
    {
        row = rowIndex;
        col = colIndex;
        button.onClick.AddListener(OnClick);
        ResetCell();
    }

    private void OnClick()
    {
        if (!isOccupied && GameManager.Instance != null)
        {
            GameManager.Instance.OnCellClicked(row, col, this);
        }
    }

    public void SetSymbol(Sprite sprite, Color color)
    {
        isOccupied = true;
        symbolImage.sprite = sprite;
        symbolImage.color = color;
        symbolImage.enabled = true;
    }

    public void ResetCell()
    {
        isOccupied = false;
        symbolImage.enabled = false;
        symbolImage.sprite = null;
        symbolImage.color = Color.white;
    }
}

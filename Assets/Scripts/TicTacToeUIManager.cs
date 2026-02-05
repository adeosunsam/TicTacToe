using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;

public class TicTacToeUIManager : MonoBehaviour
{
	[Header("Sprites (assign from Assets/Images)")]
	[SerializeField] private Sprite backgroundSprite;
	[SerializeField] private Sprite boardSprite;
	[SerializeField] private Sprite buttonSprite;
	[SerializeField] private Sprite backButtonSprite;
	[SerializeField] private Sprite xSprite;
	[SerializeField] private Sprite oSprite;

	[Header("Colors")]
	[SerializeField] private Color backgroundTint = Color.white;
	[SerializeField] private Color topBarTint = new Color(0.2f, 0.2f, 0.25f, 0.9f);
	[SerializeField] private Color buttonTextColor = Color.black;
	[SerializeField] private Color titleTextColor = Color.white;
	[SerializeField] private Color scoreTextColor = Color.white;

	[Header("Layout")]
	[SerializeField] private Vector2 referenceResolution = new Vector2(1080, 1920);
	[SerializeField] private Vector2 boardSize = new Vector2(650, 650);
	[SerializeField] private Vector2 cellSize = new Vector2(200, 200);
	[SerializeField] private Vector2 cellSpacing = new Vector2(25, 25);
	[SerializeField] private float topBarHeight = 140f;
	[SerializeField] private float titleY = 600f;
	[SerializeField] private float menuBtnY1 = -200f;
	[SerializeField] private float menuBtnY2 = -350f;
	[SerializeField] private float backBtnX = 80f;

	[Header("Fonts")]
	[SerializeField] private int titleSize = 80;
	[SerializeField] private int buttonTextSize = 32;
	[SerializeField] private int scoreTextSize = 30;
	[SerializeField] private int thinkingTextSize = 45;

	private GameObject canvas;
	private GameObject mainMenu;
	private GameObject gameUI;
	private GameObject thinkingPanel;

	private void Start()
	{
		BuildUI();
		ShowMainMenu();
	}

	// ================= BUILD =================

	private void BuildUI()
	{
		if (canvas != null)
		{
			Destroy(canvas);
		}

		canvas = new GameObject("UICanvas");
		Canvas c = canvas.AddComponent<Canvas>();
		c.renderMode = RenderMode.ScreenSpaceOverlay;

		CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = referenceResolution;

		canvas.AddComponent<GraphicRaycaster>();

		CreateEventSystem();

		GameObject root = CreateImagePanel("Root", canvas.transform, backgroundSprite, backgroundTint);

		mainMenu = BuildMainMenu(root.transform);
		gameUI = BuildGameUI(root.transform);
		thinkingPanel = BuildThinkingPanel(root.transform);
	}

	// ================= SCREENS =================

	private GameObject BuildMainMenu(Transform parent)
	{
		GameObject panel = CreateImagePanel("MainMenu", parent, backgroundSprite, backgroundTint);

		GameObject title = CreateText("TIC TAC TOE", panel.transform, titleSize, titleTextColor);
		SetPos(title, 0, titleY);

		GameObject botBtn = CreateButton("Play vs Bot", panel.transform, buttonSprite, buttonTextColor);
		SetPos(botBtn, 0, menuBtnY1);

		GameObject friendBtn = CreateButton("Play vs Friend", panel.transform, buttonSprite, buttonTextColor);
		SetPos(friendBtn, 0, menuBtnY2);

		botBtn.GetComponent<Button>().onClick.AddListener(StartBotGame);
		friendBtn.GetComponent<Button>().onClick.AddListener(StartFriendGame);

		return panel;
	}

	private GameObject BuildGameUI(Transform parent)
	{
		GameObject panel = CreateImagePanel("GameUI", parent, backgroundSprite, backgroundTint);

		GameObject top = CreateImagePanel("TopBar", panel.transform, null, topBarTint);
		SetRect(top, 0, 1, 1, 1, 0, -topBarHeight);

		GameObject back = CreateButton("", top.transform, backButtonSprite != null ? backButtonSprite : buttonSprite, buttonTextColor);
		SetAnchor(back, 0, .5f, 0, .5f, backBtnX, 0);

		back.GetComponent<Button>().onClick.AddListener(ShowMainMenu);

		GameObject score = CreateText("YOU 0  VS  BOT 0\nHARD", top.transform, scoreTextSize, scoreTextColor);
		Center(score);

		GameObject board = new GameObject("Board");
		board.transform.SetParent(panel.transform, false);

		RectTransform rt = board.AddComponent<RectTransform>();
		rt.anchorMin = rt.anchorMax = new Vector2(.5f, .35f);
		rt.sizeDelta = boardSize;

		if (boardSprite != null)
		{
			Image boardImg = board.AddComponent<Image>();
			boardImg.sprite = boardSprite;
			boardImg.preserveAspect = true;
		}

		GridLayoutGroup grid = board.AddComponent<GridLayoutGroup>();
		grid.cellSize = cellSize;
		grid.spacing = cellSpacing;
		grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
		grid.startAxis = GridLayoutGroup.Axis.Horizontal;
		grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		grid.constraintCount = 3;

		for (int i = 0; i < 9; i++)
		{
			GameObject cell = CreateButton("", board.transform, null, buttonTextColor);
			cell.name = "Cell_" + i;
			Image img = cell.GetComponent<Image>();
			img.color = new Color(1f, 1f, 1f, 0f);
			AddSymbolHolder(cell.transform);
		}

		return panel;
	}

	private GameObject BuildThinkingPanel(Transform parent)
	{
		GameObject panel = CreateImagePanel("Thinking", parent, null, new Color(0, 0, 0, .4f));

		GameObject box = CreateImagePanel("Box", panel.transform, null, new Color(0.25f, 0.45f, 0.5f));
		SetRect(box, .1f, .4f, .9f, .6f, 0, 0);

		GameObject txt = CreateText("Bot thinking...", box.transform, thinkingTextSize, Color.white);
		Center(txt);

		panel.SetActive(false);
		return panel;
	}

	// ================= NAVIGATION =================

	public void ShowMainMenu()
	{
		if (mainMenu == null || gameUI == null || thinkingPanel == null)
		{
			return;
		}
		mainMenu.SetActive(true);
		gameUI.SetActive(false);
		thinkingPanel.SetActive(false);
	}

	public void StartBotGame()
	{
		if (mainMenu == null || gameUI == null || thinkingPanel == null)
		{
			return;
		}
		mainMenu.SetActive(false);
		gameUI.SetActive(true);
		thinkingPanel.SetActive(false);
	}

	public void StartFriendGame()
	{
		if (mainMenu == null || gameUI == null || thinkingPanel == null)
		{
			return;
		}
		mainMenu.SetActive(false);
		gameUI.SetActive(true);
		thinkingPanel.SetActive(false);
	}

	public void ShowThinking(bool show)
	{
		if (thinkingPanel != null)
		{
			thinkingPanel.SetActive(show);
		}
	}

	// ================= HELPERS =================

	private void CreateEventSystem()
	{
		if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
		{
			GameObject es = new GameObject("EventSystem");
			es.AddComponent<UnityEngine.EventSystems.EventSystem>();
			es.AddComponent<InputSystemUIInputModule>();
		}
	}

	private GameObject CreateImagePanel(string name, Transform parent, Sprite sprite, Color tint)
	{
		GameObject obj = new GameObject(name);
		obj.transform.SetParent(parent, false);

		Image img = obj.AddComponent<Image>();
		img.sprite = sprite;
		img.color = tint;
		img.preserveAspect = true;

		RectTransform rt = obj.GetComponent<RectTransform>();
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		rt.offsetMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;

		return obj;
	}

	private GameObject CreateButton(string text, Transform parent, Sprite sprite, Color textColor)
	{
		GameObject obj = new GameObject("Button");
		obj.transform.SetParent(parent, false);

		Image img = obj.AddComponent<Image>();
		img.sprite = sprite;
		img.color = Color.white;
		img.preserveAspect = true;

		Button btn = obj.AddComponent<Button>();

		RectTransform rt = obj.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(500, 100);

		if (!string.IsNullOrEmpty(text))
		{
			GameObject txt = new GameObject("Text");
			txt.transform.SetParent(obj.transform, false);

			Text t = txt.AddComponent<Text>();
			t.text = text;
			t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			t.fontSize = buttonTextSize;
			t.alignment = TextAnchor.MiddleCenter;
			t.color = textColor;

			RectTransform trt = txt.GetComponent<RectTransform>();
			trt.anchorMin = Vector2.zero;
			trt.anchorMax = Vector2.one;
			trt.offsetMin = Vector2.zero;
			trt.offsetMax = Vector2.zero;
		}

		return obj;
	}

	private GameObject CreateText(string text, Transform parent, int size, Color color)
	{
		GameObject obj = new GameObject("Text");
		obj.transform.SetParent(parent, false);

		Text t = obj.AddComponent<Text>();
		t.text = text;
		t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		t.fontSize = size;
		t.alignment = TextAnchor.MiddleCenter;
		t.color = color;

		RectTransform rt = obj.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(700, 120);

		return obj;
	}

	private void AddSymbolHolder(Transform parent)
	{
		GameObject symbol = new GameObject("Symbol");
		symbol.transform.SetParent(parent, false);

		Image img = symbol.AddComponent<Image>();
		img.sprite = null;
		img.color = Color.white;
		img.preserveAspect = true;
		img.enabled = false;

		RectTransform rt = img.GetComponent<RectTransform>();
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		rt.offsetMin = new Vector2(10, 10);
		rt.offsetMax = new Vector2(-10, -10);
	}

	private void SetPos(GameObject obj, float x, float y)
	{
		obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
	}

	private void SetRect(GameObject obj, float minX, float minY, float maxX, float maxY, float offX, float offY)
	{
		RectTransform rt = obj.GetComponent<RectTransform>();
		rt.anchorMin = new Vector2(minX, minY);
		rt.anchorMax = new Vector2(maxX, maxY);
		rt.offsetMin = Vector2.zero;
		rt.offsetMax = new Vector2(offX, offY);
	}

	private void SetAnchor(GameObject obj, float minX, float minY, float maxX, float maxY, float x, float y)
	{
		RectTransform rt = obj.GetComponent<RectTransform>();
		rt.anchorMin = new Vector2(minX, minY);
		rt.anchorMax = new Vector2(maxX, maxY);
		rt.anchoredPosition = new Vector2(x, y);
	}

	private void Center(GameObject obj)
	{
		RectTransform rt = obj.GetComponent<RectTransform>();
		rt.anchorMin = rt.anchorMax = new Vector2(.5f, .5f);
		rt.anchoredPosition = Vector2.zero;
	}
}

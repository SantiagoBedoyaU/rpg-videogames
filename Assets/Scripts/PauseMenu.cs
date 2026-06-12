using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    private GameObject pauseUI;
    private GameObject overlay;
    private InputAction pauseAction;

    private void Awake()
    {
        pauseAction = new InputAction("Pause", InputActionType.Button, "<Keyboard>/escape");
        pauseAction.performed += OnPausePerformed;
        pauseAction.Enable();
    }

    private void Start()
    {
        CreatePauseUI();
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        TogglePause();
    }

    private Canvas GetOrCreateCanvas()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null) return canvas;

        GameObject go = new GameObject("PauseCanvas");
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        go.AddComponent<CanvasScaler>();
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private TMP_FontAsset GetFont()
    {
        TMP_FontAsset font = FindFirstObjectByType<TextMeshProUGUI>()?.font;
        if (font != null) return font;
        if (TMP_Settings.defaultFontAsset != null) return TMP_Settings.defaultFontAsset;
        return Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
    }

    private void CreatePauseUI()
    {
        Canvas canvas = GetOrCreateCanvas();
        TMP_FontAsset font = GetFont();

        overlay = new GameObject("PauseOverlay");
        overlay.transform.SetParent(canvas.transform, false);
        Image overlayImg = overlay.AddComponent<Image>();
        overlayImg.color = new Color(0, 0, 0, 0.5f);
        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        overlayRect.anchoredPosition = Vector2.zero;
        overlay.SetActive(false);

        GameObject menuPanel = new GameObject("PausePanel");
        menuPanel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = menuPanel.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(270, 210);
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        Image panelImg = menuPanel.AddComponent<Image>();
        panelImg.color = new Color(0.15f, 0.13f, 0.1f, 0.92f);
        menuPanel.SetActive(false);

        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(menuPanel.transform, false);
        TextMeshProUGUI title = titleObj.AddComponent<TextMeshProUGUI>();
        title.text = "PAUSA";
        title.font = font;
        title.fontSize = 28;
        title.fontStyle = TMPro.FontStyles.Bold;
        title.color = new Color(0.95f, 0.8f, 0.4f);
        title.alignment = TextAlignmentOptions.Center;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.sizeDelta = new Vector2(240, 40);
        titleRect.anchoredPosition = new Vector2(0, 70);

        GameObject resumeBtn = CreateTMPButton("ResumeButton", "REANUDAR", font, menuPanel.transform, new Vector2(0, 10));
        resumeBtn.GetComponent<Button>().onClick.AddListener(Resume);

        GameObject menuBtn = CreateTMPButton("MenuButton", "MENÚ PRINCIPAL", font, menuPanel.transform, new Vector2(0, -40));
        menuBtn.GetComponent<Button>().onClick.AddListener(GoToMainMenu);

        pauseUI = menuPanel;
    }

    private GameObject CreateTMPButton(string name, string label, TMP_FontAsset font, Transform parent, Vector2 pos)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(220, 42);
        btnRect.anchoredPosition = pos;

        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.95f, 0.8f, 0.4f, 0.08f);

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        ColorBlock colors = btn.colors;
        colors.highlightedColor = new Color(0.95f, 0.8f, 0.4f, 0.25f);
        colors.pressedColor = new Color(0.95f, 0.8f, 0.4f, 0.4f);
        btn.colors = colors;

        GameObject lblObj = new GameObject("Label");
        lblObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI lbl = lblObj.AddComponent<TextMeshProUGUI>();
        lbl.text = label;
        lbl.font = font;
        lbl.fontSize = 18;
        lbl.fontStyle = TMPro.FontStyles.Bold;
        lbl.color = new Color(0.95f, 0.8f, 0.4f);
        lbl.alignment = TextAlignmentOptions.Center;
        RectTransform lblRect = lblObj.GetComponent<RectTransform>();
        lblRect.anchorMin = Vector2.zero;
        lblRect.anchorMax = Vector2.one;
        lblRect.sizeDelta = Vector2.zero;

        return btnObj;
    }

    private void OnDestroy()
    {
        if (isPaused)
            Time.timeScale = 1f;
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPausePerformed;
            pauseAction.Dispose();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (pauseUI != null) pauseUI.SetActive(isPaused);
        if (overlay != null) overlay.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        if (isPaused) TogglePause();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

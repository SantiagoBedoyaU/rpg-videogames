using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [Header("Botones de Dificultad")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button playButton;

    [Header("Nombre del Jugador")]
    [SerializeField] private TMP_InputField nameInputField;

    [Header("Feedback Visual")]
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private TextMeshProUGUI difficultyDescriptionText;

    [Header("Rankings")]
    [SerializeField] private TextMeshProUGUI leaderboardText;

    [Header("Créditos")]
    [SerializeField] private TextMeshProUGUI creditsText;

    private Difficulty selectedDifficulty = Difficulty.Normal;

    private void Start()
    {
        SelectDifficulty(Difficulty.Normal);
        StartCoroutine(FetchLeaderboard());
        ShowCredits();

        easyButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Easy));
        normalButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Normal));
        hardButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Hard));
        playButton.onClick.AddListener(StartGame);
    }

    private void ShowCredits()
    {
        string creditos = "Desarrollado por: Juan Manuel Figueroa · Esteban Ochoa · Santiago Bedoya";

        if (creditsText != null)
        {
            creditsText.text = creditos;
            creditsText.fontSize = 16;
            creditsText.alignment = TextAlignmentOptions.Bottom;
            creditsText.color = new Color(0.7f, 0.7f, 0.75f);
            RectTransform rt = creditsText.rectTransform;
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0, 20);
            return;
        }

        GameObject canvasObj = GameObject.Find("Canvas");
        Canvas canvas = canvasObj != null ? canvasObj.GetComponent<Canvas>() : null;
        if (canvas == null)
        {
            canvasObj = new GameObject("CreditsCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        GameObject go = new GameObject("CreditsText");
        go.transform.SetParent(canvas.transform, false);
        Text txt = go.AddComponent<Text>();
        txt.text = creditos;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = 16;
        txt.fontStyle = FontStyle.Italic;
        txt.color = new Color(0.65f, 0.65f, 0.7f, 0.9f);
        txt.alignment = TextAnchor.LowerCenter;
        RectTransform r = go.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(0.5f, 0f);
        r.anchorMax = new Vector2(0.5f, 0f);
        r.pivot = new Vector2(0.5f, 0f);
        r.anchoredPosition = new Vector2(0, 20);
        r.sizeDelta = new Vector2(600, 40);
    }

    private void SelectDifficulty(Difficulty difficulty)
    {
        selectedDifficulty = difficulty;

        ResetButtonColors();
        Image targetImage = GetButtonImage(difficulty);
        if (targetImage != null)
            targetImage.color = selectedColor;

        UpdateDescription(difficulty);
    }

    private void ResetButtonColors()
    {
        SetButtonColor(easyButton, defaultColor);
        SetButtonColor(normalButton, defaultColor);
        SetButtonColor(hardButton, defaultColor);
    }

    private void SetButtonColor(Button button, Color color)
    {
        Image img = button?.GetComponent<Image>();
        if (img != null) img.color = color;
    }

    private Image GetButtonImage(Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Easy => easyButton?.GetComponent<Image>(),
            Difficulty.Normal => normalButton?.GetComponent<Image>(),
            Difficulty.Hard => hardButton?.GetComponent<Image>(),
            _ => null,
        };
    }

    private void UpdateDescription(Difficulty difficulty)
    {
        if (difficultyDescriptionText == null) return;

        string desc = difficulty switch
        {
            Difficulty.Easy => "<color=#5dbf82>Fácil</color> · Enemigos más débiles y oleadas con escalado de daño reducido.",
            Difficulty.Normal => "<color=#e0c060>Normal</color> · Dificultad equilibrada con escalado de daño estándar.",
            Difficulty.Hard => "<color=#e05040>Difícil</color> · Enemigos más fuertes y oleadas con escalado de daño agresivo.",
            _ => "",
        };
        difficultyDescriptionText.text = desc;
    }

    private void StartGame()
    {
        string name = nameInputField != null ? nameInputField.text.Trim() : "";

        if (string.IsNullOrEmpty(name))
        {
            if (nameInputField != null)
                nameInputField.image.color = new Color(0.545f, 0, 0, 1);
            return;
        }

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            GameObject gm = new GameObject("GameManager");
            gameManager = gm.AddComponent<GameManager>();
        }

        gameManager.selectedDifficulty = selectedDifficulty;
        gameManager.currentWave = 0;
        gameManager.playerName = name;

        SceneManager.LoadScene("MainScene");
    }

    [System.Serializable]
    private class PlayerEntry
    {
        public string name;
        public int points;
    }

    [System.Serializable]
    private class PlayerList
    {
        public PlayerEntry[] items;
    }

    private IEnumerator FetchLeaderboard()
    {
        string url = "https://rpg-videogames-api-production.up.railway.app/players";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = "{\"items\":" + request.downloadHandler.text + "}";
                PlayerList list = JsonUtility.FromJson<PlayerList>(json);

                if (list.items != null && list.items.Length > 0)
                {
                    System.Array.Sort(list.items, (a, b) => b.points.CompareTo(a.points));

                    string display = "<color=#ffd700><size=32><b>RANKINGS</b></size></color>\n<color=#887766>━━━━━━━━━━━━━━</color>\n";
                    for (int i = 0; i < list.items.Length; i++)
                    {
                        string medal = i == 0 ? "<color=#ffd700>#1</color>" : i == 1 ? "<color=#e8e8e8>#2</color>" : i == 2 ? "<color=#dd8833>#3</color>" : $"<color=#bbaa88>#{i + 1}</color>";
                        display += $"{medal} <color=#f0e8d8>{list.items[i].name}</color> <color=#ffd700>-</color> <color=#fff8e8>{list.items[i].points} pts</color>\n";
                    }
                    if (leaderboardText != null)
                        leaderboardText.text = display;
                }
            }
            else
            {
                Debug.LogWarning("Error al obtener rankings: " + request.error);
            }
        }
    }
}

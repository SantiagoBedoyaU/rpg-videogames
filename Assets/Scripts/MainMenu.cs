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

    private Difficulty selectedDifficulty = Difficulty.Normal;

    private void Start()
    {
        SelectDifficulty(Difficulty.Normal);
        StartCoroutine(FetchLeaderboard());

        easyButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Easy));
        normalButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Normal));
        hardButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Hard));
        playButton.onClick.AddListener(StartGame);
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
            Difficulty.Easy => "Enemigos más débiles y oleadas con escalado de daño reducido.",
            Difficulty.Normal => "Dificultad equilibrada con escalado de daño estándar.",
            Difficulty.Hard => "Enemigos más fuertes y oleadas con escalado de daño agresivo.",
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
                nameInputField.image.color = Color.red;
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

                    string display = "<b>Rankings</b>\n";
                    for (int i = 0; i < list.items.Length; i++)
                    {
                        display += $"{i + 1}. {list.items[i].name} - {list.items[i].points} pts\n";
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

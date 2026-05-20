using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Botones de Dificultad")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button playButton;

    [Header("Feedback Visual")]
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private TextMeshProUGUI difficultyDescriptionText;

    private Difficulty selectedDifficulty = Difficulty.Normal;

    private void Start()
    {
        SelectDifficulty(Difficulty.Normal);

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
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            GameObject gm = new GameObject("GameManager");
            gameManager = gm.AddComponent<GameManager>();
        }

        gameManager.selectedDifficulty = selectedDifficulty;
        gameManager.currentWave = 0;

        SceneManager.LoadScene("MainScene");
    }
}

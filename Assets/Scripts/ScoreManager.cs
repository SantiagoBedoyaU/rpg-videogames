using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    private int score = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoints(int points)
    {
        score += points;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Puntos: " + score;
        }
    }

    public int GetScore()
    {
        return score;
    }

    public void SendScoreToAPI()
    {
        if (score <= 0) return;

        string playerName = GameManager.Instance != null ? GameManager.Instance.playerName : "Unknown";
        StartCoroutine(PostScore(playerName, score));
    }

    [System.Serializable]
    private class ScorePayload
    {
        public string name;
        public int points;
    }

    private IEnumerator PostScore(string playerName, int points)
    {
        string url = "https://rpg-videogames-api-production.up.railway.app/players/points";

        ScorePayload payload = new ScorePayload { name = playerName, points = points };
        string jsonPayload = JsonUtility.ToJson(payload);

        Debug.Log("Enviando puntaje: " + jsonPayload);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Puntaje enviado correctamente: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogWarning("Error al enviar puntaje (" + request.responseCode + "): " + request.downloadHandler.text);
            }
        }
    }
}

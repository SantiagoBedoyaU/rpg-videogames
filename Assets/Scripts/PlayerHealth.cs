using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Estadísticas de Vida")]
    public float maxHealth = 100f;
    public float currentHealth;
    private Animator myAnimator;
    private PlayerController moveScript;

    [Header("Interfaz")]
    public Slider healthSlider;
    public GameObject gameOverText;
    public Button restartButton;

    void Start()
    {
        currentHealth = maxHealth;
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private bool isDead = false;

    void Awake() {
        myAnimator = GetComponent<Animator>();
        moveScript = GetComponent<PlayerController>();
    }
    
    public void HealToFull()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Jugador derrotado");
        myAnimator.SetTrigger("Death");
        
        if (moveScript != null) moveScript.enabled = false;
        GetComponent<Collider2D>().enabled = false;

        if (gameOverText != null) gameOverText.SetActive(true);
        
        if (restartButton != null) restartButton.gameObject.SetActive(true);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.SendScoreToAPI();
    }

    void RestartGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
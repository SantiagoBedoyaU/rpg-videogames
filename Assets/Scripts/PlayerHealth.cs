using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Estadísticas de Vida")]
    public float maxHealth = 100f;
    public float currentHealth;
    private Animator myAnimator;
    private PlayerController moveScript;

    [Header("Interfaz")]
    public Slider healthSlider;
    public GameObject gameOverText; // Arrastra el objeto "Game Over" aquí

    void Start()
    {
        currentHealth = maxHealth;
        
        // Configuramos el Slider con los valores de vida
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        
        // Actualizamos la barra visual
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

    void Awake() {
        myAnimator = GetComponent<Animator>();
        moveScript = GetComponent<PlayerController>();
    }
    
    void Die()
    {
        Debug.Log("Jugador derrotado");
        myAnimator.SetTrigger("Death");
        
        if (moveScript != null) moveScript.enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // Mostrar el mensaje de Game Over
        if (gameOverText != null) gameOverText.SetActive(true);
    }
}
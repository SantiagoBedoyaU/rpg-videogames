using UnityEngine;
using UnityEngine.UI; // <--- ¡IMPORTANTE! Necesario para manejar la UI

public class PlayerHealth : MonoBehaviour
{
    [Header("Estadísticas de Vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Interfaz")]
    public Slider healthSlider; // Arrastra el Slider aquí en el Inspector

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

    void Die()
    {
        Debug.Log("Jugador derrotado");
        // Aquí podrías activar una pantalla de Game Over
    }
}
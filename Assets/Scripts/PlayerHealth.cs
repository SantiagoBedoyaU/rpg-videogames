using UnityEngine;
using UnityEngine.UI; // <--- ¡IMPORTANTE! Necesario para manejar la UI

public class PlayerHealth : MonoBehaviour
{
    [Header("Estadísticas de Vida")]
    public float maxHealth = 100f;
    public float currentHealth;
    private Animator myAnimator;
    private PlayerController moveScript;

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

    void Awake() {
        myAnimator = GetComponent<Animator>();
        moveScript = GetComponent<PlayerController>();
    }
    
    void Die()
    {
        Debug.Log("Jugador derrotado");
        myAnimator.SetTrigger("Death"); // Asegúrate de que el Trigger en el Animator se llame "Death"
        
        // Desactivamos el script de movimiento para que no pueda seguir caminando
        if (moveScript != null) moveScript.enabled = false;
        
        // Opcional: Desactivar el Collider para que los enemigos lo ignoren al morir
        GetComponent<Collider2D>().enabled = false;
    }
}
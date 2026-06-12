using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;

    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;

    [Header("Configuración de Ataque")]
    public Transform attackPoint; // Crea un objeto vacío frente al jugador y arrástralo aquí
    public float attackRange = 0.5f;
    public LayerMask enemyLayers; // Selecciona la capa "Enemy" en el Inspector

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();

        if (FindFirstObjectByType<PauseMenu>() == null)
        {
            new GameObject("PauseMenu", typeof(PauseMenu));
        }
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void OnDestroy()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
        if (playerControls.Movement.Kick.triggered && movement.sqrMagnitude < 0.1f)
        {
            // Esto evita que fuerzas externas muevan al personaje durante el frame inicial
            rb.linearVelocity = Vector2.zero;
            myAnimator.SetTrigger("Kick");
            PerformAttack(100f);
        }
        if (playerControls.Movement.Punch.triggered && movement.sqrMagnitude < 0.1f)
        {
            // Esto evita que fuerzas externas muevan al personaje durante el frame inicial
            rb.linearVelocity = Vector2.zero;
            myAnimator.SetTrigger("Punch");
            PerformAttack(50f);
        }
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        Move();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection()
    {
        if (movement.x < 0f)
        {
            mySpriteRenderer.flipX = true; // face left
        }
        else if (movement.x > 0f)
        {
            mySpriteRenderer.flipX = false; // face right
        }
    }

    void PerformAttack(float damage) 
    {
        Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(attackPoint.position, 0.5f);
        bool hit = false;

        foreach (Collider2D col in enemigosGolpeados) 
        {
            EnemyAI enemyAI = col.GetComponent<EnemyAI>();
            if (enemyAI != null) 
            {
                enemyAI.TakeDamage(damage); 
                hit = true;
                break;
            }

            BossAI bossAI = col.GetComponent<BossAI>();
            if (bossAI != null) 
            {
                bossAI.TakeDamage(damage); 
                hit = true;
                break;
            }
        }

        if (hit && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.hitSound);
    }

    // Para ver el círculo de ataque en el editor de Unity
    private void OnDrawGizmosSelected() {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}


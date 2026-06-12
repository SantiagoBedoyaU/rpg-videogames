using System.Collections;
using UnityEngine;
using TealFalconEnemySeries;

public class BossAI : MonoBehaviour
{
    private EnemyPathfinding enemyPathfinding;
    private Animator animator;
    private Transform player;
    private DarkKnightController darkKnightController;
    private Rigidbody2D mainRb;

    [Header("Stats")]
    public float health = 500f;
    public float attackDamage = 30f;
    public float stopDistance = 1.5f;
    public float attackCooldown = 0.8f;
    public int scoreValue = 200;

    private bool isAttacking = false;
    private bool isDead = false;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        animator = transform.Find("Root").GetComponent<Animator>();
        mainRb = GetComponent<Rigidbody2D>();

        darkKnightController = GetComponent<DarkKnightController>();
        if (darkKnightController != null)
        {
            darkKnightController.enabled = false;
        }

        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        Collider2D mainCollider = GetComponent<Collider2D>();
        foreach (Collider2D col in allColliders)
        {
            if (col != mainCollider)
            {
                col.isTrigger = true;
            }
        }

        Rigidbody2D[] allRigidbodies = GetComponentsInChildren<Rigidbody2D>();
        foreach (Rigidbody2D rb in allRigidbodies)
        {
            if (rb != mainRb)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        if (mainRb != null)
        {
            mainRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            mainRb.gravityScale = 0f;
        }
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            StartCoroutine(ChaseRoutine());
        }
    }

    private IEnumerator ChaseRoutine()
    {
        while (player != null && !isDead)
        {
            float distancia = Vector2.Distance(transform.position, player.position);

            GirarHaciaObjetivo(player.position);

            if (distancia > stopDistance)
            {
                enemyPathfinding.MoveTo(player.position);
                animator.SetFloat("Speed", 2f);
                animator.SetBool("Guard", false);
                animator.SetBool("Busy", false);
            }
            else
            {
                enemyPathfinding.MoveTo(transform.position);
                animator.SetFloat("Speed", 0f);
                animator.SetBool("Guard", true);

                if (!isAttacking)
                {
                    StartCoroutine(AttackRoutine());
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.25f);
        CausarDano();
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    public void CausarDano()
    {
        if (player == null || isDead) return;

        float distancia = Vector2.Distance(transform.position, player.position);
        if (distancia <= stopDistance + 0.5f)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        health -= damage;

        if (animator != null)
        {
            animator.SetBool("Busy", false);
            animator.SetTrigger("Hurt");
        }

        if (health <= 0)
        {
            BossDie();
        }
    }

    private void BossDie()
    {
        if (isDead) return;
        isDead = true;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddPoints(scoreValue);
        }

        StopAllCoroutines();
        enemyPathfinding.enabled = false;
        GetComponent<Collider2D>().enabled = false;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (darkKnightController != null)
        {
            if (darkKnightController.ExplosionEffect != null)
            {
                Instantiate(darkKnightController.ExplosionEffect, transform.position, Quaternion.identity);
            }
            if (darkKnightController.DeathExplosionSound != null && darkKnightController._Channel != null)
            {
                darkKnightController._Channel.PlayOneShot(darkKnightController.DeathExplosionSound);
            }
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.explosionSound);

        Destroy(gameObject);
    }

    private void GirarHaciaObjetivo(Vector3 objetivo)
    {
        if (objetivo.x > transform.position.x && transform.localScale.x < 0)
        {
            Flip();
        }
        else if (objetivo.x < transform.position.x && transform.localScale.x > 0)
        {
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
}

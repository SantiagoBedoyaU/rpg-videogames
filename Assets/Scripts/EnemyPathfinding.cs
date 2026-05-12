using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float obstacleCheckRadius = 0.5f;
    [SerializeField] private float raycastDistance = 1.2f;

    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private Collider2D enemyCollider;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();
    }

    private void FixedUpdate() {
        Vector2 directionToTarget = (targetPosition - rb.position).normalized;
        
        // Verificar si hay obstáculo en el camino
        RaycastHit2D hit = Physics2D.Raycast(rb.position, directionToTarget, raycastDistance);
        
        if (hit.collider != null && hit.collider != enemyCollider && !hit.collider.isTrigger) {
            // Hay un obstáculo, calcular dirección de evasión
            Vector2 avoidanceDir = CalculateAvoidance(directionToTarget, hit.normal);
            directionToTarget = (directionToTarget + avoidanceDir * 2f).normalized;
        }
        
        rb.MovePosition(rb.position + directionToTarget * (moveSpeed * Time.fixedDeltaTime));
    }

    private Vector2 CalculateAvoidance(Vector2 desiredDir, Vector2 hitNormal) {
        // Reflejar la dirección basada en la normal del obstáculo
        Vector2 reflected = Vector2.Reflect(desiredDir, hitNormal);
        
        // Verificar si la dirección reflejada está libre
        RaycastHit2D hitReflected = Physics2D.Raycast(rb.position, reflected, raycastDistance);
        if (hitReflected.collider == null || hitReflected.collider.isTrigger) {
            return reflected;
        }
        
        // Si no, probar perpendicular
        Vector2 perpendicular = Vector2.Perpendicular(desiredDir);
        RaycastHit2D hitPerp = Physics2D.Raycast(rb.position, perpendicular, raycastDistance);
        if (hitPerp.collider == null || hitPerp.collider.isTrigger) {
            return perpendicular;
        }
        
        // Probar el otro lado
        perpendicular = -Vector2.Perpendicular(desiredDir);
        hitPerp = Physics2D.Raycast(rb.position, perpendicular, raycastDistance);
        if (hitPerp.collider == null || hitPerp.collider.isTrigger) {
            return perpendicular;
        }
        
        // Último recurso: retroceder
        return -desiredDir;
    }

    public void MoveTo(Vector2 newTargetPosition) {
        targetPosition = newTargetPosition;
    }
    
    private void OnDrawGizmos() {
        if (!Application.isPlaying) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, (targetPosition - rb.position).normalized * raycastDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, obstacleCheckRadius);
    }
}

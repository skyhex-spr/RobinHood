using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator Animator;

    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement")]
    public float patrolSpeed = 2f;

    private Rigidbody2D rb;
    private bool goingToB = true;



    [Header("Chase")]
    public float chaseSpeed = 4f;

    [Header("Detection")]
    public float detectRadius = 20f;
    public LayerMask targetLayer;

    [Header("Stop / Idle")]
    public float stopNearPlayerDistance = 5f;
    public float releaseDistance = 7f;

    private Transform target;
    private bool isWaiting = false;

    public bool IsHitting;
    public bool IsDead;

    public int Health = 3;

    private Vector2 attackDir = Vector2.right;
    public float AttackDinstance = 10;

    public PlayerController player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        player = FindAnyObjectByType<PlayerController>();
    }

    private void FixedUpdate()
    {
        if (pointA == null || pointB == null) return;

        float minX = Mathf.Min(pointA.position.x, pointB.position.x);
        float maxX = Mathf.Max(pointA.position.x, pointB.position.x);

        DetectTarget();

        if (target == null)
        {
            isWaiting = false;
            Patrol(minX, maxX);
            return;
        }

        // 2) Player detected but outside A-B -> patrol
        if (!IsTargetInRange(minX, maxX))
        {
            isWaiting = false;
            Patrol(minX, maxX);
            return;
        }

        float distanceToPlayer = Vector2.Distance(rb.position, (Vector2)target.position);

        if (isWaiting)
        {
            //Animator.SetInteger("move", 0);

            attackDir = spriteRenderer.flipX ? Vector2.left : Vector2.right;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, attackDir, AttackDinstance, targetLayer);

            if (hit)
            {
                Animator.SetTrigger("attack");
            }
            else
            {
                Animator.SetInteger("move", 0);
            }

            // Still too close -> keep idle
            if (distanceToPlayer < releaseDistance)
                return;

            // Player moved far enough -> unlock
            isWaiting = false;
        }

        if (distanceToPlayer <= stopNearPlayerDistance)
        {
            FaceTarget(target.position.x);

            float dir = Mathf.Sign(rb.position.x - target.position.x);
            float stopX = target.position.x + (dir * stopNearPlayerDistance);
            stopX = Mathf.Clamp(stopX, minX, maxX);

            rb.MovePosition(new Vector2(stopX, rb.position.y));

            Animator.SetInteger("move", 0);
            isWaiting = true;
            return;
        }

        ChaseTarget(minX, maxX);
    }

    private void Patrol(float minX, float maxX)
    {
        Animator.SetInteger("move", 1);

        float targetX = goingToB ? pointB.position.x : pointA.position.x;
        MoveToX(targetX, patrolSpeed, minX, maxX);

        if (Mathf.Abs(rb.position.x - targetX) < 0.1f)
            goingToB = !goingToB;
    }

    private void MoveToX(float targetX, float speed, float minX, float maxX)
    {
        if (IsHitting)
            return;

        Vector2 pos = rb.position;

        float nextX = Mathf.MoveTowards(pos.x, targetX, speed * Time.fixedDeltaTime);
        nextX = Mathf.Clamp(nextX, minX, maxX);

        rb.MovePosition(new Vector2(nextX, pos.y));

        if (!Mathf.Approximately(targetX, pos.x))
            spriteRenderer.flipX = targetX < pos.x;
    }


    private void DetectTarget()
    {
        Collider2D hit = Physics2D.OverlapCircle(rb.position, detectRadius, targetLayer);
        target = hit ? hit.transform : null;
    }

    private bool IsTargetInRange(float minX, float maxX)
    {
        return target.position.x >= minX && target.position.x <= maxX;
    }

    private void ChaseTarget(float minX, float maxX)
    {
        Animator.SetInteger("move", 1);

        float clampedX = Mathf.Clamp(target.position.x, minX, maxX);
        MoveToX(clampedX, chaseSpeed, minX, maxX);
    }

    private void FaceTarget(float targetX)
    {
        spriteRenderer.flipX = targetX < rb.position.x;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("arrow"))
        {
            Destroy(collision.gameObject);
            Hit();
        }
    }

    private void Hit()
    {
        Animator.SetTrigger("hit");
        IsHitting = true;

        Health--;
        if (Health == 0)
        {
            Animator.SetTrigger("dead");
            Destroy(gameObject, 2);
        }
    }

    public void StopHittong()
    {
        IsHitting = false;
    }

    public void AttackToPlayer()
    {
        player.Hit();
    }

    private void OnDrawGizmosSelected()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stopNearPlayerDistance);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, releaseDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.15f);
            Gizmos.DrawWireSphere(pointB.position, 0.15f);

            Gizmos.color = Color.purple;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)(attackDir.normalized * AttackDinstance));

        }
    }


}

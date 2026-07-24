using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyAI : MonoBehaviour, IDamageable
{
    private enum State { Idle, Chase, Attack, Dead }

    [Header("Stats")]
    [SerializeField] private int maxHealth = 30;
    [SerializeField] private float moveSpeed = 2.5f;

    [Header("Detection & Range")]
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float loseInterestRange = 9f; // gives up chase beyond this, prevents endless following off-screen
    [SerializeField] private float attackRange = 1.2f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 10;

    // Not exposed in the Inspector — resolved at runtime so this works
    // whether the enemy is placed in a scene or spawned from a prefab
    // (e.g. by AmbushTrigger). Requires the Player object to be tagged "Player".
    private Transform player;

    private Rigidbody2D rb;
    private State currentState = State.Idle;
    private int currentHealth;
    private float lastAttackTime = -999f;
    private bool facingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        if (player != null) return;

        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null) player = playerGO.transform;
    }

    private void Update()
    {
        if (currentState == State.Dead) return;

        if (player == null)
        {
            FindPlayer(); // in case this enemy spawned before the player did
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                HandleIdle(distance);
                break;

            case State.Chase:
                HandleChase(distance);
                break;

            case State.Attack:
                HandleAttack(distance);
                break;
        }
    }

    private void HandleIdle(float distance)
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (distance <= detectionRange)
            currentState = State.Chase;
    }

    private void HandleChase(float distance)
    {
        if (distance <= attackRange)
        {
            currentState = State.Attack;
            return;
        }

        if (distance > loseInterestRange)
        {
            currentState = State.Idle;
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        if (direction.x > 0f && !facingRight) Flip();
        else if (direction.x < 0f && facingRight) Flip();
    }

    private void HandleAttack(float distance)
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        if (distance > attackRange)
        {
            currentState = State.Chase;
            return;
        }

        TryAttack();
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        lastAttackTime = Time.time;

        IDamageable target = player.GetComponent<IDamageable>();
        if (target != null) target.TakeDamage(attackDamage);

        Debug.Log($"[EnemyAI] '{name}' attacked player for {attackDamage} damage");
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    // Called by anything that damages enemies — e.g. DatuSuloJetSequence's
    // explosion, which finds IDamageable via Physics2D.OverlapBoxAll.
    public void TakeDamage(int amount)
    {
        if (currentState == State.Dead) return;

        currentHealth -= amount;
        Debug.Log($"[EnemyAI] '{name}' took {amount} damage ({currentHealth}/{maxHealth} HP left)");

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        currentState = State.Dead;
        rb.linearVelocity = Vector2.zero;
        Debug.Log($"[EnemyAI] '{name}' died");

        // TODO: play death animation / VFX before destroying, once art is in.
        Destroy(gameObject, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, loseInterestRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
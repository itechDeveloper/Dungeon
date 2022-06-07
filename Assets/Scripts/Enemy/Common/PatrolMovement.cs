using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
    new Rigidbody2D rigidbody;
    Animator animator;

    RealizingSystem realizingSystem;
    EnemyHealthSystem enemyHealthSystem;

    [SerializeField] Transform groundCheckPos;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask whatIsGround;

    [SerializeField] float patrolSpeed;
    float tempSpeed;
    bool movingRight;

    // Randomize patrol movement
    [SerializeField] float startPatrolTimer;
    float patrolTimer;
    [SerializeField] float startPatrolCooldown;
    float patrolCooldown;
    bool makePatrolMovement;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        realizingSystem = GetComponent<RealizingSystem>();
        enemyHealthSystem = GetComponent<EnemyHealthSystem>();
    }

    void Update()
    {
        if (!realizingSystem.realizedPlayer && !enemyHealthSystem.getHit && !enemyHealthSystem.dead)
        {
            MakePatrolMovement();
        }
        else
        {
            patrolCooldown = startPatrolCooldown;
            patrolTimer = 0f;
        }
    }

    void MakePatrolMovement()
    {
        Collider2D groundInfo = Physics2D.OverlapCircle(groundCheckPos.position, groundCheckRadius, whatIsGround);

        if (patrolTimer > 0)
        {
            makePatrolMovement = true;
            patrolTimer -= Time.deltaTime;
        }
        else
        {
            if (patrolCooldown > 0)
            {
                makePatrolMovement = false;
                patrolCooldown -= Time.deltaTime;
            }
            else
            {
                movingRight = RandomBoolean();
                patrolTimer = startPatrolTimer;
                patrolCooldown = startPatrolCooldown;
            }
        }

        if (!groundInfo)
        {
            if (movingRight)
            {
                movingRight = false;
                transform.eulerAngles = new Vector3(0, 180, 0);
                tempSpeed = -patrolSpeed;
            }
            else
            {
                movingRight = true;
                transform.eulerAngles = new Vector3(0, 0, 0);
                tempSpeed = patrolSpeed;
            }
        }

        if (makePatrolMovement)
        {
            animator.SetTrigger("run");
            animator.SetBool("running", true);
            rigidbody.velocity = Vector2.right * tempSpeed;
        }
        else
        {
            animator.SetBool("running", false);
        }
    }

    bool RandomBoolean()
    {
        if (Random.value >= 0.5)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            tempSpeed = patrolSpeed;
            return true;
        }

        transform.eulerAngles = new Vector3(0, 180, 0);
        tempSpeed = -patrolSpeed;
        return false;
    }
}

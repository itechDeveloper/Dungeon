using UnityEngine;

public class EnemyChasingSystem : MonoBehaviour
{
    GameObject player;
    new Rigidbody2D rigidbody;
    Animator animator;

    RealizingSystem realizingSystem;

    float distanceBetweenPlayer;

    [SerializeField] float speed;

    public bool canChase;
    public bool canRun;

    [SerializeField] float chaseRange;

    Collider2D groundInfo;
    [SerializeField] Transform groundCheckPos;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask whatIsGround;

    [SerializeField] float runDistance;
    [SerializeField] float chaseDistance;
    bool shouldRun;
    bool noWayRight;
    bool noWayLeft;
    [SerializeField] float startRunningCooldown;
    float runningCooldown;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        realizingSystem = GetComponent<RealizingSystem>();
    }

    void Update()
    {
        if (player != null)
        {
            if (realizingSystem.realizedPlayer && !GetComponent<EnemyHealthSystem>().getHit && !GetComponent<EnemyHealthSystem>().dead)
            {
                groundInfo = Physics2D.OverlapCircle(groundCheckPos.position, groundCheckRadius, whatIsGround);
                distanceBetweenPlayer = Mathf.Abs(transform.position.x - player.transform.position.x);
                ChasePlayer();
                RunFromPlayer();

                if (GetComponent<EnemyHealthSystem>().rage)
                {
                    canRun = false;
                    canChase = true;
                }
            }
        }
    }

    void ChasePlayer()
    {
        if (canChase && distanceBetweenPlayer > chaseRange)
        {
            animator.SetTrigger("run");
            animator.SetBool("running", true);

            if (groundInfo)
            {
                if (player.transform.position.x < transform.position.x)
                {
                    rigidbody.velocity = new Vector2(-speed, rigidbody.velocity.y);
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    rigidbody.velocity = new Vector2(speed, rigidbody.velocity.y);
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
            }
            else
            {
                rigidbody.velocity = Vector2.zero;
                animator.SetBool("running", false);
            }
        }
        else
        {
            animator.SetBool("running", false);
        }
    }

    void RunFromPlayer()
    {
        if (canRun)
        {
            if (player.GetComponent<PlayerMovement>().isGrounded)
            {
                if (Mathf.Abs(transform.position.x - player.transform.position.x) < runDistance && Mathf.Abs(transform.position.y - player.transform.position.y) < 1f)
                {
                    shouldRun = true;
                }
                else
                {
                    shouldRun = false;
                    animator.SetBool("running", false);
                    rigidbody.velocity = Vector2.zero;
                }
            }

            if (shouldRun)
            {
                if (!noWayRight && !noWayLeft)
                {
                    if (transform.position.x < player.transform.position.x)
                    {
                        transform.eulerAngles = new Vector3(0, 180, 0);
                        rigidbody.velocity = Vector2.left * speed;
                        animator.SetTrigger("run");
                        animator.SetBool("running", true);
                        if (!groundInfo)
                        {
                            noWayLeft = true;
                            runningCooldown = startRunningCooldown;
                        }
                    }
                    else
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        rigidbody.velocity = Vector2.right * speed;
                        animator.SetTrigger("run");
                        animator.SetBool("running", true);
                        if (!groundInfo)
                        {
                            noWayRight = true;
                            runningCooldown = startRunningCooldown;
                        }
                    }
                }
                else if (noWayRight)
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                    rigidbody.velocity = Vector2.left * speed;
                    animator.SetTrigger("run");
                    animator.SetBool("running", true);

                    if (player.transform.position.x < transform.position.x && runningCooldown <= 0)
                    {
                        noWayRight = false;
                    }
                    else if (runningCooldown > 0)
                    {
                        runningCooldown -= Time.deltaTime;
                    }
                }
                else if (noWayLeft)
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    rigidbody.velocity = Vector2.right * speed;
                    animator.SetTrigger("run");
                    animator.SetBool("running", true);

                    if (player.transform.position.x > transform.position.x && runningCooldown <= 0)
                    {
                        noWayLeft = false;
                    }
                    else if (runningCooldown > 0)
                    {
                        runningCooldown -= Time.deltaTime;
                    }
                }
            }
            else if (Mathf.Abs(transform.position.x - player.transform.position.x) > chaseDistance && Mathf.Abs(transform.position.y - player.transform.position.y) < 1f && groundInfo)
            {
                if (transform.position.x < player.transform.position.x)
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    rigidbody.velocity = Vector2.right * speed;
                    animator.SetTrigger("run");
                    animator.SetBool("running", true);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                    rigidbody.velocity = Vector2.left * speed;
                    animator.SetTrigger("run");
                    animator.SetBool("running", true);
                }
            }
            else
            {
                rigidbody.velocity = Vector2.zero;
                animator.SetBool("running", false);
                if (transform.position.x < player.transform.position.x)
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }
            }
        }
    }
}

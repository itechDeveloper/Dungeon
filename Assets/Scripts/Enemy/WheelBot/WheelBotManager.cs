using UnityEngine;

public class WheelBotManager : MonoBehaviour
{
    GameObject player;
    Animator animator;

    bool wake;

    RealizingSystem realizingSystem;
    EnemyHealthSystem enemyHealthSystem;
    EnemyChasingSystem enemyChasingSystem;

    float distanceBetweenPlayer;

    bool attacking;
    bool canGiveDamage;
    [SerializeField] Transform attackPos;
    [SerializeField] float damage;
    [SerializeField] LayerMask whatIsPlayer;
    [SerializeField] float attackRangeX;
    [SerializeField] float attackRangeY;

    [SerializeField] float startAttackCooldown;
    float attackCooldown;

    Collider2D dashInfo;
    [SerializeField] Transform dashGroundCheckPos;
    [SerializeField] float dashGroundCheckRadius;
    [SerializeField] LayerMask whatIsGround;

    bool dashing;
    [SerializeField] Transform dashPoint;
    [SerializeField] float distanceToDash;
    [SerializeField] float dashCooldown;
    float dashCd;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        realizingSystem = GetComponent<RealizingSystem>();
        enemyHealthSystem = GetComponent<EnemyHealthSystem>();
        enemyChasingSystem = GetComponent<EnemyChasingSystem>();
        canGiveDamage = false;
    }

    void Update()
    {
        if (player != null)
        {
            if (realizingSystem.realizedPlayer && !enemyHealthSystem.getHit && !enemyHealthSystem.dead && wake)
            {
                distanceBetweenPlayer = Mathf.Abs(transform.position.x - player.transform.position.x);

                FaceToPlayer();
                AttackCondition();
                DashCondition();

                if (attacking && canGiveDamage)
                {
                    GiveDamage();
                }
            }
            else
            {
                if (realizingSystem.realizedPlayer && !wake)
                {
                    wake = true;
                    animator.SetTrigger("wakeUp");
                    enemyHealthSystem.canTakeDamage = true;
                }
                else if(wake)
                {
                    wake = false;
                    animator.SetTrigger("sleep");
                    enemyHealthSystem.canTakeDamage = false;
                }
            }

            if (enemyHealthSystem.getHit)
            {
                attacking = false;
                attackCooldown = 0f;
                enemyChasingSystem.canChase = true;
            }
        }
    }

    void FaceToPlayer()
    {
        if (!enemyHealthSystem.rage)
        {
            if (!attacking && !enemyChasingSystem.canRun)
            {
                if (player.transform.position.x < transform.position.x)
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
            }
        }
        else
        {
            if (player.transform.position.x < transform.position.x)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

    void AttackCondition()
    {
        if (!attacking)
        {
            if (attackCooldown <= 0)
            {
                enemyChasingSystem.canChase = true;
                enemyChasingSystem.canRun = false;

                if (distanceBetweenPlayer < attackRangeX)
                {
                    FaceToPlayer();
                    attacking = true;
                    animator.SetTrigger("attack");
                    animator.SetBool("attacking", true);
                    if (!enemyHealthSystem.rage)
                    {
                        enemyChasingSystem.canChase = false;
                        attackCooldown = startAttackCooldown;
                    }
                    else
                    {
                        attackCooldown = (startAttackCooldown / 2);
                    }
                }
            }
            else
            {
                if (!enemyHealthSystem.rage)
                {
                    enemyChasingSystem.canChase = false;
                    enemyChasingSystem.canRun = true;
                }
                attackCooldown -= Time.deltaTime;
            }
        }
    }

    void DashCondition()
    {
        if (!dashing && !attacking)
        {
            if (dashCd <= 0)
            {
                if (player.GetComponent<PlayerMovement>().attacking)
                {
                    if (Mathf.Abs(player.transform.position.x - transform.position.x) < distanceToDash && Mathf.Abs(player.transform.position.y - transform.position.y) < 1f)
                    {
                        dashInfo = Physics2D.OverlapCircle(dashGroundCheckPos.position, dashGroundCheckRadius, whatIsGround);
                        if (dashInfo)
                        {
                            FaceToPlayer();
                            dashing = true;
                            animator.SetTrigger("dash");
                            animator.SetBool("dashing", true);
                            enemyHealthSystem.canTakeDamage = false;
                        }
                    }
                }
            }
            else
            {
                dashCd -= Time.deltaTime;
            }
        }
    }

    public void Dash()
    {
        transform.position = dashPoint.position;
        animator.SetBool("dashing", false);
        dashing = false;
        dashCd = dashCooldown;
        enemyHealthSystem.canTakeDamage = true;
        attackCooldown = 0f;
    }

    void GiveDamage()
    {
        if (canGiveDamage)
        {
            Collider2D[] playerToDamage = Physics2D.OverlapBoxAll(attackPos.position, new Vector2(attackRangeX, attackRangeY), whatIsPlayer);
            for (int i = 0; i < playerToDamage.Length; i++)
            {
                if (playerToDamage[i].GetComponent<PlayerHealthSystem>() != null)
                {
                    playerToDamage[i].GetComponent<PlayerHealthSystem>().TakeDamage(damage);
                    canGiveDamage = false;
                }
            }
        }
    }

    public void CanGiveDamage()
    {
        canGiveDamage = true;
    }

    public void CannotGiveDamage()
    {
        canGiveDamage = false;
    }

    public void EndOfAttack()
    {
        attacking = false;
        animator.SetBool("attacking", false);
        attackCooldown = startAttackCooldown;

        enemyChasingSystem.canChase = true;
    }

    public void WakeUp()
    {
        animator.SetBool("wake", true);
        animator.SetTrigger("run");
    }

    public void Sleep()
    {
        animator.SetBool("wake", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos.position, new Vector2(attackRangeX, attackRangeY));
    }
}

using UnityEngine;

public class HellBotManager : MonoBehaviour
{
    GameObject player;
    Animator animator;

    RealizingSystem realizingSystem;
    EnemyHealthSystem enemyHealthSystem;
    EnemyChasingSystem enemyChasingSystem;

    float distanceBetweenPlayer;

    bool attacking;
    bool canGiveDamage;
    [SerializeField] Transform attackPos;
    [SerializeField] Transform shootPos;
    [SerializeField] float damage;
    [SerializeField] LayerMask whatIsPlayer;
    [SerializeField] float attackRange;

    [SerializeField] float startAttackCooldown;
    float attackCooldown;

    [SerializeField] float shootRangeX;
    [SerializeField] float shootRangeY;
    [SerializeField] float shootRange;

    [SerializeField] float startShootCooldown;
    float shootCooldown;

    bool meleeAttacking;

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
            if (realizingSystem.realizedPlayer && !enemyHealthSystem.getHit && !enemyHealthSystem.dead)
            {
                distanceBetweenPlayer = Mathf.Abs(transform.position.x - player.transform.position.x);

                FaceToPlayer();
                AttackCondition();

                if (attacking && canGiveDamage)
                {
                    GiveDamage();
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
            if (shootCooldown <= 0)
            {
                enemyChasingSystem.canChase = true;
                enemyChasingSystem.canRun = false;

                if (distanceBetweenPlayer < shootRange)
                {
                    FaceToPlayer();
                    attacking = true;
                    animator.SetTrigger("shoot");
                    animator.SetBool("shooting", true);
                    if (!enemyHealthSystem.rage)
                    {
                        enemyChasingSystem.canChase = false;
                        attackCooldown = startAttackCooldown;
                        shootCooldown = startShootCooldown;
                    }
                    else
                    {
                        attackCooldown = (startAttackCooldown / 2);
                        shootCooldown = (startShootCooldown / 2);
                    }

                    meleeAttacking = false;
                }
            }
            else
            {
                if (attackCooldown <= 0)
                {
                    enemyChasingSystem.canChase = true;
                    enemyChasingSystem.canRun = false;

                    if (distanceBetweenPlayer < attackRange)
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

                        meleeAttacking = true;
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

                shootCooldown -= Time.deltaTime;
            }
        }
    }

    void GiveDamage()
    {
        if (canGiveDamage)
        {
            if (meleeAttacking)
            {
                Collider2D[] playerToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsPlayer);
                for (int i = 0; i < playerToDamage.Length; i++)
                {
                    if (playerToDamage[i].GetComponent<PlayerHealthSystem>() != null)
                    {
                        playerToDamage[i].GetComponent<PlayerHealthSystem>().TakeDamage(damage);
                        canGiveDamage = false;
                    }
                }
            }
            else
            {
                Collider2D[] playerToDamage = Physics2D.OverlapBoxAll(shootPos.position, new Vector2(shootRangeX, shootRangeY), whatIsPlayer);
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
        animator.SetBool("shooting", false);
        attackCooldown = startAttackCooldown;

        enemyChasingSystem.canChase = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(shootPos.position, new Vector2(shootRangeX, shootRangeY));
    }
}
using UnityEngine;

public class FlamerManager : MonoBehaviour
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
    [SerializeField] float damage;
    [SerializeField] LayerMask whatIsPlayer;
    [SerializeField] float attackRangeX;
    [SerializeField] float attackRangeY;
    [SerializeField] float startAttackRange;

    [SerializeField] float startAttackCooldown;
    float attackCooldown;

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

            if (enemyHealthSystem.rage)
            {
                attackCooldown = 0f;
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

                if (distanceBetweenPlayer < startAttackRange)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos.position, new Vector2(attackRangeX, attackRangeY));
    }
}

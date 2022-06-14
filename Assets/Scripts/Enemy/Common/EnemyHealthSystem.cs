using UnityEngine;

public class EnemyHealthSystem : MonoBehaviour
{
    Animator animator;
    new Rigidbody2D rigidbody;

    internal bool dead;
    internal bool getHit;

    internal bool canTakeDamage;

    public float maxHealth;
    float currentHealth;

    Color defaultColor;
    new SpriteRenderer renderer;

    [SerializeField] GameObject deathEffect;

    int hitCounter;
    [SerializeField] float startHitCounterCooldown;
    float hitCounterCooldown;

    [SerializeField] GameObject hitEffect;
    [SerializeField] Transform effectPos;
    [SerializeField] float damageDazeSpeed;

    internal bool rage;
    [SerializeField] bool canRage;
    [SerializeField] int rageCounter;
    [SerializeField] float startRageTimer;
    float rageTimer;
    [SerializeField] float startRageCooldown;
    float rageCooldown;
    [SerializeField] Color rageColor;

    public float startGetHitCooldown;
    float getHitCooldown;

    void Start()
    {
        animator = GetComponent<Animator>();
        SetHealth();

        canTakeDamage = true;
        renderer = GetComponent<SpriteRenderer>();
        defaultColor = renderer.color;

        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (getHitCooldown > 0)
        {
            getHitCooldown -= Time.deltaTime;
        }
        else
        {
            getHit = false;
            animator.SetBool("hit", false);
        }

        if (canRage)
        {
            RageCondition();
        }
    }

    public void TakeDamage(float damage)
    {
        if (canTakeDamage)
        {
            if (getHitCooldown <= 0)
            {
                currentHealth -= damage;

                if (currentHealth <= 0)
                {
                    Death();
                }
                else
                {
                    getHit = true;
                    getHitCooldown = startGetHitCooldown;
                    animator.SetTrigger("getHit");
                    animator.SetBool("hit", true);

                    if (rageCooldown <= 0)
                    {
                        hitCounter++;
                        hitCounterCooldown = startHitCounterCooldown;
                    }

                    Instantiate(hitEffect, effectPos.position, Quaternion.identity);

                    if (GameObject.FindGameObjectWithTag("Player").transform.position.x < transform.position.x)
                    {
                        rigidbody.AddForce(new Vector2(damageDazeSpeed, rigidbody.velocity.y));
                    }
                    else
                    {
                        rigidbody.AddForce(new Vector2(-damageDazeSpeed, rigidbody.velocity.y));
                    }
                }
            }
        }
    }

    void RageCondition()
    {
        if (rageTimer <= 0)
        {
            rage = false;
            canTakeDamage = true;
            renderer.color = defaultColor;

            if (rageCooldown <= 0)
            {
                if (hitCounter >= rageCounter)
                {
                    rageTimer = startRageTimer;
                    rageCooldown = startRageCooldown;
                    canTakeDamage = false;
                }

                if (hitCounterCooldown <= 0)
                {
                    hitCounter = 0;
                }
                else
                {
                    hitCounterCooldown -= Time.deltaTime;
                }
            }
            else
            {
                rageCooldown -= Time.deltaTime;
            }
        }
        else
        {
            rage = true;
            canTakeDamage = false;
            renderer.color = new Color(1, 0, 0, 1);
            hitCounter = 0;
            rageTimer -= Time.deltaTime;
        }
    }

    void SetHealth()
    {
        currentHealth = maxHealth;
    }

    void Death()
    {
        if (!dead)
        {
            animator.SetTrigger("death");
            animator.SetBool("dead", true);
            dead = true;
            Instantiate(deathEffect, effectPos.position, Quaternion.identity);
        }
        else
        {
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        }
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}

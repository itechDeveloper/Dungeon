using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    // Components
    new Rigidbody2D rigidbody;
    Animator animator;
    PlayerHealthSystem playerHealthSystem;

    // Stamina
    public Slider slider;
    internal float currentStamina;
    public float maxStamina;

    public float staminaRegen;

    // Horizontal movement
    float horizontalMove;
    public float runSpeed;
    public float accelaration;
    public float minSpeed;
    public float maxSpeed;
    public float boostStaminaRequirement;
    public float boostSpeed;
    public float boostAccelaration;

    // Jump
    public float jumpStaminaRequirement;
    private float verticalMove;
    public float jumpForce;

    public Transform groundCheck;
    internal bool isGrounded;
    public float checkRadius;
    public LayerMask whatIsGround;

    // Double Jump
    public int extraJumpsValue;
    private int extraJumps;

    // Attack
    public float attackStaminaRequirement;
    float startAttackTimer;
    float attackTimer;
    internal bool canReceiveInput;
    internal bool attacking;
    internal bool attack1;
    internal bool attack2;
    internal bool attack3;
    bool canAirAttack;

    bool canGiveUpwardDamage;
    bool canGiveDamage;

    public float damage;
    public Transform upwardAttackPosition;
    public Transform attackPosition;
    public float attackRange;
    public LayerMask whatIsEnemies;

    [SerializeField] float attackDazeSpeed;

    // Dash
    public float rollStaminaRequirement;
    internal bool rolling;
    public float rollSpeed;
    public float rollCoolDown;
    private float rollCoolDownTimer;

    // Block
    public float blockStaminaRequirement;
    internal bool blocking;
    public float startBlockCooldown;
    float blockCooldown;

    // Wall Slide
    bool isTouchingWall;
    public Transform wallCheck;
    bool wallSliding;
    public float wallSlidingSpeed;

    public float wallForceX;
    public float wallForceY;
    public float startWallJumpTimer;
    float wallJumpTimer;

    private void init()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerHealthSystem = GetComponent<PlayerHealthSystem>();
        canReceiveInput = true;
        startAttackTimer = .2f;
    }

    void Awake()
    {
        SetMaxStamina();
        init();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, whatIsGround);

        verticalMove = rigidbody.velocity.y;

        if (attacking)
        {
            Attack();
        }

        if (!GetComponent<PlayerHealthSystem>().dead)
        {
            StaminaRegen();

            if (!GetComponent<PlayerHealthSystem>().getHit)
            {
                if (isGrounded)
                {
                    canAirAttack = true;

                    if (!rolling)
                    {
                        attackTimer -= Time.deltaTime;

                        if (!blocking && GetComponent<PlayerSkillSystem>().canUseSkill)
                        {
                            if (attackTimer <= 0)
                            {
                                AttackCondition();
                            }

                            if (!attacking)
                            {
                                Roll();
                                BlockCondition();
                            }
                        }
                    }
                }
                else
                {
                    WallSliding();
                    AirAttack();
                    attackTimer = startAttackTimer;
                }

                if (!attacking && !blocking && GetComponent<PlayerSkillSystem>().canUseSkill)
                {
                    if (!rolling && wallJumpTimer <= 0)
                    {
                        Move();
                        Flip();
                        Jump();
                    }
                }
                else
                {
                    rigidbody.velocity = Vector2.zero;
                    if (!attacking)
                    {

                    }
                }
            }
            else
            {
                rigidbody.velocity = Vector2.zero;
            }
        }
        else
        {
            if (isGrounded)
            {
                rigidbody.velocity = Vector2.zero;
            }
            else
            {
                rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
            }
        }
    }
    
    void Move()
    {
        animator.SetFloat("horizontalMovement", Mathf.Abs(horizontalMove));

        if (horizontalMove == 0)
        {
            runSpeed = 5;
        }

        horizontalMove = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.F))
        {
            if (currentStamina >= boostStaminaRequirement)
            {
                runSpeed = Mathf.Clamp(runSpeed, minSpeed, boostSpeed);
                runSpeed += boostAccelaration * Time.deltaTime;
                currentStamina -= boostStaminaRequirement * Time.deltaTime;
                SetCurrentStamina();
            }
        }
        else
        {
            runSpeed = Mathf.Clamp(runSpeed, minSpeed, maxSpeed);
            runSpeed += accelaration * Time.deltaTime;
        }

        rigidbody.velocity = new Vector2(horizontalMove * runSpeed, rigidbody.velocity.y);
    }

    void Flip()
    {
        if (horizontalMove < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (horizontalMove > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void Jump()
    {
        animator.SetFloat("verticalMovement", verticalMove);

        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

        if (Input.GetKeyDown(KeyCode.Space) && extraJumps > 0)
        {
            if (currentStamina >= jumpStaminaRequirement)
            {
                animator.SetTrigger("jump");
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
                extraJumps--;
                currentStamina -= jumpStaminaRequirement;
                SetCurrentStamina();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space) && extraJumps == 0 && isGrounded)
        {
            if (currentStamina >= jumpStaminaRequirement)
            {
                animator.SetTrigger("jump");
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
                currentStamina -= jumpStaminaRequirement;
                SetCurrentStamina();
            }
        }
    }

    void AttackCondition()
    {
        if (canReceiveInput)
        {
            if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.E))
            {
                if (currentStamina >= attackStaminaRequirement)
                {
                    attacking = true;
                    animator.SetTrigger("upwardAttack");
                    animator.SetBool("attacking", true);
                    canReceiveInput = false;

                    currentStamina -= attackStaminaRequirement;
                    SetCurrentStamina();
                }
            }
            else if (Input.GetKey(KeyCode.E))
            {
                if (currentStamina >= attackStaminaRequirement)
                {
                    if (transform.localScale == new Vector3(1,1,1))
                    {
                        rigidbody.AddForce(new Vector2(attackDazeSpeed, 0));
                    }
                    else
                    {
                        rigidbody.AddForce(new Vector2(-attackDazeSpeed, 0));
                    }

                    if (!attack1)
                    {
                        attacking = true;
                        attack1 = true;
                        animator.SetTrigger("attack1Trigger");
                        animator.SetBool("attack1", true);
                        animator.SetBool("attacking", true);
                        canReceiveInput = false;
                    }
                    else if (!attack2)
                    {
                        attack2 = true;
                        animator.SetTrigger("attack2Trigger");
                        animator.SetBool("attack2", true);
                        canReceiveInput = false;
                    }
                    else
                    {
                        attack3 = true;
                        animator.SetTrigger("attack3Trigger");
                        animator.SetBool("attack3", true);
                        canReceiveInput = false;
                    }

                    currentStamina -= attackStaminaRequirement;
                    SetCurrentStamina();
                }
            }
        }
    }

    void AirAttack()
    {
        if (canReceiveInput)
        {
            if (Input.GetKey(KeyCode.E) && !attacking)
            {
                if (currentStamina > attackStaminaRequirement && canAirAttack)
                {
                    canReceiveInput = false;
                    attacking = true;
                    animator.SetTrigger("airAttack");
                    animator.SetBool("attacking", true);
                    currentStamina -= attackStaminaRequirement;
                    SetCurrentStamina();

                    canAirAttack = false;
                    rigidbody.velocity = Vector2.zero;
                }
            }
        }
    }
    
    public void EndOfAttack1()
    {
        canReceiveInput = true;
        animator.SetBool("attack1", false);
    }

    public void EndOfAttack2()
    {
        canReceiveInput = true;
        animator.SetBool("attack2", false);
    }

    public void EndOfAttacks()
    {
        attacking = false;
        attack1 = false;
        attack2 = false;
        attack3 = false;
        animator.SetBool("attack1", false);
        animator.SetBool("attack2", false);
        animator.SetBool("attack3", false);
        animator.SetBool("attacking", false);
        canReceiveInput = true;
    }

    void Attack()
    {
        if (canGiveDamage)
        {
            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, whatIsEnemies);
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                if (enemiesToDamage[i].GetComponent<EnemyHealthSystem>() != null)
                {
                    enemiesToDamage[i].GetComponent<EnemyHealthSystem>().TakeDamage(damage);
                }
            }
        }

        if (canGiveUpwardDamage)
        {
            Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(upwardAttackPosition.position, attackRange, whatIsEnemies);
            for (int i = 0; i < enemiesToDamage.Length; i++)
            {
                if (enemiesToDamage[i].GetComponent<EnemyHealthSystem>() != null)
                {
                    enemiesToDamage[i].GetComponent<EnemyHealthSystem>().TakeDamage(damage);
                }
            }
        }
    }

    public void CanGiveDamage()
    {
        canGiveDamage = true;
    }

    public void CanGiveUpwardAttack()
    {
        canGiveUpwardDamage = true;
    }

    public void CannotGiveDamage()
    {
        canGiveUpwardDamage = false;
        canGiveDamage = false;
    }

    void Roll()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (rollCoolDownTimer < 0)
            {
                if (currentStamina >= rollStaminaRequirement)
                {
                    rolling = true;
                    rollCoolDownTimer = rollCoolDown;
                    animator.SetTrigger("roll");
                    animator.SetBool("rolling", true);
                    currentStamina -= rollStaminaRequirement;
                    SetCurrentStamina();

                    playerHealthSystem.canTakeDamage = false;
                }
            }
        }
        else
        {
            rollCoolDownTimer -= Time.deltaTime;
        }

        if (rolling)
        {
            if (transform.localScale == new Vector3(-1, 1, 1))
            {
                rigidbody.velocity = Vector2.left * rollSpeed;
            }
            else
            {
                rigidbody.velocity = Vector2.right * rollSpeed;
            }
        }
    }

    public void EndOfRolling()
    {
        rolling = false;
        animator.SetBool("rolling", false);
        playerHealthSystem.canTakeDamage = true;
    }

    void BlockCondition()
    {
        if (blockCooldown <= 0)
        {
            if (canReceiveInput)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    if (currentStamina >= blockStaminaRequirement)
                    {
                        animator.SetTrigger("block");
                        animator.SetBool("blocking", true);
                        blocking = true;
                        canReceiveInput = false;
                        currentStamina -= blockStaminaRequirement;
                        SetCurrentStamina();
                    }
                }
            }
        }
        else
        {
            blockCooldown -= Time.deltaTime;
        }
    }

    public void EndOfBlocking()
    {
        animator.SetBool("blocking", false);
        blocking = false;
        canReceiveInput = true;
        blockCooldown = startBlockCooldown;
    }

    void SetMaxStamina()
    {
        slider.maxValue = maxStamina;
        currentStamina = maxStamina;
        slider.value = currentStamina;
    }

    void SetCurrentStamina()
    {
        slider.value = currentStamina;
    }

    void StaminaRegen()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegen * Time.deltaTime;
        }
        else if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }

        SetCurrentStamina();
    }

    void WallSliding()
    {
        if (isTouchingWall)
        {
            if (transform.localScale == new Vector3(1, 1, 1) && Input.GetKey(KeyCode.RightArrow))
            {
                animator.SetTrigger("wallSlide");
                animator.SetBool("wallSliding", true);
                wallSliding = true;
                extraJumps = 1;
            }
            else if (transform.localScale == new Vector3(-1, 1, 1) && Input.GetKey(KeyCode.LeftArrow))
            {
                animator.SetTrigger("wallSlide");
                animator.SetBool("wallSliding", true);
                wallSliding = true;
                extraJumps = 1;
            }
            else
            {
                wallSliding = false;
                animator.SetBool("wallSliding", false);
            }
        }
        else
        {
            wallSliding = false;
            animator.SetBool("wallSliding", false);
        }

        if (wallSliding)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Clamp(rigidbody.velocity.y, -wallSlidingSpeed, float.MaxValue));

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (currentStamina >= jumpStaminaRequirement)
                {
                    animator.SetTrigger("jump");
                    wallJumpTimer = startWallJumpTimer;
                    currentStamina -= jumpStaminaRequirement;
                    SetCurrentStamina();
                }      
            }
        }

        if (wallJumpTimer > 0)
        {
            if (transform.localScale == new Vector3(1, 1, 1))
            {
                rigidbody.velocity = new Vector2(-wallForceX, wallForceY);
            }
            else
            {
                rigidbody.velocity = new Vector2(wallForceX, wallForceY);
            }

            wallJumpTimer -= Time.deltaTime;
        }
    }

    public void ResetParams() {
        attacking = false;
        attack1 = false;
        attack2 = false;
        attack3 = false;
        animator.SetBool("attack1", false);
        animator.SetBool("attack2", false);
        animator.SetBool("attack3", false);
        animator.SetBool("attacking", false);
        canReceiveInput = true;
        animator.SetBool("blocking", false);
        blocking = false;
        canReceiveInput = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition.position, attackRange);
    }
}
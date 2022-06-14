using UnityEngine;

public class LaserGuardManager : MonoBehaviour
{
    Animator animator;

    public bool lookingRight;
    public float viewDistance;
    public float hearDistance;
    public float teleportDistance;

    GameObject player;
    bool realized;
    bool teleported;

    public float landingOffsetX;
    public float landingOffsetY;

    bool attacking;
    bool readyToAttack;
    bool playerRight;

    bool teleporting;

    public float teleportingCooldown;
    float teleportingTimer;
    public float delayCooldown;
    float delayTimer;

    Vector3 target;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (!GetComponent<EnemyHealthSystem>().dead)
        {
            if (!realized)
            {
                // sprite is looking right as default
                if (lookingRight)
                {
                    if (!teleported)
                    {
                        //looking right
                        if (transform.eulerAngles.y < 90)
                        {
                            //player is at left side
                            if (transform.position.x > player.transform.position.x)
                            {
                                if (Vector2.Distance(transform.position, player.transform.position) < hearDistance)
                                {
                                    realized = true;
                                    TeleportToPlayer();
                                    playerRight = false;
                                }
                            }
                            else
                            {
                                if (Vector2.Distance(transform.position, player.transform.position) < viewDistance)
                                {
                                    realized = true;
                                    TeleportToPlayer();
                                    playerRight = true;
                                }
                            }
                        }
                        else //looking left
                        {
                            //player is at left side
                            if (transform.position.x > player.transform.position.x)
                            {
                                if (Vector2.Distance(transform.position, player.transform.position) < viewDistance)
                                {
                                    realized = true;
                                    TeleportToPlayer();
                                    playerRight = false;
                                }
                            }
                            else
                            {
                                if (Vector2.Distance(transform.position, player.transform.position) < hearDistance)
                                {
                                    realized = true;
                                    TeleportToPlayer();
                                    playerRight = true;
                                }
                            }
                        }
                    }
                }

                else
                {
                    //looking left
                    if (transform.eulerAngles.y < 90)
                    {
                        //player is at left side
                        if (transform.position.x > player.transform.position.x)
                        {
                            if (Vector2.Distance(transform.position, player.transform.position) < viewDistance)
                            {
                                realized = true;
                                TeleportToPlayer();
                                playerRight = false;
                            }
                        }
                        else
                        {
                            if (Vector2.Distance(transform.position, player.transform.position) < hearDistance)
                            {
                                realized = true;
                                TeleportToPlayer();
                                playerRight = true;
                            }
                        }
                    }
                    else //looking right
                    {
                        //player is at left side
                        if (transform.position.x > player.transform.position.x)
                        {
                            if (Vector2.Distance(transform.position, player.transform.position) < hearDistance)
                            {
                                realized = true;
                                TeleportToPlayer();
                                playerRight = false;
                            }
                        }
                        else
                        {
                            if (Vector2.Distance(transform.position, player.transform.position) < viewDistance)
                            {
                                realized = true;
                                TeleportToPlayer();
                                playerRight = true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (player.transform.position.x < transform.position.x)
                {
                    playerRight = false;
                }
                else
                {
                    playerRight = true;
                }

                if (teleportingTimer <= 0)
                {
                    Action();
                }
                else
                {
                    teleportingTimer -= Time.deltaTime;
                }
            }

            FacingToPlayer();
            LaserAttack();
        }
    }

    void FaceToPlayer()
    {
        if (!transform.GetComponent<EnemyHealthSystem>().getHit)
        {
            if (lookingRight)
            {
                if (transform.position.x > player.transform.position.x)
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
            }
            else
            {
                if (transform.position.x > player.transform.position.x)
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

    void FacingToPlayer()
    {
        if (realized && !attacking)
        {
            FaceToPlayer();
        }
    }

    void Action()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > teleportDistance || (Mathf.Abs(transform.position.y - player.transform.position.y) > 1f)
            || Vector2.Distance(transform.position, player.transform.position) < 1.5f)
        {
            TeleportToPlayer();
        }
        else
        {
            teleportingTimer = teleportingCooldown;
            readyToAttack = true;
            delayTimer = delayCooldown;
        }
    }

    void TeleportToPlayer()
    {
        if (player.GetComponent<PlayerMovement>().isGrounded && !teleporting)
        {
            teleporting = true;
            animator.SetBool("teleporting", teleporting);
            teleportingTimer = teleportingCooldown;
            target = player.transform.position + ReturnOffset(playerRight);
        }
    }

    Vector3 ReturnOffset(bool playerOnRight)
    {
        Vector3 offset;
        if (playerOnRight)
        {
            offset = new Vector3(-landingOffsetX, landingOffsetY, 0);
        }
        else
        {
            offset = new Vector3(landingOffsetX, landingOffsetY, 0);
        }

        return offset;
    }

    public void Teleport()
    {
        readyToAttack = true;
        delayTimer = delayCooldown;

        transform.position = target;
        teleported = true;
        teleporting = false;
        animator.SetBool("teleporting", teleporting);
    }

    public void LaserAttack()
    {
        if (readyToAttack)
        {
            if (delayTimer <= 0)
            {
                transform.GetChild(0).GetComponent<LaserAttack>().canAttack = true;
                readyToAttack = false;
            }
            else
            {
                FaceToPlayer();
                delayTimer -= Time.deltaTime;
            }
        }
    }

    public void StartAttacking()
    {
        attacking = true;
    }

    public void EndAttacking()
    {
        attacking = false;
    }
}

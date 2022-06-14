using UnityEngine;

public class LaserAttack : MonoBehaviour
{
    private Animator animator;
    public float rayLength;
    public float rotationSpeed;
    private Vector3 rotation;
    float laserTimer;
    public float startLaserTimer;
    private float delayRotation;
    public float startDelayRotation;

    bool hitPlayer;
    bool playedAnim;

    internal bool canAttack;
    public float damage;

    GameObject player;

    void Awake()
    {
        animator = GetComponentInParent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (!transform.parent.GetComponent<EnemyHealthSystem>().dead)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * rayLength, Color.red);

            if (canAttack && !playedAnim)
            {
                laserTimer = startLaserTimer;
                delayRotation = startDelayRotation;
                animator.SetTrigger("attack");
                playedAnim = true;
                hitPlayer = false;
                if (transform.parent.eulerAngles.y <= 90)
                {
                    transform.eulerAngles = new Vector3(0, 0, -140f);
                    rotation = new Vector3(0, 0, 1);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, 140f);
                    rotation = new Vector3(0, 0, -1);
                }

            }

            Attack();
        }
    }

    public void Attack()
    {
        if (transform.parent.GetComponent<EnemyHealthSystem>().getHit)
        {
            laserTimer = 0f;
            playedAnim = false;
        }

        if (laserTimer > 0)
        {
            laserTimer -= Time.deltaTime;
            if (delayRotation <= 0)
            {
                transform.Rotate(rotation * rotationSpeed * Time.deltaTime);
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.TransformDirection(Vector2.up), rayLength);
                foreach (var hit in hits)
                {
                    if (hit.collider.name == "Player" && !hitPlayer)
                    {
                        player.GetComponent<PlayerHealthSystem>().TakeDamage(damage);
                        hitPlayer = true;
                    }
                }
                playedAnim = false;
            }
            else
            {
                delayRotation -= Time.deltaTime;
            }

            canAttack = false;
        }
    }
}

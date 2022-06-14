using UnityEngine;

public class RealizingSystem : MonoBehaviour
{
    [SerializeField] bool circleRealizing;
    [SerializeField] bool twoDirectionBoxRealizing;

    [SerializeField] float realizeRadius;
    [SerializeField] float outRealizeRadius;

    [SerializeField] float realizeDistanceX;
    [SerializeField] float realizeDistanceY;

    [SerializeField] Transform center;
    Vector2 viewPoint;

    internal bool realizedPlayer;

    GameObject player;

    EnemyHealthSystem enemyHealthSystem;

    [SerializeField] GameObject exclamationMark;
    [SerializeField] GameObject questionMark;
    [SerializeField] float markCooldown;
    float excMarkCd;
    float qstMarkCd;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyHealthSystem = GetComponent<EnemyHealthSystem>();
    }

    void Update()
    {
        if (realizedPlayer)
        {
            questionMark.SetActive(false);
            qstMarkCd = 0f;

            if (excMarkCd > 0)
            {
                exclamationMark.SetActive(true);
                excMarkCd -= Time.deltaTime;
            }
            else
            {
                exclamationMark.SetActive(false);
            }
        }
        else
        {
            exclamationMark.SetActive(false);
            excMarkCd = 0f;

            if (qstMarkCd > 0)
            {
                questionMark.SetActive(true);
                qstMarkCd -= Time.deltaTime;
            }
            else
            {
                questionMark.SetActive(false);
            }
        }

        if (enemyHealthSystem.getHit)
        {
            realizedPlayer = true;
            excMarkCd = markCooldown;

            if (player.transform.position.x > transform.position.x)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }

        if (circleRealizing)
        {
            if (Vector2.Distance(center.position, player.transform.position) < realizeRadius)
            {
                realizedPlayer = true;
                excMarkCd = markCooldown;
            }
            else if (Vector2.Distance(center.position, player.transform.position) > outRealizeRadius)
            {
                realizedPlayer = false;
                qstMarkCd = markCooldown;
            }
        }
        else
        {
            if (twoDirectionBoxRealizing)
            {
                if (player != null)
                {
                    if (player.GetComponent<PlayerMovement>().isGrounded)
                    {
                        if (!realizedPlayer)
                        {
                            if (Mathf.Abs(center.position.x - player.transform.position.x) < realizeDistanceX && Mathf.Abs(center.position.y - player.transform.position.y) < realizeDistanceY)
                            {
                                realizedPlayer = true;
                                excMarkCd = markCooldown;
                            }
                        }
                        else
                        {
                            if (Mathf.Abs(center.transform.position.x - player.transform.position.x) > realizeDistanceX * 3 || Mathf.Abs(center.transform.position.y - player.transform.position.y) > realizeDistanceY)
                            {
                                realizedPlayer = false;
                                qstMarkCd = markCooldown;
                            }
                        }
                    }
                }
            }
            else
            {
                if (transform.eulerAngles == new Vector3(0, 0, 0))
                {
                    viewPoint = new Vector2(center.position.x + realizeDistanceX, center.position.y);
                }
                else
                {
                    viewPoint = new Vector2(center.position.x - realizeDistanceX, center.position.y);
                }

                if (player != null)
                {
                    if (player.GetComponent<PlayerMovement>().isGrounded)
                    {
                        if (!realizedPlayer)
                        {
                            if (Mathf.Abs(viewPoint.x - player.transform.position.x) < realizeDistanceX && Mathf.Abs(viewPoint.y - player.transform.position.y) < realizeDistanceY)
                            {
                                realizedPlayer = true;
                                excMarkCd = markCooldown;
                            }
                        }
                        else
                        {
                            if (Mathf.Abs(center.transform.position.x - player.transform.position.x) > realizeDistanceX * 2 || Mathf.Abs(center.transform.position.y - player.transform.position.y) > realizeDistanceY)
                            {
                                realizedPlayer = false;
                                qstMarkCd = markCooldown;
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (circleRealizing)
        {
            Gizmos.DrawWireSphere(center.position, realizeRadius);
            Gizmos.DrawWireSphere(center.position, outRealizeRadius);
        }
        else
        {
            if (twoDirectionBoxRealizing)
            {
                Gizmos.DrawWireCube(center.position, new Vector2(realizeDistanceX, realizeDistanceY));
            }
            else
            {
                Gizmos.DrawWireCube(new Vector2(center.position.x + realizeDistanceX, center.position.y), new Vector2(realizeDistanceX * 2, realizeDistanceY * 2));
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthSystem : MonoBehaviour
{
    public Slider slider;
    Animator animator;

    internal bool dead;
    internal bool getHit;

    public float maxHealth;
    internal float currentHealth;

    internal bool canTakeDamage;

    void Start()
    {
        canTakeDamage = true;
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        SetMaxHealth();
    }

    public void TakeDamage(float damage)
    {
        if (canTakeDamage && !dead)
        {
            currentHealth -= damage;
            SetHealth(currentHealth);
            if (currentHealth <= 0)
            {
                animator.SetTrigger("death");
                animator.SetBool("dead", true);
                dead = true;
            }
            else
            {
                animator.SetTrigger("getHit");
                animator.SetBool("hit", true);
                getHit = true;
            }
        }
    }

    public void EndOfTakeDamage()
    {
        getHit = false;
        canTakeDamage = true; ;
        animator.SetBool("hit", false);
    }

    public void EndOfDeath()
    {
        Destroy(gameObject);
    }

    public void SetMaxHealth()
    {
        slider.maxValue = maxHealth;
        currentHealth = maxHealth;
        slider.value = currentHealth;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
    }

    public void CannotTakeDamage()
    {
        canTakeDamage = false;
    }
}

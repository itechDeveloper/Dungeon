using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillSystem : MonoBehaviour
{
    Animator animator;

    public Slider slider;
    public float maxMana;
    float currentMana;

    internal bool canUseSkill;
    internal bool usingSkill;

    float skillCooldown;

    public float stabAttackManaRequirement;
    public float startStabAttackCooldown;
    float stabAttackCooldown;

    public float stabSequenceAttackManaRequirement;
    public float startStabSequenceAttackCooldown;
    float stabSequenceAttackCooldown;

    public float slamAttackManaRequirement;
    public float startSlamAttackCooldown;
    float slamAttackCooldown;

    public float powerAttackManaRequirement;
    public float startPowerAttackCooldown;
    float powerAttackCooldown;

    void Start()
    {
        canUseSkill = true;
        animator = GetComponent<Animator>();
        SetMaxMana();
    }

    void Update()
    {
        if (GetComponent<PlayerMovement>().isGrounded)
        {
            if (skillCooldown <= 0)
            {
                StabAttack();
                StabSequenceAttack();
                SlamAttack();
                PowerAttack();
            }
            else
            {
                skillCooldown -= Time.deltaTime;
            }
        }
        else
        {
            skillCooldown = 0.1f;
        }

        if (stabAttackCooldown > 0)
        {
            stabAttackCooldown -= Time.deltaTime;
        }

        if (stabSequenceAttackCooldown > 0)
        {
            stabSequenceAttackCooldown -= Time.deltaTime;
        }
    }

    public void EndOfSkills()
    {
        animator.SetBool("usingSkill", false);
        canUseSkill = true;
    }

    void StabAttack()
    {
        if (stabAttackCooldown <= 0)
        {
            if (canUseSkill)
            {
                if (Input.GetKey(KeyCode.Alpha1))
                {
                    if (currentMana >= stabAttackManaRequirement)
                    {
                        animator.SetBool("usingSkill", true);
                        animator.SetTrigger("stabAttack");
                        canUseSkill = false;
                        stabAttackCooldown = startStabAttackCooldown;
                        currentMana -= stabAttackManaRequirement;
                        SetManaBar();
                    }
                }
            }
        }
    }

    void StabSequenceAttack()
    {
        if (stabSequenceAttackCooldown <= 0)
        {
            if (canUseSkill)
            {
                if (Input.GetKey(KeyCode.Alpha2))
                {
                    if (currentMana >= stabSequenceAttackManaRequirement)
                    {
                        animator.SetBool("usingSkill", true);
                        animator.SetTrigger("stabSequenceAttack");
                        canUseSkill = false;
                        stabSequenceAttackCooldown = startStabSequenceAttackCooldown;
                        currentMana -= stabSequenceAttackManaRequirement;
                        SetManaBar();
                    }
                }
            }
        }
    }

    void SlamAttack()
    {
        if (slamAttackCooldown <= 0)
        {
            if (canUseSkill)
            {
                if (Input.GetKey(KeyCode.Alpha3))
                {
                    if (currentMana >= slamAttackManaRequirement)
                    {
                        animator.SetBool("usingSkill", true);
                        animator.SetTrigger("slamAttack");
                        canUseSkill = false;
                        slamAttackCooldown = startSlamAttackCooldown;
                        currentMana -= slamAttackManaRequirement;
                        SetManaBar();
                    }
                }
            }
        }
    }

    void PowerAttack()
    {
        if (powerAttackCooldown <= 0)
        {
            if (canUseSkill)
            {
                if (Input.GetKey(KeyCode.Alpha4))
                {
                    if (currentMana >= powerAttackManaRequirement)
                    {
                        animator.SetBool("usingSkill", true);
                        animator.SetTrigger("powerAttack");
                        canUseSkill = false;
                        powerAttackCooldown = startPowerAttackCooldown;
                        currentMana -= powerAttackManaRequirement;
                        SetManaBar();
                    }
                }
            }
        }
    }

    void SetMaxMana()
    {
        slider.maxValue = maxMana;
        currentMana = maxMana;
        slider.value = currentMana;
    }

    void SetManaBar()
    {
        slider.value = currentMana;
    }
}

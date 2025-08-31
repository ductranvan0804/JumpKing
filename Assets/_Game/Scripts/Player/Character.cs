using UnityEngine;

public class Character : MonoBehaviour
{
    // Animator component for controlling animations
    [SerializeField] private Animator anim;
    // Health bar UI component
    [SerializeField] protected HealthBar healthBar;

    // Current health of the character
    protected float hp;
    protected float maxHp = 500f;
    // Current animation name to prevent redundant triggers
    private string currentAnimName;

    // Check if the character is dead (health <= 0)
    public bool IsDead => hp <= 0;

    // Called when the character is created
    private void Start()
    {
        // Initialize character properties
        OnInit();
    }

    // Initialize character: set initial health and configure health bar
    public virtual void OnInit()
    {
        hp = maxHp;
        healthBar.OnInit(hp, transform);
    }

    // Handle character despawn (called before destruction)
    public virtual void OnDespawn()
    {
    }

    // Handle character death
    protected virtual void OnDeath()
    {
        // Switch to death animation
        ChangeAnim(Constant.ANIM_FAIL);
        // Schedule despawn after 2 seconds
        Invoke(nameof(OnDespawn), 2f);
    }

    protected virtual void OnRevive()
    {
        hp = maxHp;
        healthBar.SetNewHp(hp);
        ChangeAnim(Constant.ANIM_IDLE);
    }

    // Change animation, avoiding unnecessary trigger resets
    protected void ChangeAnim(string animName)
    {
        if (currentAnimName != animName)
        {
            anim.ResetTrigger(animName);
            currentAnimName = animName;
            anim.SetTrigger(currentAnimName);
        }
    }

    // Handle damage taken by the character
    public void OnHit(float damage)
    {
        Debug.Log("Hit");
        if (!IsDead)
        {
            // Reduce health by damage amount
            hp -= damage;

            if (IsDead)
            {
                hp = 0;
                // Trigger death if health is depleted
                OnDeath();
            }
            // Update health bar with new health value
            healthBar.SetNewHp(hp);
        }
    }
}
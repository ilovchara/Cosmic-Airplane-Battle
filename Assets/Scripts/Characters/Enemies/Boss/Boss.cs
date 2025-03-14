        using UnityEngine;

public class Boss : Enemy
{
    BossHealthBar healthBar;
    // 获取血条画布组件
    Canvas healthBarCanvas;

    [System.Obsolete]
    void Awake()
    {
        healthBar = FindObjectOfType<BossHealthBar>();
        // 获取子对象canvas组件
        healthBarCanvas = healthBar.GetComponentInChildren<Canvas>();
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        healthBar.Initialize(health, maxHealth);
        healthBarCanvas.enabled = true;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.Die();
        }
    }

    public override void Die()
    {
        healthBarCanvas.enabled = false;
        base.Die();
    }

    // 受伤时更新血条
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        healthBar.UpdateStats(health, maxHealth);
    }

    protected override void SetHealth()
    {
        maxHealth += EnemyManager.Instance.WaveNumber * healthFactor;
    }
}

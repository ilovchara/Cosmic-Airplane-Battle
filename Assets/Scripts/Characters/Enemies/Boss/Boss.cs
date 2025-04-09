using UnityEngine;

// Boss类，继承自Enemy，代表游戏中的Boss角色，包含额外的血条显示和与玩家的碰撞处理
public class Boss : Enemy
{
    BossHealthBar healthBar;
    Canvas healthBarCanvas;

    // 初始化Boss组件，获取BossHealthBar和其子Canvas组件
    protected override void Awake()
    {
        base.Awake();
        healthBar = FindAnyObjectByType<BossHealthBar>();
        healthBarCanvas = healthBar.GetComponentInChildren<Canvas>();
    }

    // Boss启用时初始化血条并显示血条画布
    protected override void OnEnable()
    {
        base.OnEnable();
        healthBar.Initialize(health, maxHealth);
        healthBarCanvas.enabled = true;
    }

    // 处理与玩家的碰撞，如果碰撞到玩家则使其死亡
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.Die();
        }
    }

    // Boss死亡时隐藏血条画布，并调用父类死亡逻辑
    public override void Die()
    {
        healthBarCanvas.enabled = false;
        base.Die();
    }

    // Boss受到伤害时，更新血条显示
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        healthBar.UpdateStats(health, maxHealth);
    }

    // 设置Boss最大生命值，随波数增加
    protected override void SetHealth()
    {
        maxHealth += EnemyManager.Instance.WaveNumber * healthFactor;
    }
}

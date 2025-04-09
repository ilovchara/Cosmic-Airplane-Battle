using NUnit.Framework;
using UnityEngine;

// Enemy类，继承自Character，用于定义敌人的基础行为，如死亡处理、掉落物品生成、与玩家的碰撞反应等
public class Enemy : Character
{
    [SerializeField] int deathEnergyBonus = 3;
    [SerializeField] int scorePoint = 100;
    [SerializeField] protected int healthFactor;

    LootSpawner lootSpawner;

    // 获取LootSpawner组件
    protected virtual void Awake()
    {
        lootSpawner = GetComponent<LootSpawner>();
    }

    // 启用时设置生命值
    protected override void OnEnable()
    {
        SetHealth();
        base.OnEnable();
    }

    // 敌人死亡时，增加分数与能量，移出管理列表，生成掉落物
    public override void Die()
    {
        ScoreManager.Instance.AddScore(scorePoint);
        PlayerEnergy.Instance.Obtain(deathEnergyBonus);
        EnemyManager.Instance.RemoveFromList(gameObject);
        lootSpawner.Spawn(transform.position);
        base.Die();
    }

    // 检测与玩家碰撞并触发玩家死亡，同时敌人自我销毁
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.Die();
            Die();
        }
    }

    // 根据当前波次设置最大生命值
    protected virtual void SetHealth()
    {
        maxHealth += (int)(EnemyManager.Instance.WaveNumber / healthFactor);
    }
}

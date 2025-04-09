using System.Collections;
using UnityEngine;

/// <summary>
/// 角色类，管理角色的血量、死亡逻辑以及与 UI 血条的交互。
/// 可被玩家或敌人继承扩展。
/// </summary>
public class Character : MonoBehaviour
{
    #region Fields

    [Header("Health Settings")]
    [SerializeField] private GameObject deathVFX;               // 死亡时的特效对象
    [SerializeField] protected float maxHealth;                 // 最大血量
    [SerializeField] private bool showOnHeadHealthBar = true;   // 是否显示头顶血条

    [Header("UI Settings")]
    [SerializeField] private StatsBar onHeadHealthBar;          // 头顶血条引用

    [Header("Audio Settings")]
    [SerializeField] private AudioData[] deathSFX;              // 死亡音效数组（备用）

    protected float health; // 当前血量值

    #endregion

    #region Unity Methods

    /// <summary>
    /// 初始化角色血量和血条状态。
    /// </summary>
    protected virtual void OnEnable()
    {
        health = maxHealth;

        if (showOnHeadHealthBar)
        {
            ShowOnHeadHealthBar();
        }
        else
        {
            HideOnHeadHealthBar();
        }
    }

    #endregion

    #region Health Management

    /// <summary>
    /// 显示头顶血条，并进行初始化。
    /// </summary>
    public void ShowOnHeadHealthBar()
    {
        onHeadHealthBar.gameObject.SetActive(true);
        onHeadHealthBar.Initialize(health, maxHealth);
    }

    /// <summary>
    /// 隐藏头顶血条。
    /// </summary>
    public void HideOnHeadHealthBar()
    {
        onHeadHealthBar.gameObject.SetActive(false);
    }

    /// <summary>
    /// 对角色造成伤害，更新血量并刷新 UI。
    /// 若血量归零，触发死亡逻辑。
    /// </summary>
    /// <param name="damage">伤害值。</param>
    public virtual void TakeDamage(float damage)
    {
        if (health == 0f) return; // 已死亡则忽略

        health -= damage;

        if (showOnHeadHealthBar && gameObject.activeSelf)
        {
            onHeadHealthBar.UpdateStats(health, maxHealth);
        }

        if (health <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// 恢复角色血量并刷新 UI。
    /// </summary>
    /// <param name="value">恢复值。</param>
    public virtual void RestoreHealth(float value)
    {
        if (health == maxHealth) return;

        health = Mathf.Clamp(health + value, 0f, maxHealth);

        if (showOnHeadHealthBar)
        {
            onHeadHealthBar.UpdateStats(health, maxHealth);
        }
    }

    /// <summary>
    /// 执行角色死亡逻辑，包括死亡特效、隐藏角色。
    /// </summary>
    public virtual void Die()
    {
        health = 0f;

        // 生成死亡特效
        PoolManager.Release(deathVFX, transform.position);

        // 隐藏角色对象
        gameObject.SetActive(false);
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// 持续恢复血量的协程。
    /// </summary>
    /// <param name="waitTime">恢复间隔。</param>
    /// <param name="percent">每次恢复的最大血量百分比。</param>
    /// <returns>协程迭代器。</returns>
    protected IEnumerator HealthRegenerateCoroutine(WaitForSeconds waitTime, float percent)
    {
        while (health < maxHealth)
        {
            yield return waitTime;
            RestoreHealth(maxHealth * percent);
        }
    }

    /// <summary>
    /// 持续造成伤害的协程。
    /// </summary>
    /// <param name="waitTime">伤害间隔。</param>
    /// <param name="percent">每次造成的最大血量百分比伤害。</param>
    /// <returns>协程迭代器。</returns>
    protected IEnumerator DamageOverTimeCoroutine(WaitForSeconds waitTime, float percent)
    {
        while (health > 0f)
        {
            yield return waitTime;
            TakeDamage(maxHealth * percent);
        }
    }

    #endregion
}

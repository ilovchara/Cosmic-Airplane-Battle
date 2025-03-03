using System.Collections;
using UnityEngine;

/// <summary>
/// 角色类，管理角色的血量、死亡以及与血条的交互。
/// </summary>
public class Character : MonoBehaviour
{
    #region Fields

    [Header("Health Settings")]
    [SerializeField] private GameObject deathVFX; // 死亡效果
    [SerializeField] protected float maxHealth; // 最大血量
    [SerializeField] private bool showOnHeadHealthBar = true; // 是否显示头顶血条

    [Header("UI Settings")]
    [SerializeField] private StatsBar onHeadHealthBar; // 头顶血条引用

    [Header("Audio Settings")]
    [SerializeField] private AudioData[] deathSFX; // 死亡音效

    protected float health; // 当前血量

    #endregion

    #region Unity Methods

    /// <summary>
    /// 初始化角色状态。
    /// </summary>
    [System.Obsolete("Use a more specific initialization method if needed.")]
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
    /// 显示头顶血条。
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
    /// 对角色造成伤害。
    /// </summary>
    /// <param name="damage">伤害值。</param>
    public virtual void TakeDamage(float damage)
    {
        // 血条归零时取消显示大血条
        if(health == 0f) return;
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
    /// 恢复角色的血量。
    /// </summary>
    /// <param name="value">恢复的血量值。</param>
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
    /// 处理角色死亡。
    /// </summary>
    public virtual void Die()
    {
        health = 0f;

        // 生成并激活死亡特效
        PoolManager.Release(deathVFX, transform.position);

        // 隐藏角色
        gameObject.SetActive(false);
    }

    #endregion

    #region Coroutines

    /// <summary>
    /// 持续恢复血量的协程。
    /// </summary>
    /// <param name="waitTime">恢复的间隔时间。</param>
    /// <param name="percent">每次恢复的百分比。</param>
    /// <returns>协程枚举器。</returns>
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
    /// <param name="waitTime">伤害的间隔时间。</param>
    /// <param name="percent">每次造成的百分比伤害。</param>
    /// <returns>协程枚举器。</returns>
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

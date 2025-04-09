using System.Collections;
using UnityEngine;

/// <summary>
/// 玩家能量管理类，继承自单例模式 Singleton<PlayerEnergy>
/// 负责能量的获取、消耗、以及过载模式下的持续消耗逻辑
/// </summary>
public class PlayerEnergy : Singleton<PlayerEnergy>
{
    [SerializeField] EnergyBar energyBar; // 能量条 UI 组件
    [SerializeField] float overdriveInterval = 0.1f; // 过载模式的能量消耗间隔

    // 能量相关常量
    public const int MAX = 100;   // 最大能量值
    public const int PERCENT = 1; // 每次消耗的能量单位

    int energy;                  // 当前能量值
    bool available = true;      // 能量是否可获取

    WaitForSeconds waitForOverdriveInterval; // 过载模式中协程的等待时间

    /// <summary>
    /// 初始化单例及协程等待时间对象
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        waitForOverdriveInterval = new WaitForSeconds(overdriveInterval);
    }

    /// <summary>
    /// 初始化能量条并填充最大能量
    /// </summary>
    void Start()
    {
        energyBar.Initialize(energy, MAX);
        Obtain(MAX);
    }

    /// <summary>
    /// 启用时订阅过载事件
    /// </summary>
    void OnEnable()
    {
        PlayerOverdrive.on += PlayerOverdriveOn;
        PlayerOverdrive.off += PlayerOverdriveOff;
    }

    /// <summary>
    /// 禁用时取消订阅过载事件
    /// </summary>
    void OnDisable()
    {
        PlayerOverdrive.on -= PlayerOverdriveOn;
        PlayerOverdrive.off -= PlayerOverdriveOff;
    }

    /// <summary>
    /// 获取能量（增加能量值）
    /// </summary>
    /// <param name="value">要获取的能量值</param>
    public void Obtain(int value)
    {
        // 能量已满、不可用或对象未激活时不进行获取
        if (energy == MAX || !available || !gameObject.activeSelf) return;

        // 更新能量并刷新 UI
        energy = Mathf.Clamp(energy + value, 0, MAX);
        energyBar.UpdateStats(energy, MAX);
    }

    /// <summary>
    /// 消耗能量（减少能量值）
    /// </summary>
    /// <param name="value">要消耗的能量值</param>
    public void Use(int value)
    {
        energy -= value;
        energyBar.UpdateStats(energy, MAX);

        // 能量耗尽时关闭过载模式
        if (energy == 0 && !available)
        {
            PlayerOverdrive.off.Invoke();
        }
    }

    /// <summary>
    /// 判断当前能量是否足够指定值
    /// </summary>
    /// <param name="value">所需能量</param>
    /// <returns>是否足够</returns>
    public bool IsEnough(int value) => energy >= value;

    /// <summary>
    /// 启动过载模式（开始持续消耗能量）
    /// </summary>
    void PlayerOverdriveOn()
    {
        available = false;
        StartCoroutine(nameof(KeepUsingCoroutine));
    }

    /// <summary>
    /// 退出过载模式（停止持续消耗）
    /// </summary>
    void PlayerOverdriveOff()
    {
        available = true;
        StopCoroutine(nameof(KeepUsingCoroutine));
    }

    /// <summary>
    /// 过载模式持续消耗能量的协程
    /// </summary>
    IEnumerator KeepUsingCoroutine()
    {
        while (gameObject.activeSelf && energy > 0)
        {
            yield return waitForOverdriveInterval;
            Use(PERCENT);
        }
    }
}

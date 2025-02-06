using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

// 玩家能量管理类，继承自单例模式 Singleton<PlayerEnergy>
public class PlayerEnergy : Singleton<PlayerEnergy>
{
    [SerializeField] EnergyBar energyBar; // 能量条 UI 组件
    [SerializeField] float overdriveInterval = 0.1f; // 过载模式的消耗间隔

    // 能量相关常量
    public const int MAX = 100; // 最大能量值
    public const int PERCENT = 1; // 每次消耗的能量单位
    
    int energy; // 当前能量值
    bool available = true; // 是否可以正常获取能量

    WaitForSeconds waitForOverdriveInterval; // 过载模式的等待时间对象

    protected override void Awake()
    {
        base.Awake();
        waitForOverdriveInterval = new WaitForSeconds(overdriveInterval);
    }

    void Start()
    {
        // 初始化能量条，并直接填充最大能量值
        energyBar.Initialize(energy, MAX);
        Obtain(MAX);
    }

    void OnEnable()
    {
        // 订阅过载模式事件
        PlayerOverdrive.on += PlayerOverdriveOn;
        PlayerOverdrive.off += PlayerOverdriveOff;
    }

    void OnDisable()
    {
        // 取消订阅过载模式事件
        PlayerOverdrive.on -= PlayerOverdriveOn;
        PlayerOverdrive.off -= PlayerOverdriveOff;
    }

    // 获取能量
    public void Obtain(int value)
    {
        // 如果能量已满、不可用或对象未激活，则不执行
        if (energy == MAX || !available || !gameObject.activeSelf) return;

        // 限制能量范围在 0 - MAX，并更新 UI
        energy = Mathf.Clamp(energy + value, 0, MAX);
        energyBar.UpdateStats(energy, MAX);
    }

    // 消耗能量
    public void Use(int value)
    {
        energy -= value;
        energyBar.UpdateStats(energy, MAX);

        // 如果能量耗尽且不可用，则关闭过载模式
        if (energy == 0 && !available)
        {
            PlayerOverdrive.off.Invoke();
        }
    }

    // 判断能量是否足够
    public bool IsEnough(int value) => energy >= value;

    // 进入过载模式
    void PlayerOverdriveOn()
    {
        available = false;
        StartCoroutine(nameof(KeepUsingCoroutine));
    }

    // 退出过载模式
    void PlayerOverdriveOff()
    {
        available = true;
        StopCoroutine(nameof(KeepUsingCoroutine));
    }

    // 过载模式持续消耗能量的协程
    IEnumerator KeepUsingCoroutine()
    {
        while (gameObject.activeSelf && energy > 0)
        {
            yield return waitForOverdriveInterval;
            Use(PERCENT);
        }
    }
}

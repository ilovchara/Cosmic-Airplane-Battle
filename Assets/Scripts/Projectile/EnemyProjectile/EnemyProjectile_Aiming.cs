using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile_Aiming : Projectile
{
    // 在Awake方法中，查找带有"Player"标签的GameObject并赋值给target变量
    // 该方法在脚本实例被加载时调用一次
    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // 重写OnEnable方法，当脚本启用时调用
    // 该方法在脚本组件被启用时调用，例如当GameObject被激活时
    protected override void OnEnable()
    {
        // 启动MoveDirectionCoroutine协程
        StartCoroutine(nameof(MoveDirectionCoroutine));
        // 调用基类Projectile的OnEnable方法
        base.OnEnable();
    }

    // MoveDirectionCoroutine协程，用于计算敌方弹丸的移动方向
    IEnumerator MoveDirectionCoroutine()
    {
        // 等待一个帧，让协程在下一帧开始执行
        yield return null;

        // 如果目标（玩家）处于激活状态
        if (target.activeSelf)
        {
            // 计算从弹丸当前位置到目标位置的方向，并将其标准化
            // 用于确定弹丸的移动方向
            moveDirection = (target.transform.position - transform.position).normalized;
        }
    }
}
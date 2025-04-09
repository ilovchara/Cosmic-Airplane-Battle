using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 追踪玩家的敌人弹幕类
public class EnemyProjectile_Aiming : Projectile
{
    // 初始化目标为带有 "Player" 标签的对象
    void Awake()
    {
        SetTarget(GameObject.FindGameObjectWithTag("Player"));
    }

    // 启用时启动方向协程并调用父类启用逻辑
    protected override void OnEnable()
    {
        StartCoroutine(nameof(MoveDirectionCoroutine));
        base.OnEnable();
    }

    // 协程：在启用后一帧设置追踪方向
    IEnumerator MoveDirectionCoroutine()
    {
        yield return null;

        if (target.activeSelf)
        {
            moveDirection = (target.transform.position - transform.position).normalized;
        }
    }
}

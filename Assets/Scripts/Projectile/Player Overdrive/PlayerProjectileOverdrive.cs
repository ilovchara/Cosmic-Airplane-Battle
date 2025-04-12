using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileOverdrive : PlayerProjectile
{
    [SerializeField] ProjectileGuidanceSystem guidanceSystem;

    // 启用时设置目标，如果有目标则启动追踪协程，否则按普通子弹行为处理
    protected override void OnEnable()
    {
        SetTarget(EnemyManager.Instance.RandomEnemy);
        transform.rotation = Quaternion.identity;

        if (target == null)
        {
            base.OnEnable();
        }
        else
        {
            StartCoroutine(guidanceSystem.HomingCoroutine(target));
        }
    }
}


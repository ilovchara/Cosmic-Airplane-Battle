using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 敌人控制脚本，控制敌人的随机移动和随机射击
public class EnemyController : MonoBehaviour
{
    [Header("---- Movement Settings ----")]
    [SerializeField] private float paddingX; // X 轴的边距
    [SerializeField] private float paddingY; // Y 轴的边距
    [SerializeField] private float moveSpeed = 2f; // 敌人移动速度
    [SerializeField] private float moveRotationAngle = 25f; // 移动时的旋转角度

    [Header("---- Fire Settings ----")]
    [SerializeField] private GameObject[] projectiles; // 子弹的预制体数组
    [SerializeField] private Transform muzzle; // 子弹发射的起始位置（枪口）
    [SerializeField] private float minFireInterval; // 最小射击间隔时间
    [SerializeField] private float maxFireInterval; // 最大射击间隔时间
    [SerializeField] private AudioData[] projectileLaunchSFX; // 子弹发射音效

    float maxMoveDistancePerFrame;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    void Start()
    {
        maxMoveDistancePerFrame = moveSpeed * Time.fixedDeltaTime;
    }

    void OnEnable()
    {
        StartCoroutine(nameof(RandomlyMovingCoroutine));
        StartCoroutine(nameof(RandomlyFireCoroutine));
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 协程：随机移动逻辑
    /// </summary>
    IEnumerator RandomlyMovingCoroutine()
    {
        transform.position = Viewport.Instance.RandomEnemySpawnPosition(paddingX, paddingY);
        Vector3 targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);

        while (gameObject.activeSelf)
        {
            if (Vector3.Distance(transform.position, targetPosition) >= maxMoveDistancePerFrame)
            {   // moveSpeed * Time.fixedDeltaTime
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxMoveDistancePerFrame);
                transform.rotation = Quaternion.AngleAxis((targetPosition - transform.position).normalized.y * moveRotationAngle, Vector3.right);
            }
            else
            {
                targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);
            }
            yield return waitForFixedUpdate;
        }
    }

    /// <summary>
    /// 协程：随机射击逻辑
    /// </summary>
    IEnumerator RandomlyFireCoroutine()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(minFireInterval, maxFireInterval));

            foreach (var projectile in projectiles)
            {
                PoolManager.Release(projectile, muzzle.position);
            }
            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);
        }
    }
}

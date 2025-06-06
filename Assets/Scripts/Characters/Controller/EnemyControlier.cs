using System.Collections;
using UnityEngine;

/// 敌人控制脚本，控制敌人的随机移动和随机射击
public class EnemyController : MonoBehaviour
{
    [Header("---- Movement Settings ----")]
    [SerializeField] private float moveSpeed = 2f; // 敌人移动速度
    [SerializeField] private float moveRotationAngle = 25f; // 移动时的旋转角度

    [Header("---- Fire Settings ----")]
    [SerializeField] protected GameObject[] projectiles; // 子弹的预制体数组
    [SerializeField] protected Transform muzzle; // 子弹发射的起始位置（枪口）
    [SerializeField] protected float minFireInterval; // 最小射击间隔时间
    [SerializeField] protected float maxFireInterval; // 最大射击间隔时间
    [SerializeField] protected AudioData[] projectileLaunchSFX; // 子弹发射音效
    [SerializeField] protected ParticleSystem muzzleVFX;

    protected float paddingX; // X 轴的边距
    float paddingY; // Y 轴的边距

    protected Vector3 targetPosition; // 追踪目标

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    // 初始化敌人边距
    protected virtual void Awake()
    {
        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2f;
        paddingY = size.y / 2f;
    }

    // 启动时开始随机移动和随机射击
    protected virtual void OnEnable()
    {
        StartCoroutine(nameof(RandomlyMovingCoroutine));
        StartCoroutine(nameof(RandomlyFireCoroutine));
    }

    // 禁用时停止所有协程
    void OnDisable()
    {
        StopAllCoroutines();
    }

    /// 协程：随机移动逻辑
    IEnumerator RandomlyMovingCoroutine()
    {
        transform.position = Viewport.Instance.RandomEnemySpawnPosition(paddingX, paddingY);
        targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);

        while (gameObject.activeSelf)
        {
            if (Vector3.Distance(transform.position, targetPosition) >= moveSpeed * Time.fixedDeltaTime)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
                transform.rotation = Quaternion.AngleAxis((targetPosition - transform.position).normalized.y * moveRotationAngle, Vector3.right);
            }
            else
            {
                targetPosition = Viewport.Instance.RandomRightHalfPosition(paddingX, paddingY);
            }
            yield return waitForFixedUpdate;
        }
    }

    /// 协程：随机射击逻辑
    protected virtual IEnumerator RandomlyFireCoroutine()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(minFireInterval, maxFireInterval));

            if (GameManager.GameState == GameState.GameOver) yield break;

            foreach (var projectile in projectiles)
            {
                PoolManager.Release(projectile, muzzle.position);
            }
            AudioManager.Instance.PlayRandomSFX(projectileLaunchSFX);
            muzzleVFX.Play();
        }
    }
}

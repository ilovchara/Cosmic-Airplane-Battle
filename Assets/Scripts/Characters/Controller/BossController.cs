using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BossController类，继承自EnemyController，控制Boss的攻击行为、激光技能与玩家检测
public class BossController : EnemyController
{
    // --- Variables ---

    [Header("--- Fire Settings ---")]
    [SerializeField] float continuousFireDuration = 1.5f;

    [Header("--- Player Detection ---")]
    [SerializeField] Transform playerDetyectionTransform;
    [SerializeField] Vector3 palyerDetectionSize;
    [SerializeField] LayerMask playerLayer;

    [Header("--- Beam ---")]
    [SerializeField] float beamCooldownTime = 12f;
    [SerializeField] AudioData beamChargingSFX;
    [SerializeField] AudioData beamLaunchSFX;

    [Header("--- State ---")]
    bool isBeamReady;
    int launchBeamID = Animator.StringToHash("launchBeam");

    [Header("--- Components ---")]
    Animator animator;
    Transform playerTransform;

    [Header("--- Timers ---")]
    WaitForSeconds waitForContinuousFireInterval;
    WaitForSeconds waitForFireInterval;
    WaitForSeconds waitBeamCooldownTime;

    [Header("--- Projectiles ---")]
    List<GameObject> magazine;
    AudioData launchSFX;

    // --- Unity Methods ---

    // 初始化Boss控制器，设置计时器、获取组件和玩家Transform
    protected override void Awake()
    {
        base.Awake();
        waitForContinuousFireInterval = new WaitForSeconds(minFireInterval);
        waitForFireInterval = new WaitForSeconds(maxFireInterval);
        waitBeamCooldownTime = new WaitForSeconds(beamCooldownTime);

        magazine = new List<GameObject>(projectiles.Length);
        animator = GetComponent<Animator>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Boss启用时重置激光状态并开始冷却计时
    protected override void OnEnable()
    {
        muzzleVFX.Stop();
        isBeamReady = false;
        StartCoroutine(nameof(BeamCooldownCoroutine));
        base.OnEnable();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(playerDetyectionTransform.position, palyerDetectionSize);
    }

    // --- Attack Methods ---

    // 激活激光武器并播放充能动画与音效
    void ActivateBeamWeapon()
    {
        isBeamReady = false;
        animator.SetTrigger(launchBeamID);
        AudioManager.Instance.PlayRandomSFX(beamChargingSFX);
    }

    // 根据玩家位置加载适当的子弹进入弹匣
    void LoadProjectile()
    {
        magazine.Clear();
        if (Physics2D.OverlapBox(playerDetyectionTransform.position, palyerDetectionSize, 0f, playerLayer))
        {
            magazine.Add(projectiles[0]);
            launchSFX = projectileLaunchSFX[0];
        }
        else
        {
            if (Random.value < 0.5f)
            {
                magazine.Add(projectiles[1]);
                launchSFX = projectileLaunchSFX[1];
            }
            else
            {
                for (int i = 2; i < projectiles.Length; i++)
                {
                    magazine.Add(projectiles[i]);
                }
                launchSFX = projectileLaunchSFX[2];
            }
        }
    }

    // 随机发射子弹的协程，可能会中断以发射激光
    protected override IEnumerator RandomlyFireCoroutine()
    {
        while (isActiveAndEnabled)
        {
            if (GameManager.GameState == GameState.GameOver) yield break;

            if (isBeamReady)
            {
                ActivateBeamWeapon();
                StartCoroutine(nameof(ChasingPlayerCoroutine));
                yield break;
            }

            yield return waitForFireInterval;
            yield return StartCoroutine(nameof(ContinuousFireCoroutine));
        }
    }

    // 连续发射子弹的协程，持续一段时间内多次发射
    IEnumerator ContinuousFireCoroutine()
    {
        LoadProjectile();
        muzzleVFX.Play();
        float continuousFireTimer = 0f;

        while (continuousFireTimer < continuousFireDuration)
        {
            foreach (var projectile in magazine)
            {
                PoolManager.Release(projectile, muzzle.position);
            }
            continuousFireTimer += minFireInterval;
            AudioManager.Instance.PlayRandomSFX(launchSFX);

            yield return waitForContinuousFireInterval;
        }
        muzzleVFX.Stop();
    }

    // 激光武器冷却计时协程，冷却完成后可再次使用
    IEnumerator BeamCooldownCoroutine()
    {
        yield return waitBeamCooldownTime;
        isBeamReady = true;
    }

    // 持续追踪玩家的协程，用于激光发射阶段锁定Y轴位置
    IEnumerator ChasingPlayerCoroutine()
    {
        while (isActiveAndEnabled)
        {
            targetPosition.x = Viewport.Instance.MaxX - paddingX;
            targetPosition.y = playerTransform.position.y;
            yield return null;
        }
    }

    // --- Animation Event Methods ---
    // 用于在动画事件中调用的函数

    // 动画事件：播放激光发射音效
    void AnimationEventLaunchBeam()
    {
        AudioManager.Instance.PlayRandomSFX(beamLaunchSFX);
    }

    // 动画事件：停止激光发射，恢复普通攻击逻辑并开始冷却
    void AnimationEventStopBeam()
    {
        StopCoroutine(nameof(ChasingPlayerCoroutine));
        StartCoroutine(nameof(ContinuousFireCoroutine));
        StartCoroutine(nameof(RandomlyFireCoroutine));
        StartCoroutine(nameof(BeamCooldownCoroutine));
    }
}

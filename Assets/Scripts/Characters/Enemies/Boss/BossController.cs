using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController
{
    // --- Variables ---

    // 发射子弹的持续时间
    [SerializeField] float continuousFireDuration = 1.5f;

    [Header(" --- Player Detection ---")]
    [SerializeField] Transform playerDetyectionTransform; // 玩家检测的Transform
    [SerializeField] Vector3 palyerDetectionSize; // 玩家检测区域的大小
    [SerializeField] LayerMask playerLayer; // 玩家层

    [Header("--- Beam --- ")]
    [SerializeField] float beamCooldownTime = 12f; // 激光冷却时间

    bool isBeamReady; // 激光是否准备好
    // 在动画器中的bool变量 - 使用字符串的方式来链接
    int launchBeamID = Animator.StringToHash("launchBeam");

    Animator animator; // 动画控制器

    WaitForSeconds waitForContinuousFireInterval; // 连续发射间隔
    WaitForSeconds waitForFireInterval; // 发射间隔
    // 时间延迟变量 -记录时间
    WaitForSeconds waitBeamCooldownTime; // 激光冷却时间

    // 弹匣
    List<GameObject> magazine; // 存储子弹的列表

    AudioData launchSFX; // 发射音效
    // 发射的音效
    [SerializeField] AudioData beamChargingSFX; // 激光充能音效
    [SerializeField] AudioData beamLaunchSFX; // 激光发射音效

    Transform playerTransform; // 玩家Transform


    // --- Unity Methods ---

    protected override void Awake()
    {
        base.Awake();
        waitForContinuousFireInterval = new WaitForSeconds(minFireInterval);
        waitForFireInterval = new WaitForSeconds(maxFireInterval);
        waitBeamCooldownTime = new WaitForSeconds(beamCooldownTime);

        magazine = new List<GameObject>(projectiles.Length);
        animator = GetComponent<Animator>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // 找到player的位置 - 为Boss服务
    }

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

    // 激活激光武器
    void ActivateBeamWeapon()
    {
        isBeamReady = false;
        animator.SetTrigger(launchBeamID);
        AudioManager.Instance.PlayRandomSFX(beamChargingSFX);
    }

    // 装载子弹
    void LoadProjectile()
    {
        magazine.Clear();
        // 检测物理重叠效果
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

    // 随机发射协程
    protected override IEnumerator RandomlyFireCoroutine()
    {
        while (isActiveAndEnabled)
        {
            if (GameManager.GameState == GameState.GameOver) yield break;

            if (isBeamReady) // 发射激光的时候
            {
                ActivateBeamWeapon();
                StartCoroutine(nameof(ChasingPlayerCoroutine));
                yield break;
            }

            yield return waitForFireInterval;
            yield return StartCoroutine(nameof(ContinuousFireCoroutine));
        }
    }

    // 连续发射协程
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

    // 激光冷却协程
    IEnumerator BeamCooldownCoroutine()
    {
        yield return waitBeamCooldownTime;
        isBeamReady = true;
    }

    // 追踪玩家协程
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
    // 这里使用动画事件调用

    // 动画事件：发射激光
    void AnimationEventLaunchBeam()
    {
        AudioManager.Instance.PlayRandomSFX(beamLaunchSFX);
    }

    // 动画事件：停止激光
    void AnimationEventStopBeam()
    {
        StopCoroutine(nameof(ChasingPlayerCoroutine)); // 追踪玩家协程 - 停止
        StartCoroutine(nameof(ContinuousFireCoroutine));
        StartCoroutine(nameof(RandomlyFireCoroutine));

        StartCoroutine(nameof(BeamCooldownCoroutine));
    }
}
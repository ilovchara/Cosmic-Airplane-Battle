using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class BossController : EnemyController
{
    // --- Variables ---

    // 发射子弹的持续时间
    [SerializeField] float continuousFireDuration = 1.5f;

    [Header(" --- Player Detection ---")]
    [SerializeField] Transform playerDetyectionTransform;
    [SerializeField] Vector3 palyerDetectionSize;
    [SerializeField] LayerMask playerLayer;

    [Header("--- Beam --- ")]
    [SerializeField] float beamCooldownTime = 12f;

    bool isBeamReady;
    // 再动画器中的bool变量 - 使用字符串的方式来链接
    int launchBeamID = Animator.StringToHash("launchBeam");

    Animator animator;

    WaitForSeconds waitForContinuousFireInterval;
    WaitForSeconds waitForFireInterval;
    // 时间延迟变量 -记录时间
    WaitForSeconds waitBeamCooldownTime;

    // 弹匣
    List<GameObject> magazine;

    AudioData launchSFX;
    // 发射的音效
    [SerializeField] AudioData beamChargingSFX;
    [SerializeField] AudioData beamLaunchSFX;

    Transform playerTransform;


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

    void ActivateBeamWeapon()
    {
        isBeamReady = false;
        animator.SetTrigger(launchBeamID);
        AudioManager.Instance.PlayRandomSFX(beamChargingSFX);
    }

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

    IEnumerator BeamCooldownCoroutine()
    {
        yield return waitBeamCooldownTime;
        isBeamReady = true;
    }

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
    void AnimationEventLaunchBeam()
    {
        AudioManager.Instance.PlayRandomSFX(beamLaunchSFX);
    }

    void AnimationEventStopBeam()
    {
        StopCoroutine(nameof(ChasingPlayerCoroutine)); // 追踪玩家协程 - 停止
        StartCoroutine(nameof(ContinuousFireCoroutine));
        StartCoroutine(nameof(RandomlyFireCoroutine));

        StartCoroutine(nameof(BeamCooldownCoroutine));
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController
{
    // 发射子弹的持续时间
    [SerializeField] float continuousFireDuration = 1.5f;

    [Header(" --- Player Detection ---")]
    [SerializeField] Transform playerDetyectionTransform;
    [SerializeField] Vector3 palyerDetectionSize;
    [SerializeField] LayerMask playerLayer;


    WaitForSeconds waitForContinuousFireInterval;
    WaitForSeconds waitForFireInterval;

    List<GameObject> magazine;

    AudioData launchSFX;

    protected override void Awake()
    {
        base.Awake();
        waitForContinuousFireInterval = new WaitForSeconds(minFireInterval);
        waitForFireInterval = new WaitForSeconds(maxFireInterval);

        magazine = new List<GameObject>(projectiles.Length);
    }
    // 侦测玩家位置 - 发射不同子弹
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



    // Boss独特的攻击模式
    protected override IEnumerator RandomlyFireCoroutine()
    {

        // 当游戏结束的时候停止协程
        if (GameManager.GameState == GameState.GameOver) yield break;

        while (isActiveAndEnabled)
        {
            // 间隔一段时间之后开火
            yield return waitForFireInterval;
            yield return StartCoroutine(nameof(ContinuousFireConroutine));
        }
    }

    IEnumerator ContinuousFireConroutine()
    {
        LoadProjectile();
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

    }





}

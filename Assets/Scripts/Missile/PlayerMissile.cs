using System.Collections;
using UnityEngine;

public class PlayerMissile : PlayerProjectileOverdrive
{
    [SerializeField] private AudioData targetAcquiredVoice = null;

    [Header("=== SPEED CHANGE ===")]
    [SerializeField] private float lowSpeed = 8f;
    [SerializeField] private float highSpeed = 25f;
    [SerializeField] private float variableSpeedDelay = 0.5f;

    [Header("=== Explosion ===")]
    [SerializeField] GameObject explosionVFX = null;
    [SerializeField] AudioData explosionSFX = null;

    [SerializeField] LayerMask enemyLayerMas = default;
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] float explosionDamage = 100f;

    private WaitForSeconds waitVariableSpeedDelay;

    // 初始化延迟时间
    protected override void Awake()
    {
        base.Awake();
        waitVariableSpeedDelay = new WaitForSeconds(variableSpeedDelay);
    }

    // 激活时启动变速协程
    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(nameof(VariableSpeedCoroutine));
    }

    // 碰撞时触发爆炸效果与范围伤害
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        // 在当前位置释放爆炸特效与音效
        PoolManager.Release(explosionVFX, transform.position);
        AudioManager.Instance.PlayRandomSFX(explosionSFX);

        // 检测爆炸范围内的敌人并造成伤害
        var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayerMas);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(explosionDamage);
            }
        }
    }

    // 控制导弹速度的协程
    private IEnumerator VariableSpeedCoroutine()
    {
        float moveSpeed = lowSpeed;
        yield return waitVariableSpeedDelay;
        moveSpeed = highSpeed;

        // 如果锁定了目标，则播放语音提示
        if (target != null)
        {
            AudioManager.Instance.PlayRandomSFX(targetAcquiredVoice);
        }
    }
}

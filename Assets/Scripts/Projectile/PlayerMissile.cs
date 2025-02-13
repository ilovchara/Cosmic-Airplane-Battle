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

    protected override void Awake()
    {
        base.Awake();
        waitVariableSpeedDelay = new WaitForSeconds(variableSpeedDelay);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(nameof(VariableSpeedCoroutine));
    }

    // 可视化爆炸范围
    // void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireSphere(transform.position, explosionRadius);
    // }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        // 碰撞位置释放这个特效
        PoolManager.Release(explosionVFX,transform.position);
        AudioManager.Instance.PlayRandomSFX(explosionSFX);

        var colliders =  Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayerMas);
        foreach( var collider in colliders)
        {
            if(collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(explosionDamage);
            }

        }

    }

    private IEnumerator VariableSpeedCoroutine()
    {
        float moveSpeed = lowSpeed;
        yield return waitVariableSpeedDelay;
        moveSpeed = highSpeed;

        if (target != null)
        {
            AudioManager.Instance.PlayRandomSFX(targetAcquiredVoice);
        }
    }
}
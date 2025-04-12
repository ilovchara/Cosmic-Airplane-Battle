using System.Collections;
using UnityEngine;
// 导弹爆炸检测
public class Explosion : MonoBehaviour
{
    [SerializeField] float explosionDamage = 100f; // 爆炸伤害
    [SerializeField] Collider2D explosionCollider; // 爆炸碰撞体

    WaitForSeconds waitExplosionTime = new WaitForSeconds(0.1f); // 等待爆炸持续时间

    // 当Explosion组件启用时，开始爆炸过程
    void OnEnable()
    {
        StartCoroutine(ExplosionCoroutine());
    }

    // 当触发器与其他碰撞体发生接触时触发
    void OnTriggerEnter2D(Collider2D other)
    {
        // 如果接触到敌人，敌人会受到爆炸伤害
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.TakeDamage(explosionDamage);
        }
    }

    // 爆炸协程，控制爆炸碰撞体的启用和禁用
    IEnumerator ExplosionCoroutine()
    {
        // 当特效生成时启用爆炸检测碰撞体
        explosionCollider.enabled = true;

        // 等待一段时间（爆炸持续时间）
        yield return waitExplosionTime;

        // 爆炸检测完毕后关闭碰撞体
        explosionCollider.enabled = false;
    }
}

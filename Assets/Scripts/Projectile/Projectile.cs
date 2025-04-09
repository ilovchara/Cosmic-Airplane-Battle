using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

// 子弹的基类，定义了子弹的基本属性和行为
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected float moveSpeed = 10f; 
    [SerializeField] protected Vector2 moveDirection; 
    [SerializeField] float damage; 

    [Header("Visual and Audio Effects")]
    [SerializeField] GameObject hitVFX; 
    [SerializeField] AudioData[] hitSFX;

    [Header("Targeting")]
    protected GameObject target; 

    // 激活时启动移动协程
    protected virtual void OnEnable()
    {
        StartCoroutine(MoveDirectly());
    }

    // 控制子弹持续移动的协程
    IEnumerator MoveDirectly()
    {
        while (gameObject.activeSelf)
        {
            Move();
            yield return null;
        }
    }

    // 碰撞检测并处理伤害、特效和音效
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Character>(out Character character))
        {
            character.TakeDamage(damage);
            PoolManager.Release(hitVFX, collision.GetContact(0).point, Quaternion.LookRotation(collision.GetContact(0).normal));
            AudioManager.Instance.PlayRandomSFX(hitSFX);
            gameObject.SetActive(false);
        }
    }

    // 设置目标对象
    protected void SetTarget(GameObject target) => this.target = target;

    // 控制子弹朝指定方向移动
    public void Move() => transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
}

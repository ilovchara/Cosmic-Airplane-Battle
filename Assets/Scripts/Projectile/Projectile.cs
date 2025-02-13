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

    /// <summary>
    /// 当脚本组件被启用时调用，例如当GameObject被激活时
    /// </summary>
    protected virtual void OnEnable()
    {
        StartCoroutine(MoveDirectly());
    }

    /// <summary>
    /// MoveDirectly协程，用于控制子弹的移动
    /// </summary>
    /// <returns>协程的IEnumerator</returns>
    IEnumerator MoveDirectly()
    {
        while (gameObject.activeSelf)
        {
            Move();
            yield return null;
        }
    }

    /// <summary>
    /// 当发生2D碰撞时调用
    /// </summary>
    /// <param name="collision">碰撞信息</param>
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

    protected void SetTarget(GameObject target) => this.target = target;
    public void Move() => transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

}

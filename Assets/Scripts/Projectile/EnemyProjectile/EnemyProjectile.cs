using UnityEngine;
using UnityEngine.EventSystems;

// 敌人子弹类，继承自基类 Projectile
public class EnemyProjectile : Projectile
{
    // 在对象初始化时设置朝向
    void Awake()
    {
        if (moveDirection != Vector2.left)
        {
            transform.rotation = Quaternion.FromToRotation(Vector2.left, moveDirection);
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

// 敌人子弹类，继承自基类 Projectile
public class EnemyProjectile : Projectile
{
    // 初始化方法
    void Awake()
    {
        // 如果移动方向不是默认的向左方向，则调整子弹的旋转方向
        if (moveDirection != Vector2.left)
        {
            // 根据移动方向旋转子弹，使其朝向指定方向
            transform.rotation = Quaternion.FromToRotation(Vector2.left, moveDirection);
        }
    }
}

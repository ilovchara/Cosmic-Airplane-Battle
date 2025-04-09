using UnityEngine;

public class PlayerProjectile : Projectile
{
    TrailRenderer trail;

    // 初始化轨迹组件，并根据发射方向调整子物体旋转
    protected virtual void Awake()
    {
        trail = GetComponentInChildren<TrailRenderer>();

        if (moveDirection != Vector2.right)
        {
            transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector2.right, moveDirection);
        }
    }

    // 子弹禁用时清除轨迹
    void OnDisable()
    {
        trail.Clear();
    }

    // 碰撞时触发能量获取
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        PlayerEnergy.Instance.Obtain(PlayerEnergy.PERCENT);
    }
}

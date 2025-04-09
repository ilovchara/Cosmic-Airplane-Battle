using System.Collections;
using UnityEngine;

// 用于控制导弹（或其他投射物）在飞行过程中追踪目标
public class ProjectileGuidanceSystem : MonoBehaviour
{
    // 导弹制导系统
    [SerializeField] Projectile projectile;
    [SerializeField] float minBallisticAngle = 50f;
    [SerializeField] float maxBallisticAngle = 75f;

    float ballisticAngle;
    Vector3 targetDirection;

    // 控制投射物追踪目标的协程
    public IEnumerator HomingCoroutine(GameObject target)
    {
        ballisticAngle = Random.Range(minBallisticAngle, maxBallisticAngle);

        while (gameObject.activeSelf)
        {
            if (target.activeSelf)
            {
                // 计算朝向目标的方向，并添加一个抛物线角度偏移
                targetDirection = target.transform.position - transform.position;
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg, Vector3.forward);
                transform.rotation *= Quaternion.Euler(0f, 0f, ballisticAngle);
                projectile.Move();
            }
            else
            {
                // 如果目标失效，继续直线前进
                projectile.Move();
            }

            yield return null;
        }
    }
}

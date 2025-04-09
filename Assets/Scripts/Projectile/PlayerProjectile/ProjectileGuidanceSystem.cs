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

    public IEnumerator HomingCoroutine(GameObject target)
    {
        ballisticAngle = Random.Range(minBallisticAngle, maxBallisticAngle);

        while (gameObject.activeSelf)
        {
            if (target.activeSelf)
            {
                targetDirection = target.transform.position - transform.position;
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg, Vector3.forward);
                transform.rotation *= Quaternion.Euler(0f, 0f, ballisticAngle);
                projectile.Move();
            }
            else
            {
                projectile.Move();
            }

            yield return null;
        }
    }
}

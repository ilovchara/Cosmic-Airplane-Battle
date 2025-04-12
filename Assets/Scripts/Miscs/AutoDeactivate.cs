using System.Collections;
using UnityEngine;
/// <summary>
/// 自动回收系统
/// </summary> <summary>
/// 
/// </summary>
public class AutoDeactivate : MonoBehaviour
{
    [Header("GameObject Deactivation Settings")]
    [SerializeField] bool destroyGameObject; // 是否销毁 GameObject，默认为禁用
    [SerializeField] float lifetime = 3f; // 对象存在的时间，单位：秒

    private WaitForSeconds waitLifetime; // 等待指定时间的对象

    void Awake()
    {
        waitLifetime = new WaitForSeconds(lifetime); // 初始化等待时间对象
    }

    void OnEnable()
    {
        StartCoroutine(DeactivateCoroutine()); // 启动协程，等待指定时间后禁用或销毁 GameObject
    }

    /// <summary>
    /// 协程：等待指定时间后，禁用或销毁 GameObject
    /// </summary>
    /// <returns>返回协程的执行流</returns>
    IEnumerator DeactivateCoroutine()
    {
        yield return waitLifetime; // 等待指定的时间

        // 根据设置选择销毁或禁用对象
        if (destroyGameObject)
        {
            Destroy(gameObject); // 销毁 GameObject
        }
        else
        {
            gameObject.SetActive(false); // 禁用 GameObject
        }
    }
}

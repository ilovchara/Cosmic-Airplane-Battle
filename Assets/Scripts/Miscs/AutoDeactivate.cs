using System.Collections;
using UnityEngine;

public class AutoDeactivate : MonoBehaviour
{
    [Header("GameObject Deactivation Settings")]
    [SerializeField] bool destroyGameObject;
    [SerializeField] float lifetime = 3f;

    private WaitForSeconds waitLifetime;

    void Awake()
    {
        waitLifetime = new WaitForSeconds(lifetime);
    }

    void OnEnable()
    {
        StartCoroutine(DeactivateCoroutine());
    }

    /// <summary>
    /// 协程：等待指定时间后，禁用或销毁 GameObject
    /// </summary>
    /// <returns>返回协程的执行流</returns>
    IEnumerator DeactivateCoroutine()
    {
        yield return waitLifetime;

        if (destroyGameObject)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

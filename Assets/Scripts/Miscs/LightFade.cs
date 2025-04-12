using System.Collections;
using UnityEngine;
// 实现后处理光照的类
class LightFade : MonoBehaviour
{
    // 淡出持续时间
    [SerializeField] float fadeDuration = 1f;
    // 是否延迟淡出
    [SerializeField] bool delay = false;
    // 延迟时间
    [SerializeField] float delayTime = 0f;
    // 初始光强度
    [SerializeField] float startIntensity = 30f;
    // 最终光强度
    [SerializeField] float finalIntensity = 0f;

    // Light 组件的引用
    new Light light;

    // 延迟时间的等待对象
    WaitForSeconds waitDelayTime;

    // 在脚本实例化时调用
    void Awake()
    {
        // 获取 Light 组件
        light = GetComponent<Light>();

        // 初始化等待对象，用于延迟操作
        waitDelayTime = new WaitForSeconds(delayTime);
    }

    // 当游戏对象启用时调用
    void OnEnable()
    {
        // 启动淡出协程
        StartCoroutine(nameof(LightCoroutine));
    }

    // 淡出协程
    IEnumerator LightCoroutine()
    {
        // 如果需要延迟，等待指定时间
        if (delay)
        {
            yield return waitDelayTime;
        }

        // 设置初始光强度并启用光源
        light.intensity = startIntensity;
        light.enabled = true;

        // 变量 t 用于计算淡出的进度
        float t = 0f;

        // 在指定的持续时间内逐渐降低光强度
        while (t < fadeDuration)
        {
            t += Time.deltaTime / fadeDuration;
            // 通过线性插值 (Lerp) 计算当前光强度
            light.intensity = Mathf.Lerp(startIntensity, finalIntensity, t / fadeDuration);

            // 等待下一帧继续执行
            yield return null;
        }

        // 淡出完成后禁用光源
        light.enabled = false;
    }
}

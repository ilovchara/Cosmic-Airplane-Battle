using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 继承自 Singleton 模式，确保 TimeController 在整个游戏中唯一
public class TimeController : Singleton<TimeController>
{
    [SerializeField, Range(0f, 1f)] float bulletTimeScale = 0.1f; // 子弹时间的时间缩放值

    float defaultFixedDeltaTime; // 记录默认的 FixedDeltaTime 值
    float t; // 用于插值计算的时间变量

    protected override void Awake()
    {
        base.Awake();
        defaultFixedDeltaTime = Time.fixedDeltaTime; // 记录初始的 FixedDeltaTime
    }

    // 触发子弹时间，包含缓慢进入、保持和缓慢退出的过程
    public void BulletTime(float inDuration, float keepingDuration, float outDuration)
    {
        StartCoroutine(SlowInKeepAndOutDuration(inDuration, keepingDuration, outDuration));
    }

    // 触发子弹时间，并在指定的持续时间后缓慢恢复
    public void BulletTime(float duration)
    {
        Time.timeScale = bulletTimeScale;
        StartCoroutine(SlowOutCoroutine(duration));
    }

    // 仅执行缓慢进入子弹时间
    public void SlowIn(float duration)
    {
        StartCoroutine(SlowInCoroutine(duration));
    }

    // 协程：先缓慢进入，再保持一段时间，最后缓慢退出
    IEnumerator SlowInKeepAndOutDuration(float inDuration, float keepingDuration, float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration)); // 执行缓慢进入
        yield return new WaitForSecondsRealtime(keepingDuration); // 按实际时间等待，避免受 Time.timeScale 影响
        StartCoroutine(SlowOutCoroutine(outDuration)); // 执行缓慢退出
    }

    // 仅执行缓慢退出子弹时间
    public void SlowOut(float duration)
    {
        StartCoroutine(SlowOutCoroutine(duration));
    }

    // 触发子弹时间，只有缓慢进入和缓慢退出过程
    public void BulletTime(float inDuration, float outDuration)
    {
        StartCoroutine(SlowInAndOutCoroutine(inDuration, outDuration));
    }

    // 协程：先缓慢进入，然后直接缓慢退出
    IEnumerator SlowInAndOutCoroutine(float inDuration, float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration)); // 先缓慢进入
        StartCoroutine(SlowOutCoroutine(outDuration)); // 然后缓慢退出
    }

    // 协程：缓慢降低 Time.timeScale，进入子弹时间
    IEnumerator SlowInCoroutine(float duration)
    {
        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration; // 使用 unscaledDeltaTime 确保时间变化不受 Time.timeScale 影响
            Time.timeScale = Mathf.Lerp(1f, bulletTimeScale, t); // 逐步降低 Time.timeScale
            Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale; // 调整 FixedDeltaTime 以匹配新 TimeScale
            
            yield return null;
        }
    }

    // 协程：缓慢恢复 Time.timeScale，退出子弹时间
    IEnumerator SlowOutCoroutine(float duration)
    {
        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            Time.timeScale = Mathf.Lerp(bulletTimeScale, 1f, t); // 逐步恢复 Time.timeScale
            Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale; // 调整 FixedDeltaTime 以匹配新 TimeScale
            
            yield return null;
        }
    }
}

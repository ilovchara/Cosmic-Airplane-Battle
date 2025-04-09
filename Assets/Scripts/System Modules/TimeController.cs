using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 继承自 Singleton 模式，确保 TimeController 在整个游戏中唯一
public class TimeController : Singleton<TimeController>
{
    [SerializeField, Range(0f, 1f)] float bulletTimeScale = 0.1f; // 子弹时间的时间缩放值

    float defaultFixedDeltaTime; // 记录默认的 FixedDeltaTime 值
    float timeScaleBeforePause;  // 暂停前的时间缩放值
    float t;                     // 用于插值计算的时间变量

    // 初始化记录默认的 FixedDeltaTime
    protected override void Awake()
    {
        base.Awake();
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    // 暂停游戏
    public void Pause()
    {
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f;
    }

    // 取消暂停，恢复到暂停前的时间缩放
    public void Unpause()
    {
        Time.timeScale = timeScaleBeforePause;
    }

    // 触发子弹时间：缓慢进入 → 保持 → 缓慢退出
    public void BulletTime(float inDuration, float keepingDuration, float outDuration)
    {
        StartCoroutine(SlowInKeepAndOutDuration(inDuration, keepingDuration, outDuration));
    }

    // 立即进入子弹时间并在指定时长后缓慢恢复
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

    // 仅执行缓慢退出子弹时间
    public void SlowOut(float duration)
    {
        StartCoroutine(SlowOutCoroutine(duration));
    }

    // 触发子弹时间：缓慢进入 → 缓慢退出
    public void BulletTime(float inDuration, float outDuration)
    {
        StartCoroutine(SlowInAndOutCoroutine(inDuration, outDuration));
    }

    // 协程：缓慢进入 → 保持 → 缓慢退出
    IEnumerator SlowInKeepAndOutDuration(float inDuration, float keepingDuration, float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));
        yield return new WaitForSecondsRealtime(keepingDuration); // 实时等待，不受时间缩放影响
        StartCoroutine(SlowOutCoroutine(outDuration));
    }

    // 协程：缓慢进入 → 缓慢退出（无保持）
    IEnumerator SlowInAndOutCoroutine(float inDuration, float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));
        StartCoroutine(SlowOutCoroutine(outDuration));
    }

    // 协程：缓慢降低 Time.timeScale 进入子弹时间
    IEnumerator SlowInCoroutine(float duration)
    {
        t = 0f;
        while (t < 1f)
        {
            if (GameManager.GameState != GameState.Paused)
            {
                t += Time.unscaledDeltaTime / duration;
                Time.timeScale = Mathf.Lerp(1f, bulletTimeScale, t); // 从1缓慢插值到子弹时间
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            }
            yield return null;
        }
    }

    // 协程：缓慢恢复 Time.timeScale 退出子弹时间
    IEnumerator SlowOutCoroutine(float duration)
    {
        t = 0f;
        while (t < 1f)
        {
            if (GameManager.GameState != GameState.Paused)
            {
                t += Time.unscaledDeltaTime / duration;
                Time.timeScale = Mathf.Lerp(bulletTimeScale, 1f, t); // 从子弹时间缓慢恢复到1
                Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            }
            yield return null;
        }
    }
}

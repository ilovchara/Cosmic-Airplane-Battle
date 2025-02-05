using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
    [Header(" --- UI Elements ---")]
    [SerializeField] Image fillImageBack;
    [SerializeField] Image fillImageFront;

    [Header(" --- Fill Settings ---")]
    [SerializeField] bool delayFill = true;
    [SerializeField] float fillDelay = 0.5f; // 填充延迟时间
    [SerializeField] float fillSpeed = 0.1f; // 填充速度

    protected float currentFillAmount;
    protected float targetFillAmount;

    float t;

    WaitForSeconds waitForDelayFill;

    Canvas canvas;
    Coroutine bufferedFillingCoroutine;

    /// <summary>
    /// 初始化状态条，设置填充的初始值
    /// </summary>
    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    /// <summary>
    /// 停止所有协程
    /// </summary>
    void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 初始化血条的当前值与最大值
    /// </summary>
    /// <param name="currentValue">当前值</param>
    /// <param name="maxValue">最大值</param>
    public virtual void Initialize(float currentValue, float maxValue)
    {
        currentFillAmount = currentValue / maxValue;
        targetFillAmount = currentFillAmount;
        fillImageBack.fillAmount = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
    }

    /// <summary>
    /// 更新状态条，设置新的当前值与最大值
    /// </summary>
    /// <param name="currentValue">当前值</param>
    /// <param name="maxValue">最大值</param>
    public void UpdateStats(float currentValue, float maxValue)
    {
        targetFillAmount = currentValue / maxValue;

        if (bufferedFillingCoroutine != null)
        {
            StopCoroutine(bufferedFillingCoroutine);
        }

        if (gameObject.activeInHierarchy)
        {
            if (currentFillAmount > targetFillAmount)
            {
                fillImageFront.fillAmount = targetFillAmount;
                bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageBack));
            }
            else if (currentFillAmount < targetFillAmount)
            {
                fillImageBack.fillAmount = targetFillAmount;
                bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageFront));
            }
        }
        else
        {
            fillImageFront.fillAmount = targetFillAmount;
            fillImageBack.fillAmount = targetFillAmount;
        }
    }

    /// <summary>
    /// 缓冲区填充协程，用于平滑填充状态条
    /// </summary>
    /// <param name="image">需要更新的图像对象</param>
    /// <returns>协程的IEnumerator</returns>
    protected virtual IEnumerator BufferedFillingCoroutine(Image image)
    {
        if (delayFill)
        {
            yield return waitForDelayFill;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fillSpeed;
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, t);
            image.fillAmount = currentFillAmount;

            yield return null;
        }
    }
}

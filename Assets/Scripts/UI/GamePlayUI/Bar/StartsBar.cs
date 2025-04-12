using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
    [Header(" --- UI Elements ---")]
    [SerializeField] Image fillImageBack;  // 后景填充图像
    [SerializeField] Image fillImageFront; // 前景填充图像

    [Header(" --- Fill Settings ---")]
    [SerializeField] bool delayFill = true; // 是否延迟填充
    [SerializeField] float fillDelay = 0.5f; // 填充延迟时间
    [SerializeField] float fillSpeed = 0.1f; // 填充速度

    protected float currentFillAmount; // 当前填充量
    protected float targetFillAmount;  // 目标填充量

    float t; // 用于插值计算的时间变量

    WaitForSeconds waitForDelayFill; // 延迟填充的等待时间

    Canvas canvas; // Canvas组件
    Coroutine bufferedFillingCoroutine; // 缓冲区填充的协程引用

    /// <summary>
    /// 初始化状态条，设置填充的初始值
    /// </summary>
    void Awake()
    {
        // 获取并设置Canvas的世界相机为主相机
        if (TryGetComponent<Canvas>(out canvas))
        {
            canvas.worldCamera = Camera.main;
        }

        // 初始化延迟填充等待时间
        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    /// <summary>
    /// 停止所有协程
    /// 在对象禁用时调用，确保停止任何正在进行的协程
    /// </summary>
    void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 初始化血条，设置当前值和最大值
    /// </summary>
    /// <param name="currentValue">当前值</param>
    /// <param name="maxValue">最大值</param>
    public virtual void Initialize(float currentValue, float maxValue)
    {
        currentFillAmount = currentValue / maxValue;  // 计算当前填充比例
        targetFillAmount = currentFillAmount;         // 目标填充量初始化为当前填充量

        // 设置后景和前景填充量
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
        targetFillAmount = currentValue / maxValue; // 计算目标填充量

        // 停止之前的填充协程（如果有）
        if (bufferedFillingCoroutine != null)
        {
            StopCoroutine(bufferedFillingCoroutine);
        }

        // 如果对象在场景中活跃，执行填充动画
        if (gameObject.activeInHierarchy)
        {
            if (currentFillAmount > targetFillAmount) // 当前值大于目标值时，填充后景
            {
                fillImageFront.fillAmount = targetFillAmount;
                bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageBack));
            }
            else if (currentFillAmount < targetFillAmount) // 当前值小于目标值时，填充前景
            {
                fillImageBack.fillAmount = targetFillAmount;
                bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageFront));
            }
        }
        else
        {
            // 如果对象不活跃，直接设置填充量
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
        // 如果启用了延迟填充，等待一段时间
        if (delayFill)
        {
            yield return waitForDelayFill;
        }

        t = 0f; // 重置插值变量

        // 使用线性插值逐步填充
        while (t < 1f)
        {
            t += Time.deltaTime * fillSpeed; // 计算填充进度
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, t); // 逐步更新当前填充量
            image.fillAmount = currentFillAmount; // 更新填充图像的填充量

            yield return null; // 等待下一帧
        }
    }
}

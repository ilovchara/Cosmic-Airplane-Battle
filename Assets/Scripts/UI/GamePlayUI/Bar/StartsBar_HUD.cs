using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartsBar_HUD : StatsBar
{
    [Header(" --- UI Elements ---")]
    [SerializeField] protected Text percentText; // 显示百分比的文本组件

    /// <summary>
    /// 设置百分比文本
    /// 将当前目标填充量转换为百分比并更新显示
    /// </summary>
    protected virtual void SetPercentText()
    {
        percentText.text = Mathf.RoundToInt(targetFillAmount * 100f) + "%";
    }

    /// <summary>
    /// 初始化状态栏
    /// 设置当前值和最大值，并更新百分比文本
    /// </summary>
    /// <param name="currentValue">当前值</param>
    /// <param name="maxValue">最大值</param>
    public override void Initialize(float currentValue, float maxValue)
    {
        base.Initialize(currentValue, maxValue);
        SetPercentText(); // 初始化时设置百分比文本
    }

    /// <summary>
    /// 覆盖缓冲区填充协程
    /// 更新缓冲区填充过程中百分比文本
    /// </summary>
    /// <param name="image">UI图像对象</param>
    /// <returns>协程的IEnumerator</returns>
    protected override IEnumerator BufferedFillingCoroutine(Image image)
    {
        SetPercentText(); // 每次缓冲填充时更新百分比文本
        return base.BufferedFillingCoroutine(image); // 调用父类的缓冲填充协程
    }
}

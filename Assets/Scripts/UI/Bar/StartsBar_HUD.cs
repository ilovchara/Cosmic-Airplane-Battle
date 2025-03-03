using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class StartsBar_HUD : StatsBar
{
    [Header(" --- UI Elements ---")]
    [SerializeField] protected Text percentText;

    /// <summary>
    /// 设置百分比文本
    /// </summary>
    protected virtual void SetPercentText()
    {
        percentText.text = Mathf.RoundToInt(targetFillAmount * 100f) + "%";
    }

    /// <summary>
    /// 初始化状态栏，设置当前值和最大值，并更新百分比文本
    /// </summary>
    /// <param name="currentValue">当前值</param>
    /// <param name="maxValue">最大值</param>
    public override void Initialize(float currentValue, float maxValue)
    {
        base.Initialize(currentValue, maxValue);
        SetPercentText();
    }

    /// <summary>
    /// 覆盖缓冲区填充协程，更新百分比文本
    /// </summary>
    /// <param name="image">UI图像对象</param>
    /// <returns>协程的IEnumerator</returns>
    protected override IEnumerator BufferedFillingCoroutine(Image image)
    {
        SetPercentText();
        return base.BufferedFillingCoroutine(image);
    }
}

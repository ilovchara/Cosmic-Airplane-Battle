using UnityEngine;

// BossHealthBar类，用于控制Boss的血条显示，继承自StartsBar_HUD
public class BossHealthBar : StartsBar_HUD
{
    // 设置血条的百分比文本
    protected override void SetPercentText()
    {
        base.SetPercentText();
        percentText.text = (targetFillAmount * 100f).ToString("F2") + "%";
    }
}

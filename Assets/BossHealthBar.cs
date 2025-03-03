using UnityEngine;

public class BossHealthBar : StartsBar_HUD
{

    protected override void SetPercentText()
    {
        base.SetPercentText();
        percentText.text = (targetFillAmount * 100f).ToString("F2") + "%";

    }




}

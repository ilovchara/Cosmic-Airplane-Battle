using System;
using UnityEngine;
using UnityEngine.UI;
// 03 查找导弹的位置 更新其容量 UI组件
public class MissileDisplay : MonoBehaviour
{
    static Text amountText;
    static Image cooldownImage;


    void Awake()
    {
        amountText = transform.Find("Amount Text").GetComponent<Text>();
        cooldownImage = transform.Find("Cooldown Image").GetComponent<Image>();
    }
    // 调用函数可以更改显示容量
    public static void UpdateAmountText(int amount) => amountText.text = amount.ToString();

    public static void UpdateCooldownImage(float fillAmount) => cooldownImage.fillAmount = fillAmount;


}

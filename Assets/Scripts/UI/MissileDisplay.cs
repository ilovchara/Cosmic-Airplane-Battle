using System;
using UnityEngine;
using UnityEngine.UI;

// 03 查找导弹的位置，更新其容量和冷却UI组件
public class MissileDisplay : MonoBehaviour
{
    static Text amountText; // 显示导弹数量的文本组件
    static Image cooldownImage; // 显示冷却状态的图片组件

    // 初始化时查找并缓存相关UI组件
    void Awake()
    {
        amountText = transform.Find("Amount Text").GetComponent<Text>(); // 获取显示导弹数量的文本组件
        cooldownImage = transform.Find("Cooldown Image").GetComponent<Image>(); // 获取显示冷却状态的图片组件
    }

    // 更新导弹数量显示
    public static void UpdateAmountText(int amount) => amountText.text = amount.ToString(); // 更新导弹数量显示文本

    // 更新导弹冷却状态显示
    public static void UpdateCooldownImage(float fillAmount) => cooldownImage.fillAmount = fillAmount; // 更新冷却进度条的填充值
}

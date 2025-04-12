using System.Collections;
using UnityEngine;

// 02 导弹发射系统
public class MissileSystem : MonoBehaviour
{
    // --- 可以被继承 之后想一想 ---
    [SerializeField] private int defaultAmount = 5;               // 初始导弹数量
    [SerializeField] private float cooldownTime = 1f;             // 导弹发射冷却时间
    [SerializeField] private GameObject missilePrefab = null;     // 导弹预设体
    [SerializeField] private AudioData launchSFX = null;          // 发射音效

    private int amount;           // 当前剩余导弹数量
    private bool isReady = true;  // 发射系统是否准备好

    // 初始化导弹数量
    void Awake()
    {
        amount = defaultAmount;
    }

    // 显示初始导弹数量
    void Start()
    {
        MissileDisplay.UpdateAmountText(amount);
    }

    // 发射导弹的方法
    public void Launch(Transform muzzleTransform)
    {
        // 检查导弹数量和冷却状态
        if (amount == 0 || !isReady) return;

        isReady = false;

        // 生成导弹并播放音效
        PoolManager.Release(missilePrefab, muzzleTransform.position);
        AudioManager.Instance.PlayRandomSFX(launchSFX);

        // 更新数量与UI
        amount--;
        MissileDisplay.UpdateAmountText(amount);

        // 根据导弹数量决定是否开始冷却
        if (amount == 0)
        {
            MissileDisplay.UpdateCooldownImage(1f);
        }
        else
        {
            StartCoroutine(CooldownCoroutine());
        }
    }

    // 冷却协程：等待指定时间后恢复发射状态
    private IEnumerator CooldownCoroutine()
    {
        float cooldownValue = cooldownTime;
        while (cooldownValue > 0f)
        {
            MissileDisplay.UpdateCooldownImage(cooldownValue / cooldownTime);
            cooldownValue = Mathf.Max(cooldownValue - Time.deltaTime, 0f);
            yield return null;
        }
        isReady = true;
    }

    // 拾取导弹时调用，增加数量并更新UI
    public void PickUp()
    {
        amount++;
        MissileDisplay.UpdateAmountText(amount);

        if (amount == 1)
        {
            MissileDisplay.UpdateCooldownImage(0f);
            isReady = true;
        }
    }
}

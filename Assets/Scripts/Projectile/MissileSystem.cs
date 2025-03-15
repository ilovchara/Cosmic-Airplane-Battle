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

    void Awake()
    {
        amount = defaultAmount;  // 初始化剩余导弹数量
    }

    void Start()
    {
        // 显示初始的导弹数量
        MissileDisplay.UpdateAmountText(amount);
    }

    // 发射导弹的接口
    public void Launch(Transform muzzleTransform)
    {
        // 检查是否有剩余导弹且系统是否准备好
        if (amount == 0 || !isReady) return;

        isReady = false;  // 设置系统为不可用，避免连续发射

        // 从对象池释放一个导弹，并播放发射音效
        PoolManager.Release(missilePrefab, muzzleTransform.position);
        AudioManager.Instance.PlayRandomSFX(launchSFX);

        // 减少剩余导弹数量并更新UI显示
        amount--;
        MissileDisplay.UpdateAmountText(amount);

        // 如果导弹用完了，更新UI冷却图像为满
        if (amount == 0)
        {
            MissileDisplay.UpdateCooldownImage(1f);
        }
        else
        {
            // 否则，开始冷却协程，处理重载时间
            StartCoroutine(CooldownCoroutine());
        }
    }

    // 协程，用于处理导弹的冷却时间
    private IEnumerator CooldownCoroutine()
    {
        float cooldownValue = cooldownTime;
        // 从冷却时间开始倒计时，直到为0
        while (cooldownValue > 0f)
        {
            // 根据剩余冷却时间更新冷却进度条
            MissileDisplay.UpdateCooldownImage(cooldownValue / cooldownTime);
            
            // 减少冷却时间 deltatime 是上一帧的时间 一般是一秒
            cooldownValue = Mathf.Max(cooldownValue - Time.deltaTime, 0f);

            yield return null;  
        }
        isReady = true;
    }
    // 拾取获取导弹
    public void PickUp()
    {
        amount++;
        MissileDisplay.UpdateAmountText(amount);
        // 剩下一个就
        if(amount == 1)
        {
            MissileDisplay.UpdateCooldownImage(0f);
            isReady = true;
        }

    }

}

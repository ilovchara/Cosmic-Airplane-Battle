using System.Collections;
using UnityEngine;

public class MissileSystem : MonoBehaviour
{
    [SerializeField] int defaultAmount = 5;
    [SerializeField] float cooldownTime = 1f;
    [SerializeField] private GameObject missilePrefab = null;
    [SerializeField] private AudioData launchSFX = null;

    int amount;
    bool isReady = true;

    void Awake()
    {
        amount = defaultAmount;
    }

    void Start()
    {
        MissileDisplay.UpdateAmountText(amount);
    }


    public void Launch(Transform muzzleTransform)
    {
        if (amount == 0 || !isReady) return;

        isReady = false;

        // Release a missile clone from object pool
        PoolManager.Release(missilePrefab, muzzleTransform.position);
        // Play missile launch SFX
        AudioManager.Instance.PlayRandomSFX(launchSFX);
        amount--;
        MissileDisplay.UpdateAmountText(amount);

        if (amount == 0)
        {
            MissileDisplay.UpdateCooldownImage(1f);
        }
        else
        {
            StartCoroutine(CooldownCoroutine());
        }
    }
    // 冷却一秒
    IEnumerator CooldownCoroutine()
    {
        var cooldownValue = cooldownTime;

        while (cooldownValue > 0f)
        {
            MissileDisplay.UpdateCooldownImage(cooldownValue / cooldownTime);
            cooldownValue = Mathf.Max(cooldownValue - Time.deltaTime, 0f);

            yield return null;
        }

        isReady = true;
    }
}
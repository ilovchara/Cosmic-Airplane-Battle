using UnityEngine;

public class MissileSystem : MonoBehaviour
{
    [SerializeField] private GameObject missilePrefab = null;
    [SerializeField] private AudioData launchSFX = null;

    public void Launch(Transform muzzleTransform)
    {
        // Release a missile clone from object pool
        PoolManager.Release(missilePrefab, muzzleTransform.position);
        
        // Play missile launch SFX
        AudioManager.Instance.PlayRandomSFX(launchSFX);
    }
}
using UnityEngine;

/// <summary>
/// 掉落物设置类：用于配置掉落物的预制体和掉落概率。
/// 可用于敌人死亡后按概率生成对应的掉落物。
/// </summary>
[System.Serializable]
public class LootSetting
{
    public GameObject prefab; // 掉落物的预制体
    [Range(0f, 100f)] public float dropPercentage; // 掉落概率（0-100%）

    /// <summary>
    /// 尝试在指定位置生成掉落物，根据设置的掉落概率决定是否生成。
    /// </summary>
    /// <param name="position">掉落物的生成位置。</param>
    public void Spawn(Vector3 position)
    {
        if (Random.Range(0f, 100f) <= dropPercentage)
        {
            PoolManager.Release(prefab, position); // 从对象池中生成
        }
    }
}

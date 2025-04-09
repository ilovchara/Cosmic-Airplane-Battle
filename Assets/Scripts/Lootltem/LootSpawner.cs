using UnityEngine;

/// <summary>
/// 战利品生成器：根据预设的掉落设置，在指定位置生成战利品。
/// </summary>
public class LootSpawner : MonoBehaviour
{
    [SerializeField] LootSetting[] lootSettings; // 战利品设置数组（每项包含预制体和掉落概率）

    /// <summary>
    /// 在指定位置生成所有战利品（每个有自己的掉落概率）
    /// </summary>
    /// <param name="position">生成战利品的位置</param>
    public void Spawn(Vector2 position)
    {
        // 遍历所有战利品设置项
        foreach (var item in lootSettings)
        {
            // 每个战利品使用随机单位圆偏移，避免完全重叠
            item.Spawn(position + Random.insideUnitCircle);
        }
    }
}

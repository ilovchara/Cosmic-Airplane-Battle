using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    // 战利品设置数组
    [SerializeField] LootSetting[] lootSettings;

    /// <summary>
    /// 在指定位置生成战利品
    /// </summary>
    /// <param name="position">生成战利品的位置</param>
    public void Spawn(Vector2 position)
    {
        // 遍历战利品设置数组
        foreach (var item in lootSettings)
        {
            // 在指定位置加上一个随机偏移量生成战利品
            item.Spawn(position + Random.insideUnitCircle);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Header("Object Pool Arrays")]
    [SerializeField] Pool[] enemyPools; // 敌人对象池数组
    [SerializeField] Pool[] playerProjectilePools; // 玩家弹药对象池数组
    [SerializeField] Pool[] enemyProjectilePools; // 敌人弹药对象池数组
    [SerializeField] Pool[] vFXPools; // 特效对象池数组
    [SerializeField] Pool[] lootItemPools; // 掉落物品对象池数组

    [Header("Internal Pool Management")]
    static Dictionary<GameObject, Pool> dictionary; // 用于管理对象池的字典

    // 在脚本实例化时调用
    void Awake()
    {
        dictionary = new Dictionary<GameObject, Pool>();

        // 初始化所有对象池
        Initialize(enemyPools);
        Initialize(playerProjectilePools);
        Initialize(enemyProjectilePools);
        Initialize(vFXPools);
        Initialize(lootItemPools);
    }

    #if UNITY_EDITOR
    // 在编辑器模式下，当对象销毁时调用
    void OnDestroy()
    {
        // 检查每个对象池的大小是否超过初始大小
        CheckPoolSize(enemyPools);
        CheckPoolSize(playerProjectilePools);
        CheckPoolSize(enemyProjectilePools);
        CheckPoolSize(vFXPools);
        CheckPoolSize(lootItemPools);
    }
    #endif

    /// <summary>
    /// 检查每个对象池的运行时大小是否超过初始大小
    /// </summary>
    void CheckPoolSize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
            if (pool.RuntimeSize > pool.Size)
            {
                Debug.LogWarning(
                    string.Format("Pool: {0} has a runtime size {1} bigger than its initial size {2}!",
                    pool.Prefab.name,
                    pool.RuntimeSize,
                    pool.Size));
            }
        }
    }

    /// <summary>
    /// 初始化给定的对象池数组
    /// </summary>
    void Initialize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
        #if UNITY_EDITOR
            if (dictionary.ContainsKey(pool.Prefab))
            {
                Debug.LogError("Same prefab in multiple pools! Prefab: " + pool.Prefab.name);
                continue;
            }
        #endif
            dictionary.Add(pool.Prefab, pool);

            // 创建一个新的父对象来组织池中的对象
            Transform poolParent = new GameObject("Pool: " + pool.Prefab.name).transform;
            poolParent.parent = transform;
            pool.Initialize(poolParent);
        }
    }

    /// <summary>
    /// 根据传入的<paramref name="prefab"></paramref>参数，返回对象池中预备好的游戏对象。
    /// </summary>
    /// <param name="prefab">指定的游戏对象预制体。</param>
    /// <returns>对象池中预备好的游戏对象。</returns>
    public static GameObject Release(GameObject prefab)
    {
        #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
        }
        #endif
        return dictionary[prefab].PreparedObject();
    }

    /// <summary>
    /// 根据传入的prefab参数，在position参数位置释放对象池中预备好的游戏对象。
    /// </summary>
    /// <param name="prefab">指定的游戏对象预制体。</param>
    /// <param name="position">指定释放位置。</param>
    /// <returns>对象池中预备好的游戏对象。</returns>
    public static GameObject Release(GameObject prefab, Vector3 position)
    {
        #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
        }
        #endif
        return dictionary[prefab].PreparedObject(position);
    }

    /// <summary>
    /// 根据传入的prefab参数和rotation参数，在position参数位置释放对象池中预备好的游戏对象。
    /// </summary>
    /// <param name="prefab">指定的游戏对象预制体。</param>
    /// <param name="position">指定释放位置。</param>
    /// <param name="rotation">指定的旋转值。</param>
    /// <returns>对象池中预备好的游戏对象。</returns>
    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
        }
        #endif
        return dictionary[prefab].PreparedObject(position, rotation);
    }

    /// <summary>
    /// 根据传入的prefab参数, rotation参数和localScale参数，在position参数位置释放对象池中预备好的游戏对象。
    /// </summary>
    /// <param name="prefab">指定的游戏对象预制体。</param>
    /// <param name="position">指定释放位置。</param>
    /// <param name="rotation">指定的旋转值。</param>
    /// <param name="localScale">指定的缩放值。</param>
    /// <returns>对象池中预备好的游戏对象。</returns>
    public static GameObject Release(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 localScale)
    {
        #if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager could NOT find prefab: " + prefab.name);
            return null;
        }
        #endif
        return dictionary[prefab].PreparedObject(position, rotation, localScale);
    }
}

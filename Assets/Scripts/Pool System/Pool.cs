using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池（Object Pool）类，用于管理和复用游戏对象，减少频繁实例化和销毁的性能损耗。
/// </summary>
[System.Serializable]
public class Pool
{
    /// <summary>
    /// 预制体（Prefab），即池中管理的对象类型。
    /// </summary>
    public GameObject Prefab => prefab;

    /// <summary>
    /// 初始池大小（即预分配的对象数量）。
    /// </summary>
    public int Size => size;

    /// <summary>
    /// 运行时池中对象的数量。
    /// </summary>
    public int RuntimeSize => queue.Count;

    [SerializeField] private GameObject prefab; // 预制体对象
    [SerializeField] private int size = 1; // 预分配的对象数量

    private Queue<GameObject> queue; // 存储池中对象的队列
    private Transform parent; // 父级对象，用于组织层级结构

    /// <summary>
    /// 初始化对象池，创建指定数量的对象并存入队列。
    /// </summary>
    /// <param name="parent">对象池管理的父对象</param>
    public void Initialize(Transform parent)
    {
        queue = new Queue<GameObject>();
        this.parent = parent;

        for (var i = 0; i < size; i++)
        {
            queue.Enqueue(Copy()); // 预创建对象并存入队列
        }
    }

    /// <summary>
    /// 复制一个新的对象，并设为非激活状态。
    /// </summary>
    /// <returns>新创建的游戏对象</returns>
    private GameObject Copy()
    {
        var copy = GameObject.Instantiate(prefab, parent);
        copy.SetActive(false);
        return copy;
    }

    /// <summary>
    /// 获取一个可用的对象，如果池中无可用对象，则创建新对象。
    /// </summary>
    /// <returns>可用的游戏对象</returns>
    private GameObject AvailableObject()
    {
        GameObject availableObject = null;

        // 如果队列中有未激活的对象，则取出使用
        if (queue.Count > 0 && !queue.Peek().activeSelf)
        {
            availableObject = queue.Dequeue();
        }
        else
        {
            availableObject = Copy(); // 创建新对象
        }

        queue.Enqueue(availableObject); // 重新入队，确保对象池管理
        return availableObject;
    }

    /// <summary>
    /// 获取一个对象并激活。
    /// </summary>
    /// <returns>已激活的游戏对象</returns>
    public GameObject PreparedObject()
    {
        GameObject preparedObject = AvailableObject();
        preparedObject.SetActive(true);
        return preparedObject;
    }

    /// <summary>
    /// 获取一个对象，激活并设置位置。
    /// </summary>
    /// <param name="position">对象位置</param>
    /// <returns>已激活的游戏对象</returns>
    public GameObject PreparedObject(Vector3 position)
    {
        GameObject preparedObject = AvailableObject();
        preparedObject.SetActive(true);
        preparedObject.transform.position = position;
        return preparedObject;
    }

    /// <summary>
    /// 获取一个对象，激活并设置位置和旋转。
    /// </summary>
    /// <param name="position">对象位置</param>
    /// <param name="rotation">对象旋转</param>
    /// <returns>已激活的游戏对象</returns>
    public GameObject PreparedObject(Vector3 position, Quaternion rotation)
    {
        GameObject preparedObject = AvailableObject();
        preparedObject.SetActive(true);
        preparedObject.transform.position = position;
        preparedObject.transform.rotation = rotation;
        return preparedObject;
    }

    /// <summary>
    /// 获取一个对象，激活并设置位置、旋转和缩放。
    /// </summary>
    /// <param name="position">对象位置</param>
    /// <param name="rotation">对象旋转</param>
    /// <param name="localScale">对象缩放</param>
    /// <returns>已激活的游戏对象</returns>
    public GameObject PreparedObject(Vector3 position, Quaternion rotation, Vector3 localScale)
    {
        GameObject preparedObject = AvailableObject();
        preparedObject.SetActive(true);
        preparedObject.transform.position = position;
        preparedObject.transform.rotation = rotation;
        preparedObject.transform.localScale = localScale;
        return preparedObject;
    }
}

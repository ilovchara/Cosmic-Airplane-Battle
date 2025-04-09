using UnityEngine;

/// <summary>
/// 跨场景持久单例模式基类（泛型）
/// 作用：确保某个 MonoBehaviour 类型的组件在整个游戏生命周期中只有一个实例，并在场景切换时不被销毁。
/// 用法：其他类继承 PersistentSingleton<T>，T 为自身类名。
/// </summary>
public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; } // 单例实例

    /// <summary>
    /// Awake 生命周期回调，处理单例初始化和重复实例销毁
    /// </summary>
    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T; // 设置为当前实例
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 已存在其他实例，销毁当前对象
        }

        DontDestroyOnLoad(gameObject); // 场景切换时保留该对象
    }
}

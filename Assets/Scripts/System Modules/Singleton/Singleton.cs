using UnityEngine;

/// <summary>
/// 通用的场景内单例模式基类（泛型）
/// 作用：确保某个 MonoBehaviour 类型的对象在当前场景中只有一个实例，不跨场景持久化。
/// 用法：其他类继承 Singleton<T>，T 为自身类名。
/// </summary>
public class Singleton<T> : MonoBehaviour where T : Component
{
    // 静态单例实例
    public static T Instance { get; private set; }

    /// <summary>
    /// Awake 生命周期回调，初始化单例引用
    /// </summary>
    protected virtual void Awake()
    {
        Instance = this as T;
    }
}

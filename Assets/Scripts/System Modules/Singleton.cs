using UnityEngine;
// 继承这个类实现单例
public class Singleton<T> : MonoBehaviour where T : Component
{
    // 声明属性
    public static T Instance { get; private set;}

    protected virtual void Awake()
    {
        Instance = this as T;
    }
}

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 管理玩家的过载模式视觉与音效效果
/// 通过静态事件实现过载模式的开启与关闭
/// </summary>
public class PlayerOverdrive : MonoBehaviour
{
    // 静态事件：用于通知进入或退出过载模式
    public static UnityAction on = delegate { };
    public static UnityAction off = delegate { };

    // 视觉特效相关对象
    [SerializeField] GameObject triggerVFX;           // 启动过载时的触发特效
    [SerializeField] GameObject engineVFXNormal;      // 正常状态下的引擎特效
    [SerializeField] GameObject engineVFXOverdrive;   // 过载状态下的引擎特效

    // 音效数据（进入和退出过载模式的音效）
    [SerializeField] AudioData onSFX;
    [SerializeField] AudioData offSFX;

    /// <summary>
    /// 注册事件监听
    /// </summary>
    void Awake()
    {
        on += On;
        off += Off;
    }

    /// <summary>
    /// 注销事件监听
    /// </summary>
    void OnDestroy()
    {
        on -= On;
        off -= Off;
    }

    /// <summary>
    /// 进入过载模式时的处理逻辑
    /// 播放过载视觉特效与音效
    /// </summary>
    void On()
    {
        triggerVFX.SetActive(true);
        engineVFXNormal.SetActive(false);
        engineVFXOverdrive.SetActive(true);
        AudioManager.Instance.PlayRandomSFX(onSFX);
    }

    /// <summary>
    /// 退出过载模式时的处理逻辑
    /// 恢复正常视觉特效并播放关闭音效
    /// </summary>
    void Off()
    {
        triggerVFX.SetActive(false);
        engineVFXOverdrive.SetActive(false); // 关闭过载引擎特效
        engineVFXNormal.SetActive(true);     // 启动普通引擎特效
        AudioManager.Instance.PlayRandomSFX(offSFX); // 播放退出音效
    }
}

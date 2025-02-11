using UnityEngine;
using UnityEngine.Events;

// 负责管理玩家的过载模式特效与音效
public class PlayerOverdrive : MonoBehaviour
{
    // 静态事件，触发和关闭过载模式
    public static UnityAction on = delegate { };
    public static UnityAction off = delegate { };

    // 过载模式相关的视觉效果对象
    [SerializeField] GameObject triggerVFX;
    [SerializeField] GameObject engineVFXNormal;
    [SerializeField] GameObject engineVFXOverdrive;

    // 过载模式的音效数据
    [SerializeField] AudioData onSFX;
    [SerializeField] AudioData offSFX;

    void Awake()
    {
        // 订阅过载模式事件
        on += On;
        off += Off;
    }

    void OnDestroy()
    {
        on -= On;
        off -= Off;
    }

    // 进入过载模式的处理
    void On()
    {
        // if(this == null) return;
        triggerVFX.SetActive(true);
        engineVFXNormal.SetActive(false);
        engineVFXOverdrive.SetActive(true);
        AudioManager.Instance.PlayRandomSFX(onSFX);

    }


    // 退出过载模式的处理
    void Off()
    {
        // if(this == null) return;
        triggerVFX.SetActive(false);
        engineVFXOverdrive.SetActive(false); // 关闭过载引擎特效
        engineVFXNormal.SetActive(true); // 启动普通引擎特效
        AudioManager.Instance.PlayRandomSFX(offSFX); // 播放过载模式关闭音效
    }
}

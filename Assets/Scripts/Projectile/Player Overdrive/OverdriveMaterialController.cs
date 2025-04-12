using UnityEngine;

/// 控制玩家Overdrive状态下的材质切换
public class OverdriveMaterialController : MonoBehaviour
{
    [SerializeField] Material overdriveMaterial;

    Material defaultMaterial;

    new Renderer renderer;

    // 初始化组件并记录默认材质
    void Awake()
    {
        renderer = GetComponent<Renderer>();
        defaultMaterial = renderer.material;
    }

    // 订阅 Overdrive 状态事件
    void OnEnable()
    {
        PlayerOverdrive.on += PlayerOverdriveOn;
        PlayerOverdrive.off += PlayerOverdriveOff;
    }

    // 取消订阅事件
    void OnDisable()
    {
        PlayerOverdrive.on -= PlayerOverdriveOn;
        PlayerOverdrive.off -= PlayerOverdriveOff;
    }

    // 启动 Overdrive：切换至 Overdrive 材质
    void PlayerOverdriveOn() => renderer.material = overdriveMaterial;

    // 关闭 Overdrive：切换回默认材质
    void PlayerOverdriveOff() => renderer.material = defaultMaterial;
}

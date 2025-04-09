using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{
    Text waveText;  // 用于显示波次信息的Text组件

    // 在脚本启用时调用
    void Awake()
    {
        // 获取Canvas组件并设置其worldCamera为主相机
        GetComponent<Canvas>().worldCamera = Camera.main;

        // 获取子物体中的Text组件，作为显示波次的文本
        waveText = GetComponentInChildren<Text>();
    }

    // 当脚本启用时调用
    void OnEnable()
    {
        // 设置waveText显示当前的波次，波次由EnemyManager管理
        waveText.text = "- WAVE " + EnemyManager.Instance.WaveNumber + " -"; 
    }
}

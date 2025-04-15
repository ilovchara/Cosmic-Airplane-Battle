using UnityEngine;
using UnityEngine.UI; // 添加UI命名空间

[RequireComponent(typeof(Slider))] // 确保物体上有Slider组件
public class AudioController : MonoBehaviour
{
    [Header("音量控制")]
    [SerializeField, Range(0, 1)] 
    private float volumeValue = 1f; // 在Inspector中可视化的音量值

    [SerializeField] 
    private Slider volumeSlider;    // 拖拽绑定UI滑动条

    private AudioSource[] audioSources;

    void Awake()
    {
        // 初始化音频源
        audioSources = GetComponentsInChildren<AudioSource>();
        
        // 自动获取Slider组件（如果未手动赋值）
        if (volumeSlider == null)
        {
            volumeSlider = GetComponent<Slider>();
        }

        // 加载保存的音量值
        volumeValue = PlayerPrefs.GetFloat("MasterVolume", 1f);
        
        // 初始化滑动条
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0;
            volumeSlider.maxValue = 1;
            volumeSlider.value = volumeValue;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // 应用初始音量
        SetVolume(volumeValue);
    }

    // 音量设置方法
    public void SetVolume(float volume)
    {
        volumeValue = volume;
        foreach (AudioSource source in audioSources)
        {
            source.volume = volumeValue;
        }

        // 保存音量设置
        PlayerPrefs.SetFloat("MasterVolume", volumeValue);
    }

#if UNITY_EDITOR
    // 在Inspector数值变化时实时更新（仅在编辑器模式生效）
    void OnValidate()
    {
        if (Application.isPlaying && audioSources != null)
        {
            SetVolume(volumeValue);
            if (volumeSlider != null) volumeSlider.value = volumeValue;
        }
    }
#endif
}
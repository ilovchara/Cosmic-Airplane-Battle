using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 引入 UI 命名空间
using UnityEngine.SceneManagement; // 引入场景管理命名空间

public class AudioManager : PersistentSingleton<AudioManager>
{
    [Header(" --- Audio Sources ---")]
    [SerializeField] AudioSource sfxPlayer;

    [Header(" --- Volume Control ---")]
    [SerializeField] Slider volumeSlider; // 拖入音量滑动条
    [SerializeField] float defaultVolume = 1f;

    const float MIN_PITCH = 0.9f;
    const float MAX_PITCH = 1.1f;

    private void Start()
    {
        // 设置初始音量
        SetVolume(defaultVolume);

        // 尝试通过名称查找音量滑动条并绑定监听事件
        Slider slider = GameObject.Find("SFXSlider")?.GetComponent<Slider>();
        if (slider != null)
        {
            volumeSlider = slider;
            volumeSlider.value = defaultVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // 监听场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 场景加载完成后的回调
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu") // 如果当前场景是 MainMenu
        {
            // 尝试再次查找音量滑动条并绑定监听事件
            Slider slider = GameObject.Find("SFXSlider")?.GetComponent<Slider>();
            if (slider != null)
            {
                volumeSlider = slider;
                volumeSlider.value = defaultVolume;
                volumeSlider.onValueChanged.AddListener(SetVolume);
            }
        }
    }

    public void PlaySFX(AudioData audioData)
    {
        sfxPlayer.PlayOneShot(audioData.audioClip, audioData.volume);
    }

    public void PlayRandomSFX(AudioData audioData)
    {
        sfxPlayer.pitch = Random.Range(MIN_PITCH, MAX_PITCH);
        PlaySFX(audioData);
    }

    public void PlayRandomSFX(AudioData[] audioData)
    {
        PlayRandomSFX(audioData[Random.Range(0, audioData.Length)]);
    }

    /// <summary>
    /// 设置音效播放器的音量（0~1）
    /// </summary>
    /// <param name="volume">音量</param>
    public void SetVolume(float volume)
    {
        sfxPlayer.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// 获取当前音量（供其他系统使用）
    /// </summary>
    public float GetVolume()
    {
        return sfxPlayer.volume;
    }

    // 确保在销毁时移除监听
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

[System.Serializable]
public class AudioData
{
    public AudioClip audioClip;
    public float volume = 1f;
}

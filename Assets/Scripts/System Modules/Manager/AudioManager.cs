using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 引入 UI 命名空间

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

        // 如果拖入了 slider，则绑定监听事件
        if (volumeSlider != null)
        {
            volumeSlider.value = defaultVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
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
}

[System.Serializable]
public class AudioData
{
    public AudioClip audioClip;
    public float volume = 1f;
}

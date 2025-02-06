using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [Header(" --- Audio Sources ---")]
    [SerializeField] AudioSource sfxPlayer; // 音效播放源

    const float MIN_PITCH = 0.9f; // 最低音调
    const float MAX_PITCH = 1.1f; // 最高音调

    /// <summary>
    /// 播放单次音效（UI使用）
    /// </summary>
    /// <param name="audioData">音效数据</param>
    public void PlaySFX(AudioData audioData)
    {
        sfxPlayer.PlayOneShot(audioData.audioClip, audioData.volume);
    }

    /// <summary>
    /// 播放随机音效（循环播放）
    /// </summary>
    /// <param name="audioData">音效数据</param>
    public void PlayRandomSFX(AudioData audioData)
    {
        sfxPlayer.pitch = Random.Range(MIN_PITCH, MAX_PITCH);
        PlaySFX(audioData);
    }

    /// <summary>
    /// 从多个音效中随机播放一个
    /// </summary>
    /// <param name="audioData">音效数据数组</param>
    public void PlayRandomSFX(AudioData[] audioData)
    {
        PlayRandomSFX(audioData[Random.Range(0, audioData.Length)]);
    }
}

[System.Serializable]
public class AudioData
{
    public AudioClip audioClip; // 音频剪辑
    public float volume; // 音量
}

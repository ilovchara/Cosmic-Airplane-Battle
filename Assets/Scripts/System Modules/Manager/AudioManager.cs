using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 音频管理器，用于播放音效
public class AudioManager : PersistentSingleton<AudioManager>
{
    [Header(" --- Audio Sources ---")]
    [SerializeField] AudioSource sfxPlayer; // 音效播放源

    const float MIN_PITCH = 0.9f; // 最低音调
    const float MAX_PITCH = 1.1f; // 最高音调

    /// <summary>
    /// 播放指定音效（不改变音调）
    /// </summary>
    /// <param name="audioData">音效数据</param>
    public void PlaySFX(AudioData audioData)
    {
        sfxPlayer.PlayOneShot(audioData.audioClip, audioData.volume);
    }

    /// <summary>
    /// 播放带有随机音调的单个音效（适合循环音效使用）
    /// </summary>
    /// <param name="audioData">音效数据</param>
    public void PlayRandomSFX(AudioData audioData)
    {
        sfxPlayer.pitch = Random.Range(MIN_PITCH, MAX_PITCH);
        PlaySFX(audioData);
    }

    /// <summary>
    /// 从音效数组中随机选择一个播放（附加随机音调）
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
    public AudioClip audioClip;
    public float volume;
}

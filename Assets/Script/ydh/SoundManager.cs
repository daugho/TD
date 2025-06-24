using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("BGM")]
    [SerializeField] private AudioSource bgmSource;

    [Header("SFX")]
    [SerializeField] private AudioClip[] sfxClips;
    [SerializeField] private int sfxChannelCount = 10;

    private Dictionary<string, AudioClip> sfxDict = new();
    private AudioSource[] sfxChannels;

    private void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitSFXDictionary();
            InitSFXChannels();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitSFXDictionary()
    {
        foreach (var clip in sfxClips)
        {
            if (clip != null)
                sfxDict[clip.name] = clip;
        }
    }

    private void InitSFXChannels()
    {
        sfxChannels = new AudioSource[sfxChannelCount];
        for (int i = 0; i < sfxChannelCount; i++)
        {
            GameObject channelObj = new GameObject($"SFX_Channel_{i}");
            channelObj.transform.SetParent(transform);
            sfxChannels[i] = channelObj.AddComponent<AudioSource>();
            sfxChannels[i].playOnAwake = false;
        }
    }

    // ----------- Public Methods -----------

    public void PlaySFX(string clipName, float volume = 1f)
    {
        if (!sfxDict.TryGetValue(clipName, out var clip))
        {
            Debug.LogWarning($"[SoundManager] SFX '{clipName}' not found.");
            return;
        }

        foreach (var channel in sfxChannels)
        {
            if (!channel.isPlaying)
            {
                channel.volume = volume;
                channel.clip = clip;
                channel.Play();
                return;
            }
        }

        // 모든 채널 사용 중일 경우, 첫 번째 채널을 강제로 재생
        sfxChannels[0].Stop();
        sfxChannels[0].volume = volume;
        sfxChannels[0].clip = clip;
        sfxChannels[0].Play();
    }

    public void PlayBGM(AudioClip clip, float volume = 1f, bool loop = true)
    {
        if (bgmSource == null)
        {
            Debug.LogWarning("[SoundManager] BGM AudioSource가 없습니다.");
            return;
        }

        bgmSource.clip = clip;
        bgmSource.volume = volume;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource?.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null)
            bgmSource.volume = volume;
    }

    public void StopAllSFX()
    {
        foreach (var channel in sfxChannels)
        {
            channel.Stop();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSourcePrefab;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgmVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private readonly Queue<AudioSource> _poolSFX = new();

    private Dictionary<string, AudioClip> _clipCache = new();

    private void Awake()
    {
        PlayBGM("ArcadeGameBGM#17");

        // �ʿ��� ����
        // sfx 
        // 1. �� ���̴� ȿ��      //
        // 2. ���� ���̴� ȿ��     //
        // 3. ������ ������ ȿ���� //
        // 4. ���� ����� ȿ����
        // 5. ���Ǵ� ȿ��
        // 6. ������ ������ ȿ����   //
    }

    public void PlayBGM(string clipName, bool loop = true)
    {
        AudioClip clip = LoadClip(clipName);
        if (clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(string clipName)
    {
        AudioClip clip = LoadClip(clipName);
        if (clip == null) return;
        
        var sfx = GameManager.GetGameManager.Pool.AudioSourcePool.Get();
        sfx.clip = clip;
        sfx.volume = sfxVolume;
        sfx.Play();

        StartCoroutine(ReturnToPoolAfterDelay(sfx, clip.length));
    }

    private IEnumerator ReturnToPoolAfterDelay(AudioSource sfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameManager.GetGameManager.Pool.AudioSourcePool.Release(sfx);
    }

    private AudioClip LoadClip(string clipName)
    {
        if (_clipCache.TryGetValue(clipName, out var cachedClip))
            return cachedClip;

        AudioClip newClip = Resources.Load<AudioClip>("Sounds/" + clipName);
        if (newClip != null)
            _clipCache[clipName] = newClip;

        return newClip;
    }
}

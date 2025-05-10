using System;
using System.Collections;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Music")]
    [SerializeField] private Sound[] musicSounds;
    [SerializeField] private AudioSource musicSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource sfxSource;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        PlayMusic("MainMenu_BGM", true);
    }

    public void SetMusicPitch(float pitch)
    {
        musicSource.pitch = pitch;
    }

    public void PlayMusic(string musicName, bool musicLoop)
    {
        Sound musicSound = Array.Find(musicSounds, sound => sound.name == musicName);

        if(musicSound == null)
        {
            Debug.LogError("Music Not found");
        }

        else
        {
            musicSource.clip = musicSound.audioClip;
            musicSource.volume = 0.5f;
            musicSource.loop = musicLoop;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(Sound sfx, Transform spawnTransform, float volume, bool spatialBlend)
    {
       AudioSource audioSource = Instantiate(sfxSource, spawnTransform.position, Quaternion.identity);
       audioSource.clip = sfx.audioClip;
       audioSource.spatialBlend = spatialBlend == true ? 1f : 0f;
       audioSource.volume = volume;
       audioSource.Play();
       float clipLength = audioSource.clip.length;
       Destroy(audioSource.gameObject, clipLength);
    }

    public static IEnumerator StartFade(float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = Instance.musicSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            Instance.musicSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalAudioManager : MonoBehaviour
{
    public static LocalAudioManager Instance;

    [SerializeField]
    private AudioClip mainMenuMusic;
    [SerializeField]
    private AudioClip countDownSound;
    [SerializeField]
    private AudioClip buzzerSound;
    [SerializeField]
    private AudioClip audienceSound;

    private AudioSource audioSource;
    private AudioSource backgroundMusicSource;

    public float volume = 0.1f;

    private void Awake()
    {
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        backgroundMusicSource = gameObject.AddComponent<AudioSource>();

        if (!audioSource || !backgroundMusicSource)
        {
            Debug.LogError("Missing AudioSource component!");
            return;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
            Debug.Log("Playing sound: " + clip.name);
        }
        else
        {
            Debug.LogError("AudioSource or AudioClip is missing");
        }
    }

    public void PlayCountdownSound()
    {
        PlaySound(countDownSound);
        Debug.Log("Local sprint sound played.");
    }

    public void PlayBuzzerSound()
    {
        PlaySound(buzzerSound);
        Debug.Log("Local run sound played.");
    }

    public void PlayAudienceSound()
    {
        PlaySound(audienceSound);
        Debug.Log("Local run sound played.");
    }

    public void PlayMainMenuMusic()
    {
        if (backgroundMusicSource != null && mainMenuMusic != null)
        {
            backgroundMusicSource.clip = mainMenuMusic;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
            backgroundMusicSource.volume = volume;
        }
        else
        {
            Debug.LogError("BackgroundMusicSource or MainMenuMusic clip is missing!");
        }
    }

    public void StopMusic()
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
        }
    }
}

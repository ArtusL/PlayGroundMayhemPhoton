//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class LocalAudioManager : MonoBehaviour
//{
//    [SerializeField]
//    private AudioClip LocalrunSound;
//    [SerializeField]
//    private AudioClip LocalsprintSound;

//    private AudioSource audioSource;

//    private void Awake()
//    {
//        audioSource = GetComponent<AudioSource>();

//        if (!audioSource)
//        {
//            Debug.LogError("Missing AudioSource component!");
//            return;
//        }

//    }
//    private void PlaySound(AudioClip clip)
//    {
//        if (audioSource && clip)
//        {
//            audioSource.clip = clip;
//            audioSource.Play();
//            Debug.Log("Playing sound: " + clip.name);
//        }
//        else
//        {
//            Debug.LogError("AudioSource or AudioClip is missing");
//        }
//    }

//    public void PlayLocalSprintSound()
//    {
//        PlaySound(LocalsprintSound);
//        Debug.Log("Local sprint sound played.");
//    }

//    public void PlayLocalRunSound()
//    {
//        PlaySound(LocalrunSound);
//        Debug.Log("Local run sound played.");
//    }
//}

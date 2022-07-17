using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jukebox : MonoBehaviour
{
    public AudioClip [] Songs;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = Songs[Random.Range(0, Songs.Length)];
            audioSource.Play();
        }
    }

    public void togglePitch(){
        if (audioSource.pitch == 1){
            audioSource.pitch = 0.5f;
        }else {
            audioSource.pitch = 1;
        }
    }
}
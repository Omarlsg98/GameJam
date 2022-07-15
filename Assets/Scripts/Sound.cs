using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MyAudioClip{
    public AudioClip audio;
    public float secondsToWait;
}

public abstract class SoundController
{
    protected AudioSource audioSource;
    private Main mainController;

    public void setAudioSource(AudioSource audioSource){
        mainController = GameObject.FindWithTag("GameController").GetComponent<Main>();
        if (audioSource == null){
            this.audioSource = mainController.gameObject.GetComponent<AudioSource>();
        }else {
            this.audioSource = audioSource;
        }
    }
    
    protected void reproduceSound(MyAudioClip clip){
        if(mainController != null)
            mainController.StartCoroutine(_reproduceSound(clip));
    }

    private IEnumerator _reproduceSound(MyAudioClip clip){
        yield return new WaitForSeconds(clip.secondsToWait);
        if (audioSource != null){
            audioSource.pitch = Random.Range(0.1f, 1.0f);
            audioSource.PlayOneShot(clip.audio);
        }
    }
}

[System.Serializable]
public class DemonSoundController : SoundController
{
    public MyAudioClip damageAudio;
    public MyAudioClip movementAudio;
    public MyAudioClip attackAudio;

   
    public void reproduceAttack(){
        reproduceSound(attackAudio);
    }

    public void reproduceAttack(MyAudioClip attackAudio){
        if (attackAudio == null){
            reproduceSound(this.attackAudio);
        }else {
            reproduceSound(attackAudio);
        }
    }

    public void reproduceMovement(){
        reproduceSound(movementAudio);
    }

    public void reproduceDamage(){
        reproduceSound(damageAudio);
    }
}

[System.Serializable]
public class AttackSoundController : SoundController
{
    public List<MyAudioClip> attackSounds;
   
    public void reproduceAttack(){
        int randNumber = Random.Range(0, attackSounds.Count);
        reproduceSound(attackSounds[randNumber]);
    }
}
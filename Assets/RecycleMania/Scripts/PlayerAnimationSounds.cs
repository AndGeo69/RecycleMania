using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationSounds : MonoBehaviour
{
    AudioSource animationSoundPlayer; 
    public PlayerController playerController;
    private List<String> sounds = new List<string>();
    // Start is called before the first frame update
    void Start()
    {
        animationSoundPlayer = GetComponent<AudioSource> ();   
        for (int i = 1; i <= 9; i++) {
            sounds.Add("FootStep0" + i);
        }
    }

    private void PlayerFootStepSound() 
    {
        if (playerController.canInteract && Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S)) {
            animationSoundPlayer.Play();            
            SimpleSoundPlayer.PlayRandomSound(sounds);
        }
     
    }
}

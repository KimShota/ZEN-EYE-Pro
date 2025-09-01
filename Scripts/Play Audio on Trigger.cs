using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioonTrigger : MonoBehaviour
{
    [SerializeField] private AudioSource myAudioSource;
    [SerializeField] private AudioClip[] myAudioClips;
    private int currentCLipIndex = 0;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !myAudioSource.isPlaying && myAudioClips.Length>0)
        {   
            myAudioSource.PlayOneShot(myAudioClips[currentCLipIndex]);

            currentCLipIndex++;
            if (currentCLipIndex >= myAudioClips.Length)
            { 
                currentCLipIndex = 0; 
            }
        }
    }
}

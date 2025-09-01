using System.Collections;
using UnityEngine;

public class PlaySpecificAudio : MonoBehaviour
{
    [SerializeField] private AudioSource myAudioSource1;
    [SerializeField] private AudioSource myAudioSource2;
    [SerializeField] private AudioSource myAudioSource3;
    [SerializeField] private AudioSource myAudioSource4;
    [SerializeField] private AudioSource myAudioSource5;
    [SerializeField] private AudioSource myAudioSource6;
    [SerializeField] private AudioSource myAudioSource7;
    [SerializeField] private AudioSource myAudioSource8;

    private void Start()
    {
        StartCoroutine(PlayAudioSequentially());
    }

    private IEnumerator PlayAudioSequentially()
    {
        while (true)
        {
            yield return PlayAndWait(myAudioSource8); // Adjust the order if needed
            yield return PlayAndWait(myAudioSource7);
            yield return PlayAndWait(myAudioSource6);
            yield return PlayAndWait(myAudioSource5);
            yield return PlayAndWait(myAudioSource4); 
            yield return PlayAndWait(myAudioSource3);
            yield return PlayAndWait(myAudioSource2);
            yield return PlayAndWait(myAudioSource1);
        }
    }

    private IEnumerator PlayAndWait(AudioSource source)
    {
        source.Play();
        yield return new WaitForSeconds(source.clip.length);
    }
}
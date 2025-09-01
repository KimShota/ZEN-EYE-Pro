using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAudio : MonoBehaviour
{
    AudioSource audioData;
    // Start is called before the first frame update
    void Start()
    {
        audioData = GetComponent<AudioSource>();
        audioData.PlayDelayed(7); 
        Debug.Log("started");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

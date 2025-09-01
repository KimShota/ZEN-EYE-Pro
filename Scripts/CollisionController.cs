using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionController : MonoBehaviour
{
    [SerializeField] AudioClip audioClip1;
    [SerializeField] string sceneName;
    [SerializeField] Material portalHover;

    // Start is called before the first frame update
    void Start()
    {
        ReproduceSound();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        ReproduceSound();
        gameObject.transform.position = gameObject.transform.position + new Vector3(0, -0.5f, 0);
        //gameObject.GetComponent<Renderer>().material.color = new Color(02156f, 0.635f, 0.862f, 1);
        //gameObject.GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
        gameObject.GetComponent<Renderer>().material = portalHover;
        Invoke("DelayMethod", 2.0f);
    }

    void DelayMethod()
    {
        SceneManager.LoadScene(sceneName);
    }

    void ReproduceSound()
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = audioClip1;
        audioSource.Play();
    }
}

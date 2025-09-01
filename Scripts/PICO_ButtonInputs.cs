using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PICO_ButtonInputs : MonoBehaviour
{
    public InputActionReference incrementButton;
    public InputActionReference decrementButton;
    public IncrementUIText      incrementScript;
    public ParticleSystem       sparkleParticles;
    public AudioSource          audis;

    // started: called only once when button is activated
    // peformed: called once per frame while button is active
    //canceled: called once when button is released
    void Start()
    {
        //incrementButton.action.started += DoIncrement;
        //decrementButton.action.started += DoDecrement;
    }

    private void OnDestroy()
    {
        //incrementButton.action.started -= DoIncrement;
        //decrementButton.action.started -= DoDecrement;
    }

    private void DoIncrement(InputAction.CallbackContext obj)
    {
        incrementScript.IncrementText();
        audis.Play();
        sparkleParticles.Play();
    }

    private void DoDecrement(InputAction.CallbackContext obj)
    {
        //incrementScript.DecrementText();
        audis.Play();
    }

}

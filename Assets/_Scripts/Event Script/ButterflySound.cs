using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflySound : MonoBehaviour
{
    public FMODUnity.StudioEventEmitter eventEmitter;
    public bool doPlaySound;

    // Update is called once per frame
    void Update()
    {
        if (doPlaySound)
        {
            eventEmitter.Play();
            doPlaySound = false;
        }
    }
}

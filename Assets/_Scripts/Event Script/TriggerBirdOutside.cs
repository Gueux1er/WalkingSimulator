using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBirdOutside : MonoBehaviour
{
    private bool isActivated;

    public FMODUnity.StudioEventEmitter birdEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!isActivated)
            {
                birdEvent.Play();
            }

            isActivated = true;
        }
    }
}

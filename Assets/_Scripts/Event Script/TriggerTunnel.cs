using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTunnel : MonoBehaviour
{
    private bool isActivated;

    public FMODUnity.StudioEventEmitter snapshotTunnel;
    public FMODUnity.StudioEventEmitter snapshotNormal;

    public FMODUnity.StudioEventEmitter[] eventEmitters;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!isActivated)
            {
                snapshotTunnel.Play();

                for (int i = 0; i < eventEmitters.Length; i++)
                {
                    eventEmitters[i].Play();
                }
            }

            isActivated = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (isActivated)
            {
                snapshotNormal.Play();

                for (int i = 0; i < eventEmitters.Length; i++)
                {
                    eventEmitters[i].Stop();
                }
            }

            isActivated = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpider : MonoBehaviour
{
    private bool isActivated;

    public FlySpiderWeb flySpiderWeb;
    public FMODUnity.StudioEventEmitter[] events;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!isActivated)
            {
                flySpiderWeb.canFly = true;

                for (int i = 0; i < events.Length; i++)
                {
                    events[i].Play();
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
                flySpiderWeb.canFly = false;

                for (int i = 0; i < events.Length; i++)
                {
                    events[i].Stop();
                }
            }

            isActivated = false;
        }
    }
}

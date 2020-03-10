using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySpiderWeb : MonoBehaviour
{
    public bool canFly = false;

    public Transform[] positionTarget;

    public FMODUnity.StudioEventEmitter fly_event;

    IEnumerator Start()
    {
        while (true)
        {
            while (!canFly)
            {
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(3f, 10f));

            fly_event.transform.position = positionTarget[Random.Range(0, positionTarget.Length)].position;
            fly_event.Play();
        }
    }
}

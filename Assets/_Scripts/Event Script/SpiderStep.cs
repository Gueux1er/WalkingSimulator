using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SpiderStep : MonoBehaviour
{
    public StudioEventEmitter spiderStepEvent;

    Vector3 lastPosition;

    void LateUpdate()
    {
        if (lastPosition == transform.position)
            spiderStepEvent.Stop();
        else if (!spiderStepEvent.IsPlaying())
            spiderStepEvent.Play();

        lastPosition = transform.position;
    }
}

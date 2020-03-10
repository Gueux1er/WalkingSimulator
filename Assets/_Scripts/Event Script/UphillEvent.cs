using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using EventInstance = FMOD.Studio.EventInstance;
using RuntimeManager = FMODUnity.RuntimeManager;

public class UphillEvent : MonoBehaviour
{
    public Transform playerTransform;

    [EventRef]
    public string uphillRef;
    private EventInstance uphillEvent;

    private void Awake()
    {
        uphillEvent = RuntimeManager.CreateInstance(uphillRef);
        uphillEvent.start();
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            return;
        }

        uphillEvent.setParameterByName("Height", Mathf.Clamp(playerTransform.position.y, 0f, 5f) / 5f);
    }
}

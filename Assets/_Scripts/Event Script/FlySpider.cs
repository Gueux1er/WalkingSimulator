using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FlySpider : MonoBehaviour
{
    public StudioEventEmitter[] flyEvents;

    void Start()
    {
        StartCoroutine(PlaySound());
    }

    IEnumerator PlaySound()
    {
        yield return null;
        while (SnailAvatar.Instance.noMove)
            yield return null;

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(15f, 35f));

            flyEvents[Random.Range(0, flyEvents.Length)].Play();
        }
    }
}

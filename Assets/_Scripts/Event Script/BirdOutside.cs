using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class BirdOutside : MonoBehaviour
{
    public StudioEventEmitter[] birdsEvent;

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
            yield return new WaitForSeconds(Random.Range(20f, 60f));

            birdsEvent[Random.Range(0, birdsEvent.Length)].Play();
        }
    }
}

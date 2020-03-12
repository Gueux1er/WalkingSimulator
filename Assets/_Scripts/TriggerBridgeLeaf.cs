using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class TriggerBridgeLeaf : MonoBehaviour
{
    public List<GameObject> leafList;
    public FMODUnity.StudioEventEmitter hardWindEventEmitter;

    private List<Collider> colliderList;

    private bool isActivated;

    private void Awake()
    {
        colliderList = GetComponentsInChildren<Collider>().ToList();
    }

    private void Start()
    {
        if (leafList.Count == 0)
        {
            Debug.LogWarning("Event trigger on scene don't have leaf assigned. Please add leaf's bridge on list of \"Trigger Bridge Leaf\".");
        }
    }

    private void GustOfWind()
    {
        hardWindEventEmitter.Play();

        int count = 0;

        for (int i = 0; i < leafList.Count; i++)
        {
            DOVirtual.DelayedCall(Random.Range(0f, 1.5f), () =>
            {
                //leafList[count].GetComponent<Collider>().enabled = false;
                leafList[count].GetComponent<Rigidbody>().useGravity = true;
                leafList[count].GetComponent<Rigidbody>().isKinematic = false;
                leafList[count].GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-14f, -21f), 20f, Random.Range(10f, 18f)) * Random.Range(0.8f, 1.2f), ForceMode.VelocityChange);
                leafList[count].GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(0.2f, 0.4f), Random.Range(0.8f, 1f), Random.Range(0.2f, 0.4f));
                count++;
            });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!isActivated)
            {
                for (int i = 0; i < colliderList.Count; i++)
                {
                    colliderList[i].enabled = false;
                }

                GustOfWind();

                isActivated = true;
            }
        }
    }
}

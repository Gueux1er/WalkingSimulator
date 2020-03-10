using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        for (int i = 0; i < leafList.Count; i++)
        {
            leafList[i].GetComponent<Collider>().enabled = false;
            leafList[i].GetComponent<Rigidbody>().useGravity = true;
            leafList[i].GetComponent<Rigidbody>().isKinematic = false;
            leafList[i].GetComponent<Rigidbody>().AddForce(new Vector3(0, 5f, -7f), ForceMode.VelocityChange);
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

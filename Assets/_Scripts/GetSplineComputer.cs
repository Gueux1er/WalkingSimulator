using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class GetSplineComputer : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<SplineFollower>().spline = transform.parent.GetChild(0).GetComponent<SplineComputer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<SplineFollower>().result.percent > 0.99f)
        {
            Destroy(this.gameObject);
        }
    }
}

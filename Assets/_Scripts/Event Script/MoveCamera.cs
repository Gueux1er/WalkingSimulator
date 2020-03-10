using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestFMOD
{
    public class MoveCamera : MonoBehaviour
    {
        public float speed;

        // Update is called once per frame
        void Update()
        {
            transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * speed * Time.deltaTime);
        }
    }
}

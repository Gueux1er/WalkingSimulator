using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using FMODUnity;
using EventInstance = FMOD.Studio.EventInstance;
using RuntimeManager = FMODUnity.RuntimeManager;

public class MouvementAvatar : MonoBehaviour
{
    Rigidbody rb;

    [Header("Post Process")]
    [SerializeField] PostProcessVolume processVolume;
    [SerializeField] PostProcessProfile blurryBase, blurryRange;
    [SerializeField] float blurringTime;

    [Header("Physics Material")]
    [SerializeField] PhysicMaterial ice;
    [SerializeField] PhysicMaterial snailPhysics;

    [Header("WayPoint")]
    [SerializeField] KeyCode[] inputWaypoint;
    [SerializeField] Vector3[] waypointPosition;

    [Header ("Other")]
    [SerializeField] float mouvementSpeed;
    float mouvementSpeedBase = 0;
    [SerializeField] float rotationSpeed;
    [SerializeField] Vector3 gravityOrientation;
    [SerializeField] float powerOfGravity;
    bool onfloor = false;
    [SerializeField] bool modeZoom = false;
    [SerializeField] bool sliding = false;
    [SerializeField] bool onIsFeet = false;
    [SerializeField] float jumpPower;

    bool canPlayEvent;

    [EventRef]
    public string snailMovementRef;
    private EventInstance snailMovementEvent;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        mouvementSpeedBase = mouvementSpeed;


        snailMovementEvent = RuntimeManager.CreateInstance(snailMovementRef);
        snailMovementEvent.start();
        snailMovementEvent.setPaused(true);
    }

    // Update is called once per frame
    void Update()
    {
        

        float moving = 0;
        float rotate = 0;


        


        #region Cheat Input

        /// Speed
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            mouvementSpeed += mouvementSpeedBase;
        }
        if (Input.GetKeyUp(KeyCode.KeypadMinus) && mouvementSpeed > mouvementSpeedBase)
        {
            mouvementSpeed -= mouvementSpeedBase;
        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            sliding = true;
            onIsFeet = false;
            StartCoroutine(JumpAndRecenter());
        }

        /// Waypoint
        for (int i = 0; i < inputWaypoint.Length; i++)
        {
            if (Input.GetKeyDown(inputWaypoint[i]))
            {
                transform.position = waypointPosition[i];
            }
        }





        #endregion


        #region InputMvt


        if (Input.GetKey(KeyCode.Z))
        {
            //print("move");
            moving = mouvementSpeed;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            //print("rotate-");
            rotate -= rotationSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            //print("rotate+");
            rotate += rotationSpeed;
        }

        if (moving != 0 && canPlayEvent)
        {
            canPlayEvent = false;
            snailMovementEvent.setPaused(false);
        }
        else if (moving == 0  && !canPlayEvent)
        {
            canPlayEvent = true;
            snailMovementEvent.setPaused(true);
        }
        #endregion



        #region Input ModeCarapace
        if (Input.GetKeyDown(KeyCode.Space))
        {
            modeZoom = true;


        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            modeZoom = false;
            GetComponent<MeshCollider>().enabled = true;
            GetComponent<SphereCollider>().enabled = false;
        }
        #endregion

        

        if (modeZoom == false)
        {
            if (onIsFeet == true)
                Moving(moving, rotate);
        }
        else
        {
            print("Zoom In");
            if (moving > 0)
            {
                print("Slide");
                sliding = true;
                GetComponent<MeshCollider>().enabled = false;
                GetComponent<SphereCollider>().enabled = true;
            }
        }



        if (sliding == true && rb.velocity.magnitude < 0.001f && GetComponent<MeshCollider>().enabled == true)
        {
            print("recentrage");
            sliding = false;

        }

    }

    private void FixedUpdate()
    {
        CheckForGravity();
    }

    void Moving (float forwardMovement, float rotationMovement)
    {
        if (onfloor == true)
        {
            rb.MovePosition(transform.forward * forwardMovement * Time.deltaTime + transform.position);
            if (forwardMovement > 0)
                transform.Rotate(0, rotationMovement * Time.deltaTime, 0);
        }
    }

    void CheckForGravity()
    {
        RaycastHit hit;
        
        if (sliding == false)
        {

            #region SphereCast
            /*RaycastHit[] allHit = Physics.SphereCastAll(transform.position, 10f, -transform.up, 3f);
            print(allHit.Length);
            if (allHit.Length != 0)
            {
                rb.useGravity = false;
                Vector3 averageNormal = Vector3.zero;
                for (int i =0; i < allHit.Length; i++)
                {
                    averageNormal += allHit[i].normal;
                }
                averageNormal /= allHit.Length;
                gravityOrientation = averageNormal;

                if (gravityOrientation.y > 0)
                {
                    rb.AddRelativeForce(0, -gravityOrientation.y * powerOfGravity, 0);

                }
                else
                {
                    rb.AddRelativeForce(0, gravityOrientation.y * powerOfGravity, 0);

                }
            }*/
            #endregion

            if (Physics.Raycast(transform.position, -transform.up, out hit, transform.localScale.y * 0.5f + 0.75f))
            {
                onIsFeet = true;
                Debug.DrawRay(transform.position, -transform.up * 0.75f, Color.yellow);
                rb.useGravity = false;
                gravityOrientation = hit.normal.normalized;
                if (gravityOrientation.y > 0)
                {
                    rb.AddRelativeForce(0, -gravityOrientation.y * powerOfGravity, 0);

                }
                else
                {
                    rb.AddRelativeForce(0, gravityOrientation.y * powerOfGravity, 0);

                }
            }
            else
            {
                onIsFeet = false;
                rb.AddForce(Physics.gravity);
            }
        }
        else
        {
            onIsFeet = false;
            print("normal gravity");
            rb.AddForce(Physics.gravity);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        onfloor = true;
        if (onIsFeet == false && rb.velocity.magnitude < 0.01f)
        {
            print("Jump");
            StartCoroutine(JumpAndRecenter());
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        onfloor = false;
    }


    IEnumerator JumpAndRecenter()
    {
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        Vector3 baseRotation = transform.rotation.eulerAngles;
        float startTime = Time.time;
        Vector3 floorRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0).eulerAngles;
        for (int i = 0; i < 50; i++)
        {
            print(baseRotation);
            transform.rotation = Quaternion.Euler(Vector3.Slerp(baseRotation, floorRotation, (Time.time - startTime)));
            yield return new WaitForSeconds(0.005f);
            print("rotate");
        }
        sliding = false;
        onIsFeet = true;
        print("Sliding Off");
    }
}

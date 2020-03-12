using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class SnailAvatar : MonoBehaviour
{
    bool touchingTheFloor = false;
    bool inFakeRotation = false;
    bool modeSlide = false;

    [SerializeField] Image fadeImage;

    [SerializeField] Vector3 gravityOrientation;
    Vector3 gravityToGive = -Vector3.up * 9.81f;



    bool noMove = false;
    bool drugEnable = false;

    [System.Serializable]
    public struct transformSnail
    {
        public Vector3 pos;
        public Quaternion orientation;

        public transformSnail(Vector3 posOfSnail, Quaternion orientationOfSnail)
        {
            pos = posOfSnail;
            orientation = orientationOfSnail;
        }
    }
    [SerializeField] public List<transformSnail> previousPosition = new List<transformSnail>();

    RaycastHit previousHit;

    MeshCollider baseCollider;
    BoxCollider slideCollider;
    [SerializeField] PhysicMaterial normalMat;
    [SerializeField] PhysicMaterial iceMat;

    float timerBetweenTwoNormal = 0;

    [SerializeField] Rigidbody rb;
    [SerializeField] Transform downRaycast;
    [SerializeField] float powerOfGravity;
    [SerializeField] float speedOfGravityOrientation;
    [SerializeField] float speedOfSnail;
    float speedOfSnailInGame;
    [SerializeField] float speedOfSnailRotation;
    float speedOfSnailRotationInGame;
    [SerializeField] float fakeSpeed;
    [SerializeField] float speedOfSlide;
    [SerializeField] GameObject cameraController;

    [SerializeField] PostProcessVolume pPV;
    ChromaticAberration chromaticAberration;
    LensDistortion lensDistortion;
    ColorGrading colorGrading;

    [SerializeField]
    bool gameStartFadeInActivation = false;
    [SerializeField]
    float gameStartFadeInDelay = 4.0f;
    [SerializeField]
    float gameStartFadeInDuration = 3.0f;



    // Start is called before the first frame update
    void Start()
    {
        baseCollider = GetComponent<MeshCollider>();
        slideCollider = GetComponent<BoxCollider>();

        speedOfSnailInGame = speedOfSnail;
        speedOfSnailRotationInGame = speedOfSnailRotation;


        pPV.profile.TryGetSettings(out chromaticAberration);
        pPV.profile.TryGetSettings(out lensDistortion);
        pPV.profile.TryGetSettings(out colorGrading);

        if (gameStartFadeInActivation)
        {
            StartCoroutine(FadeInStart());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (modeSlide == true)
        {
            AddPositionRemember();
        }

        InputManager();
        CheckFloor();
        ChangeGravity();

        if (drugEnable == true)
        {
            randomDrug();
        }
        else
        {
            DesableDrug();
        }

    }

    void InputManager()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            speedOfSnail += speedOfSnailInGame;
            speedOfSnailRotation += speedOfSnailRotationInGame;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus) && speedOfSnailInGame > speedOfSnailRotationInGame)
        {
            speedOfSnail -= speedOfSnailInGame;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus) && speedOfSnailRotation > speedOfSnailRotationInGame)
        {
            speedOfSnailRotation -= speedOfSnailRotationInGame;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            print("A");
            StartCoroutine(Die());
        }



        if (Input.GetKeyDown(KeyCode.Space))
        {
            modeSlide = true;
            ChangePhysicInteraction(modeSlide);

        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            modeSlide = false;
            ChangePhysicInteraction(modeSlide);
            rb.velocity = Vector3.zero;
        }


        if (Input.GetKey(KeyCode.Z) && modeSlide == false && noMove == false)
        {
            Moving();
        }
    }

    IEnumerator Die()
    {
        noMove = true;
        for (int i = 0; i < 100; i++)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, ((float)i / 100f));
            yield return new WaitForSeconds(0.01f);
        }

        transform.position = previousPosition[0].pos;
        transform.rotation = previousPosition[0].orientation;
        previousPosition.Clear();

        for (int i = 0; i < 100; i++)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f - ((float)i / 100f));
            yield return new WaitForSeconds(0.01f);
        }
        noMove = false;
    }

    public IEnumerator Teleport(Transform target)
    {
        noMove = true;
        for (int i = 0; i < 100; i++)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, ((float)i / 100f));
            yield return new WaitForSeconds(0.01f);
        }

        transform.position = target.position;
        transform.rotation = target.rotation;

        for (int i = 0; i < 100; i++)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f - ((float)i / 100f));
            yield return new WaitForSeconds(0.01f);
        }
        noMove = false;
    }

    IEnumerator FadeInStart()
    {
        noMove = true;

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1.0f);
        yield return new WaitForSeconds(gameStartFadeInDelay);

        for (int i = 0; i < 100; i++)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f - ((float)i / 100f));
            yield return new WaitForSeconds(0.01f);
        }

        noMove = false;
    }

    void ChangePhysicInteraction(bool ice)
    {
        if (ice == true)
        {
            //baseCollider.enabled = false;
            //slideCollider.enabled = true;
            baseCollider.material = iceMat;
        }
        else
        {
            baseCollider.material = normalMat;
            //baseCollider.enabled = true;
            //slideCollider.enabled = false;
        }
    }

    void ChangeGravity()
    {
        if (modeSlide == true)
        {
            rb.AddForce(Physics.gravity * speedOfSlide);
            rb.AddForce(gravityToGive* (1/powerOfGravity));
        }
        else
        {
            if (inFakeRotation == false)
            {
                rb.AddForce(gravityToGive);
            }
        }

        Debug.DrawLine(transform.position, transform.position + gravityToGive, Color.blue);
    }


    void Moving()
    {
        rb.MovePosition(transform.forward * speedOfSnail * Time.deltaTime + transform.position);
        float rotationOnCamera = cameraController.transform.localRotation.y * speedOfSnailRotation * Time.deltaTime;
        transform.Rotate(0, rotationOnCamera, 0);
        cameraController.GetComponent<CameraController>().rotY -= rotationOnCamera;
        AddPositionRemember();
    }

    void AddPositionRemember()
    {
        transformSnail tS;
        tS.pos = transform.position;
        tS.orientation = transform.rotation;
        previousPosition.Add(tS);
        if (previousPosition.Count > 300)
        {
            previousPosition.RemoveAt(0);
        }
    }


    void CheckFloor()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, transform.localScale.y + 5f))
        {

            Debug.DrawRay(transform.position, -transform.up * 0.75f, Color.yellow);


            gravityOrientation = Vector3.Lerp(gravityOrientation, hit.normal, Time.deltaTime * speedOfGravityOrientation);
            //gravityOrientation = hit.normal;
            //gravityOrientation = hit.normal;

        }
        else
        {
            //transform.Rotate(Vector3.right * Time.deltaTime * 10);
            /*gravityOrientation = transform.up;
            gravityToGive = -gravityOrientation * powerOfGravity;
            touchingTheFloor = false;*/

            if (Physics.Raycast(downRaycast.position, -downRaycast.up, out hit, 5f))
            {
                Debug.DrawRay(downRaycast.position, -downRaycast.up * 0.75f, Color.black);
                /*if (Input.GetKey(KeyCode.Z) == false)
                {
                    Moving();
                }
                gravityOrientation = Vector3.Lerp(gravityOrientation, hit.normal, 0.0001f);*/
                if (inFakeRotation == false)
                    StartCoroutine(Fake90());
            }


        }

        gravityToGive = -gravityOrientation * powerOfGravity;


        touchingTheFloor = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Mushroom")
        {
            drugEnable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Mushroom")
        {
            drugEnable = false;
        }
    }


    #region RandomDrugVar
    float randomX = 0.25f;
    float randomY = 0.5f;
    float timerX = 0f;
    float timerY = 0f;
    float gradingTemp = 101f;
    float gradingTint = 110f;

    #endregion

    [SerializeField] AnimationCurve curve;

    void randomDrug()
    {
        if (chromaticAberration.intensity.value < 1)
        {
            chromaticAberration.intensity.value += 0.01f;
        }
        lensDistortion.intensityX.value = curve.Evaluate( Mathf.PingPong(timerX * randomX, 1));
        print(lensDistortion.intensityX.value);
        lensDistortion.intensityY.value = curve.Evaluate( Mathf.PingPong(timerY * randomY, 1));

        colorGrading.temperature.value = Mathf.PingPong(gradingTemp, 200f) - 100f;
        colorGrading.tint.value = - (Mathf.PingPong(gradingTint, 200f) - 100f);

        timerX += Time.deltaTime;
        timerY += Time.deltaTime;
        gradingTemp += Time.deltaTime*25;
        gradingTint += Time.deltaTime*15;
    }

    void DesableDrug()
    {
        timerX = 0;
        timerY = 0;
        if (chromaticAberration.intensity.value > 0)
        {
            chromaticAberration.intensity.value -= 0.01f;
        }
        if (lensDistortion.intensityX.value > 0)
        {
            lensDistortion.intensityX.value -= 0.01f;
        }
        if (lensDistortion.intensityY.value > 0)
        {
            lensDistortion.intensityY.value -= 0.01f;
        }

        if (colorGrading.tint.value != 10)
        {
            colorGrading.tint.value += (10 - colorGrading.tint.value)*0.05f;
        }
        if (colorGrading.temperature.value != 1)
        {
            colorGrading.temperature.value += (1 - colorGrading.temperature.value) * 0.05f;
        }
    }

    IEnumerator Fake90()
    {
        inFakeRotation = true;
        this.GetComponent<MeshCollider>().isTrigger = true;
        for (int i = 0; i < fakeSpeed; i++)
        {
            transform.Rotate(90 / fakeSpeed, 0, 0);
            Moving();
            yield return new WaitForSeconds(1 / fakeSpeed);
        }

        inFakeRotation = false;
        this.GetComponent<MeshCollider>().isTrigger = false;
    }
}

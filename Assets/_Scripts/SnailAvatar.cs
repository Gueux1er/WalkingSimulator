using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using FMOD.Studio;
using FMODUnity;
using DG.Tweening;

public class SnailAvatar : MonoBehaviour
{
    public static SnailAvatar Instance;

    bool touchingTheFloor = false;
    bool inFakeRotation = false;
    bool modeSlide = false;

    [SerializeField] Image fadeImage;

    [SerializeField] Vector3 gravityOrientation;
    Vector3 gravityToGive = -Vector3.up * 9.81f;

    [EventRef]
    public string startCrackFallingRef;
    public EventInstance startCrackFallingEvent;
    [EventRef]
    public string snailMovementRef;
    public EventInstance snailMovementEvent;
    [EventRef]
    public string snailDrownRef;
    public EventInstance snailDrownEvent;
    [EventRef]
    public string snailSlidingRef;
    public EventInstance snailSlidingEvent;
    [EventRef]
    public string shellSnapshotRef;
    public EventInstance shellSnapshotEvent;
    [EventRef]
    public string heartRef;
    public EventInstance heartEvent;
    [EventRef]
    public string exitShellRef;
    public EventInstance exitShellEvent;
    [EventRef]
    public string tunnelSnapshotRef;
    public EventInstance tunnelSnapshotEvent;
    public StudioGlobalParameterTrigger drugEventEmitter;

    public bool noMove = false;
    bool drugEnable = false;
    bool logEnable = false;

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

    [SerializeField]
    GameObject waterSOundEventParent;

    Vector3 lastPosition;

    [SerializeField] float yPosToDeath;
    bool insideThePipe = false;
    bool isDead;

    [Header("Zoom")]
    [SerializeField] float durationOfZoomInAndOut;
    int stepInTheZoom =0;
    [SerializeField] GameObject gaze;
    [SerializeField] int valueMaxFieldOfView;
    [SerializeField] int valueMinFieldOfView;
    [SerializeField] int valueMaxFocal;
    [SerializeField] int valueMinFocal;
    DepthOfField depthOfField;
    bool stopZoom = true;

    [Header("WayPoint")]
    [SerializeField] KeyCode[] inputWaypoint;
    [SerializeField] Transform[] positionWaypoint;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.fieldOfView = valueMaxFieldOfView;
        gaze.SetActive(false);
        baseCollider = GetComponent<MeshCollider>();
        slideCollider = GetComponent<BoxCollider>();

        speedOfSnailInGame = speedOfSnail;
        speedOfSnailRotationInGame = speedOfSnailRotation;

        pPV.profile.TryGetSettings(out chromaticAberration);
        pPV.profile.TryGetSettings(out lensDistortion);
        pPV.profile.TryGetSettings(out colorGrading);
        pPV.profile.TryGetSettings(out depthOfField);
        depthOfField.focalLength.value = valueMaxFocal;

        if (gameStartFadeInActivation)
        {
            StartCoroutine(FadeInStart());
        }
        else
        {
            waterSOundEventParent.SetActive(true);
        }

        snailMovementEvent = RuntimeManager.CreateInstance(snailMovementRef);
        snailMovementEvent.start();
        snailMovementEvent.setPaused(true);
        snailDrownEvent = RuntimeManager.CreateInstance(snailDrownRef);
        snailSlidingEvent = RuntimeManager.CreateInstance(snailSlidingRef);
        snailSlidingEvent.start();
        shellSnapshotEvent = RuntimeManager.CreateInstance(shellSnapshotRef);
        heartEvent = RuntimeManager.CreateInstance(heartRef);
        exitShellEvent = RuntimeManager.CreateInstance(exitShellRef);
        tunnelSnapshotEvent = RuntimeManager.CreateInstance(tunnelSnapshotRef);

        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y < yPosToDeath && insideThePipe == false && !isDead)
        {
            print("die");
            StartCoroutine(Die());
            snailDrownEvent.start();
        }
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
        if (Input.GetKeyDown(KeyCode.KeypadMinus) && speedOfSnail > speedOfSnailInGame)
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
        if (Input.GetKeyDown(KeyCode.E) && !noMove)
        {
            StartCoroutine(SetZoomVision (stopZoom));
            stopZoom = !stopZoom;
            if (!stopZoom)
            {
                shellSnapshotEvent.start();
                heartEvent.start();
            }
            else
            {
                shellSnapshotEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                heartEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                exitShellEvent.start();
            }
        }

        for (int i = 0; i < inputWaypoint.Length; i++)
        {
            if (Input.GetKeyDown(inputWaypoint[i]))
            {
                transform.position = positionWaypoint[i].position;
                transform.rotation = positionWaypoint[i].rotation;
                InstantNewGravity();
            }
        }


        if (!noMove)
        {
            snailMovementEvent.setPaused(!Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space));
            snailSlidingEvent.setParameterByName("SnailDelta", Input.GetKey(KeyCode.Space) ? (transform.position - lastPosition).magnitude * 30 : 0);

            lastPosition = transform.position;
        }
    }


    

    IEnumerator SetZoomVision(bool v)
    {
        noMove = true;

        fadeImage.DOColor(new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f), durationOfZoomInAndOut)
            .OnComplete(() =>
            {
                if (v == true)
                {
                    gaze.SetActive(true);
                    Camera.main.fieldOfView = valueMinFieldOfView;
                    depthOfField.focalLength.value = valueMinFocal;
                }
                else
                {
                    gaze.SetActive(false);
                    Camera.main.fieldOfView = valueMaxFieldOfView;
                    depthOfField.focalLength.value = valueMaxFocal;
                }

                fadeImage.DOColor(new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f), durationOfZoomInAndOut)
                    .OnComplete(() => noMove = false);
            });

        yield break;

        for (int i =0; i <100; i++)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, (float)i / 100f);
            yield return new WaitForSeconds(durationOfZoomInAndOut / 100);
        }
        if (v == true)
        {
            gaze.SetActive(true);
            Camera.main.fieldOfView = valueMinFieldOfView;
            depthOfField.focalLength.value = valueMinFocal;
        }
        else
        {
            gaze.SetActive(false);
            Camera.main.fieldOfView = valueMaxFieldOfView;
            depthOfField.focalLength.value = valueMaxFocal;
            yield return new WaitForSeconds(durationOfZoomInAndOut / 100);
        }
        for (int i = 0; i < 100; i++)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1 - (float)i / 100f);
            yield return new WaitForSeconds(durationOfZoomInAndOut / 100);
        }
        noMove = false;
    }

    IEnumerator Die()
    {
        isDead = true;
        noMove = true;

        for (int i = 0; i < 100; i++)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, ((float)i / 100f));
            yield return new WaitForSeconds(0.01f);
        }

        transform.position = previousPosition[0].pos;
        transform.rotation = previousPosition[0].orientation;
        InstantNewGravity();
        ChangeGravity();
        previousPosition.Clear();

        for (int i = 0; i < 100; i++)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f - ((float)i / 100f));
            yield return new WaitForSeconds(0.01f);
        }

        isDead = false;
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

        startCrackFallingEvent = RuntimeManager.CreateInstance(startCrackFallingRef);

        yield return null;

        startCrackFallingEvent.start();

        waterSOundEventParent.SetActive(false);

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1.0f);
        yield return new WaitForSeconds(gameStartFadeInDelay);

        waterSOundEventParent.SetActive(true);

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

    void InstantNewGravity()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, transform.localScale.y + 5f))
        {
            gravityOrientation = hit.normal;
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
            if (!drugEnable)
            {
                if (DrugParameterLerpCoco != null)
                {
                    StopCoroutine(DrugParameterLerpCoco);
                }
                DrugParameterLerpCoco = DrugParameterLerp(0.5f);
                StartCoroutine(DrugParameterLerpCoco);
            }
            drugEnable = true;
        }
        else if (other.tag == "Log")
        {
            if (!logEnable)
            {
                if (LogParameterLerpCoco != null)
                {
                    StopCoroutine(LogParameterLerpCoco);
                }
                LogParameterLerpCoco = LogParameterLerp(0.8f);
                StartCoroutine(LogParameterLerpCoco);
            }
            logEnable = true;
        }

        if (other.tag == "Entry")
        {
            if (!insideThePipe)
            {
                tunnelSnapshotEvent.start();
            }
            insideThePipe = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Mushroom")
        {
            if (drugEnable)
            {
                if (DrugParameterLerpCoco != null)
                {
                    StopCoroutine(DrugParameterLerpCoco);
                }
                DrugParameterLerpCoco = DrugParameterLerp(0f);
                StartCoroutine(DrugParameterLerpCoco);
            }
            drugEnable = false;
        }
        else if (other.tag == "Log")
        {
            if (logEnable)
            {
                if (LogParameterLerpCoco != null)
                {
                    StopCoroutine(LogParameterLerpCoco);
                }
                LogParameterLerpCoco = LogParameterLerp(0f);
                StartCoroutine(LogParameterLerpCoco);
            }
            logEnable = false;
        }

        if (other.tag == "Exit")
        {
            if (insideThePipe)
            {
                tunnelSnapshotEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            insideThePipe = false;
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

    IEnumerator DrugParameterLerpCoco;
    IEnumerator DrugParameterLerp(float goalValue)
    {
        float startTime = Time.time;
        float currentValue = 0;
        RuntimeManager.StudioSystem.getParameterByName("Drug", out currentValue);

        while (Time.time - startTime < 1f)
        {
            RuntimeManager.StudioSystem.setParameterByName("Drug", Mathf.Lerp(currentValue, goalValue, Time.time - startTime));
            yield return null;
        }
    }

    IEnumerator LogParameterLerpCoco;
    IEnumerator LogParameterLerp(float goalValue)
    {
        float startTime = Time.time;
        float currentValue = 0;
        RuntimeManager.StudioSystem.getParameterByName("Reverb", out currentValue);

        while (Time.time - startTime < 1f)
        {
            RuntimeManager.StudioSystem.setParameterByName("Reverb", Mathf.Lerp(currentValue, goalValue, Time.time - startTime));
            yield return null;
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

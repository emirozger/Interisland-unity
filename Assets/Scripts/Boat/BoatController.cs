using System;
using DG.Tweening;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public static BoatController Instance;
    [Space(15)]
    public float speed = 1.0f;
    public float steerSpeed = 400.0f;
    public float boatRotateSpeed = 4.0f;
    public float movementThresold = 10.0f;
    public float maxSpeed = 10.0f;
    float vertical;
    float movementFactor;
    float horizontalInput;
    float steerFactor;
    public Transform steerTransform;
    public Rigidbody rb;
    public bool boatIsAnchored = false;
    public int anchoredForce = 50;
    public Camera playerCamera;
    public float strength = 90;
    public float randomness = 90;
    public int vibrato = 10;
    [SerializeField] private BoatInteract boatInteract;
    public Transform anchorSteer;

    private bool isSteeringSoundPlaying;

    private void Awake()
    {
        Instance = this;
        playerCamera = Camera.main;
    }

    void Start()
    {
        //   rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (UIAnimationController.Instance.IsGamePaused)
            return;
        
        Movement();
        AnchorBoat();
        Steer();
    }

    void AnchorBoat()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (boatInteract.isDriving)
            {
                if (!boatIsAnchored)
                {
                    AudioManager.Instance.PlayOneShot("Anchor Drop");
                    var rotateGoal = new Vector3(0, 0, 90);
                    anchorSteer.DOLocalRotate(rotateGoal, 4f).OnComplete(() =>
                    {
                        if (movementFactor > 0.0f)
                        {
                            movementFactor = 0f;
                            vertical = 0;
                        }
                        var anchorPoint = this.transform.position;
                        float currentSpeed = rb.velocity.magnitude;
                        float shakeStrength = currentSpeed < 4f ? strength : (currentSpeed < 6f ? strength * 3f : strength * 5f);

                        rb.AddForceAtPosition(Vector3.down * anchoredForce, anchorPoint, ForceMode.Acceleration);
                        playerCamera.DOShakeRotation(1f, shakeStrength, vibrato, randomness);
                        boatIsAnchored = true;
                    });
                }
            }
        }
    }

    void Movement()
    {
        if (!boatIsAnchored)
        {
            if (boatInteract.isDriving)
            {
                vertical = Input.GetKey(KeyCode.W) ? 1 : 0;
            }
        }

        movementFactor = Mathf.Lerp(movementFactor, vertical, Time.deltaTime / movementThresold);
        rb.AddForce(transform.forward * movementFactor * speed, ForceMode.Acceleration);
        // Speed limit
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void Steer()
    {
        if (!boatInteract.isDriving) return;

        if (!boatIsAnchored)
        {
            horizontalInput = Input.GetAxis("Horizontal");
        }

        steerFactor = Mathf.Lerp(steerFactor, horizontalInput * vertical, Time.deltaTime / movementThresold);
        steerFactor = Mathf.Clamp(steerFactor, -0.5f, 0.5f);
        steerTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, steerFactor * Mathf.Pow(steerSpeed, 2));
        transform.Rotate(0.0f, steerFactor * boatRotateSpeed, 0.0f);
        
        if ((steerFactor > 0.01f && steerFactor < 0.47f) || (steerFactor < -0.01f && steerFactor > -0.47f))
        {
            if (!isSteeringSoundPlaying)
            {
                AudioManager.Instance.Play("SteerSFX");
                isSteeringSoundPlaying = true;
            }
        }
        else
        {
            if (isSteeringSoundPlaying)
            {
                AudioManager.Instance.Stop("SteerSFX");
                isSteeringSoundPlaying = false;
            }
        }
    }
}

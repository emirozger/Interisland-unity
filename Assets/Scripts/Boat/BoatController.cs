using DG.Tweening;
using UnityEngine;

public class BoatController : MonoBehaviour
{
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
    bool boatIsAnchored = false;
    public int anchoredForce = 50;
    public Camera boatCamera;
    public float strength = 90;
    public float randomness = 90;
    public int vibrato = 10;
    [SerializeField] private BoatInteract boatInteract;

    void Start()
    {
     //   rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            boatInteract.isDriving = true;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            boatInteract.isDriving = false;
        }
        Movement();
        AnchorBoat();
        Steer();
    }
    
    void AnchorBoat()
    {
        if (!boatInteract.isDriving) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!boatIsAnchored)
            {
                boatIsAnchored = true;
                var anchorPoint = this.transform.position;
                float currentSpeed = rb.velocity.magnitude;
                float shakeStrength = currentSpeed < 4f ? strength : (currentSpeed < 6f ? strength * 3f : strength * 5f);

                rb.AddForceAtPosition(Vector3.down * anchoredForce, anchorPoint, ForceMode.Acceleration);
                boatCamera.DOShakeRotation(1f,shakeStrength,vibrato,randomness);

                     
                if (movementFactor > 0.0f)
                {
                    movementFactor = Mathf.Lerp(movementFactor, 0.0f, Time.deltaTime * movementThresold * 20);
                    vertical = 0;
                }
              
            }
            else
            {
                boatIsAnchored = false;
            }
         
          
          
          
        }
    }

    void Movement()
    {
        if (!boatIsAnchored)
        {
            //TODO : PLAYER BOTTA DEGİLSE RETURN
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
        steerTransform.localRotation = Quaternion.Euler(steerFactor * Mathf.Pow(steerSpeed, 2), 90.0f, 0.0f);
        transform.Rotate(0.0f, steerFactor * boatRotateSpeed, 0.0f);
    }
}

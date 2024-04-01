using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class BoatController : MonoBehaviour
{
    [Space(15)]
    public float speed = 1.0f;
    public float steerSpeed = 400.0f;
    public float boatRotateSpeed = 4.0f;
    public float movementThresold = 10.0f;
    float vertical;
    float movementFactor;
    float horizontalInput;
    float steerFactor;
    public Transform steerTransform;

    bool boatIsAnchored = false;

    void Update()
    {
        AnchorBoat();
        Movement();
        Steer();
    }
    void AnchorBoat()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            boatIsAnchored = !boatIsAnchored;
            if (movementFactor > 0.0f)
            {
                movementFactor = Mathf.Lerp(movementFactor, 0.0f, Time.deltaTime * movementThresold);
                vertical = 0;
            }
        }

    }
    void Movement()
    {
        if (!boatIsAnchored)
        {
            vertical = Input.GetKey(KeyCode.W) ? 1 : 0;
        }

        movementFactor = Mathf.Lerp(movementFactor, vertical, Time.deltaTime / movementThresold);
        transform.Translate(0.0f, 0.0f, movementFactor * speed);
    }

    void Steer()
    {
        if (!boatIsAnchored)
        {
            horizontalInput = Input.GetAxis("Horizontal");
        }
       
        steerFactor = Mathf.Lerp(steerFactor, horizontalInput * vertical, Time.deltaTime / movementThresold);
        steerFactor=Mathf.Clamp(steerFactor,-0.2f,0.2f);
        steerTransform.localRotation = Quaternion.Euler(steerFactor * Mathf.Pow(steerSpeed,2), 90.0f, 90.0f);
        transform.Rotate(0.0f, steerFactor * boatRotateSpeed, 0.0f);
    }
}
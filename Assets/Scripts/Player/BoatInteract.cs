using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;

public class BoatInteract : MonoBehaviour
{
    bool inBoat;
    bool isDriving;
    private float groundDistance = 0.2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask boatMask;
    [SerializeField] private BoatController boatController;

    public UnityEvent OnEnterDriveBoat;
    public UnityEvent OnExitDriveBoat;

    public Transform inBoatPlayerPos;
    public Transform cameraRig;
    public GameObject boatCamera;

    void Start()
    {
        boatController.enabled = false;
        boatCamera.gameObject.SetActive(false);
    }



    void Update()
    {
        if (Physics.CheckSphere(groundCheck.position, groundDistance, boatMask))
        {
            inBoat = true;
        }
        else
        {
            inBoat = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (inBoat)
            {
                if (isDriving)
                {
                    OnExitDriveBoat?.Invoke();
                }
                else
                {
                    OnEnterDriveBoat?.Invoke();
                }

            }



        }

    }

    public void EnterDriveHandler()
    {

        this.GetComponent<Collider>().enabled = false;
        this.transform.parent = inBoatPlayerPos;
        this.transform.localPosition = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        this.GetComponent<CharacterController>().enabled = false;
        this.GetComponent<PlayerMovement>().enabled = false;
        this.GetComponent<PlayerPickAndDrop>().enabled = false;
        this.GetComponent<DialogueManager>().enabled = false;
        this.GetComponent<Rigidbody>().isKinematic = true;
        boatController.enabled = true;
        cameraRig.gameObject.SetActive(false);
        boatCamera.gameObject.SetActive(true);
        isDriving = true;

    }
    public void ExitDriveHandler()
    {

        this.transform.parent = null;
        this.GetComponent<CharacterController>().enabled = true;
        this.GetComponent<PlayerMovement>().enabled = true;
        this.GetComponent<PlayerPickAndDrop>().enabled = true;
        this.GetComponent<DialogueManager>().enabled = true;
        this.GetComponent<Rigidbody>().isKinematic = true;
        boatController.enabled = false;
        cameraRig.gameObject.SetActive(true);
        boatCamera.gameObject.SetActive(false);
        isDriving = false;

    }
}


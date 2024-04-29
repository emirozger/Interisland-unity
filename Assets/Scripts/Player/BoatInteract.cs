using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;

public class BoatInteract : MonoBehaviour
{
    bool inBoat;
    public bool isDriving;
    private float groundDistance = 0.2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask boatMask;
    [SerializeField] private BoatController boatController;
    [SerializeField] private Camera playerFpsCamera;
    [SerializeField] private LayerMask steerLayerMask;
    [SerializeField] private Highlight steerHighlight;
    public UnityEvent OnEnterDriveBoat;
    public UnityEvent OnExitDriveBoat;

    public Transform inBoatPlayerPos;
    public Transform cameraRig;
    public GameObject boatCamera;
    Rigidbody rb;
    RaycastHit hit;
    public bool InBoat => inBoat;

    void Start()
    {
        rb=GetComponent<Rigidbody>();
        boatController.enabled = false;
        boatCamera.gameObject.SetActive(false);
    }



    void Update()
    {
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
                    print("drive suan");
                    OnEnterDriveBoat?.Invoke();
                }
            }
        }
        if (isDriving) return;
        if (hit.collider != null)
        {
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);
            inBoat = false;
        }
       
        Debug.DrawRay(playerFpsCamera.transform.position, playerFpsCamera.transform.forward*30, Color.red);

        if (Physics.Raycast(playerFpsCamera.transform.position,playerFpsCamera.transform.forward,out  hit, 3f, steerLayerMask))
        {
            print(hit.collider.name);
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
            inBoat = true;
        }      
    
       

    }

    public void EnterDriveHandler()
    {
        print("invoke");
        //steerHighlight.ToggleHighlight(false);
        Destroy(this.GetComponent<CapsuleCollider>());
        rb.isKinematic = true;
        this.transform.parent = inBoatPlayerPos;
        this.transform.localPosition = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        this.GetComponent<CharacterController>().enabled = false;
        this.GetComponent<PlayerMovement>().enabled = false;
        this.GetComponent<PlayerPickAndDrop>().enabled = false;
        this.GetComponent<DialogueManager>().enabled = false;
        boatController.enabled = true;
        cameraRig.transform.parent = this.transform;
        cameraRig.gameObject.SetActive(false);
        boatCamera.gameObject.SetActive(true);
        isDriving = true;
        

    }
    public void ExitDriveHandler()
    {
        rb.isKinematic=false;
        this.AddComponent<CapsuleCollider>();
        this.transform.parent = null;
        this.GetComponent<CharacterController>().enabled = true;
        this.GetComponent<PlayerMovement>().enabled = true;
        this.GetComponent<PlayerPickAndDrop>().enabled = true;
        this.GetComponent<DialogueManager>().enabled = true;
        //boatController.enabled = false;
        cameraRig.parent = null;
        cameraRig.gameObject.SetActive(true);
        boatCamera.gameObject.SetActive(false);
        isDriving = false;

    }
}


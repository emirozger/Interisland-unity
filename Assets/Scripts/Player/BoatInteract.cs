using System;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;

public class BoatInteract : MonoBehaviour
{
    public static BoatInteract Instance;
    bool isLookingSteer;
    public bool isDriving;
    private float groundDistance = 0.2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask boatMask;
    [SerializeField] private BoatController boatController;
    [SerializeField] private PlayerPickAndDrop pickAndDrop;
    [SerializeField] private Camera playerFpsCamera;
    [SerializeField] private LayerMask steerLayerMask;
    [SerializeField] private LayerMask anchorSteerMask;
    [SerializeField] private Highlight steerHighlight;
    public UnityEvent OnEnterDriveBoat;
    public UnityEvent OnExitDriveBoat;

    public Transform inBoatPlayerPos;
    public Transform cameraRig;
    public Transform driveBoatCameraPos;
    private Rigidbody rb;
    private RaycastHit hit;
    private bool inBoat;
    public bool InBoat => inBoat;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boatController.enabled = false;
    }

  
    void Update()
    {
        inBoat = Physics.CheckSphere(groundCheck.position, groundDistance, boatMask);
        if(pickAndDrop.InHand)  return;
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isLookingSteer  )
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
            isLookingSteer = false;
        }

        Debug.DrawRay(playerFpsCamera.transform.position, playerFpsCamera.transform.forward * 30, Color.red);

        if (Physics.Raycast(playerFpsCamera.transform.position, playerFpsCamera.transform.forward, out hit, 2f, steerLayerMask))
        {
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
            print(hit.collider.name);
            isLookingSteer = true;
           
        }
        else if ((Physics.Raycast(playerFpsCamera.transform.position, playerFpsCamera.transform.forward, out hit, 2f, anchorSteerMask)))
        {
              hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
                if (!Input.GetKeyDown(KeyCode.E)) return;
                if (!BoatController.Instance.boatIsAnchored) return;
                var rotateGoal = new Vector3(0, 180, 90);
                PlayerMovement.Instance.enabled = false;
                CameraController.Instance.enabled = false;
                BoatController.Instance.anchorSteer.DOLocalRotate(rotateGoal, 5f)
                    .OnComplete((() =>
                    {
                        BoatController.Instance.boatIsAnchored = false;
                        PlayerMovement.Instance.enabled = true;
                        CameraController.Instance.enabled = true;
                    }));
        }
    }

    public void EnterDriveHandler()
    {
        steerHighlight.ToggleHighlight(false);
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
        cameraRig.transform.parent = driveBoatCameraPos;
        cameraRig.transform.localRotation=Quaternion.identity;
        cameraRig.transform.localPosition = Vector3.zero;
        cameraRig.GetComponent<CameraController>().enabled = false;
        isDriving = true;
    }

    public void ExitDriveHandler()
    {
        rb.isKinematic = false;
        this.AddComponent<CapsuleCollider>();
        this.transform.parent = null;
        this.GetComponent<CharacterController>().enabled = true;
        this.GetComponent<PlayerMovement>().enabled = true;
        this.GetComponent<PlayerPickAndDrop>().enabled = true;
        this.GetComponent<DialogueManager>().enabled = true;
        playerFpsCamera.transform.parent = cameraRig;
        cameraRig.parent = null;
        cameraRig.GetComponent<CameraController>().enabled = true;
        isDriving = false;
    }
}
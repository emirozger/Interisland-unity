using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BoatInteract : MonoBehaviour
{
    [Header("Interaction Panels")] 
    [SerializeField]
    private GameObject driveBoatInteractPanel;    
    [SerializeField]
    private GameObject anchorInteractPanel;    
    [SerializeField]
    private GameObject anchorRotateInteractPanel;
    [SerializeField]
    private GameObject boatCameraPosPanel;   
        
    
    
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
    [SerializeField] private LayerMask npcInteractLayerMask;
    [SerializeField] private CompassController compassController;
    [SerializeField] private Highlight steerHighlight;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private MinimapController minimapController;

    public UnityEvent OnEnterDriveBoat;
    public UnityEvent OnExitDriveBoat;

    public Transform inBoatPlayerPos;
    public Transform cameraRig;
  
    private Rigidbody rb;
    private RaycastHit hit;
    private bool inBoat;
    public bool InBoat => inBoat;

    
    [SerializeField] private Transform[] driveBoatCameraPositions;
    private KeyCode[] cameraSwitchKeys = new KeyCode[] 
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5
    };
    
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
        if (pickAndDrop.InHand) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isLookingSteer)
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

        
        if (isDriving)
        {
            for (int i = 0; i < cameraSwitchKeys.Length; i++)
            {
                if (Input.GetKeyDown(cameraSwitchKeys[i]))
                {
                    Debug.Log("switch cam to {i}");
                    SwitchCameraPosition(i);
                    break;
                }
            }
        }

        if (isDriving) return;

        if (hit.collider != null)
        {
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);
            isLookingSteer = false;
            CloseInteractPanels();
        }

        Debug.DrawRay(playerFpsCamera.transform.position, playerFpsCamera.transform.forward * 30, Color.red);

        if (Physics.Raycast(playerFpsCamera.transform.position, playerFpsCamera.transform.forward, out hit, 2f,
                steerLayerMask))
        {
           // hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
            driveBoatInteractPanel.SetActive(true);
            print(hit.collider.name);
            isLookingSteer = true;
        }
        else if ((Physics.Raycast(playerFpsCamera.transform.position, playerFpsCamera.transform.forward, out hit, 2f,
                     anchorSteerMask)))
        {
            if (!BoatController.Instance.boatIsAnchored) return;
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
            anchorRotateInteractPanel.SetActive(true);
            if (!Input.GetKeyDown(KeyCode.E)) return;
            var rotateGoal = new Vector3(0, 180, 90);
            PlayerMovement.Instance.enabled = false;
            CameraController.Instance.enabled = false;
            BoatController.Instance.anchorSteer.DOLocalRotate(rotateGoal, 5f)
                .OnComplete((() =>
                {
                    BoatController.Instance.boatIsAnchored = false;
                    PlayerMovement.Instance.enabled = true;
                    CameraController.Instance.enabled = true;
                    anchorRotateInteractPanel.SetActive(false);
                }));
        }
    }
    void SwitchCameraPosition(int index)
    {
        cameraRig.transform.parent = null;
        cameraRig.transform.parent = driveBoatCameraPositions[index];
        cameraRig.transform.localPosition = Vector3.zero;
        cameraRig.transform.localRotation = Quaternion.identity;
    }
    private void CloseInteractPanels()
    {
        anchorRotateInteractPanel.SetActive(false);
        driveBoatInteractPanel.SetActive(false);
        anchorInteractPanel.SetActive(false);
    }
    public void EnterDriveHandler()
    {
        
        AudioManager.Instance.Stop("Walk");
        anchorInteractPanel.SetActive(true);
        driveBoatInteractPanel.SetActive(false);
        boatCameraPosPanel.SetActive(true);
        steerHighlight.ToggleHighlight(false);
        Destroy(this.GetComponent<CapsuleCollider>());
        compassController.SetCompass(boatController.transform);
        rb.isKinematic = true;
        this.transform.parent = inBoatPlayerPos;
        this.transform.localPosition = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        this.GetComponent<CharacterController>().enabled = false;
        this.GetComponent<PlayerMovement>().enabled = false;
        this.GetComponent<PlayerPickAndDrop>().enabled = false;
        this.GetComponent<DialogueManager>().enabled = false;
        boatController.enabled = true;
        cameraRig.transform.parent = driveBoatCameraPositions[0];
        cameraRig.transform.localRotation = Quaternion.identity;
        cameraRig.transform.localPosition = Vector3.zero;
        //cameraRig.GetComponent<CameraController>().enabled = false;
        minimapController.SetPlayer(boatController.transform);
        isDriving = true;
    }

    public void ExitDriveHandler()
    {
        anchorInteractPanel.SetActive(false);
        boatCameraPosPanel.SetActive(false);
        compassController.SetCompass(PlayerMovement.Instance.orientation);
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
        minimapController.SetPlayer(this.transform);
        isDriving = false;
    }
}
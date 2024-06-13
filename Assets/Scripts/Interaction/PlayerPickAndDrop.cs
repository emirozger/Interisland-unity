using UnityEngine;

public class PlayerPickAndDrop : MonoBehaviour
{
    public static PlayerPickAndDrop Instance;

    public ObjectGrabbling currentObjectGrabbling;
    public ObjectType inHandObjType;
    public GameObject objectToHand;

    [Header("Interaction Settings")] [SerializeField]
    private GameObject grabInteractionPanel;

    [SerializeField] private GameObject placeInteractionPanel;
    [SerializeField] private GameObject rotateInteractionPanel;
    [SerializeField] private GameObject areaFullPanel;
    [SerializeField] private GameObject sfxPlayPanel;

    [Space(30)] [SerializeField] private LayerMask pickableLayer;
    [SerializeField] private LayerMask placeableLayer;

    [SerializeField] public Transform objectGrabPointTransform;
    public GameObject inHandObject;

    private const int RAY_DISTANCE = 10;
    private Transform cameraMain;
    private RaycastHit hit;
    private bool inHand;

    public bool InHand
    {
        get { return inHand; }
        set { inHand = value; }
    }

    private void Awake() => Instance = this;

    private void Start() => cameraMain = Camera.main.transform;

    private void Update()
    {
        if (UIAnimationController.Instance.IsGamePaused)
            return;
        
            if (Physics.Raycast(cameraMain.transform.position, cameraMain.transform.forward, out RaycastHit sfxHit, 5f))
            {
                if (sfxHit.collider != null)
                {
                    if (sfxHit.collider.CompareTag("Drum"))
                    {
                        sfxPlayPanel.SetActive(true);
                        if (Input.GetMouseButtonDown(0))
                        {
                            AudioManager.Instance.PlayWithRandomPitch("Drum", 0.5f, 1.5f);
                            sfxPlayPanel.SetActive(false);
                        }
                    }

                    if (sfxHit.collider.CompareTag("Gong"))
                    {
                        sfxPlayPanel.SetActive(true);
                        if (Input.GetMouseButtonDown(0))
                        {
                            AudioManager.Instance.PlayWithRandomPitch("Gong", .6f, 1f);
                            sfxPlayPanel.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                sfxPlayPanel.SetActive(false);
            }


        if (hit.collider != null)
            CloseInteractVisual();

        if (inHand)
        {
            if (Physics.Raycast(cameraMain.transform.position, cameraMain.transform.forward, out hit, RAY_DISTANCE,
                    placeableLayer))
            {
                if (ObjectSorting.Instance.areaLimit > ObjectSorting.Instance.placedObjList.Count)
                {
                    //yerleştirilebilire bakıyorsam && elimde obje varsa && area full degilse
                    OpenPlaceVisual();
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        currentObjectGrabbling.Place(hit.collider.transform); //place
                        PlayerMoneyManager.Instance.GetPlacedArea++;
                        rotateInteractionPanel.SetActive(false);
                        currentObjectGrabbling = null;
                    }
                }
                else
                    areaFullPanel.SetActive(true); //area full
            }
            else
            {
                //yerleştirilebilir alana bakmıyorsam ve elimde obje varsa ve botta değilsem
                if (Input.GetMouseButtonDown(0) && !BoatInteract.Instance.InBoat)
                {
                    if (currentObjectGrabbling != null)
                    {
                        currentObjectGrabbling.Drop(); //drop
                        SaleInteract.Instance.SetSaleInteractPanelActive(false);
                        currentObjectGrabbling = null;
                        rotateInteractionPanel.SetActive(false);
                    }
                }
            }

            return; // elimde obje varsa pickable objeye hit atmasın.
        }

        if (Physics.Raycast(cameraMain.transform.position, cameraMain.transform.forward, out hit, RAY_DISTANCE,
                pickableLayer))
        {
            OpenGrabVisual();
            if (hit.transform.TryGetComponent(out currentObjectGrabbling)) //var ise objectgrabbling atandı
            {
                if (Input.GetMouseButtonDown(0) && !inHand)
                {
                    currentObjectGrabbling.Grab(objectGrabPointTransform); //e key object grabb
                    rotateInteractionPanel.SetActive(true);
                }
            }
            else
            {
                if (hit.collider.CompareTag("Letter"))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Destroy(hit.collider.gameObject);
                        CloseAllInteractionPanels();
                    }
                }
            }
        }
    }

    public void CloseAllInteractionPanels()
    {
        grabInteractionPanel.SetActive(false);
        placeInteractionPanel.SetActive(false);
        rotateInteractionPanel.SetActive(false);
        areaFullPanel.SetActive(false);
    }

    private void CloseInteractVisual()
    {
        hit.collider?.GetComponent<Highlight>()?.ToggleHighlight(false);
        grabInteractionPanel.SetActive(false);
        placeInteractionPanel.SetActive(false);
        //rotateInteractionPanel.SetActive(false);
        areaFullPanel.SetActive(false);
    }

    private void OpenGrabVisual()
    {
        hit.collider?.GetComponent<Highlight>()?.ToggleHighlight(true);
        grabInteractionPanel.SetActive(true);
        placeInteractionPanel.SetActive(false);
    }

    private void OpenPlaceVisual()
    {
        hit.collider?.GetComponent<Highlight>()?.ToggleHighlight(true);
        grabInteractionPanel.SetActive(false);
        placeInteractionPanel.SetActive(true);
    }
}
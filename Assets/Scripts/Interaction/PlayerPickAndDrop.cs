using UnityEngine;

public class PlayerPickAndDrop : MonoBehaviour
{
    public static PlayerPickAndDrop Instance;

    public ObjectGrabbling currentObjectGrabbling;
    public ObjectType inHandObjType;
    public GameObject objectToHand;
    [Header("Interaction Settings")] [SerializeField]
    private GameObject interactionPanel;

    [SerializeField] private GameObject areaFullPanel;
    [SerializeField] private LayerMask pickableLayer;
    [SerializeField] private LayerMask placeableLayer;
    [SerializeField] private LayerMask saleableLayer;
    
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
        if (hit.collider != null)
            CloseInteractVisual();

        if (inHand)
        {
            if (Physics.Raycast(cameraMain.transform.position, cameraMain.transform.forward, out hit, RAY_DISTANCE, placeableLayer))
            {
                if (ObjectSorting.Instance.areaLimit > ObjectSorting.Instance.placedObjList.Count)
                {
                    //yerleştirilebilire bakıyorsam && elimde obje varsa && area full degilse
                    OpenInteractVisual();
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        currentObjectGrabbling.Place(hit.collider.transform); //place
                        currentObjectGrabbling = null;
                    }
                }
                else
                    areaFullPanel.SetActive(true);  //area full
            }
            else 
            {
                //yerleştirilebilir alana bakmıyorsam ve elimde obje varsa ve botta değilsem
                if (Input.GetKeyDown(KeyCode.E) && !BoatInteract.Instance.InBoat)
                    if (currentObjectGrabbling!=null)
                    {
                        currentObjectGrabbling.Drop(); //drop
                        currentObjectGrabbling = null;
                    }
                
            }

            return; // elimde obje varsa pickable objeye hit atmasın.
        }

        if (Physics.Raycast(cameraMain.transform.position, cameraMain.transform.forward, out hit, RAY_DISTANCE, pickableLayer))
        {
            OpenInteractVisual();
            if (hit.transform.TryGetComponent(out currentObjectGrabbling)) //var ise objectgrabbling atandı
            {
                if (Input.GetKeyDown(KeyCode.E) && !inHand)
                    currentObjectGrabbling.Grab(objectGrabPointTransform); //e key object grabb
            }
        }
    }

    private void CloseInteractVisual()
    {
        hit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);
        interactionPanel.SetActive(false);
        areaFullPanel.SetActive(false);
    }

    private void OpenInteractVisual()
    {
        hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
        interactionPanel.SetActive(true);
    }
}
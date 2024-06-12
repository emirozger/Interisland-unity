using System;
using UnityEngine;

public class SaleInteract : MonoBehaviour
{
    public static SaleInteract Instance;

    [Header("Buy Interact Settings")] private Camera cameraMain;
    private RaycastHit hit;
    private const float RAY_DISTANCE = 5f;
    [SerializeField] private LayerMask InteractableLayer;
    [SerializeField] private GameObject buyInteractPanel;


    [Header("Sale Interact Settings")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionPointRadius = 1.5f;
    [SerializeField] private LayerMask saleInteractableLayer;
    [SerializeField] private int numFound;
    [SerializeField] private GameObject saleInteractPanel;

    private readonly Collider[] colliders = new Collider[3];
    private static bool isSaleNow = false;
    public static bool IsSaleNow
    {
        get => isSaleNow;
        set => isSaleNow = value;
    }

    private void Awake()
    {
        Instance = this;
        cameraMain = Camera.main;
    }

    public void SetSaleInteractPanelActive(bool isActive)
    {
        saleInteractPanel.SetActive(isActive);
    }
    void Update()
    {
        if (UIAnimationController.Instance.IsGamePaused)
            return;
        
        BuyInteractFunc();
        if (PlayerPickAndDrop.Instance.InHand == false)
        {
            return;
        }
        SaleInteractFunc();
    }

    public MissionCompleted currentNPC;

    private void SaleInteractFunc()
    {
        numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionPointRadius, colliders,
            saleInteractableLayer);
        if (numFound > 0)
        {
            currentNPC = colliders[0].GetComponent<MissionCompleted>(); // NPC'ye eri�im sa�lay�n

            if (currentNPC != null)
            {
                saleInteractPanel.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    currentNPC.offerInputField.onEndEdit.RemoveAllListeners();
                    currentNPC.offerInputField.onEndEdit.AddListener(delegate { currentNPC.SubmitOffer(); });
                    playerMovement.enabled = false;
                    saleInteractPanel.SetActive(false);
                    currentNPC.Interact();
                    this.enabled = false;
                }
            }
        }
        else
        {
            saleInteractPanel.SetActive(false);
        }
    }


    private void BuyInteractFunc()
    {
        if (hit.collider != null)
        {
            hit.collider.GetComponent<Highlight>().ToggleHighlight(false);
            buyInteractPanel.SetActive(false);
        }

        if (Physics.Raycast(cameraMain.transform.position, cameraMain.transform.forward, out hit, RAY_DISTANCE,
                InteractableLayer))
        {
            hit.collider.TryGetComponent(out IInteractable saleInteract);
            hit.collider.GetComponent<Highlight>().ToggleHighlight(true);
            buyInteractPanel.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                saleInteract?.OnInteract();
                buyInteractPanel.SetActive(false);
               
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionPointRadius);
    }
}
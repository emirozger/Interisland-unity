using System;
using UnityEngine;

public class SaleInteract : MonoBehaviour
{
    [Header("Buy Interact Settings")] private Camera cameraMain;
    private RaycastHit hit;
    private const float RAY_DISTANCE = 5f;
    [SerializeField] private LayerMask InteractableLayer;
    [SerializeField] private GameObject buyInteractPanel;


    [Header("Sale Interact Settings")] [SerializeField]
    private PlayerPickAndDrop player;

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
        cameraMain = Camera.main;
    }


    void Update()
    {
        BuyInteractFunc();
        if (PlayerPickAndDrop.Instance.InHand == false)
        {
            return;
        }
        SaleInteractFunc();
    }

    private void SaleInteractFunc()
    {
        numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionPointRadius, colliders,
            saleInteractableLayer);
        if (numFound > 0)
        {
            var missionCompleted = colliders[0].GetComponent<MissionCompleted>();

            if (missionCompleted != null && isSaleNow == false)
            {
                saleInteractPanel.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    isSaleNow = true;
                    saleInteractPanel.SetActive(false);
                    missionCompleted.Interact();
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
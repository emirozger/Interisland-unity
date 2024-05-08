using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaleInteract : MonoBehaviour
{
    private Camera cameraMain;
    private RaycastHit hit;
    private const float RAY_DISTANCE = 5f;
    [SerializeField] private LayerMask InteractableLayer;

    private void Awake()
    {
        cameraMain = Camera.main;
    }


    void Update()
    {
        if (hit.collider != null)
        {
            hit.collider.GetComponent<Highlight>().ToggleHighlight(false);
        }

        if (Physics.Raycast(cameraMain.transform.position, cameraMain.transform.forward, out hit, RAY_DISTANCE,
                InteractableLayer))
        {
            hit.collider.TryGetComponent(out IInteractable saleInteract);
            hit.collider.GetComponent<Highlight>().ToggleHighlight(true);

            if (Input.GetKeyDown(KeyCode.R))
            {
                saleInteract?.OnInteract();
            }
        }
    }
}
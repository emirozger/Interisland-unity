using System;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [SerializeField]private Transform player;
    [SerializeField] private Transform minimapBorder;
    private Vector3 minimapBorderStartPos;
    bool isMinimapMaximized = false;

    private void Start()
    {
        minimapBorderStartPos = minimapBorder.localPosition;
       // Cursor.lockState = CursorLockMode.None;
       // Cursor.visible = true;
    }


    private void LateUpdate()
    {
        var newPos = player.position;
        newPos.y = transform.position.y;
        transform.position = newPos;
        
        var targetRot = Quaternion.Euler(90f,player.eulerAngles.y,0f);
        transform.rotation = Quaternion.Lerp(transform.rotation,targetRot, Time.deltaTime*2);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!isMinimapMaximized)
            {
                this.GetComponent<Camera>().DOOrthoSize(85, 2);
                minimapBorder.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                minimapBorder.GetChild(0).localScale *= 2;
                isMinimapMaximized = true;
            }
            else
            {
                this.GetComponent<Camera>().DOOrthoSize(15, 2);
                minimapBorder.localPosition = minimapBorderStartPos;
                minimapBorder.GetChild(0).localScale /= 2;
                isMinimapMaximized = false;
            }
        }
    }
}

using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    
    [Header("Camera Movement Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;

    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerMesh;
    [SerializeField] private Transform orientation;

    private float xRotation = 0f;
    private float yRotation = 0f;
    
    private bool mouseLookEnabled = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Invoke("ActivateMouseLook", 4);
    }
    private void ActivateMouseLook()
    {
        mouseLookEnabled = true;
    }
    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
  

    private void LateUpdate()
    {
         //if (!mouseLookEnabled) return;
        
        Vector3 desiredPosition = target.position + offset;
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed*Time.fixedDeltaTime);
        transform.position = desiredPosition;
        transform.LookAt(target);
        HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        //if (!mouseLookEnabled) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(orientation.rotation.x, yRotation, orientation.rotation.z);

        playerMesh.Rotate(Vector3.up * mouseX);
    }
}
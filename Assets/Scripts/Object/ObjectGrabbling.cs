using System;
using UnityEngine;

public class ObjectGrabbling : MonoBehaviour
{
    #region Object Settings
    public ObjectType thisObjType;
    private Rigidbody rb;
    [SerializeField] public Transform objectGrabPointTransform;
    private Collider collider;
    private bool wasPlaced = false;
    #endregion
    
    #region Object Rotation Settings
    private float targetRotationX = -90f;
    private float currentRotationX = 0f;
    private const float SCROLL_SPEED = 10f;
    private const float ROTATION_SMOOTHING = 5f;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        if (this.wasPlaced && PlayerPickAndDrop.Instance.InHand == false)
        {
            this.objectGrabPointTransform = objectGrabPointTransform;
           // this.transform.parent = objectGrabPointTransform;
            //this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.Euler(-90,90,0);
            PlayerPickAndDrop.Instance.InHand = true;
            PlayerPickAndDrop.Instance.inHandObject = this.gameObject;
            this.rb.useGravity = false;
            this.rb.isKinematic = true;
            this.collider.enabled = false;
            //this.transform.parent = null;
            this.rb.interpolation = RigidbodyInterpolation.Interpolate;
            PlayerPickAndDrop.Instance.inHandObjType = this.thisObjType;
            ObjectSorting.Instance.RemoveItem(this.gameObject);
            PlayerPickAndDrop.Instance.objectToHand = this.gameObject;
            print(this.thisObjType);
        }

        if (!this.wasPlaced && PlayerPickAndDrop.Instance.InHand == false)
        {
            this.objectGrabPointTransform = objectGrabPointTransform;
            //this.transform.parent = objectGrabPointTransform;
            //this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.Euler(-90,90,0);
            PlayerPickAndDrop.Instance.inHandObject = this.gameObject;
            PlayerPickAndDrop.Instance.InHand = true;
            this.rb.useGravity = false;
            this.rb.isKinematic = true;
            this.collider.enabled = false;
            this.rb.interpolation = RigidbodyInterpolation.Interpolate;
            PlayerPickAndDrop.Instance.inHandObjType = this.thisObjType;
            print(this.thisObjType);
        }
    }

    public void Drop()
    {
        if (this.rb == null)
            return;

        this.rb.useGravity = true;
        this.rb.isKinematic = false;
        this.collider.enabled = true;
        this.objectGrabPointTransform = null;
        this.transform.parent = null;
        PlayerPickAndDrop.Instance.inHandObjType = ObjectType.Null;
        PlayerPickAndDrop.Instance.InHand = false;
    }

    public void Place(Transform parent)
    {
        this.objectGrabPointTransform = null;
        this.transform.parent = parent;
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        this.rb.isKinematic = true;
        this.collider.enabled = true;
        this.wasPlaced = true;
        this.rb.interpolation = RigidbodyInterpolation.None;
        PlayerPickAndDrop.Instance.InHand = false;
        PlayerPickAndDrop.Instance.inHandObjType = ObjectType.Null;
        ObjectSorting.Instance.AddItem(this.gameObject);
    }
    
   

    private void Update()
    {
        if (!PlayerPickAndDrop.Instance.InHand) return;
        if (PlayerPickAndDrop.Instance.inHandObject != this.gameObject) return;
        
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        
        if (scrollInput != 0)
        {
            if (scrollInput > 0f)
                targetRotationX += 90f * SCROLL_SPEED * Time.deltaTime;
            else if (scrollInput < 0f)
                targetRotationX -= 90f * SCROLL_SPEED * Time.deltaTime;
        }
        
        currentRotationX = Mathf.LerpAngle(currentRotationX, targetRotationX, Time.deltaTime * ROTATION_SMOOTHING);
        Quaternion targetRotation = Quaternion.Euler(currentRotationX, this.transform.rotation.y, this.transform.rotation.z);
        this.rb.MoveRotation(targetRotation);
    }
    
    private void FixedUpdate()
    {
        if (this.objectGrabPointTransform != null)
        {
            if (PlayerPickAndDrop.Instance.InHand == false) 
                return;

            Vector3 newPos = Vector3.Lerp(this.transform.position, objectGrabPointTransform.position, Time.fixedDeltaTime * 10);
            this.rb.MovePosition(newPos);
        }
    }
}
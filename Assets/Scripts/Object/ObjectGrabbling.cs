using System;
using UnityEngine;

public class ObjectGrabbling : MonoBehaviour
{
    public ObjectType thisObjType;
    private Rigidbody rb;
    [SerializeField] private Transform objectGrabPointTransform;
    private Collider collider;
    private bool wasPlaced = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        if (this.wasPlaced)
        {
            this.objectGrabPointTransform = objectGrabPointTransform;
            PlayerPickAndDrop.Instance.InHand = true;
            this.rb.useGravity = false;
            this.rb.isKinematic = true;
            this.collider.enabled = false;
            this.transform.parent = null;
            this.rb.interpolation = RigidbodyInterpolation.Interpolate;
            PlayerPickAndDrop.Instance.inHandObjType = this.thisObjType;
            ObjectSorting.Instance.RemoveItem(this.gameObject);
            print(this.thisObjType);
        }
        else
        {
            this.objectGrabPointTransform = objectGrabPointTransform;
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
        this.rb.useGravity = true;
        this.rb.isKinematic = false;
        this.collider.enabled = true;
        this.objectGrabPointTransform = null;
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
        float rotationAmount = Input.GetKey(KeyCode.Q) ? -100 * Time.deltaTime : Input.GetKey(KeyCode.R) ? 100 * Time.deltaTime : 0;
        this.transform.Rotate(Vector3.right, rotationAmount);
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            if (PlayerPickAndDrop.Instance.InHand == false)
                return;
          


            Vector3 newPos = Vector3.Lerp(transform.position, objectGrabPointTransform.position,
                Time.fixedDeltaTime * 10);
            rb.MovePosition(newPos);
        }
    }
}
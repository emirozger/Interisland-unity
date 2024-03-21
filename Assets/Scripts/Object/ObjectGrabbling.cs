using UnityEngine;

public class ObjectGrabbling : MonoBehaviour
{
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
            ObjectSorting.Instance.RemoveItem(this.gameObject);
        }
        else
        {
            this.objectGrabPointTransform = objectGrabPointTransform;
            PlayerPickAndDrop.Instance.InHand = true;
            this.rb.useGravity = false;
            this.rb.isKinematic = true;
            this.collider.enabled = false;
        }
        
    }
    public void Drop()
    {
        this.rb.useGravity = true;
        this.rb.isKinematic = false;
        this.collider.enabled = true;
        this.objectGrabPointTransform = null;
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
        PlayerPickAndDrop.Instance.InHand = false;
        ObjectSorting.Instance.AddItem(this.gameObject);
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            if (PlayerPickAndDrop.Instance.InHand == false)
                return;
            Vector3 newPos = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.fixedDeltaTime * 5);
            rb.MovePosition(newPos);
        }
    }
}
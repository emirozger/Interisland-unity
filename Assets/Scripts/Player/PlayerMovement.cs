using System;
using DG.Tweening;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    private Transform playerTransform;

    [Header("Movement Settings")] [SerializeField]
    private float speed = 4.5f;

    [SerializeField] private float gravity = -9.81f * 2f;
    [SerializeField] private float jumpHeight = .8f;

    [Header("Ground Check Settings")] [SerializeField]
    private float groundDistance = 0.2f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform climbPoint;
    [SerializeField] private Transform climbEndPoint;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask boatMask;
    [SerializeField] private LayerMask climbMask;
    public bool PlayerInBoat;

    [Space(20)] [SerializeField] private Transform orientation;

    private bool isClimbing;
    private bool isGrounded;
    private Vector3 velocity;
    private Vector3 input;
    private CharacterController characterController;

    public Rigidbody shipRB;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }

    public void TeleportToShip(Transform target)
    {
        this.transform.position = target.transform.position;
        this.transform.rotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        if (PlayerInBoat)
        {
            Vector3 targetPosition = shipRB.velocity;
            characterController.Move(targetPosition * Time.fixedDeltaTime);
        }
    }


    void Update()
    {
        isClimbing = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit,
            2f, climbMask);

        if (isClimbing && Input.GetKeyDown(KeyCode.Space) && hit.collider != null)
            transform.DOLocalMove(climbPoint.position, 2f)
                .OnComplete((() => transform.DOLocalMove(climbEndPoint.position, 2f)));
        
        PlayerInBoat = Physics.CheckSphere(groundCheck.position, groundDistance * 2, boatMask);
        
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        Vector3 move = orientation.right * input.x + orientation.forward * input.y;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask );
        if ((isGrounded) && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        characterController.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded || PlayerInBoat && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
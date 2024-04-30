using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;
    private Transform playerTransform;
    
    [Header("Movement Settings")]
    [SerializeField]
    private float speed = 4.5f;

    [SerializeField] private float gravity = -9.81f * 2f;
    [SerializeField] private float jumpHeight = .8f;

    [Header("Ground Check Settings")]
    [SerializeField]
    private float groundDistance = 0.2f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    [Space(20)][SerializeField] private Transform orientation;

    private bool isGrounded;
    private Vector3 velocity;
    private Vector3 input;
    private CharacterController characterController;
    
    public Rigidbody playerRB;
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

    public LayerMask boatMask;
    public bool PlayerInBoat;

    private void FixedUpdate()
    {
        if (PlayerInBoat)
        {
            Vector3 targetPosition = shipRB.velocity;
            characterController.Move(targetPosition*Time.fixedDeltaTime);
          
        }
    }

    void Update()
    {
        
        PlayerInBoat = Physics.CheckSphere(groundCheck.position, groundDistance*2, boatMask);
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        Vector3 move = orientation.right * input.x + orientation.forward * input.y;
      
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if ((isGrounded) && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        characterController.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
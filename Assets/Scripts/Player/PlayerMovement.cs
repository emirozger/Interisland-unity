using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 4.5f;
    [SerializeField] private float gravity = -9.81f * 2f;
    [SerializeField] private float jumpHeight = .8f;

    [Header("Ground Check Settings")]
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private GameObject climbInteractPanel;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform climbPoint;
    [SerializeField] private Transform climbEndPoint;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask boatMask;
    public bool PlayerInBoat;

    [Space(20)]
    [SerializeField] public Transform orientation;

    private bool isClimbing;
    private bool isGrounded;
    private Vector3 velocity;
    private Vector3 input;
    private CharacterController characterController;

    public Rigidbody shipRB;

    private bool isMoving;
    private bool wasMoving;

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

    private RaycastHit hit;

    void Update()
    {
        if (hit.collider != null)
        {
            climbInteractPanel.SetActive(false);
            isClimbing = false;
        }
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2f))
        {
            if (hit.collider != null && hit.collider.CompareTag("Climb") && !isClimbing)
            {
                isClimbing = true;
                climbInteractPanel.SetActive(true);
            }
        }

        if (isClimbing && Input.GetKeyDown(KeyCode.Space))
        {
            climbInteractPanel.SetActive(false);
            isClimbing = false;
            transform.DOLocalMove(climbPoint.position, 2f).OnComplete(() => transform.DOLocalMove(climbEndPoint.position, 2f));
        }

        PlayerInBoat = Physics.CheckSphere(groundCheck.position, groundDistance * 2, boatMask);

        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        Vector3 move = orientation.right * input.x + orientation.forward * input.y;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        characterController.Move(move * speed * Time.deltaTime);

        if ((Input.GetButtonDown("Jump") && isGrounded) || (PlayerInBoat && Input.GetButtonDown("Jump")))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // Check if player is moving
        isMoving = move != Vector3.zero;

        if (isMoving && !wasMoving)
        {
            AudioManager.Instance.Play("Walk");
        }
        else if (!isMoving && wasMoving)
        {
            AudioManager.Instance.Stop("Walk");
        }

        wasMoving = isMoving;
    }
}

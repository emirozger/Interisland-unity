using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using WaterSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")] [SerializeField]
    private float speed = 4.5f;

    [SerializeField] private float gravity = -9.81f * 2f;
    [SerializeField] private float jumpHeight = .8f;

    [Header("Ground Check Settings")] [SerializeField]
    private float groundDistance = 0.2f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    [Space(20)] [SerializeField] private Transform orientation;

    private bool isGrounded;
    private Vector3 velocity;
    private Vector3 input;
    private CharacterController characterController;

    public bool inSea;
    public GameObject inSeaCamera;
    public Rigidbody rb;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sea"))
        {
            Debug.Log("trigger enter");
            inSeaCamera.SetActive(true);
            this.characterController.enabled = false;
            GameObject.FindObjectOfType<CameraController>().enabled = false;
            this.GetComponent<BuoyantObject>().enabled = true;
            rb.isKinematic = false;
            inSea = true;
            other.gameObject.SetActive(false);
        }
    }

    float xRotation;
    float yRotation;

    void Update()
    {
        if (inSea)
        {
            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");
            Vector3 move = orientation.right * input.x + orientation.forward * input.y;
            rb.velocity = move * speed;

            float mouseX = Input.GetAxis("Mouse X") * 300 * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * 300 * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            yRotation += mouseX;
            inSeaCamera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
            orientation.rotation = Quaternion.Euler(orientation.rotation.x, yRotation, orientation.rotation.z);

            orientation.Rotate(Vector3.up * mouseX);
            
        }
        else
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            input.x = Input.GetAxis("Horizontal");
            input.y = Input.GetAxis("Vertical");

            Vector3 move = orientation.right * input.x + orientation.forward * input.y;
            characterController.Move(move * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
    }
}
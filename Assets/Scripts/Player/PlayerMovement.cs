using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")] 
    [SerializeField] private float speed = 4.5f;
    [SerializeField] private float gravity = -9.81f * 2f;
    [SerializeField] private float jumpHeight = .8f;

    [Header("Ground Check Settings")] 
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    [Space(20)]
    [SerializeField] private Transform orientation;

    private bool isGrounded;
    private Vector3 velocity;
    private Vector3 input;
    private CharacterController characterController;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }

    void Update()
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
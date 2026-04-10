using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public Transform cameraTransform;
    private Vector2 moveInput;

    [Header("Sprint Settings")]
    public float sprintMultiplier = 1.5f;
    private bool isRunning;

    [Header("Jump Settings")]
    public float jumpForce = 4f;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private PlayerInputActions controls;

    public float CurrentSpeed { get; private set; }
    public bool IsRunning => isRunning;
    public bool IsGrounded => isGrounded; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        controls = new PlayerInputActions();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        moveInput = controls.Player.Move.ReadValue<Vector2>();
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (controls.Player.Run.triggered && moveInput.y > 0.1f && isGrounded)
            isRunning = !isRunning;

        if (moveInput.y <= 0f || !isGrounded)
            isRunning = false;

        if (controls.Player.Jump.triggered && isGrounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        float curSpeed = isRunning ? speed * sprintMultiplier : speed;
        Vector3 move = cameraTransform.right * moveInput.x + cameraTransform.forward * moveInput.y;
        move.y = 0f;

        CurrentSpeed = rb.linearVelocity.magnitude;
        Vector3 targetVelocity = move * curSpeed;
        targetVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVelocity;
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    // Movimento

    [Header("Movement Settings")]
    public float speed = 3f;                 

    public Transform cameraTransform;        
    private Vector2 moveInput;

    //-------------------------------------------

    // Corrida

    [Header("Sprint Settings")]
    public float sprintMultiplier = 1.5f;
    private bool isRunning;  
    private bool runPressed;    

    //-------------------------------------------

    // Salto

    [Header("Jump Settings")]

    public float jumpForce = 3f;
    private bool isGrounded;   
    private bool jumpPressed;         
    public Transform groundCheck;       
    public float groundDistance = 0.2f;
    public LayerMask groundMask; 

    //-------------------------------------------

    private Rigidbody rb;                    
    private PlayerInputActions controls;

    private void Awake()
    {
        // Get Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Para não cair em colisão com algo
        rb.freezeRotation = true;

        // Initialize the Input Actions
        controls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
           moveInput = controls.Player.Move.ReadValue<Vector2>();

            // Ground check
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            // Jump input
            jumpPressed = controls.Player.Jump.triggered;

            // Read sprint input
            // Toggle run
            if (controls.Player.Run.triggered && moveInput != Vector2.zero)
            {
                isRunning = !isRunning;
            }

            // Reset run when player stops
            if (moveInput == Vector2.zero)
            {
                isRunning = false;
            }

            if (moveInput.y < 0f) 
            {
                isRunning = false;
            }

    }

    private void FixedUpdate()
    {
        float currentSpeed = speed;
        if (isRunning)
        currentSpeed *= sprintMultiplier;


        Vector3 move = cameraTransform.right * moveInput.x + cameraTransform.forward * moveInput.y;
        move.y = 0f;

        // Calculate new position using Rigidbody
        Vector3 newPos = rb.position + move * currentSpeed * Time.fixedDeltaTime;

        if (controls.Player.Jump.triggered && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        rb.MovePosition(newPos);
    }
}